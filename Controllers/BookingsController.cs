using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CldvPOEnew.Models;



namespace CldvPOEnew.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public BookingsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Bookings
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDBContext = _context.Booking.Include(b => b.Event).Include(b => b.Venue);
        //    return View(await applicationDBContext.ToListAsync());
        //}
        //public async Task<IActionResult> Index(string searchString)
        //{
        //    var bookings = _context.Booking
        //        .Include(b => b.Event)
        //        .Include(b => b.Venue)
        //        .AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        bookings = bookings.Where(b =>
        //            b.BookingId.ToString().Contains(searchString) ||
        //            b.Event.EventName.Contains(searchString));
        //    }

        //    return View(await bookings.ToListAsync());
        //}

        public async Task<IActionResult> Index(string searchString, int? eventTypeId, DateTime? startDate, DateTime? endDate, bool? isAvailable)
{
    var bookings = _context.Booking
        .Include(b => b.Event)
            .ThenInclude(e => e.EventType)
        .Include(b => b.Venue)
        .AsQueryable();

    // Keyword search
    if (!string.IsNullOrEmpty(searchString))
    {
        bookings = bookings.Where(b =>
            b.BookingId.ToString().Contains(searchString) ||
            b.Event.EventName.Contains(searchString));
    }

    // Filter by EventType
    if (eventTypeId.HasValue)
    {
        bookings = bookings.Where(b => b.Event.EventTypeId == eventTypeId.Value);
    }

    // Filter by Date Range
    if (startDate.HasValue)
    {
        bookings = bookings.Where(b => b.BookingDate >= startDate.Value);
    }

    if (endDate.HasValue)
    {
        bookings = bookings.Where(b => b.BookingDate <= endDate.Value);
    }

    // Filter by Venue Availability
    if (isAvailable.HasValue)
    {
        bookings = bookings.Where(b => b.Venue.IsAvailable == isAvailable.Value);
    }

    // Populate dropdown filters
    ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name");
    return View(await bookings.ToListAsync());
}


        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventId");
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueId");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
    }
}
