using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BookSearchApp.Models;
using BookSearchApp.Services;

public partial class BookDetailViewModel : ObservableObject
{
    private readonly OpenAiService _aiService = new();
    private DateTime _lastRequestTime = DateTime.MinValue;

    [ObservableProperty]
    private Book book;

    [ObservableProperty]
    private string summary;

    [ObservableProperty]
    private bool isLoading;

    public BookDetailViewModel(Book book)
    {
        Book = book;
    }

    [RelayCommand]
    public async Task GenerateSummaryAsync()
    {
        if ((DateTime.Now - _lastRequestTime).TotalSeconds < 5)
        {
            Summary = "요청 간격이 너무 짧습니다. 잠시 후 다시 시도하세요.";
            return;
        }

        _lastRequestTime = DateTime.Now;

        if (string.IsNullOrWhiteSpace(Book?.Title) || string.IsNullOrWhiteSpace(Book?.Contents))
        {
            Summary = "책 정보가 충분하지 않습니다.";
            return;
        }

        IsLoading = true;
        Summary = "요약 요청 중..."; // 중간 상태 확인용

        try
        {
            var result = await _aiService.GetBookSummaryAsync(Book.Title, Book.Contents);

            // 응답 결과 출력
            Summary = string.IsNullOrWhiteSpace(result)
                ? "GPT 응답이 비어 있습니다."
                : result;
        }
        catch (Exception ex)
        {
            Summary = "예외 발생: " + ex.Message;
        }

        IsLoading = false;
    }

}
