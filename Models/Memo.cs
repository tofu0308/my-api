namespace MyApi.Models
{
    public class Memo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; } // "active", "done", "archived"
    }
}
