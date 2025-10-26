using Microsoft.AspNetCore.Mvc;
using SampleLibraryMgmtSystem.Models;
using SampleLibraryMgmtSystem.Services;
using Microsoft.EntityFrameworkCore;
using SampleLibraryMgmtSystem.Data;
using Microsoft.Data.Sqlite;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SampleLibraryMgmtSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LibraryManagementController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly LibraryManagementService _libraryManagementService;

        public LibraryManagementController(AppDbContext context, LibraryManagementService libraryManagementService)
        {
            _appDbContext = context;
            _libraryManagementService = libraryManagementService;
        }

        // Test endpoint to check SQLite connection
        [HttpGet("testsqlite-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                // Check if database exists and can be opened
                var canConnect = await _appDbContext.Database.CanConnectAsync();

                if (canConnect)
                {
                    var provider = _appDbContext.Database.ProviderName;
                    return Ok($"Connected to database successfully using SQLite.");
                }
                else
                {
                    return StatusCode(500, "Cannot connect to the SQLite database.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Connection failed: {ex.Message}");
            }
        }

        #region Books
        [HttpGet]
        [Route("GetAllBooks")]
        public ActionResult<List<Books>> GetAllBooks() => _libraryManagementService.GetAllBooks();

        [HttpGet]
        [Route("GetBookById/{id}")]
        public ActionResult<Books> GetBookById(int id)
        {
            var book = _libraryManagementService.GetBookByID(id);

            if (book == null)
                return NotFound();
            return book;
        }

        [HttpGet]
        [Route("SearchBookByTitle")]
        public ActionResult<List<Books>> SearchBookByTitle([FromQuery] string bookTitle)
        {
            // Case-insensitive search by title (supports partial matches) — return all matches
            if (string.IsNullOrWhiteSpace(bookTitle))
                return BadRequest("bookTitle query is required.");

            var books = _libraryManagementService.GetBooksByTitleCaseInsensitive(bookTitle);

            if (!books.Any())
                return NotFound(Constants.BOOK_TITLE_NOT_FOUND);

            return books;
        }

        [HttpGet]
        [Route("SearchBookByAuthor")]
        public ActionResult<List<Books>> SearchBookByAuthor([FromQuery] string authorName)
        {
            // Case-insensitive search for author
            if (string.IsNullOrWhiteSpace(authorName))
                return BadRequest("authorName query is required.");

            var booksByAuthor = _libraryManagementService.GetBooksByAuthorCaseInsensitive(authorName);

            if (!booksByAuthor.Any())
                return NotFound(Constants.NO_BOOKS_BY_THIS_AUTHOR);

            return booksByAuthor;
        }

        [HttpGet]
        [Route("SearchBookByPublisher")]
        public ActionResult<List<Books>> SearchBookByPublisher([FromQuery] string publisherName)
        {
            if (string.IsNullOrWhiteSpace(publisherName))
                return BadRequest("publisherName query is required.");

            var books = _libraryManagementService.GetBooksByPublisherCaseInsensitive(publisherName);

            if (!books.Any())
                return NotFound("No books found from this publisher.");

            return books;
        }

        [HttpGet]
        [Route("SearchBooks")]
        public ActionResult<List<Books>> SearchBooks([FromQuery] string? title = null, [FromQuery] string? author = null, 
            [FromQuery] string? genre = null, [FromQuery] string? isbn = null, [FromQuery] string? publisher = null)
        {
            var books = _libraryManagementService.SearchBooksCaseInsensitive(title, author, genre, isbn, publisher);

            if (!books.Any())
                return NotFound("No books found matching the search criteria.");

            return books;
        }

        [HttpPost]
        [Route("AddBooks")]
        public IActionResult AddBooks(Books book)
        {
            // handle when this is empty;
            book.Id = _libraryManagementService.GetAllBooks().ToList().Count == 0 ? 1 : (_libraryManagementService.GetAllBooks().ToList().Max(b => b.Id) + 1);
            _libraryManagementService.AddBooks(book);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        [HttpPut("UpdateBookById/{id}")]
        public IActionResult Update(int id, Books book)
        {
            if (id != book.Id)
                return BadRequest();

            var existingBook = _libraryManagementService.GetBookByID(id);
            if (existingBook is null)
                return NotFound();

            _libraryManagementService.UpdateBook(book);

            return NoContent();
        }

        // DELETE action
        [HttpDelete("DeleteBookById/{id}")]
        public IActionResult DeleteBook(int id)
        {
            var book = _libraryManagementService.GetBookByID(id);

            if (book is null)
                return NotFound();

            _libraryManagementService.DeleteBook(id);

            return NoContent();
        }

        #endregion

        #region Members
        [HttpGet]
        [Route("GetAllMembers")]
        public ActionResult<List<Members>> GetAllMembers() => _libraryManagementService.GetAllMembers();

        [HttpGet]
        [Route("GetMemberById/{id}")]
        public ActionResult<Members> GetMemberById(int id)
        {
            var member = _libraryManagementService.GetMemberByID(id);

            if (member == null)
                return NotFound();
            return member;
        }

        [HttpGet]
        [Route("SearchMembersByFirstName")]
        public ActionResult<Members> SearchMembersByFirstName([FromQuery] string firstName)
        {
            // Case-insensitive search by first name
            if (string.IsNullOrWhiteSpace(firstName))
                return BadRequest("firstName query is required.");

            var member = _libraryManagementService.GetMemberByFirstNameCaseInsensitive(firstName);

            if (member == null)
                return NotFound(Constants.NO_MEMBERS_WITH_THIS_FIRST_NAME);

            return member;
        }

        [HttpGet]
        [Route("SearchMembersBySurname")]
        public ActionResult<Members> SearchMembersBySurname([FromQuery] string surname)
        {
            // Case-insensitive search by surname
            if (string.IsNullOrWhiteSpace(surname))
                return BadRequest("surname query is required.");

            var member = _libraryManagementService.GetMemberBySurnameCaseInsensitive(surname);

            if (member == null)
                return NotFound(Constants.NO_MEMBERS_WITH_THIS_SURNAME);

            return member;
        }

        [HttpPost]
        [Route("AddMembers")]
        public IActionResult AddMembers(Members member)
        {
            member.MemberId = _libraryManagementService.GetAllMembers().ToList().Count == 0 ? 1 : (_libraryManagementService.GetAllMembers().ToList().Max(b => b.MemberId) + 1);
            _libraryManagementService.AddMembers(member);
            return CreatedAtAction(nameof(GetMemberById), new { id = member.MemberId }, member);
        }

        [HttpPut]
        [Route("UpdateMemberById/{id}")]
        public IActionResult UpdateMemberById(int id, Members member)
        {
            if (id != member.MemberId)
                return BadRequest();

            var existingMember = _libraryManagementService.GetMemberByID(id);
            if (existingMember is null)
                return NotFound();

            _libraryManagementService.UpdateMemberById(member);
            return NoContent();
        }

        // Not in use. We need to use the Microsoft.AspNetCore.Mvc.NewtonsoftJson and Newtonsoft.Json package to support PATCH method
        // Patch updates an attribute of the request. POST updates the entire request.
        [HttpPut]
        [Route("SetMemberToInactive/{id}")]
        public IActionResult SetMemberToInactive(int id, bool isActive = false)
        {
            var member = _libraryManagementService.GetMemberByID(id);
            if (member == null)
                return NotFound();

            member.IsActive = isActive;
            _libraryManagementService.UpdateMemberById(member);
            return NoContent();
        }

        [HttpDelete]
        [Route("DeleteMembers/{id}")]
        public IActionResult DeleteMembers(int id)
        {
            _libraryManagementService.DeleteMember(id);
            return NoContent();
        }

        #region Member Validation
        // Checks if a library member exists
        [NonAction]
        public bool CheckIfMemberExists(int id)
        {
            // Implementation for getting member by ID
            var member = _libraryManagementService.GetMemberByID(id);
            if (member == null)
                return false;

            return true;
        }

        // Checks if a library member is active or inactive
        [NonAction]
        public bool CheckIfMemberActive(int id)
        {
            // Implementation for getting member by ID
            var member = _libraryManagementService.GetMemberByID(id);
            if (member == null)
                return false;

            return member.IsActive;
        }
        #endregion

        #endregion

        #region Borrowers
        [HttpGet]
        [Route("GetAllBorrowers")]
        public ActionResult<List<Borrower>> GetAllBorrowers() => _libraryManagementService.GetAllBorrowers();

        [HttpGet]
        [Route("GetBorrowerById/{id}")]
        public ActionResult<Borrower> GetBorrowerById(int id)
        {
            var borrower = _libraryManagementService.GetBorrowerById(id);

            if (borrower == null)
                return NotFound();
            return borrower;
        }

        [HttpGet]
        [Route("GetMemberBorrowingHistory/{memberId}")]
        public ActionResult<List<Borrower>> GetMemberBorrowingHistory(int memberId)
        {
            if (!CheckIfMemberExists(memberId))
            {
                return NotFound(Constants.MEMBER_NOT_FOUND);
            }

            var borrowings = _libraryManagementService.GetAllBorrowers()
                .Where(b => b.MemberId == memberId)
                .OrderByDescending(b => b.StartDate)
                .ToList();

            return borrowings;
        }

        [HttpGet]
        [Route("GetBookBorrowingHistory/{bookId}")]
        public ActionResult<List<Borrower>> GetBookBorrowingHistory(int bookId)
        {
            var book = _libraryManagementService.GetBookByID(bookId);
            if (book == null)
            {
                return NotFound(Constants.BOOK_ID_NOT_FOUND);
            }

            var borrowings = _libraryManagementService.GetAllBorrowers()
                .Where(b => b.BookId == bookId)
                .OrderByDescending(b => b.StartDate)
                .ToList();

            return borrowings;
        }

        [HttpGet]
        [Route("GetOverdueBooks")]
        public ActionResult<List<Borrower>> GetOverdueBooks()
        {
            var overdueBorrowings = _libraryManagementService.GetAllBorrowers()
                .Where((b => b.ReturnDate == null && b.DueDate < DateTime.Now  || b.IsOverdue))
                .OrderBy(b => b.DueDate)
                .ToList();

            return overdueBorrowings;
        }

        [HttpGet]
        [Route("GetActiveBorrowings")]
        public ActionResult<List<Borrower>> GetActiveBorrowings()
        {
            var activeBorrowings = _libraryManagementService.GetAllBorrowers()
                .Where(b => b.ReturnDate == null)
                .OrderBy(b => b.DueDate)
                .ToList();

            return activeBorrowings;
        }

        // Checks if a library member can borrow books
        [NonAction]
        public string CanBorrowBooks(int memberId)
        {
            string errorMessage = string.Empty;

            if (!CheckIfMemberExists(memberId))
            {
                errorMessage = Constants.MEMBER_NOT_FOUND;
            }
            else if (!CheckIfMemberActive(memberId))
            {
                errorMessage = Constants.MEMBER_INACTIVE;
            }

            return errorMessage;
        }

        // Changed to Borrower object as it is the right way to do it, according to the coding standards
        [HttpPost]
        [Route("BorrowBooks")]
        public IActionResult BorrowBooks(Borrower objBorrower)
        {
            // Can refactor these
            #region Validate Member
            if (!string.IsNullOrEmpty(CanBorrowBooks(objBorrower.MemberId)))
            {
                return BadRequest(CanBorrowBooks(objBorrower.MemberId));
            }

            //Check if book exists
            var book = _libraryManagementService.GetBookByID(objBorrower.BookId);
            if (book == null)
            {
                return NotFound(Constants.BOOK_ID_NOT_FOUND);
            }

            // Prevent the same member from borrowing the same book if they already have an active borrowing
            var existingForMember = _libraryManagementService.GetBorrowerByMemberAndBookID(objBorrower.MemberId, objBorrower.BookId);
            if (existingForMember != null && existingForMember.ReturnDate == null)
            {
                return BadRequest(Constants.MEMBER_ALREADY_BORROWED);
            }

            // Check availability using the service which checks available copies
            else if (!_libraryManagementService.CanBorrowBook(objBorrower.BookId))
            {
                return BadRequest(Constants.BOOK_NOT_AVAILABLE);
            }
            #endregion

            //Insert into Borrowers table
            //Set due date to 14 days from start date
            var borrower = new Borrower
            {
                Id = _libraryManagementService.GetAllBorrowers().ToList().Count == 0 ? 1 : (_libraryManagementService.GetAllBorrowers().ToList().Max(b => b.Id) + 1),
                BookId = objBorrower.BookId,
                MemberId = objBorrower.MemberId,
                StartDate = DateTime.Now,
                DueDate = objBorrower.StartDate.AddDays(14),
                ReturnDate = null,
                // Set default to false. Otherwise, this will be updated as true initially
                IsOverdue = false
            };

            try
            {
                _libraryManagementService.AddBorrower(borrower);
                // Reduce available copies by 1
                book.AvailableCopies -= 1;
                _libraryManagementService.UpdateBook(book);
            }
            catch (Exception)
            {
                return BadRequest(Constants.MEMBER_ALREADY_BORROWED);
            }

            return CreatedAtAction(nameof(GetBorrowerById), new { id = objBorrower.Id }, objBorrower);
        }

        [HttpPut]
        [Route("ReturnBooks/{borrowId}")]
        public IActionResult ReturnBooks(int borrowId)
        {
            // Get the borrow record by ID
            var borrowing = _libraryManagementService.GetBorrowerById(borrowId);
            if (borrowing == null)
            {
                return NotFound("Borrowing record not found.");
            }

            // Check if the book is already returned
            if (borrowing.ReturnDate != null)
            {
                return BadRequest("Book has already been returned.");
            }

            // Update the return date and check if it's overdue
            borrowing.ReturnDate = DateTime.Now;
            borrowing.IsOverdue = borrowing.ReturnDate is not null && (borrowing.ReturnDate > borrowing.DueDate);

            _libraryManagementService.UpdateBorrower(borrowing);

            // Update the book's available copies
            var book = _libraryManagementService.GetBookByID(borrowing.BookId);
            if (book != null)
            {
                book.AvailableCopies += 1;
                _libraryManagementService.UpdateBook(book);
            }

            return NoContent();
        }

        [HttpPut]
        [Route("UpdateBorrowingDueDate/{borrowId}")]
        public IActionResult UpdateBorrowingDueDate(int borrowId, [FromQuery] DateTime newDueDate)
        {
            try
            {
                _libraryManagementService.UpdateBorrowingDueDate(borrowId, newDueDate);
                var updatedBorrowing = _libraryManagementService.GetBorrowerById(borrowId);
                return Ok(updatedBorrowing);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetOverdueStatistics")]
        public ActionResult<OverdueStatistics> GetOverdueStatistics()
        {
            var stats = _libraryManagementService.GetOverdueStatistics();
            return Ok(stats);
        }

        // DELETE action
        [HttpDelete("DeleteBorrower/{id}")]
        public IActionResult DeleteBorrower(int id)
        {
            var borrower = _libraryManagementService.GetBorrowerById(id);

            if (borrower is null)
                return NotFound();

            _libraryManagementService.DeleteBorrower(id);

            return NoContent();
        }

        #endregion
    }
}