using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;
using System.Text.Json;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DatabaseController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("backup")]
        public async Task<IActionResult> BackupDatabase()
        {
            try
            {
                var memos = await _context.Memos.ToListAsync();
                var backupData = new
                {
                    BackupDate = DateTime.UtcNow,
                    Memos = memos
                };

                var jsonString = JsonSerializer.Serialize(backupData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                return File(System.Text.Encoding.UTF8.GetBytes(jsonString), 
                    "application/json", 
                    $"db-backup-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "バックアップの作成に失敗しました", details = ex.Message });
            }
        }

        [HttpPost("restore")]
        public async Task<IActionResult> RestoreDatabase([FromBody] RestoreData restoreData)
        {
            try
            {
                // 既存のデータを全て削除
                _context.Memos.RemoveRange(_context.Memos);
                await _context.SaveChangesAsync();

                // バックアップデータを挿入
                await _context.Memos.AddRangeAsync(restoreData.Memos);
                await _context.SaveChangesAsync();

                return Ok(new { message = "データベースを復元しました", count = restoreData.Memos.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "データベースの復元に失敗しました", details = ex.Message });
            }
        }
    }

    public class RestoreData
    {
        public DateTime BackupDate { get; set; }
        public List<Memo> Memos { get; set; } = new();
    }
} 