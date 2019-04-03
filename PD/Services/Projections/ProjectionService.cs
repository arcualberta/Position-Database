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
            SalaryScaleService salaryScaleService = new SalaryScaleService(Db);
            //Creating instances of salary-calculation rules in the correct order of applying them
            List<AbstractProjectionRule> rules = new List<AbstractProjectionRule>()
            {
                new ComputeContractSettlement(Db, _dataProtector, salaryScaleService),
                new ComputeMerit(Db, _dataProtector, salaryScaleService) ,
                new ComputeFullProfessorMerit(Db, _dataProtector, salaryScaleService),
                new AggregateBaseSalaryComponents(Db, _dataProtector, salaryScaleService),
                new HandleUpperSalaryLimits(Db, _dataProtector, salaryScaleService)
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
            
            for(DateTime t = from; t < to; t = t.AddYears(1))
            {
                try
                {
                    foreach (var person in persons)
                    {
                        foreach (var rule in rules)
                            rule.Execute(person, t);
                    }
                }
                catch(Exception ex)
                {
                    AuditRecord au = new AuditRecord() { Message = ex.Message };
                    Db.AuditTrail.Add(au);
                }
            }

            Db.SaveChanges();
        }

    }
}
