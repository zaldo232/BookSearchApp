using System.Diagnostics;
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

    private void OpenUrl_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is Book book && !string.IsNullOrWhiteSpace(book.Url))
        {
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = book.Url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("브라우저를 열 수 없습니다.\n" + ex.Message);
            }
        }
    }

}
