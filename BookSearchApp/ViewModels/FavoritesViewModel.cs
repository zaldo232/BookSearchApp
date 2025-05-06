using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BookSearchApp.Models;

namespace BookSearchApp.ViewModels;

public partial class FavoritesViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Book> favorites;

    public FavoritesViewModel()
    {
        Favorites = new ObservableCollection<Book>(BookStorage.LoadFavorites());
    }

    [RelayCommand]
    public void RemoveFavorite(Book book)
    {
        if (book == null) return;
        Favorites.Remove(book);
        BookStorage.SaveFavorites(Favorites.ToList());
    }

    [RelayCommand]
    public void ExportFavorites()
    {
        BookStorage.ExportToCsv(Favorites.ToList());
    }
}
