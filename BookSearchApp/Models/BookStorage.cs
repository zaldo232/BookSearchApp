using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace BookSearchApp.Models;

public static class BookStorage
{
    private static readonly string FilePath = "favorites.json";

    public static void SaveFavorites(List<Book> favorites)
    {
        var json = JsonSerializer.Serialize(favorites, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    public static List<Book> LoadFavorites()
    {
        if (!File.Exists(FilePath)) return new List<Book>();

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<Book>();
    }

    public static void ExportToCsv(List<Book> favorites)
    {
        var dialog = new SaveFileDialog
        {
            FileName = "favorites.csv",
            Filter = "CSV files (*.csv)|*.csv"
        };

        if (dialog.ShowDialog() == true)
        {
            var sb = new StringBuilder();
            sb.AppendLine("제목,저자,출판사,가격");

            foreach (var book in favorites)
            {
                var authors = string.Join(" / ", book.Authors);
                var line = $"{Escape(book.Title)},{Escape(authors)},{Escape(book.Publisher)},{book.Price}";
                sb.AppendLine(line);
            }

            File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);
        }
    }

    private static string Escape(string text)
    {
        return "\"" + text.Replace("\"", "\"\"") + "\"";
    }
}
