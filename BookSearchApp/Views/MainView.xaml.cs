using BookSearchApp.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookSearchApp.Views
{
    // 메인 창(Window)
    public partial class MainView : Window
    {
        // 생성자 - 컴포넌트 초기화
        public MainView()
        {
            InitializeComponent();
        }

        // 즐겨찾기 창 열기 버튼 클릭 이벤트
        private void OpenFavorites_Click(object sender, RoutedEventArgs e)
        {
            var favoritesWindow = new FavoritesView();
            favoritesWindow.ShowDialog();
        }

        // 책 리스트 더블클릭 시 상세창 열기
        private void BookListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is Book selectedBook)
            {
                var detailView = new BookDetailView(selectedBook);
                detailView.ShowDialog();
            }
        }
    }
}
