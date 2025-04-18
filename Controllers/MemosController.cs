using Microsoft.AspNetCore.Mvc;
using MyApi.Models;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemosController : ControllerBase
    {
        private static List<Memo> memos = new List<Memo>();
        private static int nextId = 1;

        [HttpGet]
        public ActionResult<IEnumerable<Memo>> GetAll()
        {
            return Ok(memos);
        }

        [HttpPost]
        public ActionResult<Memo> Add([FromBody] Memo newMemo)
        {
            newMemo.Id = nextId++;
            memos.Add(newMemo);
            return CreatedAtAction(nameof(GetAll), new { id = newMemo.Id }, newMemo);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var memo = memos.FirstOrDefault(m => m.Id == id);
            if (memo == null) return NotFound();

            memos.Remove(memo);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public ActionResult UpdateStatus(int id, [FromBody] string newStatus)
        {
            var memo = memos.FirstOrDefault(m => m.Id == id);
            if (memo == null) return NotFound();

            memo.Status = newStatus;
            return Ok(memo);
        }
    }
}
