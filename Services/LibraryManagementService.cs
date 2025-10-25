using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

using SampleLibraryMgmtSystem.Models;

namespace SampleLibraryMgmtSystem.Services
{
    public class LibraryManagementService
    {
        static List<Books> _books { get; }
        static List<Members> _members { get; }
        static List<Borrower> _borrowers { get; }

        static LibraryManagementService()
        {
            _books = new List<Books>
            {
                new Books(1, "To Kill a Mockingbird", "Harper Lee", "Fiction", "9780446310789", new DateTime(1925, 4, 10), 5),
                new Books(2, "Hercule Poirot: The Complete Short Stories", "Agatha Christie", "Fiction", "9780006513773", new DateTime(1999, 1, 2), 2),
                new Books(3, "The Shipping News", "Annie Proulx", "Fiction", "0743225406", new DateTime(1993, 1, 2), 3),
                new Books(4, "Sons of Fortune", "Jeffrey Archer", "Fiction", "9781447221838", new DateTime(2002, 2, 11), 2),
                new Books(5, "Bleak House", "Charles Dickens", "Fiction", "9780140621204", new DateTime(1994, 1, 3), 10)
            };
            _members = new List<Members>
            {
                new Members(1, "Teng Joon", "Koo", new DateTime(1985, 10, 11), false, true),
                new Members(2, "Jane", "Lee", new DateTime(1988, 9, 12), false, true),
                new Members(3, "Rachel", "Lim", new DateTime(2020, 12, 3), true, true),
                new Members(4, "Luke", "Wong", new DateTime(2021, 3, 30), true, true),
                new Members(5, "Wei Yee", "Loh", new DateTime(1978, 6, 30), false, false),
            };
            _borrowers = new List<Borrower>();
        }

        // Methods to manage books, members, and borrowers can be added here
        #region Books
        [HttpGet]
        public static List<Books> GetAllBooks() => _books;
        public static Books? GetBookByID(int id) => _books.FirstOrDefault(p => p.Id == id);
        public static Books? GetBookByTitle(string title) => _books.FirstOrDefault(p => p.Title == title);
        public static Books? GetBookByAuthor(string author) => _books.FirstOrDefault(p => p.Author == author);

        public static void AddBooks(Books book)
        {
            book.Id = _books.Max(x => x.Id) + 1;
            _books.Add(book);
        }

        public static void UpdateBook(Books book)
        {
            var index = _books.FindIndex(p => p.Id == book.Id);
            if (index == -1)
                return;

            _books[index] = book;
        }        

        public static void DeleteBook(int id)
        {
            var book = GetBookByID(id);
            if (book is null)
                return;

            _books.Remove(book);
        }       

        #endregion

        #region Members
        public static List<Members> GetAllMembers() => _members;
        public static Members? GetMemberByID(int id) => _members.FirstOrDefault(p => p.MemberId == id);
        public static Members? GetMemberByFirstName(string firstName) => _members.FirstOrDefault(p => p.FirstName == firstName);
        public static Members? GetMemberBySurname(string surname) => _members.FirstOrDefault(p => p.Surname == surname);
        public static void AddMembers(Members member)
        {
            member.MemberId = _members.Max(x => x.MemberId) + 1;
            _members.Add(member);
        }

        public static void UpdateMemberById(Members member)
        {
            var index = _members.FindIndex(p => p.MemberId == member.MemberId);
            if (index == -1)
                return;

            _members[index] = member;
        }

        public static void SetMemberToInactive(Members member)
        {
            var index = _members.FindIndex(p => p.MemberId == member.MemberId);
            if (index == -1)
                return;

            _members[index] = member;
        }

        public static void DeleteMember(int id)
        {
            var member = GetMemberByID(id);
            if (member is null)
                return;

            _members.Remove(member);
        }

        #endregion

        #region Borrowers
        public static List<Borrower> GetAllBorrowers() => _borrowers;

        #endregion
    }
}