using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Riddles.Data;
using Riddles.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Riddles.Controllers
{
    public class RiddlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RiddlesController> _logger;

        public RiddlesController(ApplicationDbContext context, ILogger<RiddlesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Riddles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Riddle.ToListAsync());
        }

        // GET: Riddles/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var riddle = await _context.Riddle
                .FirstOrDefaultAsync(m => m.ID == id);
            if (riddle == null)
            {
                return NotFound();
            }

            return View(riddle);
        }

        [Authorize]
        [HttpPost]
        public async Task<object> Reveal([Bind("ID")] int id)
        {
            var riddle = await _context.Riddle.FindAsync(id);

            if (riddle == null) return NotFound();

            var status = GetSolvingStatusObject(id);

            if (status == null)
            {
                status = new SolvingStatus() { User = CurrentUser, Riddle = riddle, Status = UserRiddleStatus.Surrendered };
                await _context.SolvingStatuses.AddAsync(status);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id });
            }
            
            if(status.Status == UserRiddleStatus.None)
            {
                status.Status = UserRiddleStatus.Surrendered;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id });
            }

            return "Not eligible for a reveal!";
        }

        private User CurrentUser
        {
            get
            {
                string idStr = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int id = int.Parse(idStr);

                return _context.Users.Find(id);
            }
        }

        [NonAction]
        private SolvingStatus GetSolvingStatusObject(int riddleID)
        {
            return CurrentUser.Solved.FirstOrDefault(ss => ss.RiddleID == riddleID);
        }

        [NonAction]
        private UserRiddleStatus GetStatusOfRiddle(int riddleID)
        {
            return GetSolvingStatusObject(riddleID)?.Status ?? UserRiddleStatus.None;
        }

        [Authorize]
        // GET: Riddles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Riddles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<object> Create([Bind("ID,Question,Answer,ComplimentForWinning,InsultForLosing")] Riddle riddle)
        {
            riddle.Author = CurrentUser;
            //riddle.AuthorID = riddle.Author.Id;

            ModelState.Remove("Author");

            if (ModelState.IsValid)
            {
                _context.Add(riddle);

                var authorStatus = new SolvingStatus() { Riddle = riddle, User = CurrentUser, Status = UserRiddleStatus.Created };

                _context.Add(authorStatus);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return riddle;
        }

        [Authorize]
        public async Task<object> Solve(int? id)
        {
            if (id == null) return NotFound();

            var riddle = await _context.Riddle.FindAsync(id);

            if (riddle == null) return NotFound();

            if(GetStatusOfRiddle(id.Value) == UserRiddleStatus.None) return View(riddle);

            return "Not eligible for solving!";
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<object> Solve([Bind("ID")]int id, [Bind("Answer")] string answer)
        {
            if (!RiddleExists(id)) return NotFound();
            if (GetStatusOfRiddle(id) != UserRiddleStatus.None) return "Not eligible for solving!";

            var riddle = await _context.Riddle.FindAsync(id);

            bool result = riddle.Answer == answer;
            var newStatus = result ? UserRiddleStatus.Solved : UserRiddleStatus.None;

            var status = GetSolvingStatusObject(id);

            if (status == null)
            {
                status = new SolvingStatus() { User = CurrentUser, Riddle = riddle, Status = newStatus };
                await _context.SolvingStatuses.AddAsync(status);
            }

            status.Status = newStatus;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        // GET: Riddles/Edit/5
        public async Task<object> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var riddle = await _context.Riddle.FindAsync(id);
            if (riddle == null) return NotFound();

            return riddle.AuthorID == CurrentUser.Id ? (object)View(riddle) : "Only author can edit a riddle!";
        }

        // POST: Riddles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<object> Edit(int id, [Bind("ID,AuthorID,Question,Answer,ComplimentForWinning,InsultForLosing")] Riddle riddle)
        {
            if (id != riddle.ID) return NotFound();
            
            if(riddle.AuthorID != CurrentUser.Id) return $"Author {CurrentUser.Id} cant edit riddle of {riddle.AuthorID}";

            ModelState.Remove("Author");
            riddle.Author = await _context.Users.FindAsync(riddle.AuthorID);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(riddle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RiddleExists(riddle.ID))
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
            return riddle;
        }

        // GET: Riddles/Delete/5
        [Authorize]
        public async Task<object> Delete(int? id)
        {
            if (id == null) return NotFound();

            var riddle = await _context.Riddle.FirstOrDefaultAsync(m => m.ID == id);
            if (riddle == null) return NotFound();

            return riddle.AuthorID == CurrentUser.Id ? (object)View(riddle) : "Only author can delete a riddle!";
        }

        // POST: Riddles/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<object> DeleteConfirmed(int id)
        {
            var riddle = await _context.Riddle.FindAsync(id); 
            
            if (riddle.AuthorID != CurrentUser.Id) return "Only author can delete a riddle!";

            var statuses = _context.SolvingStatuses.Where(ss => ss.RiddleID == id);

            _context.SolvingStatuses.RemoveRange(statuses);

            _context.Riddle.Remove(riddle);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        [NonAction]
        private bool RiddleExists(int id)
        {
            return _context.Riddle.Any(e => e.ID == id);
        }
    }
}
