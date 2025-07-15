using BookSearchApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BookSearchApp.ViewModels;

// 메인 검색/정렬/필터 ViewModel
public partial class MainViewModel : ObservableObject
{
    private readonly HttpClient _httpClient; // API 호출용 HTTP 클라이언트

    // 정렬 옵션 목록
    public List<string> SortOptions { get; } = new()
    {
        "제목순",
        "가격 낮은순",
        "가격 높은순"
    };

    [ObservableProperty]
    private string query; // 검색어

    [ObservableProperty]
    private ObservableCollection<Book> books = new(); // 화면에 표시될 책 목록

    [ObservableProperty]
    private int page = 1; // 현재 페이지

    [ObservableProperty]
    private string selectedSortOption = "제목순"; // 선택된 정렬 옵션

    [ObservableProperty]
    private string publisherFilter = string.Empty; // 출판사 필터

    private List<Book> allBooks = new(); // 전체 검색 결과(필터/정렬 전)

    // 생성자 - HttpClient 및 인증 헤더 설정
    public MainViewModel()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("KakaoAK", "5200b4fa592d9f490f8b2be6f3501ab4");
    }

    // 책 검색 커맨드
    [RelayCommand]
    public async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(Query)) return; // 빈 검색어 예외 처리

        var url = $"https://dapi.kakao.com/v3/search/book?query={Uri.EscapeDataString(Query)}&page={Page}";
        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BookSearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            allBooks = result?.Documents ?? new List<Book>();
            ApplySortAndFilter(); // 정렬 및 필터 적용
        }
    }

    // 즐겨찾기 추가 커맨드
    [RelayCommand]
    public void AddToFavorites(Book selectedBook)
    {
        if (selectedBook == null) return; // null 체크

        var favorites = BookStorage.LoadFavorites();
        // 중복 여부 확인
        if (!favorites.Any(b => b.Title == selectedBook.Title && b.Publisher == selectedBook.Publisher))
        {
            favorites.Add(selectedBook);
            BookStorage.SaveFavorites(favorites);
        }
    }

    // 다음 페이지로 이동
    [RelayCommand]
    public async Task NextPageAsync()
    {
        if (Page < 50)
        {
            Page++;
            await SearchAsync();
        }
    }

    // 이전 페이지로 이동
    [RelayCommand]
    public async Task PrevPageAsync()
    {
        if (Page > 1)
        {
            Page--;
            await SearchAsync();
        }
    }

    // 정렬/필터 적용 로직
    private void ApplySortAndFilter()
    {
        IEnumerable<Book> filtered = allBooks;

        // 출판사 필터 적용
        if (!string.IsNullOrWhiteSpace(PublisherFilter))
        { 
            filtered = filtered.Where(b => b.Publisher != null && b.Publisher.Contains(PublisherFilter, StringComparison.OrdinalIgnoreCase)); 
        }

        // 정렬 옵션별 정렬
        filtered = SelectedSortOption switch
        {
            "제목순" => filtered.OrderBy(b => b.Title),
            "가격 낮은순" => filtered.OrderBy(b => b.Price),
            "가격 높은순" => filtered.OrderByDescending(b => b.Price),
            _ => filtered
        };

        Books = new ObservableCollection<Book>(filtered);
    }

    // 정렬 옵션 변경 시 자동 적용
    partial void OnSelectedSortOptionChanged(string value) => ApplySortAndFilter();

    // 출판사 필터 변경 시 자동 적용
    partial void OnPublisherFilterChanged(string value) => ApplySortAndFilter();
}
