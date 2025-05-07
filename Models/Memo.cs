namespace MyApi.Models
{
    public enum MemoStatus
    {
        Open = 0,
        ToDo = 1,
        InProgress = 2,
        Completed = 3,
        Close = 4,
        ReOpen = 5,
    }
    public class Memo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public MemoStatus Status { get; set; }
    }
    public class MemoStatusUpdateRequest
    {
        public MemoStatus Status { get; set; }
    }
}