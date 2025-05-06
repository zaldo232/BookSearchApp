using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BookSearchApp.Models;

namespace BookSearchApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;

    public List<string> SortOptions { get; } = new()
    {
        "제목순",
        "가격 낮은순",
        "가격 높은순"
    };

    [ObservableProperty]
    private string query;

    [ObservableProperty]
    private ObservableCollection<Book> books = new();

    [ObservableProperty]
    private int page = 1;

    [ObservableProperty]
    private string selectedSortOption = "제목순";

    [ObservableProperty]
    private string publisherFilter = string.Empty;

    private List<Book> allBooks = new();

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

        var url = $"https://dapi.kakao.com/v3/search/book?query={Uri.EscapeDataString(Query)}&page={Page}";
        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BookSearchResult>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            allBooks = result?.Documents ?? new List<Book>();
            ApplySortAndFilter();
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

    [RelayCommand]
    public async Task NextPageAsync()
    {
        if (Page < 50)
        {
            Page++;
            await SearchAsync();
        }
    }

    [RelayCommand]
    public async Task PrevPageAsync()
    {
        if (Page > 1)
        {
            Page--;
            await SearchAsync();
        }
    }

    private void ApplySortAndFilter()
    {
        IEnumerable<Book> filtered = allBooks;

        if (!string.IsNullOrWhiteSpace(PublisherFilter))
            filtered = filtered.Where(b => b.Publisher != null &&
                                           b.Publisher.Contains(PublisherFilter, StringComparison.OrdinalIgnoreCase));

        filtered = SelectedSortOption switch
        {
            "제목순" => filtered.OrderBy(b => b.Title),
            "가격 낮은순" => filtered.OrderBy(b => b.Price),
            "가격 높은순" => filtered.OrderByDescending(b => b.Price),
            _ => filtered
        };

        Books = new ObservableCollection<Book>(filtered);
    }

    partial void OnSelectedSortOptionChanged(string value) => ApplySortAndFilter();
    partial void OnPublisherFilterChanged(string value) => ApplySortAndFilter();
}
