using BookSearchApp.Models;
using BookSearchApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// 책 상세 정보 및 요약 ViewModel
public partial class BookDetailViewModel : ObservableObject
{
    private readonly OpenAiService _aiService = new(); // OpenAI 서비스 객체
    private DateTime _lastRequestTime = DateTime.MinValue; // 마지막 요청 시간

    [ObservableProperty]
    private Book book; // 현재 선택된 책

    [ObservableProperty]
    private string summary; // 요약 결과

    [ObservableProperty]
    private bool isLoading; // 로딩 상태 표시

    // 생성자 - 책 정보 초기화
    public BookDetailViewModel(Book book)
    {
        Book = book;
    }

    // 책 요약 생성 커맨드
    [RelayCommand]
    public async Task GenerateSummaryAsync()
    {
        // 5초 이내 중복 요청 방지
        if ((DateTime.Now - _lastRequestTime).TotalSeconds < 5)
        {
            Summary = "요청 간격이 너무 짧습니다. 잠시 후 다시 시도하세요.";
            return;
        }

        _lastRequestTime = DateTime.Now;

        // 필수 정보 누락 시 처리
        if (string.IsNullOrWhiteSpace(Book?.Title) || string.IsNullOrWhiteSpace(Book?.Contents))
        {
            Summary = "책 정보가 충분하지 않습니다.";
            return;
        }

        IsLoading = true;
        Summary = "요약 요청 중..."; // 중간 상태 안내

        try
        {
            var result = await _aiService.GetBookSummaryAsync(Book.Title, Book.Contents);

            // 응답 결과 처리
            Summary = string.IsNullOrWhiteSpace(result) ? "GPT 응답이 비어 있습니다." : result;
        }
        catch (Exception ex)
        {
            // 예외 처리
            Summary = "예외 발생: " + ex.Message;
        }
        IsLoading = false;
    }
}