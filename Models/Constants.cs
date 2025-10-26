using System;

namespace SampleLibraryMgmtSystem.Models;

public class Constants
{
    // Can also use Resources in C# for this. The software developer can use this / Resources file when needed instead of hardcoding it.
    public const string BOOK_ID_NOT_FOUND = "Book is not found.";
    public const string BOOK_TITLE_NOT_FOUND = "The title of the book is not found.";
    public const string BOOK_AUTHOR_NOT_FOUND = "The author of the book is not found.";
    public const string NO_BOOKS_BY_THIS_AUTHOR = "There are no books found, by this author.";
    public const string NO_MEMBERS_WITH_THIS_FIRST_NAME = "There are no members found with this first name.";
    public const string NO_MEMBERS_WITH_THIS_SURNAME = "There are no members found with this surname.";
    public const string MEMBER_NOT_FOUND = "Member Not Found";
    public const string MEMBER_INACTIVE = "Member is Inactive";
    public const string BOOK_NOT_AVAILABLE = "There are no more copies left for this book to be borrowed.";
    public const string MEMBER_DID_NOT_BORROW = "Member did not borrow this book.";
    public const string MEMBER_ALREADY_BORROWED = "Member has already borrowed this book.";
    public const string ANOTHER_MEMBER_ALREADY_BORROWED = "Another member has borrowed this book.";
}