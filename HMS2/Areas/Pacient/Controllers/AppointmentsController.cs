using HMS2.Data;
using HMS2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HMS2.Areas.Pacient.Controllers
{
    [Area("Pacient")]
    [Authorize(Roles = "Patient")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public string CurrentUserId
        {
            get
            {
                return User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Appointments
        public async Task<IActionResult> Index(
        string sortOrder,
        string currentFilter,
        string searchString,
        string ConfirmationStatus,
        int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CreatedSortParm"] = sortOrder == "created_asc" ? "created_desc" : "created_asc";
            ViewData["AppoDateSortParm"] = sortOrder == "appodate_asc" ? "appodate_desc" : "appodate_asc";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            var appointments = from s in _context.Appointments.Include(a => a.Author).Include(a => a.Doctor).Where(a => a.AuthorId == CurrentUserId)
                               select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                appointments = appointments.Where(s => s.CreatedBy.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(ConfirmationStatus))
            {
                Boolean var;
                if (ConfirmationStatus == "1")
                {
                    var = true;
                }
                else
                {
                    var = false;
                }
                appointments = appointments.Where(s => s.ConfirmationStatus == var);
            }

            switch (sortOrder)
            {
                case "created_asc":
                    appointments = appointments.OrderBy(s => s.CreatedBy);
                    break;
                case "created_desc":
                    appointments = appointments.OrderByDescending(s => s.CreatedBy);
                    break;
                case "appodate_asc":
                    appointments = appointments.OrderBy(s => s.DateOfAppointment);
                    break;
                case "appodate_desc":
                    appointments = appointments.OrderByDescending(s => s.DateOfAppointment);
                    break;
            }

            //var applicationDbContext = _context.Appointments.Include(a => a.Author).Include(a => a.Doctor);
            int pageSize = 25;
            return View(await PaginatedList<Appointment>.CreateAsync(appointments.AsTracking(), pageNumber ?? 1, pageSize));

            //var applicationDbContext = _context.Appointments.Include(a => a.Author).Include(a => a.Doctor).Where(a => a.AuthorId == CurrentUserId);
            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Author)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DoctorId,DateOfAppointment,ConfirmationStatus,AuthorId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.AuthorId = CurrentUserId;
                appointment.ConfirmationStatus = false;
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Users, "Id", "Email", appointment.DoctorId);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Users, "Id", "Email", appointment.DoctorId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DoctorId,DateOfAppointment,ConfirmationStatus,AuthorId,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    appointment.AuthorId = CurrentUserId;
                    appointment.ConfirmationStatus = false;
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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
            ViewData["DoctorId"] = new SelectList(_context.Users, "Id", "Email", appointment.DoctorId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Author)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
