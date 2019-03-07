using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            //Loading all objects and decrypting the names and employee IDs
            List<Person> persons = await _context.Persons.ToListAsync();
            int total = persons.Count;

            foreach (Person p in persons)
            {
                p.Name = string.IsNullOrEmpty(p.Name) ? "" : _dataService._dataProtector.Decrypt(p.Name);
                p.EmployeeId = string.IsNullOrEmpty(p.EmployeeId) ? "" : _dataService._dataProtector.Decrypt(p.EmployeeId);
            }

            IEnumerable<Person> query = persons;

            //If search is specified, search on the employee IDs or names depending on whether the 
            //search string is starting with a number or not
            if (dataTableParameters.Search != null && !string.IsNullOrWhiteSpace(dataTableParameters.Search.Value))
            {
                string search = dataTableParameters.Search.Value.Trim();
                if (Char.IsNumber(search.First()))
                    query = query.Where(p => p.EmployeeId.StartsWith(search));
                else
                    query = query.Where(p => p.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase));
            }
            int totalFiltered = query.Count();

            //Sorting with the given column and given direction
            if (dataTableParameters.Order != null && dataTableParameters.Order.Count > 0)
            {
                switch (dataTableParameters.Order[0].Column)
                {
                    case 0:
                        query = dataTableParameters.Order[0].Dir == "asc"
                            ? query.OrderBy(p => p.Name)
                            : query.OrderByDescending(p => p.Name);
                        break;
                    case 1:
                        query = dataTableParameters.Order[0].Dir == "asc"
                            ? query.OrderBy(p => p.EmployeeId)
                            : query.OrderByDescending(p => p.EmployeeId);
                        break;
                }
            }

            //Selecting appropriate subset for return
            if (dataTableParameters.Start > 0)
                query = query.Skip(dataTableParameters.Start);
            if (dataTableParameters.Length >= 0)
                query = query.Take(dataTableParameters.Length);

            var result = query
                 .Select(p => new { id = p.Id, name = p.Name, empId = p.EmployeeId })
                 .ToArray();

            return new DataTableResponse()
            {
                Draw = dataTableParameters.Draw + 1,
                RecordsTotal = total,
                RecordsFiltered = totalFiltered,
                Data = result
            };
        }


        // POST: api/People/5
        [HttpPost("{id}")]
        public async Task<ActionResult<DataTableResponse>> GetPerson(int id, [FromBody] DataTableParameters dataTableParameters)
        {
            var positionAssignments = await _context.PositionAssignments
                .Where(pa => pa.PersonId == id)
                .Include(pa => pa.Position)
                .Include(pa => pa.Compensations)
                .Select(pa => new
                {
                    id = pa.Id,
                    positionTitle = pa.Position.Title,
                    primaryDepartment = pa.Position.PrimaryDepartment.Name,
                    startDate = pa.StartDate,
                    endDate = pa.EndDate
                })
                .ToArrayAsync();

            return new DataTableResponse()
            {
                Draw = dataTableParameters.Draw + 1,
                RecordsTotal = positionAssignments.Count(),
                RecordsFiltered = positionAssignments.Count(),
                Data = positionAssignments
            };
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
