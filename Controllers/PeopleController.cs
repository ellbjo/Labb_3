using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Labb_3.Data;
using Labb_3.Models;
using System.Globalization;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Labb_3.Controllers
{
    public class PeopleController : Controller
    {
        private readonly InitialDbContext _context;

        public PeopleController(InitialDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/persons")]
        public async Task<IActionResult> GetPersons()
        {
            var persons = await _context.Persons.ToListAsync();
            var personListDto = new PersonListDto();
            foreach (var person in persons)
            {
                personListDto.PersonList.Add(new PersonDto
                { 
                    Name = person.Name,
                    PhoneNumber = person.PhoneNumber,
                }
                );
            }

            return Ok(personListDto);
        }

        [HttpGet]
        [Route("api/getInterestsByPersonId/{id}")]
        public async Task<IActionResult> GetInterestsOnPersonId(int id)
        {
            var person = await _context.Persons.Where(x => x.Id == id)
                .Include(x => x.PersonInterests)
                .ThenInclude(x => x.Interest).FirstOrDefaultAsync();
           
            if(person == null)
            {
                return NotFound();

            }
            var personList = new InterestListDto();
            foreach (var item in person.PersonInterests)
            {
                personList.List.Add(new InterestDto
                {
                    Title = item.Interest?.Title,
                    Description = item.Interest?.Description
                });
            }
            return Ok(personList);
        }


        [HttpGet]
        [Route("api/getLinksByPersonId/{id}")]
        public async Task<IActionResult> GetLinksOnPersonId(int id)
        {
            var person = await _context.Persons.Where(x => x.Id == id)
                .Include(x => x.PersonInterests)
                .ThenInclude(x => x.Interest)
                .ThenInclude(x => x.Links).FirstOrDefaultAsync();

            if (person == null)
            {
                return NotFound("Person was not found");

            }
            var linkList = new List<string>();
            foreach (var item in person.PersonInterests)
            {
                foreach(var link in item.Interest.Links)
                {
                    linkList.Add(link.Link);
                }
            }
            return Ok(linkList);
        }


        [HttpPost]
        [Route("api/addNewInterestOnPerson")]
        public async Task<IActionResult> AddInterestOnPerson(AddInterestToPersonDTO addInterestToPersonDTO)
        {
            var person = await _context.Persons.Include(p => p.PersonInterests).FirstOrDefaultAsync(person => person.Id == addInterestToPersonDTO.PersonId);

            if (person == null)
            {
                return NotFound("Person not found");

            }


            var interestExist = await _context.Interests.Where(x => x.Title == addInterestToPersonDTO.InterestTitle).FirstOrDefaultAsync();

            if(interestExist == null)
            {
                var newInterest = new Interest
                {
                    Title = addInterestToPersonDTO.InterestTitle,
                    Description = addInterestToPersonDTO.InterestDesciption
                };
                _context.Add(newInterest);
                await _context.SaveChangesAsync();

                var newInterestInDB = await _context.Interests.Where(x => x.Title == addInterestToPersonDTO.InterestTitle).FirstAsync();
                person.PersonInterests.Add(new PersonInterest { PersonId = person.Id, InterestId = newInterestInDB.Id });
            }
            else
            {
                person.PersonInterests.Add(new PersonInterest { PersonId = person.Id, InterestId = interestExist.Id });
               
            }
            try
            {
                await _context.SaveChangesAsync();
                return Ok("Interest added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        // GET: People
        public async Task<IActionResult> Index()
        {
              return _context.Persons != null ? 
                View(await _context.Persons.Include(p => p.PersonInterests).ThenInclude(pi => pi.Interest).ToListAsync()) :
                Problem("Entity set 'InitialDbContext.Persons' is null.");
        }

        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Persons == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,PhoneNumber")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Persons == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            ViewData["InterestId"] = new SelectList(_context.Interests, "Id", "Title");
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,PhoneNumber")] Person person)
        {
            if (id != person.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id))
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
            return View(person);
        }


        // GET: People/Edit/5
        public async Task<IActionResult> AddInterest(int? id)
        {
            if (id == null || _context.Persons == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            ViewData["InterestId"] = new SelectList(_context.Interests, "Id", "Title");
            return View(person);
        }


        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInterest(int id, [Bind("Id,Name,PhoneNumber")] Person person, int SelectedInterestId)
        {
            if (id != person.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var personFromDb = await _context.Persons.Include(p => p.PersonInterests).FirstOrDefaultAsync(person => person.Id == id);
                    if (personFromDb == null)
                    {
                        return NotFound();
                    }

                    personFromDb.PersonInterests.Add(new PersonInterest { PersonId = personFromDb.Id, InterestId = SelectedInterestId });
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id))
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
            return View(await _context.Persons.ToListAsync());
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Persons == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Persons == null)
            {
                return Problem("Entity set 'InitialDbContext.Persons'  is null.");
            }
            var person = await _context.Persons.FindAsync(id);
            if (person != null)
            {
                _context.Persons.Remove(person);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
          return (_context.Persons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
