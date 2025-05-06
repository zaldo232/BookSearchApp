using System.Windows;
using BookSearchApp.Models;

namespace BookSearchApp.Views;

public partial class BookDetailView : Window
{
    public BookDetailView(Book book)
    {
        InitializeComponent();
        DataContext = book;
    }
}
