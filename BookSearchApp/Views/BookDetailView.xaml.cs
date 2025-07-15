using BookSearchApp.Models;
using System.Windows;

namespace BookSearchApp.Views;

// 책 상세 정보 뷰(Window)
public partial class BookDetailView : Window
{
    // 생성자 - ViewModel 바인딩
    public BookDetailView(Book book)
    {
        InitializeComponent();
        DataContext = new BookDetailViewModel(book);
    }

    // 웹 페이지 열기 버튼 클릭 이벤트
    private void OpenUrl_Click(object sender, RoutedEventArgs e)
    {
        // URL 존재 여부 확인
        if (DataContext is BookDetailViewModel vm && !string.IsNullOrWhiteSpace(vm.Book.Url))
        {
            // 기본 브라우저로 URL 열기
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = vm.Book.Url,
                UseShellExecute = true
            });
        }
    }
}
