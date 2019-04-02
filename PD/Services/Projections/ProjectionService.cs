using Microsoft.EntityFrameworkCore;
using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.SalaryScales;
using PD.Services.Projections.Rules;
using PD.Services.Projections.Rules.ContractSettlementComputations;
using PD.Services.Projections.Rules.MeritComputations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services
{
    public abstract class ProjectionService : PdServiceBase
    {
        public ProjectionService(ApplicationDbContext db, IPdDataProtector dataProtector)
            : base(db, dataProtector)
        {
        }

        protected List<AbstractProjectionRule> GetSalaryCalculationRules()
        {
            //Creating instances of salary-calculation rules in the correct order of applying them
            List<AbstractProjectionRule> rules = new List<AbstractProjectionRule>()
            {
                new ComputeContractSettlement(Db, _dataProtector),
                new ComputeMerit(Db, _dataProtector) ,
                new ComputeFullProfessorMerit(Db, _dataProtector),
                new AggregateBaseSalaryComponents(Db, _dataProtector),
                new HandleUpperSalaryLimits(Db, _dataProtector)
            };

            return rules;
        }


        public void ComputeSalaries(DateTime from, DateTime? to = null, string employeeId = null)
        {
            IQueryable<Person> query = Db.Persons
                .Include(p => p.PositionAssignments).ThenInclude(pa => pa.Position)
                .Include(p => p.PositionAssignments).ThenInclude(pa => pa.Compensations);

            if (!string.IsNullOrEmpty(employeeId))
            {
                string hash = _dataProtector.Hash(employeeId);
                query = query.Where(p => p.Hash == hash);
            }

            List<Person> persons = query.ToList();
            List<AbstractProjectionRule> rules = GetSalaryCalculationRules();
            
            for(DateTime t = from; t<to; t = t.AddYears(1))
            {
                foreach(var person in persons)
                {
                    foreach (var rule in rules)
                        rule.Execute(person, t);
                }
            }
        }

    }
}
