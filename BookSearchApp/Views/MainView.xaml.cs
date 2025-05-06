using BookSearchApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BookSearchApp.Views
{
    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void OpenFavorites_Click(object sender, RoutedEventArgs e)
        {
            var favoritesWindow = new FavoritesView();
            favoritesWindow.ShowDialog();
        }

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
