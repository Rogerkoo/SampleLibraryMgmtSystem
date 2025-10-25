using System.Text.Json.Serialization;

namespace SampleLibraryMgmtSystem.Models;

public class Books
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public string ISBN { get; set; }
    public DateTime DatePublished { get; set; }
    public int AvailableCopies { get; set; }

    public Books(string title, string author, string genre, string isbn, DateTime datePublished, int availableCopies)
    {
        Title = title;
        Author = author;
        Genre = genre;
        ISBN = isbn;
        DatePublished = datePublished;
        AvailableCopies = availableCopies;
    }

    [JsonConstructor]
    public Books(int id, string title, string author, string genre, string isbn, DateTime datePublished, int availableCopies)
    {
        Id = id;
        Title = title;
        Author = author;
        Genre = genre;
        ISBN = isbn;
        DatePublished = datePublished;
        AvailableCopies = availableCopies;
    }
}