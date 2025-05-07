using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BookSearchApp.Services;

public class OpenAiService
{
    private readonly string _apiKey = "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";
    private readonly HttpClient _httpClient;

    public OpenAiService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    //요약
    public async Task<string> GetBookSummaryAsync(string title, string description)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
                return "제목이나 설명이 비어 있어 요청할 수 없습니다.";

            var messages = new[]
            {
                new { role = "system", content = "당신은 책 요약 전문가입니다. 사용자에게 책의 내용을 간단하고 매력적으로 설명해 주세요." },
                new { role = "user", content = $"책 제목: {title}\n설명: {description}\n\n이 책에 대해 간단히 요약하고 왜 읽을만한지 알려줘." }
            };

            var payload = new
            {
                model = "gpt-3.5-turbo",
                messages,
                max_tokens = 300,
                temperature = 0.7
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("chat/completions", content);

            Debug.WriteLine("응답 코드: " + response.StatusCode);

            var raw = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("응답 본문: " + raw);

            if (!response.IsSuccessStatusCode)
                return $"AI 응답 실패: {(int)response.StatusCode} {response.ReasonPhrase}";

            using var doc = JsonDocument.Parse(raw);
            var result = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return result ?? "요약 결과 없음.";
        }
        catch (Exception ex)
        {
            return "예외 발생: " + ex.Message;
        }
    }

    // 자연어 검색
    public async Task<string> RefineQueryAsync(string raw)
    {
        var messages = new[]
        {
        new { role = "system", content = "당신은 검색어 추천 도우미입니다. 사용자의 문장을 도서 검색 키워드로 간단하게 바꿔주세요. 불필요한 말은 제거하고 핵심만 남기세요." },
        new { role = "user", content = $"'{raw}' 이 문장을 검색 키워드로 바꿔줘. 예) 자바, 파이썬, 스프링 등처럼 간단하게." }
    };

        var payload = new
        {
            model = "gpt-3.5-turbo",
            messages = messages,
            max_tokens = 20,
            temperature = 0.3
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("chat/completions", content);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? raw;
    }

}
