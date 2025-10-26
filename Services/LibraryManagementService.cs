using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using SampleLibraryMgmtSystem.Data;

using SampleLibraryMgmtSystem.Models;

namespace SampleLibraryMgmtSystem.Services
{
    public class LibraryManagementService
    {
        private readonly AppDbContext _appDbContext;

        public LibraryManagementService(AppDbContext context)
        {
            _appDbContext = context;
        }

        // Methods to manage books, members, and borrowers can be added here
        #region Books
        [HttpGet]
        public List<Books> GetAllBooks() => _appDbContext.BooksEF.ToList();
        public Books? GetBookByID(int id) => _appDbContext.BooksEF.FirstOrDefault(p => p.Id == id);
        public Books? GetBookByTitle(string title) => _appDbContext.BooksEF.FirstOrDefault(p => p.Title == title);
        public Books? GetBookByAuthor(string author) => _appDbContext.BooksEF.FirstOrDefault(p => p.Author == author);

        // Case-insensitive searches (performed in-memory to avoid provider-specific collation issues)
        public List<Books> GetBooksByTitleCaseInsensitive(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return new List<Books>();
            return _appDbContext.BooksEF
                .ToList()
                .Where(b => !string.IsNullOrEmpty(b.Title) && b.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Books> GetBooksByAuthorCaseInsensitive(string author)
        {
            if (string.IsNullOrWhiteSpace(author)) return new List<Books>();
            return _appDbContext.BooksEF
                .ToList()
                .Where(b => !string.IsNullOrEmpty(b.Author) && b.Author.Contains(author, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Books> GetBooksByPublisherCaseInsensitive(string publisher)
        {
            if (string.IsNullOrWhiteSpace(publisher)) return new List<Books>();
            return _appDbContext.BooksEF
                .ToList()
                .Where(b => !string.IsNullOrEmpty(b.Publisher) && b.Publisher.Contains(publisher, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Books> SearchBooksCaseInsensitive(string? title = null, string? author = null, string? genre = null, string? isbn = null, string? publisher = null)
        {
            var list = _appDbContext.BooksEF.ToList().AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                list = list.Where(b => !string.IsNullOrEmpty(b.Title) && b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(author))
                list = list.Where(b => !string.IsNullOrEmpty(b.Author) && b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(genre))
                list = list.Where(b => !string.IsNullOrEmpty(b.Genre) && b.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(isbn))
                list = list.Where(b => !string.IsNullOrEmpty(b.Isbn) && b.Isbn.Contains(isbn, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(publisher))
                list = list.Where(b => !string.IsNullOrEmpty(b.Publisher) && b.Publisher.Contains(publisher, StringComparison.OrdinalIgnoreCase));

            return list.ToList();
        }

        public void AddBooks(Books book)
        {
            _appDbContext.BooksEF.Add(book);
            _appDbContext.SaveChanges();
        }

        public void UpdateBook(Books book)
        {
            var existingBook = _appDbContext.BooksEF.Find(book.Id);
            if (existingBook == null)
                return;

            _appDbContext.Entry(existingBook).CurrentValues.SetValues(book);
            _appDbContext.SaveChanges();
        }        

        public void DeleteBook(int id)
        {
            var book = _appDbContext.BooksEF.Find(id);
            if (book == null)
                return;

            _appDbContext.BooksEF.Remove(book);
            _appDbContext.SaveChanges();
        }       

        #endregion

        #region Statistics
        public OverdueStatistics GetOverdueStatistics()
        {
            var stats = new OverdueStatistics();
            var allBorrowings = _appDbContext.BorrowersEF
                .Include(b => b.Book)
                .Include(b => b.Member)
                .ToList();

            // Get all overdue borrowings (both active and returned)
            var overdueBorrowings = allBorrowings
                .Where(b => (b.ReturnDate.HasValue && b.ReturnDate > b.DueDate) || 
                           (!b.ReturnDate.HasValue && DateTime.Now > b.DueDate))
                .ToList();

            // Calculate basic statistics
            stats.TotalOverdueBooks = overdueBorrowings.Count;
            stats.TotalActiveOverdueBooks = overdueBorrowings.Count(b => b.ReturnDate == null);

            // Calculate average overdue days
            var totalOverdueDays = overdueBorrowings.Sum(b => 
            {
                var endDate = b.ReturnDate ?? DateTime.Now;
                return Math.Max(0, (endDate - b.DueDate).Days);
            });
            stats.AverageOverdueDays = overdueBorrowings.Any() 
                ? Math.Round((decimal)totalOverdueDays / overdueBorrowings.Count, 1)
                : 0;

            // Calculate overdue by genre
            stats.OverdueByGenre = overdueBorrowings
                .GroupBy(b => b.Book?.Genre ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            // Get most overdue books
            stats.MostOverdueBooks = overdueBorrowings
                .GroupBy(b => new { b.BookId, Title = b.Book?.Title ?? "Unknown" })
                .Select(g => new TopOverdueItem
                {
                    Id = g.Key.BookId,
                    Name = g.Key.Title,
                    Count = g.Count(),
                    TotalDaysOverdue = g.Sum(b => 
                    {
                        var endDate = b.ReturnDate ?? DateTime.Now;
                        return Math.Max(0, (endDate - b.DueDate).Days);
                    })
                })
                .OrderByDescending(x => x.Count)
                .ThenByDescending(x => x.TotalDaysOverdue)
                .Take(5)
                .ToList();

            // Get members with most overdue books
            stats.MostFrequentlyOverdueMembers = overdueBorrowings
                .GroupBy(b => new { b.MemberId, Name = $"{b.Member?.FirstName} {b.Member?.Surname}".Trim() })
                .Select(g => new TopOverdueItem
                {
                    Id = g.Key.MemberId,
                    Name = string.IsNullOrWhiteSpace(g.Key.Name) ? "Unknown" : g.Key.Name,
                    Count = g.Count(),
                    TotalDaysOverdue = g.Sum(b => 
                    {
                        var endDate = b.ReturnDate ?? DateTime.Now;
                        return Math.Max(0, (endDate - b.DueDate).Days);
                    })
                })
                .OrderByDescending(x => x.Count)
                .ThenByDescending(x => x.TotalDaysOverdue)
                .Take(5)
                .ToList();

            return stats;
        }
        #endregion

        #region Members
        public List<Members> GetAllMembers() => _appDbContext.MembersEF.ToList();
        public Members? GetMemberByID(int id) => _appDbContext.MembersEF.FirstOrDefault(p => p.MemberId == id);
        public Members? GetMemberByFirstName(string firstName) => _appDbContext.MembersEF.FirstOrDefault(p => p.FirstName == firstName);
        public Members? GetMemberBySurname(string surname) => _appDbContext.MembersEF.FirstOrDefault(p => p.Surname == surname);

        // Case-insensitive member searches (in-memory to keep behavior provider-independent)
        public Members? GetMemberByFirstNameCaseInsensitive(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName)) return null;
            return _appDbContext.MembersEF.ToList()
                .FirstOrDefault(m => !string.IsNullOrEmpty(m.FirstName) && m.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));
        }

        public Members? GetMemberBySurnameCaseInsensitive(string surname)
        {
            if (string.IsNullOrWhiteSpace(surname)) return null;
            return _appDbContext.MembersEF.ToList()
                .FirstOrDefault(m => !string.IsNullOrEmpty(m.Surname) && m.Surname.Equals(surname, StringComparison.OrdinalIgnoreCase));
        }
        
        public void AddMembers(Members member)
        {
            _appDbContext.MembersEF.Add(member);
            _appDbContext.SaveChanges();
        }

        public void UpdateMemberById(Members member)
        {
            var existingMember = _appDbContext.MembersEF.Find(member.MemberId);
            if (existingMember == null)
                return;

            _appDbContext.Entry(existingMember).CurrentValues.SetValues(member);
            _appDbContext.SaveChanges();
        }

        public void SetMemberToInactive(Members member)
        {
            var existingMember = _appDbContext.MembersEF.Find(member.MemberId);
            if (existingMember == null)
                return;

            _appDbContext.Entry(existingMember).CurrentValues.SetValues(member);
            _appDbContext.SaveChanges();
        }

        public void DeleteMember(int id)
        {
            var member = _appDbContext.MembersEF.Find(id);
            if (member == null)
                return;

            _appDbContext.MembersEF.Remove(member);
            _appDbContext.SaveChanges();
        }

        #endregion

        #region Borrowers
        public List<Borrower> GetAllBorrowers() => _appDbContext.BorrowersEF.ToList();
        public Borrower? GetBorrowerById(int borrowId) => 
            _appDbContext.BorrowersEF.Find(borrowId);

        // Return active (unreturned) borrowing for a member/book, if any
        public Borrower? GetBorrowerByMemberAndBookID(int memberId, int bookId) => 
            _appDbContext.BorrowersEF.FirstOrDefault(p => p.MemberId == memberId && p.BookId == bookId && p.ReturnDate == null);

        public Borrower? GetBorrowerByBookID(int bookId)
        {
            // Only return active (unreturned) borrowings
            return _appDbContext.BorrowersEF
                .Where(e => e.BookId == bookId && e.ReturnDate == null)
                .FirstOrDefault();
        }

        public void AddBorrower(Borrower borrower)
        {
            _appDbContext.BorrowersEF.Add(borrower);
            _appDbContext.SaveChanges();
        }

        public void UpdateBorrower(Borrower borrower)
        {
            var existingBorrower = _appDbContext.BorrowersEF.Find(borrower.Id);
            if (existingBorrower == null)
                return;

            _appDbContext.Entry(existingBorrower).CurrentValues.SetValues(borrower);
            _appDbContext.SaveChanges();
        }

        public void ReturnBook(int memberId, int bookId)
        {
            var borrowing = _appDbContext.BorrowersEF
                .FirstOrDefault(b => b.MemberId == memberId 
                                && b.BookId == bookId 
                                && b.ReturnDate == null);

            if (borrowing == null)
                throw new InvalidOperationException("No active borrowing found for this member and book");

            borrowing.ReturnDate = DateTime.Now;
            borrowing.IsOverdue = borrowing.ReturnDate > borrowing.DueDate;

            _appDbContext.SaveChanges();
        }

        public void UpdateBorrowingDueDate(int borrowId, DateTime newDueDate)
        {
            var borrowing = _appDbContext.BorrowersEF.Find(borrowId);
            if (borrowing == null)
                throw new InvalidOperationException("Borrowing record not found");

            borrowing.DueDate = newDueDate;
            if (borrowing.ReturnDate != null)
            {
                borrowing.IsOverdue = borrowing.ReturnDate > newDueDate;
            }

            _appDbContext.SaveChanges();
        }

        public bool CanBorrowBook(int bookId)
        {
            var book = _appDbContext.BooksEF.Find(bookId);
            if (book == null) return false;

            return book.AvailableCopies > 0;
        }

        public void DeleteBorrower(int id)
        {
            var borrower = _appDbContext.BorrowersEF.Find(id);
            if (borrower == null)
                return;

            _appDbContext.BorrowersEF.Remove(borrower);
            _appDbContext.SaveChanges();
        }

        #endregion
    }
}