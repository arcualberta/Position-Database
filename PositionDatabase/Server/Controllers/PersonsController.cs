using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PositionDatabase.Server.Data;
using PositionDatabase.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PositionDatabase.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly ILogger<PersonsController> _logger;
        private readonly ApplicationDbContext _db;
        public PersonsController(ApplicationDbContext db, ILogger<PersonsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IList<Person>> GetAsync()
        {
            try
            {
                var persons = await _db.Persons.OrderBy(p => p.FirstName).ToListAsync();
                return persons;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Faild to retrieve person list");
                throw ex;
            }
        }

        [HttpPost]
        public async Task<Person> PostAsync(Person person)
        {
            try
            {
                _db.Persons.Add(person);
                await _db.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild to retrieve person list");
                throw ex;
            }
        }

    }
}
