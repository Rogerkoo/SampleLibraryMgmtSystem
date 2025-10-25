using Microsoft.AspNetCore.Mvc;
using SampleLibraryMgmtSystem.Models;
using SampleLibraryMgmtSystem.Services;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SampleLibraryMgmtSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LibraryManagementController : ControllerBase
    {
        //protected LibraryManagementService libraryManagementService = new LibraryManagementService();

        public LibraryManagementController()
        {

        }
        // Your action methods go here

        #region Books
        [HttpGet]
        [Route("GetAllBooks")]
        public ActionResult<List<Books>> GetAllBooks() => LibraryManagementService.GetAllBooks();

        [HttpGet]
        [Route("GetBookById/{id}")]
        public ActionResult<Books> GetBookById(int id)
        {
            var book = LibraryManagementService.GetBookByID(id);

            if (book == null)
                return NotFound();
            return book;
        }

        [HttpGet]
        [Route("SearchBookByTitle/{bookTitle}")]
        public ActionResult<Books> SearchBookByTitle([FromQuery]string bookTitle)
        {
            // Implementation for checking book availability
            var book = LibraryManagementService.GetBookByTitle(bookTitle);
            if (book == null)
                return NotFound(Constants.BOOK_TITLE_NOT_FOUND);

            return book;
        }
        
        [HttpGet]
        [Route("SearchBookByAuthor/{authorName}")]
        public ActionResult<Books> SearchBookByAuthor([FromQuery] string authorName)
        {
            // Implementation for checking book availability by author
            var booksByAuthor = LibraryManagementService.GetBookByAuthor(authorName);
            if (booksByAuthor == null)
                return NotFound(Constants.NO_BOOKS_BY_THIS_AUTHOR);

            return booksByAuthor;
        }

        [HttpPost]
        [Route("AddBooks")]
        public IActionResult AddBooks(Books book)
        {
            LibraryManagementService.AddBooks(book);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        [HttpPut("UpdateBookById/{id}")]
        public IActionResult Update(int id, Books book)
        {
            if (id != book.Id)
                return BadRequest();

            var existingBook = LibraryManagementService.GetBookByID(id);
            if (existingBook is null)
                return NotFound();

            LibraryManagementService.UpdateBook(book);

            return NoContent();
        }

        // DELETE action
        [HttpDelete("DeleteBookById/{id}")]
        public IActionResult DeleteBook(int id)
        {
            var book = LibraryManagementService.GetBookByID(id);

            if (book is null)
                return NotFound();

            LibraryManagementService.DeleteBook(id);

            return NoContent();
        }

        #endregion

        #region Members
        [HttpGet]
        [Route("GetAllMembers")]
        public ActionResult<List<Members>> GetAllMembers() => LibraryManagementService.GetAllMembers();

        [HttpGet]
        [Route("GetMemberById/{id}")]
        public ActionResult<Members> GetMemberById(int id)
        {
            var member = LibraryManagementService.GetMemberByID(id);

            if (member == null)
                return NotFound();
            return member;
        }

        [HttpGet]
        [Route("SearchMembersByFirstName/{firstName}")]
        public ActionResult<Members> SearchMembersByFirstName([FromQuery] string firstName)
        {
            // Implementation for checking member availability by first name
            var member = LibraryManagementService.GetMemberByFirstName(firstName);
            if (member == null)
                return NotFound(Constants.NO_MEMBERS_WITH_THIS_FIRST_NAME);

            return member;
        }

        [HttpGet]
        [Route("SearchMembersBySurname/{surname}")]
        public ActionResult<Members> SearchMembersBySurname([FromQuery] string surname)
        {
            // Implementation for checking member availability by surname
            var member = LibraryManagementService.GetMemberBySurname(surname);
            if (member == null)
                return NotFound(Constants.NO_MEMBERS_WITH_THIS_SURNAME);

            return member;
        }

        [HttpPost]
        [Route("AddMembers")]
        public IActionResult AddMembers(Members member)
        {
            LibraryManagementService.AddMembers(member);
            return CreatedAtAction(nameof(GetMemberById), new { id = member.MemberId }, member);
        }

        [HttpPut]
        [Route("UpdateMemberById/{id}")]
        public IActionResult UpdateMemberById(int id, Members member)
        {
            if (id != member.MemberId)
                return BadRequest();

            var existingMember = LibraryManagementService.GetMemberByID(id);
            if (existingMember is null)
                return NotFound();

            LibraryManagementService.UpdateMemberById(member);
            return NoContent();
        }

        // Not in use. We need to use the Microsoft.AspNetCore.Mvc.NewtonsoftJson and Newtonsoft.Json package to support PATCH method
        // Patch updates an attribute of the request. POST updates the entire request.
        [HttpPatch]
        [Route("SetMemberToInactive/{id}")]
        public IActionResult SetMemberToInactive(int id, bool isActive = false)
        {
            var member = LibraryManagementService.GetMemberByID(id);
            if (member == null)
                return NotFound();

            member.IsActive = isActive;
            LibraryManagementService.UpdateMemberById(member);
            return NoContent();
        }

        [HttpDelete]
        [Route("DeleteMembers/{id}")]
        public IActionResult DeleteMembers(int id)
        {
            LibraryManagementService.DeleteMember(id);
            return NoContent();
        }

        #region Member Validation
        // Checks if a library member exists
        [NonAction]
        public bool CheckIfMemberExists(int id)
        {
            // Implementation for getting member by ID
            var member = LibraryManagementService.GetMemberByID(id);
            if (member == null)
                return false;

            return true;
        }

        // Checks if a library member is active or inactive
        [NonAction]
        public bool CheckIfMemberActive(int id)
        {
            // Implementation for getting member by ID
            var member = LibraryManagementService.GetMemberByID(id);
            if (member == null)
                return false;

            return member.IsActive;
        }
        #endregion

        #endregion

        #region Borrowers
        [HttpGet]
        [Route("GetAllBorrowers")]
        public ActionResult<List<Borrower>> GetAllBorrowers() => LibraryManagementService.GetAllBorrowers();

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
            if (!string.IsNullOrEmpty(CanBorrowBooks(objBorrower.MemberId)))
            {
                return BadRequest(CanBorrowBooks(objBorrower.MemberId));
            }

            //Check if book exists
            var book = LibraryManagementService.GetBookByID(objBorrower.BookId);
            if (book == null)
            {
                return NotFound(Constants.BOOK_ID_NOT_FOUND);
            }

            return Ok();
        }
        #endregion
    }
}
