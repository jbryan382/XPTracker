using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XPTracker.Models;

namespace XPTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelTrackerController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public LevelTrackerController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/LevelTracker
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LevelTrackerModel>>> GetLevelTracker()
        {
            return await _context.LevelTracker.ToListAsync();
        }

        // GET: api/LevelTracker/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LevelTrackerModel>> GetLevelTrackerModel(int id)
        {
            var levelTrackerModel = await _context.LevelTracker.FindAsync(id);

            if (levelTrackerModel == null)
            {
                return NotFound();
            }

            return levelTrackerModel;
        }

        // PUT: api/LevelTracker/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLevelTrackerModel(int id, LevelTrackerModel levelTrackerModel)
        {
            if (id != levelTrackerModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(levelTrackerModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LevelTrackerModelExists(id))
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

        // POST: api/LevelTracker
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<LevelTrackerModel>> PostLevelTrackerModel(LevelTrackerModel levelTrackerModel)
        {
            _context.LevelTracker.Add(levelTrackerModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLevelTrackerModel", new { id = levelTrackerModel.Id }, levelTrackerModel);
        }
        
        [HttpPost("bulk")]
        public async Task<IEnumerable<LevelTrackerModel>> PostLevels(IEnumerable<LevelTrackerModel> levelTracker)
        {
            var levels = new List<LevelTrackerModel>();
            // loop through the view models
            // foreach on, 
            foreach (var m in levelTracker)
            {
                var level = new LevelTrackerModel()
                {
                    Level = m.Level,
                    MinXP = m.MinXP,
                    MaxXP = m.MaxXP

                };
                levels.Add(level);
            }
            _context.LevelTracker.AddRange(levels);
            await _context.SaveChangesAsync();
            return levels;
        }

        // DELETE: api/LevelTracker/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<LevelTrackerModel>> DeleteLevelTrackerModel(int id)
        {
            var levelTrackerModel = await _context.LevelTracker.FindAsync(id);
            if (levelTrackerModel == null)
            {
                return NotFound();
            }

            _context.LevelTracker.Remove(levelTrackerModel);
            await _context.SaveChangesAsync();

            return levelTrackerModel;
        }

        private bool LevelTrackerModelExists(int id)
        {
            return _context.LevelTracker.Any(e => e.Id == id);
        }
    }
}
