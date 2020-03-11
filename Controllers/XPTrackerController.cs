using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XPTracker.Models;

namespace XPTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XPTrackerController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public XPTrackerController(DatabaseContext context)
        {
            _context = context;
        }
        
        // GET: api/XPTracker
        [HttpGet]
        public async Task<ActionResult<IEnumerable<XPTrackerModel>>> GetXPTracker()
        {
            return await _context.XPTracker.ToListAsync();
        }

        // GET: api/XPTracker/5
        [HttpGet("{id}")]
        public async Task<ActionResult<XPTrackerModel>> GetXPTrackerModel(int id)
        {
            var xPTrackerModel = await _context.XPTracker.FindAsync(id);

            if (xPTrackerModel == null)
            {
                return NotFound();
            }

            return xPTrackerModel;
        }

        // PUT: api/XPTracker/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutXPTrackerModel(int id, XPTrackerModel xPTrackerModel)
        {
            if (id != xPTrackerModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(xPTrackerModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!XPTrackerModelExists(id))
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

        // POST: api/XPTracker
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<XPTrackerModel>> PostXPTrackerModel(XPTrackerModel xPTrackerModel)
        {
            _context.XPTracker.Add(xPTrackerModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetXPTrackerModel", new { id = xPTrackerModel.Id }, xPTrackerModel);
        }

        // DELETE: api/XPTracker/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<XPTrackerModel>> DeleteXPTrackerModel(int id)
        {
            var xPTrackerModel = await _context.XPTracker.FindAsync(id);
            if (xPTrackerModel == null)
            {
                return NotFound();
            }

            _context.XPTracker.Remove(xPTrackerModel);
            await _context.SaveChangesAsync();

            return xPTrackerModel;
        }

        private bool XPTrackerModelExists(int id)
        {
            return _context.XPTracker.Any(e => e.Id == id);
        }
    }
}
