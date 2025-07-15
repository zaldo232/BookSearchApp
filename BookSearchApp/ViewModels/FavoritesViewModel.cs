using BookSearchApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BookSearchApp.ViewModels;

// 즐겨찾기 관리 ViewModel
public partial class FavoritesViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Book> favorites; // 즐겨찾기 목록

    // 생성자 - 저장된 즐겨찾기 불러오기
    public FavoritesViewModel()
    {
        Favorites = new ObservableCollection<Book>(BookStorage.LoadFavorites());
    }

    // 즐겨찾기 항목 삭제 커맨드
    [RelayCommand]
    public void RemoveFavorite(Book book)
    {
        if (book == null) return; // Null 체크
        Favorites.Remove(book); // 목록에서 삭제
        BookStorage.SaveFavorites(Favorites.ToList()); // 변경된 목록 저장
    }

    // 즐겨찾기 전체 내보내기(CSV)
    [RelayCommand]
    public void ExportFavorites()
    {
        BookStorage.ExportToCsv(Favorites.ToList());
    }
}
