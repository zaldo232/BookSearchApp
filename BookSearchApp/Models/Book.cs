namespace BookSearchApp.Models;

public class Book
{
    public string Title { get; set; }
    public string[] Authors { get; set; }
    public string Publisher { get; set; }
    public string Thumbnail { get; set; }
    public string Contents { get; set; }
    public int Price { get; set; }
    public string Url { get; set; }

}

public class BookSearchResult
{
    public List<Book> Documents { get; set; }
}
