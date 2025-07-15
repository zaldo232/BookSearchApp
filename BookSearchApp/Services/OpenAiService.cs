using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BookSearchApp.Services;

// OpenAI API를 활용한 책 요약 및 검색어 보정 서비스
public class OpenAiService
{
    private readonly string _apiKey = "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";
    private readonly HttpClient _httpClient; // HTTP 통신 객체

    // 생성자 - HttpClient 설정
    public OpenAiService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    // 책 요약 요청
    public async Task<string> GetBookSummaryAsync(string title, string description)
    {
        try
        {
            // 입력값 비어있을 때 예외 처리
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
                return "제목이나 설명이 비어 있어 요청할 수 없습니다.";

            // 프롬프트 메시지 준비
            var messages = new[]
            {
                new { role = "system", content = "당신은 책 요약 전문가입니다. 사용자에게 책의 내용을 간단하고 매력적으로 설명해 주세요." },
                new { role = "user", content = $"책 제목: {title}\n설명: {description}\n\n이 책에 대해 간단히 요약하고 왜 읽을만한지 알려줘." }
            };

            // 요청 페이로드 생성
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

            // 실패 응답 처리
            if (!response.IsSuccessStatusCode)
                return $"AI 응답 실패: {(int)response.StatusCode} {response.ReasonPhrase}";

            // 결과 파싱
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
            // 예외 발생 시 메시지 반환
            return "예외 발생: " + ex.Message;
        }
    }

    // 검색어 간소화(자연어 → 검색 키워드)
    public async Task<string> RefineQueryAsync(string raw)
    {
        // 프롬프트 메시지 준비
        var messages = new[]
        {
            new { role = "system", content = "당신은 검색어 추천 도우미입니다. 사용자의 문장을 도서 검색 키워드로 간단하게 바꿔주세요. 불필요한 말은 제거하고 핵심만 남기세요." },
            new { role = "user", content = $"'{raw}' 이 문장을 검색 키워드로 바꿔줘. 예) 자바, 파이썬, 스프링 등처럼 간단하게." }
        };

        // 요청 페이로드 생성
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
