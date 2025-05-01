using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using angular_azure_demo.Models;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MemoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Memo>>> GetMemos()
        {
            return await _context.Memos.ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Memo>>> GetMemos()
        {
            return await _context.Memos.ToListAsync();
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMemo(int id, Memo memo)
        {
            if (id != memo.Id)
                return BadRequest();

            _context.Entry(memo).State = EntityState.Modified;
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
