using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using MyApi.Data;
using angular_azure_demo.Models;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MemosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<MemoListResponse>> GetMemos()
        {
            var memos = await _context.Memos.ToListAsync();

            var summary = new MemoSummary
            {
                TotalCount = memos.Count,
                CompletedCount = memos.Count(m => m.Status == MemoStatus.Completed)
            };

            var response = new MemoListResponse
            {
                Items = memos,
                Summary = summary
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Memo>> GetMemo(int id)
        {
            var memo = await _context.Memos.FindAsync(id);
            return memo == null ? NotFound() : Ok(memo);
        }

        [HttpPost]
        public async Task<ActionResult<Memo>> PostMemo(Memo memo)
        {
            _context.Memos.Add(memo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMemo), new { id = memo.Id }, memo);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateMemoStatus(int id, [FromBody] MemoStatusUpdateRequest request)

        {
            var memo = await _context.Memos.FindAsync(id);
            if (memo == null) return NotFound();

            memo.Status = request.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMemo(int id)
        {
            var memo = await _context.Memos.FindAsync(id);
            if (memo == null)
                return NotFound();

            _context.Memos.Remove(memo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
