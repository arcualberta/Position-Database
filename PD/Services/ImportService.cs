﻿using OfficeOpenXml;
using PD.Data;
using PD.Models;
using PD.Models.AppViewModels.Faculty;
using PD.Models.ChartFields;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static PD.Models.AppViewModels.EmployeeViewModel;
using PD.Models.Compensations;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using PD.Models.Positions;
using PD.Models.SalaryScales;

namespace PD.Services
{
    public class ImportService: PdServiceBase
    {
        public ImportService(ApplicationDbContext db)
            : base(db)
        {
        }

        private enum ColIndex
        {
            deptNameCol = 1,
            rankCol,
            employeeIdCol,
            employeeNameCol,
            posNumCol,
            recordNumCol,
            ftPtStatusCol,
            statusCol,
            fundSrcCol,
            meritDecCol,
            meritReasonCol,
            pastSalaryCol,
            contractSettlementCol,
            meritCol,
            specialAdjustCol,
            currSalaryCol,
            marketSupplementCol,
            promotedCol,
            notesCol,
            scCol,
            deptIdCol,
            fundCol,
            progCol,
            accCol,
            unknown1,
            bargUnitCol
        };

        public void InjestFacultySalaryAdjustmentData(string fileName, string worksheetName, DateTime currentYearStartDate, DateTime currentYearEndDate, DateTime nextYearEndDate)
        {
            FileInfo file = new FileInfo(fileName);
            DateTime currentYearSampleDate = currentYearStartDate.AddDays(1).Date;
            DateTime nextYearStartDate = currentYearEndDate.AddDays(1).Date;
            DateTime nextYearSampleDate = nextYearStartDate.AddDays(1).Date;

            //try
            {
                byte[] dataBytes = File.ReadAllBytes(fileName);
                using (MemoryStream ms = new MemoryStream(dataBytes))
                using (ExcelPackage package = new ExcelPackage(ms))
                {
                    StringBuilder sb = new StringBuilder();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets
                        .Where(ws => ws.Name == worksheetName)
                        .FirstOrDefault();

                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;

                    List<FacultyEmployeeViewModel> employees = new List<FacultyEmployeeViewModel>();

                    //validating columns
                    List<string> errors = new List<string>();
                    List<string> expectedHeadings = new List<string>(){
                        "Faculty / Department Name",
                        "Rank",
                        "Employee ID",
                        "Employee Name",
                        "Position Number",
                        "Record #",
                        "FT/PT",
                        "Status",
                        "Fund Source",
                        "Merit Decision",
                        "Merit Reason",
                        "Salary",
                        "Contract Settlement (1.0%)",
                        "Merit",
                        "Special Adjustment",
                        "Salary",
                        "Market Supplement",
                        "Promoted",
                        "Notes",
                        "SC",
                        "DeptID",
                        "Fund",
                        "Prog",
                        "Acc#",
                        "Check1",
                        "Barg_Unit"
                    };

                    for (int i = 1; i <= Enum.GetValues(typeof(ColIndex)).Length; ++i)
                    {
                        string val = worksheet.Cells[1, i].Value.ToString().Trim();

                        if (worksheet.Cells[1, i].Value.ToString().Trim() != expectedHeadings[i - 1])
                            errors.Add("Column " + i + " must be " + expectedHeadings[i - 1] + ". Found " + worksheet.Cells[1, i].Value.ToString());
                    }

                    //Making sure past and current salaries are in the correct columns
                    int[] olderYearRange = worksheet.Cells[2, (int)ColIndex.pastSalaryCol].Value.ToString().Split("/").Select(x => int.Parse(x)).ToArray();
                    int[] latterYearRange = worksheet.Cells[2, (int)ColIndex.currSalaryCol].Value.ToString().Split("/").Select(x => int.Parse(x)).ToArray();
                    if (olderYearRange[0] != currentYearStartDate.Year || olderYearRange[1] != currentYearEndDate.Year)
                        throw new Exception("Specified salary year does not match with the data");

                    if (olderYearRange[1] != latterYearRange[0])
                        throw new Exception("The ending year of the older salary year must be as same as the begining year of the latter salary year.");

                    for (int row = 3; row <= rowCount; ++row)
                    {
                        Position.eWorkload ftptStatus = Enum.Parse<Position.eWorkload>(worksheet.Cells[row, (int)ColIndex.ftPtStatusCol].Value.ToString());
                        eFundingSource fundingSource = Enum.Parse<eFundingSource>(worksheet.Cells[row, (int)ColIndex.fundSrcCol].Value.ToString());

                        FacultyEmployeeViewModel empl = new FacultyEmployeeViewModel()
                        {
                            DeptName = worksheet.Cells[row, (int)ColIndex.deptNameCol].Value.ToString().Trim(),
                            Rank = worksheet.Cells[row, (int)ColIndex.rankCol].Value.ToString().Trim(),
                            EmployeeId = worksheet.Cells[row, (int)ColIndex.employeeIdCol].Value.ToString().Trim(),
                            EmployeeName = worksheet.Cells[row, (int)ColIndex.employeeNameCol].Value.ToString().Trim(),
                            PositionNumber = worksheet.Cells[row, (int)ColIndex.posNumCol].Value.ToString().Trim(),
                            FtPtStatus = ftptStatus,
                            FundingSource = fundingSource,
                            RecordNumber = Int32.Parse(worksheet.Cells[row, (int)ColIndex.recordNumCol].Value.ToString().Trim()),
                        };

                        empl.Salary = new FacultySalaryViewModel();
                        empl.FacultySalary.Account = new Account() { Value = worksheet.Cells[row, (int)ColIndex.accCol].Value.ToString().Trim() };
                        empl.FacultySalary.BargUnit = worksheet.Cells[row, (int)ColIndex.bargUnitCol].Value == null ? null : worksheet.Cells[row, (int)ColIndex.bargUnitCol].Value.ToString().Trim();
                        empl.FacultySalary.ContractSettlement = decimal.Parse(worksheet.Cells[row, (int)ColIndex.contractSettlementCol].Value.ToString().Trim());
                        empl.FacultySalary.NextSalary = decimal.Parse(worksheet.Cells[row, (int)ColIndex.currSalaryCol].Value.ToString().Trim());
                        empl.FacultySalary.DeptId = new DeptID() { Value = worksheet.Cells[row, (int)ColIndex.deptIdCol].Value.ToString().Trim() };
                        empl.FacultySalary.Fund = new Fund() { Value = worksheet.Cells[row, (int)ColIndex.fundCol].Value.ToString().Trim() };
                        empl.FacultySalary.IsPromoted = "Yes" == worksheet.Cells[row, (int)ColIndex.promotedCol].Value.ToString().Trim();
                        empl.FacultySalary.CurrentSalary = decimal.Parse(worksheet.Cells[row, (int)ColIndex.pastSalaryCol].Value.ToString().Trim());
                        empl.FacultySalary.MarketSupplement = decimal.Parse(worksheet.Cells[row, (int)ColIndex.marketSupplementCol].Value.ToString().Trim());
                        empl.FacultySalary.Merit = decimal.Parse(worksheet.Cells[row, (int)ColIndex.meritCol].Value.ToString().Trim());
                        empl.FacultySalary.MeritDecision = decimal.Parse(worksheet.Cells[row, (int)ColIndex.meritDecCol].Value.ToString().Trim());
                        empl.FacultySalary.MeritReason = worksheet.Cells[row, (int)ColIndex.meritReasonCol].Value == null ? null : worksheet.Cells[row, (int)ColIndex.meritReasonCol].Value.ToString().Trim();
                        empl.FacultySalary.Notes = worksheet.Cells[row, (int)ColIndex.notesCol].Value == null ? null : worksheet.Cells[row, (int)ColIndex.notesCol].Value.ToString().Trim();
                        empl.FacultySalary.Program = new Program() { Value = worksheet.Cells[row, (int)ColIndex.progCol].Value.ToString().Trim() };
                        empl.FacultySalary.SpecialAdjustment = decimal.Parse(worksheet.Cells[row, (int)ColIndex.specialAdjustCol].Value.ToString().Trim());
                        empl.FacultySalary.Speedcode = new Speedcode() { Value = worksheet.Cells[row, (int)ColIndex.scCol].Value.ToString().Trim() };
                        empl.ContractStatus = Enum.Parse<Position.eContractType>(worksheet.Cells[row, (int)ColIndex.statusCol].Value.ToString().Trim());
                        
                        employees.Add(empl);
                    }


                    //Adding new departments that are in the data file but not in the database to the database
                    string[] deptNames = employees.Select(empl => empl.DeptName).Distinct().ToArray();
                    foreach (string deptName in deptNames)
                    {
                        if (!Db.Departments.Where(dept => dept.Name == deptName).Any())
                            Db.Departments.Add(new Department() { Name = deptName });
                    }
                    Db.SaveChanges();

                    //Adding new employees that are in the data file but not in the database to the database
                    foreach (var empl in employees)
                    {
                        Person dbPersonRecord = Db.Persons.Where(p => p.EmployeeId == empl.EmployeeId).FirstOrDefault();
                        if (dbPersonRecord == null)
                        {
                            dbPersonRecord = new Person()
                            {
                                EmployeeId = empl.EmployeeId,
                                Name = DataProtector.Encrypt(empl.EmployeeName)
                            };
                            Db.Persons.Add(dbPersonRecord);
                        }
                    }
                    Db.SaveChanges();

                    //Creating all the chart fields and speed codes if they do not exist
                    foreach (var empl in employees)
                    {
                        //Account
                        if (!Db.ChartFields.Where(cf => cf is Account && cf.Value == empl.FacultySalary.Account.Value).Any())
                            Db.ChartFields.Add(empl.FacultySalary.Account);
                        else //reload the field so that it contains all relationships
                            empl.FacultySalary.Account = Db.ChartFields.Where(cf => cf is Account && cf.Value == empl.FacultySalary.Account.Value).FirstOrDefault() as Account;

                        //Program
                        if (!Db.ChartFields.Where(cf => cf is Program && cf.Value == empl.FacultySalary.Program.Value).Any())
                            Db.ChartFields.Add(empl.FacultySalary.Program);
                        else //reload the field so that it contains all relationships
                            empl.FacultySalary.Program = Db.ChartFields.Where(cf => cf is Program && cf.Value == empl.FacultySalary.Program.Value).FirstOrDefault() as Program;

                        //Fund
                        if (!Db.ChartFields.Where(cf => cf is Fund && cf.Value == empl.FacultySalary.Fund.Value).Any())
                            Db.ChartFields.Add(empl.FacultySalary.Fund);
                        else //reload the field so that it contains all relationships
                            empl.FacultySalary.Fund = Db.ChartFields.Where(cf => cf is Fund && cf.Value == empl.FacultySalary.Fund.Value).FirstOrDefault() as Fund;

                        //DeptID
                        Department dept = Db.Departments.Where(d => d.Name == empl.DeptName).FirstOrDefault();
                        if (!Db.ChartFields.Where(cf => cf is DeptID && cf.Value == empl.FacultySalary.DeptId.Value).Any())
                        {
                            Db.ChartFields.Add(empl.FacultySalary.DeptId);

                            //Adding this DeptID to the current department of the employee
                            dept.DeptIDs.Add(empl.FacultySalary.DeptId);
                        }
                        else
                        {
                            //reload the field so that it contains all relationships and 
                            //ensure that the reloaded DeptID belongs to the current employee's department
                            empl.FacultySalary.DeptId = Db.ChartFields.Where(cf => cf is DeptID && cf.Value == empl.FacultySalary.DeptId.Value).FirstOrDefault() as DeptID;
                            if (!dept.DeptIDs.Select(d => empl.FacultySalary.DeptId.Id).Any())
                                throw new Exception("DeptId " + empl.FacultySalary.DeptId + " is already registered with the department " +
                                    empl.FacultySalary.DeptId.Department.Name + ", so it cannot be associated with an employee of " +
                                    dept.Name);
                        }

                        //Speedcode
                        if (!Db.Speedcodes.Where(sc => sc.Value == empl.FacultySalary.Speedcode.Value).Any())
                            Db.Speedcodes.Add(empl.FacultySalary.Speedcode);
                        else //reload the field so that it contains all relationships
                            empl.FacultySalary.Speedcode = Db.Speedcodes.Where(sc => sc.Value == empl.FacultySalary.Speedcode.Value).FirstOrDefault();


                        Db.SaveChanges();
                    }

                    //Creating chart strings
                    foreach (var empl in employees)
                    {
                        int? csId = GetChartStringIds(empl.Salary.DeptId.Id, empl.Salary.Fund.Id, empl.Salary.Program.Id, empl.Salary.Account.Id).FirstOrDefault();

                        ChartString cs;
                        if (!csId.HasValue || csId.Value == 0)
                        {
                            cs = new ChartString();
                            cs.DeptID = empl.Salary.DeptId;
                            cs.Fund = empl.Salary.Fund;
                            cs.Program = empl.Salary.Program;
                            cs.Account = empl.Salary.Account;

                            Db.ChartStrings.Add(cs);
                            Db.SaveChanges();
                        }
                        else
                            cs = Db.ChartStrings.Find(csId);
                    }

                    //Creating speedcodes
                    foreach (var empl in employees)
                    {
                        
                    }

                    //Adding position records
                    foreach (var empl in employees)
                    {
                        //Retrieve the person record of this employee from the database
                        Person person = Db.Persons.Where(p => p.EmployeeId == empl.EmployeeId).FirstOrDefault();

                        //Retrieve the active position record with the given position number from the database
                        Position position = Db.Positions
                            .Where(p => p.Number == empl.PositionNumber
                                && (p.StartDate.HasValue == false || p.StartDate <= currentYearSampleDate)
                                && (p.EndDate.HasValue == false || p.EndDate > currentYearSampleDate)
                                )
                            .FirstOrDefault();

                        if (position == null)
                        {
                            //Create a new position record
                            position = new Faculty();
                            position.StartDate = currentYearStartDate;
                            position.EndDate = null; //No end date for employees imported throgh the spreadsheet
                            position.Number = empl.PositionNumber;
                            position.Title = empl.Rank;
                            position.Workload = empl.FtPtStatus;
                            position.ContractType = empl.ContractStatus;

                            Db.Positions.Add(position);
                            person.Positions.Add(position);
                            Db.SaveChanges();
                        }
                    }
                    
                    //Creating Position Accounts
                    foreach (var empl in employees)
                    {
                        var position = Db.Positions.Where(pos => pos.Number == empl.PositionNumber).FirstOrDefault();
                        var chartStringId = GetChartStringIds(empl.Salary.DeptId.Id, empl.Salary.Fund.Id, empl.Salary.Program.Id, empl.Salary.Account.Id).First();

                        if(!Db.PositionAccounts.Where(pa => pa.PositionId == position.Id && pa.ChartStringId == chartStringId).Any())
                        {
                            PositionAccount pa = new PositionAccount();
                            pa.ValuePercentage = 100;
                            position.PositionAccounts.Add(pa);
                            pa.ChartStringId = chartStringId;

                            Db.SaveChanges();
                        }
                    }

                    //Adding position assignments to positions
                    foreach (var empl in employees)
                    {
                        IQueryable<PositionAssignment> positionAssignments = GetPositionAssignments(null, empl.PositionNumber, currentYearSampleDate);

                        if (positionAssignments.Count() > 1)
                            throw new Exception("Data Error: Same position has multiple assign,emys at the time period containig " + currentYearSampleDate.Date);

                        if (positionAssignments.Count() == 0)
                        {
                            Position position = Db.Positions
                                .Include(pos => pos.PositionAssignments)
                                .Where(pos => pos.Number == empl.PositionNumber)
                                .FirstOrDefault();

                            if (position == null)
                                throw new Exception(string.Format("Data Error: Position with position number {0} does not exist", empl.PositionNumber));

                            PositionAssignment pa = new PositionAssignment();
                            pa.Status = PositionAssignment.eStatus.Active;
                            pa.StartDate = currentYearStartDate;
                            pa.EndDate = null; //No end date for position assignments imported through the spreadsheet

                            position.PositionAssignments.Add(pa);

                            Db.SaveChanges();
                        }
                    }


                    //Updating employee salary information
                    foreach (var empl in employees)
                    {
                        //In this data ingestion, there will be only one position assignment for this position number.
                        IQueryable<PositionAssignment> positionAssignments = Db.PositionAssignments
                            .Include(a => a.Compensations)
                            .Where(a => a.Position.Number == empl.PositionNumber
                                && (!a.StartDate.HasValue || a.StartDate <= currentYearSampleDate)
                                && (!a.EndDate.HasValue || a.EndDate >= currentYearSampleDate)
                            );

                        if (positionAssignments.Count() != 1)
                            throw new Exception(string.Format("There must be one and only one position assignment. Found {0}", positionAssignments.Count()));

                        PositionAssignment pa = positionAssignments.FirstOrDefault();

                        //Salary for the CURRENT year
                        Salary salary = Db.Salaries
                            .Where(s => s.StartDate == currentYearStartDate && s.EndDate == currentYearEndDate && s.PositionAssignmentId == pa.Id)
                            .FirstOrDefault();
                        if (salary == null)
                        {
                            salary = new Salary()
                            {
                                Value = empl.Salary.CurrentSalary,
                                StartDate = currentYearStartDate,
                                EndDate = currentYearEndDate,
                            };
                            pa.Compensations.Add(salary);
                            Db.SaveChanges();
                        }

                        //Merit for the NEXT year
                        Merit merit = Db.Merits
                            .Where(a => a.PositionAssignmentId == pa.Id && a.StartDate == nextYearStartDate && a.EndDate == nextYearEndDate)
                            .FirstOrDefault();
                        if (merit == null)
                        {
                            merit = new Merit()
                            {
                                StartDate = nextYearStartDate,
                                EndDate = nextYearEndDate,
                                Value = (empl.Salary as FacultySalaryViewModel).MeritDecision,
                                Notes = (empl.Salary as FacultySalaryViewModel).MeritReason,
                                IsPromoted = (empl.Salary as FacultySalaryViewModel).IsPromoted
                            };
                            pa.Compensations.Add(merit);
                            Db.SaveChanges();
                        }
                        //Contract Settlement for the NEXT year
                        ContractSettlement atb = Db.ContractSettlements
                            .Where(a => a.PositionAssignmentId == pa.Id && a.StartDate == nextYearStartDate && a.EndDate == nextYearEndDate)
                            .FirstOrDefault();
                        if(atb == null)
                        {
                            atb = new ContractSettlement()
                            {
                                StartDate = nextYearStartDate,
                                EndDate = nextYearEndDate,
                                Value = empl.FacultySalary.ContractSettlement
                            };
                            pa.Compensations.Add(atb);
                            Db.SaveChanges();
                        }

                        //Special adjustment for the NEXT year
                        string name = "Special Adjustment";
                        if (empl.FacultySalary.SpecialAdjustment > 0)
                        {
                            Adjustment sa = Db.Adjustments
                            .Where(a => a.PositionAssignmentId == pa.Id && a.StartDate == nextYearStartDate && a.EndDate == nextYearEndDate && a.Name == name)
                                .FirstOrDefault();

                            if (sa == null)
                            {
                                sa = new Adjustment()
                                {
                                    StartDate = nextYearStartDate,
                                    EndDate = nextYearEndDate,
                                    Value = empl.FacultySalary.SpecialAdjustment,
                                    Name = name
                                };
                                pa.Compensations.Add(sa);
                                Db.SaveChanges();
                            }
                        }

                        //Market supplement for the NEXT year
                        name = "Market Supplement";
                        if (empl.FacultySalary.MarketSupplement > 0)
                        {
                            Adjustment sa = Db.Adjustments
                                .Where(a => a.PositionAssignmentId == pa.Id && a.StartDate == nextYearStartDate && a.EndDate == nextYearEndDate && a.Name == name)
                                .FirstOrDefault();

                            if (sa == null)
                            {
                                sa = new Adjustment()
                                {
                                    StartDate = nextYearStartDate,
                                    EndDate = nextYearEndDate,
                                    Value = empl.FacultySalary.MarketSupplement,
                                    Name = name
                                };
                                pa.Compensations.Add(sa);
                                Db.SaveChanges();
                            }
                        }
                    }

                    //Salary Scales
                    //=============
                    bool newDataAdded = AddFacultySalaryScale("Assistant Professor", new DateTime(2015, 07, 01), 76534, 106402, 2489, 1);
                    newDataAdded |= AddFacultySalaryScale("Assistant Professor", new DateTime(2016, 07, 01), 77299, 107467, 2514, 1);
                    newDataAdded |= AddFacultySalaryScale("Assistant Professor", new DateTime(2017, 07, 01), 78458, 109082, 2552, 1);

                    newDataAdded |= AddFacultySalaryScale("Associate Professor", new DateTime(2015, 07, 01), 88971, 133645, 3191, 1);
                    newDataAdded |= AddFacultySalaryScale("Associate Professor", new DateTime(2016, 07, 01), 89861, 134983, 3223, 1);
                    newDataAdded |= AddFacultySalaryScale("Associate Professor", new DateTime(2017, 07, 01), 91209, 137003, 3271, 1);

                    newDataAdded |= AddFacultySalaryScale("Professor 1", new DateTime(2015, 07, 01), 110715, 133227, 3752, 1);
                    newDataAdded |= AddFacultySalaryScale("Professor 1", new DateTime(2016, 07, 01), 111822, 134562, 3790, 1);
                    newDataAdded |= AddFacultySalaryScale("Professor 1", new DateTime(2017, 07, 01), 113499, 136581, 3847, 1);

                    newDataAdded |= AddFacultySalaryScale("Professor 2", new DateTime(2015, 07, 01), 133227, 145991, 3191, 1);
                    newDataAdded |= AddFacultySalaryScale("Professor 2", new DateTime(2016, 07, 01), 134562, 147454, 3223, 1);
                    newDataAdded |= AddFacultySalaryScale("Professor 2", new DateTime(2017, 07, 01), 136581, 149665, 3271, 1);

                    newDataAdded |= AddFacultySalaryScale("Professor 3", new DateTime(2015, 07, 01), 145991, 389913, 2489, 1);
                    newDataAdded |= AddFacultySalaryScale("Professor 3", new DateTime(2016, 07, 01), 147454, 393826, 2514, 1);
                    newDataAdded |= AddFacultySalaryScale("Professor 2", new DateTime(2017, 07, 01), 149665, 399761, 2552, 1);

                    if (newDataAdded)
                        Db.SaveChanges();
                }
            }
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        protected bool AddFacultySalaryScale(string name, DateTime startDate, decimal minSalary, decimal maxSalary, decimal salaryStep, decimal contractSettlement)
        {
            if (Db.FacultySalaryScales.Where(sc => sc.StartDate == startDate && sc.Name == name).Any())
                return false;
            else
            {
                FacultySalaryScale scale = new FacultySalaryScale()
                {
                    Name = name,
                    StartDate = startDate,
                    EndDate = startDate.AddYears(1).AddDays(-1),
                    ContractSettlement = contractSettlement,
                    Minimum = minSalary,
                    Maximum = maxSalary,
                    StepValue = salaryStep
                };
                Db.SalaryScales.Add(scale);
                return true;
            }
        }

        protected IEnumerable<int> GetChartStringIds(int deptIdId, int fundId, int programId, int accountId)
        {
            var deptIdParentIds = Db.ChartField2ChartStringJoins.Where(join => join.ChartFieldId == deptIdId).Select(join => join.ChartStringId).ToList();
            var fundParentIds = Db.ChartField2ChartStringJoins.Where(join => join.ChartFieldId == fundId).Select(join => join.ChartStringId);
            var progParentIds = Db.ChartField2ChartStringJoins.Where(join => join.ChartFieldId == programId).Select(join => join.ChartStringId);
            var accountParentIds = Db.ChartField2ChartStringJoins.Where(join => join.ChartFieldId == accountId).Select(join => join.ChartStringId);

            IEnumerable<int> csIds = deptIdParentIds.Intersect(fundParentIds).Intersect(progParentIds).Intersect(accountParentIds);
            return csIds;
        }

        public void UpdateEmployee(FacultyEmployeeViewModel empl)
        {
            //Creating the employee if does not exist in the database
            Person emplDbRecord = Db.Persons.Where(p => p.EmployeeId == empl.EmployeeId).FirstOrDefault();
            if(emplDbRecord == null)
            {
                emplDbRecord = new Person()
                {
                    Name = empl.EmployeeName,
                    EmployeeId = empl.EmployeeId
                };
                Db.Persons.Add(emplDbRecord);
            }
            else
            {
                //Update the employee name if it's not null in the view model
                emplDbRecord.Name = empl.EmployeeName;
            }

            //Finding all Position records with the given position number
            

            Db.SaveChanges();
        }
    }
}