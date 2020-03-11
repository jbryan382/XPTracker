using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XPTracker.Models;

// https://discordapp.com/api/oauth2/authorize?client_id=682375344850075658&permissions=536872960&scope=bot
// GET: /gateway/bot

namespace XPTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionTrackerController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public SessionTrackerController(DatabaseContext context)
        {
            _context = context;
        }

        // [HttpPost("ping")]
        // public async Task<ActionResult> PingAsync()
        // {
        //     return Ok(new { text = "Pinged at" + DateTime.Now });
        // }

        // GET: api/SessionTracker
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SessionTrackerModel>>> GetSessionTracker()
        {
            return await _context.SessionTracker.ToListAsync();
        }

        // GET: api/SessionTracker/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SessionTrackerModel>> GetSessionTrackerModel(int id)
        {
            var sessionTrackerModel = await _context.SessionTracker.FindAsync(id);

            if (sessionTrackerModel == null)
            {
                return NotFound();
            }

            return sessionTrackerModel;
        }

        // PUT: api/SessionTracker/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSessionTrackerModel(int id, SessionTrackerModel sessionTrackerModel)
        {
            if (id != sessionTrackerModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(sessionTrackerModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SessionTrackerModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SessionTracker
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<SessionTrackerModel>> PostSessionTrackerModel(SessionTrackerModel sessionTrackerModel)
        {
            _context.SessionTracker.Add(sessionTrackerModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSessionTrackerModel", new { id = sessionTrackerModel.Id }, sessionTrackerModel);
        }

        // DELETE: api/SessionTracker/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SessionTrackerModel>> DeleteSessionTrackerModel(int id)
        {
            var sessionTrackerModel = await _context.SessionTracker.FindAsync(id);
            if (sessionTrackerModel == null)
            {
                return NotFound();
            }

            _context.SessionTracker.Remove(sessionTrackerModel);
            await _context.SaveChangesAsync();

            return sessionTrackerModel;
        }

        private bool SessionTrackerModelExists(int id)
        {
            return _context.SessionTracker.Any(e => e.Id == id);
        }
    }
}
