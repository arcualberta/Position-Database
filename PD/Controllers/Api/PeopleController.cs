using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PD.Data;
using PD.Models;
using PD.Models.AppViewModels.DataTables;
using PD.Services;

namespace PD.Controllers.Api
{
    [Authorize(Roles = "Admin,Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public PeopleController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        ////// POST: api/People/
        ////[HttpPost]
        ////public async Task<ActionResult<IEnumerable<Person>>> GetPersons([FromBody] DataTableParameters dataTableParameters)
        ////{

        ////    return await _context.Persons.ToListAsync();
        ////}

        // POST: api/People/
        [HttpPost]
        public async Task<ActionResult<DataTableResponse>> GetPersons([FromBody] DataTableParameters dataTableParameters)
        {
            var people =  await _context.Persons.ToListAsync();
            var query = people
                .Select(p => new string[] { p.Name, p.EmployeeId })
                .Skip(dataTableParameters.Start)
                .Take(20);

            var result = query.ToArray();
            foreach (var p in result)
            {
                p[0] = _dataService._dataProtector.Decrypt(p[0]);
                p[1] = _dataService._dataProtector.Decrypt(p[1]);
            }

            return new DataTableResponse()
            {
                Draw = dataTableParameters.Draw + 1,
                RecordsTotal = 600,
                RecordsFiltered = 320,
                Data = result
            };
        }


        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        // PUT: api/People/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(int id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }

            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
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

        ////// POST: api/People
        ////[HttpPost]
        ////public async Task<ActionResult<Person>> PostPerson([FromBody] Person person)
        ////{
        ////    _context.Persons.Add(person);
        ////    await _context.SaveChangesAsync();

        ////    return person;
        ////}

        // DELETE: api/People/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Person>> DeletePerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return person;
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
    }
}
