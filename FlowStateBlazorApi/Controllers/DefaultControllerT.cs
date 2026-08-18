using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowStateBlazorApi.Controllers
{
    public class DefaultControllerT<T> : ControllerBase where T : class, IId
    {
        protected readonly FlowStateContext _context;

        public DefaultControllerT(FlowStateContext context)
        {
            _context = context;
        }

        // GET: api/DailyUsageTypeStatuses
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T>>> GetInstancesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        // GET: api/DailyUsageTypeStatuses/5
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetInstanceAsync(int id, CancellationToken cancellationToken = default)
        {
            var instance = await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, cancellationToken).ConfigureAwait(false);
            if (instance == null)
            {
                return NotFound();
            }

            return instance;
        }

        // PUT: api/DailyUsageTypeStatuses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInstanceAsync(int id, T instance, CancellationToken cancellationToken = default)
        {
            if (id != instance.Id)
            {
                return BadRequest();
            }

            _context.Entry(instance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await InstanceExistsAsync(id, cancellationToken).ConfigureAwait(false))
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

        // POST: api/DailyUsageTypeStatuses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<T>> PostInstanceAsync(T instance, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Set<T>().Add(instance);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // https://github.com/aspnet/Announcements/issues/351
            // We have to use GetInstance and not GetInstanceAsync
            return CreatedAtAction("GetInstance", new { id = instance.Id }, instance);
        }

        // DELETE: api/DailyUsageTypeStatuses/5
        //[Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstanceAsync(int id, CancellationToken cancellationToken = default)
        {
            var instance = await _context.Set<T>().FirstOrDefaultAsync(o => o.Id == id, cancellationToken).ConfigureAwait(false);
            if (instance == null)
            {
                return NotFound();
            }

            _context.Set<T>().Remove(instance);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        protected Task<bool> InstanceExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return _context.Set<T>().AsNoTracking().AnyAsync(e => e.Id == id, cancellationToken);
        }

    }
}
