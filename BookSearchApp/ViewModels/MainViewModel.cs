using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BookSearchApp.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookSearchApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;

    [ObservableProperty]
    private string query;

    [ObservableProperty]
    private ObservableCollection<Book> books = new();

    public MainViewModel()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("KakaoAK", "5200b4fa592d9f490f8b2be6f3501ab4");
    }

    [RelayCommand]
    public async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(Query)) return;

        var url = $"https://dapi.kakao.com/v3/search/book?query={Uri.EscapeDataString(Query)}";
        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BookSearchResult>(json,
            new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            // null 체크 추가
            if (result?.Documents != null)
            {
                Books = new ObservableCollection<Book>(result.Documents);
            }
            else
            {
                Books = new ObservableCollection<Book>();
            }
        }
    }

    [RelayCommand]
    public void AddToFavorites(Book selectedBook)
    {
        if (selectedBook == null) return;

        var favorites = BookStorage.LoadFavorites();
        if (!favorites.Any(b => b.Title == selectedBook.Title && b.Publisher == selectedBook.Publisher))
        {
            favorites.Add(selectedBook);
            BookStorage.SaveFavorites(favorites);
        }
    }

}
