using MyApi.Models;
public class MemoListResponse
{
    public List<Memo> Items { get; set; } = new();
    public MemoSummary Summary { get; set; } = new();
}

public class MemoSummary
{
    public int TotalCount { get; set; }
    public int CompletedCount { get; set; }
}