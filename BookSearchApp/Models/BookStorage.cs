using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Text.Json;

namespace BookSearchApp.Models;

// 즐겨찾기 책 저장/불러오기/내보내기 기능 클래스
public static class BookStorage
{
    private static readonly string FilePath = "favorites.json"; // 즐겨찾기 저장 파일 경로

    // 즐겨찾기 리스트 저장
    public static void SaveFavorites(List<Book> favorites)
    {
        var json = JsonSerializer.Serialize(favorites, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    // 즐겨찾기 리스트 불러오기
    public static List<Book> LoadFavorites()
    {
        // 파일 없으면 빈 리스트 반환
        if (!File.Exists(FilePath)) return new List<Book>();

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<Book>();
    }

    // 즐겨찾기 리스트를 CSV 파일로 내보내기
    public static void ExportToCsv(List<Book> favorites)
    {
        var dialog = new SaveFileDialog
        {
            FileName = "favorites.csv",
            Filter = "CSV files (*.csv)|*.csv"
        };

        // 파일 저장 다이얼로그에서 저장 선택 시
        if (dialog.ShowDialog() == true)
        {
            var sb = new StringBuilder();
            sb.AppendLine("제목,저자,출판사,가격");

            // 즐겨찾기 책 목록 한 줄씩 기록
            foreach (var book in favorites)
            {
                var authors = string.Join(" / ", book.Authors);
                var line = $"{Escape(book.Title)},{Escape(authors)},{Escape(book.Publisher)},{book.Price}";
                sb.AppendLine(line);
            }

            File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);
        }
    }

    // CSV 특수문자 이스케이프 처리
    private static string Escape(string text)
    {
        return "\"" + text.Replace("\"", "\"\"") + "\"";
    }
}
