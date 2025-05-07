using System.Diagnostics;
using System.Windows;
using BookSearchApp.Models;

namespace BookSearchApp.Views;

public partial class BookDetailView : Window
{
    public BookDetailView(Book book)
    {
        InitializeComponent();
        DataContext = new BookDetailViewModel(book);
    }

    private void OpenUrl_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is BookDetailViewModel vm && !string.IsNullOrWhiteSpace(vm.Book.Url))
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = vm.Book.Url,
                UseShellExecute = true
            });
        }
    }
}