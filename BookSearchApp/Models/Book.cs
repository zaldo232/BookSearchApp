namespace BookSearchApp.Models;

// 책 정보 모델
public class Book
{
    // 책 제목
    public string Title { get; set; }

    // 저자 목록
    public string[] Authors { get; set; }

    // 출판사
    public string Publisher { get; set; }

    // 썸네일 이미지 URL
    public string Thumbnail { get; set; }

    // 책 소개/내용
    public string Contents { get; set; }

    // 가격
    public int Price { get; set; }

    // 상세 정보 URL
    public string Url { get; set; }
}

// 책 검색 결과 모델
public class BookSearchResult
{
    // 검색 결과 책 리스트
    public List<Book> Documents { get; set; }
}
