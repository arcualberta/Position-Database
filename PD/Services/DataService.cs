using OfficeOpenXml;
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

namespace PD.Services
{
    public class DataService: PdServiceBase
    {
        public DataService(ApplicationDbContext db)
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

        public void InjestFacultySalaryAdjustmentData(string fileName, string worksheetName)
        {
            FileInfo file = new FileInfo(fileName);
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
                    int[] pastYearRange = worksheet.Cells[2, (int)ColIndex.pastSalaryCol].Value.ToString().Split("/").Select(x => int.Parse(x)).ToArray();
                    int[] currentYearRange = worksheet.Cells[2, (int)ColIndex.currSalaryCol].Value.ToString().Split("/").Select(x => int.Parse(x)).ToArray();
                    if (pastYearRange[1] != currentYearRange[0])
                        errors.Add("The ending year of the past fiscal year must be as same as the begining year of the current fiscal year.");

                    for (int row = 3; row <= rowCount; ++row)
                    {
                        eFtPtStatus ftptStatus = Enum.Parse<eFtPtStatus>(worksheet.Cells[row, (int)ColIndex.ftPtStatusCol].Value.ToString());
                        eFundingSource fundingSource = Enum.Parse<eFundingSource>(worksheet.Cells[row, (int)ColIndex.fundSrcCol].Value.ToString());

                        FacultyEmployeeViewModel empl = new FacultyEmployeeViewModel()
                        {
                            DeptName = worksheet.Cells[row, (int)ColIndex.deptNameCol].Value.ToString().Trim(),
                            Rank = worksheet.Cells[row, (int)ColIndex.rankCol].Value.ToString().Trim(),
                            EmployeeId = worksheet.Cells[row, (int)ColIndex.employeeIdCol].Value.ToString().Trim(),
                            EmployeeName = worksheet.Cells[row, (int)ColIndex.employeeNameCol].Value.ToString().Trim(),
                            PositionNumber = worksheet.Cells[row, (int)ColIndex.employeeNameCol].Value.ToString().Trim(),
                            FtPtStatus = ftptStatus,
                            FundingSource = fundingSource,
                            RecordNumber = Int32.Parse(worksheet.Cells[row, (int)ColIndex.recordNumCol].Value.ToString().Trim()),
                        };

                        empl.Salary = new FacultySalaryViewModel();
                        empl.FacultySalary.Account = new Account() { Value = worksheet.Cells[row, (int)ColIndex.accCol].Value.ToString().Trim() };
                        empl.FacultySalary.BargUnit = worksheet.Cells[row, (int)ColIndex.bargUnitCol].Value == null ? null : worksheet.Cells[row, (int)ColIndex.bargUnitCol].Value.ToString().Trim();
                        empl.FacultySalary.ContractSettlement = decimal.Parse(worksheet.Cells[row, (int)ColIndex.contractSettlementCol].Value.ToString().Trim());
                        empl.FacultySalary.CurrentSalary = decimal.Parse(worksheet.Cells[row, (int)ColIndex.currSalaryCol].Value.ToString().Trim());
                        empl.FacultySalary.DeptId = new DeptID() { Value = worksheet.Cells[row, (int)ColIndex.deptIdCol].Value.ToString().Trim() };
                        empl.FacultySalary.FiscalYear = currentYearRange[0] + "/" + currentYearRange[1];
                        empl.FacultySalary.Fund = new Fund() { Value = worksheet.Cells[row, (int)ColIndex.fundCol].Value.ToString().Trim() };
                        empl.FacultySalary.IsPromoted = "Yes" == worksheet.Cells[row, (int)ColIndex.promotedCol].Value.ToString().Trim();
                        empl.FacultySalary.LastSalary = decimal.Parse(worksheet.Cells[row, (int)ColIndex.pastSalaryCol].Value.ToString().Trim());
                        empl.FacultySalary.MarketSupplement = decimal.Parse(worksheet.Cells[row, (int)ColIndex.marketSupplementCol].Value.ToString().Trim());
                        empl.FacultySalary.Merit = decimal.Parse(worksheet.Cells[row, (int)ColIndex.meritCol].Value.ToString().Trim());
                        empl.FacultySalary.MeritDecision = double.Parse(worksheet.Cells[row, (int)ColIndex.meritDecCol].Value.ToString().Trim());
                        empl.FacultySalary.MeritReason = worksheet.Cells[row, (int)ColIndex.meritReasonCol].Value == null ? null : worksheet.Cells[row, (int)ColIndex.meritReasonCol].Value.ToString().Trim();
                        empl.FacultySalary.Notes = worksheet.Cells[row, (int)ColIndex.notesCol].Value == null ? null : worksheet.Cells[row, (int)ColIndex.notesCol].Value.ToString().Trim();
                        empl.FacultySalary.Program = new Program() { Value = worksheet.Cells[row, (int)ColIndex.progCol].Value.ToString().Trim() };
                        empl.FacultySalary.SpecialAdjustment = decimal.Parse(worksheet.Cells[row, (int)ColIndex.specialAdjustCol].Value.ToString().Trim());
                        empl.FacultySalary.Speedcode = new Speedcode() { Value = worksheet.Cells[row, (int)ColIndex.scCol].Value.ToString().Trim() };

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
                        Person dbPersonRecord = Db.Persons.Where(p => p.EmployeeId == empl.EmployeeId && p.Name == empl.EmployeeName).FirstOrDefault();
                        if (dbPersonRecord == null)
                        {
                            dbPersonRecord = new Person()
                            {
                                EmployeeId = empl.EmployeeId,
                                Name = empl.EmployeeName
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
                        var deptIdParentIds = Db.ChartField2ChartStringJoins.Where(join => join.ChartFieldId == empl.Salary.DeptId.Id).Select(join => join.ChartStringId).ToList();
                        var fundParentIds = Db.ChartField2ChartStringJoins.Where(join => join.ChartFieldId == empl.Salary.Fund.Id).Select(join => join.ChartStringId);
                        var progParentIds = Db.ChartField2ChartStringJoins.Where(join => join.ChartFieldId == empl.Salary.Program.Id).Select(join => join.ChartStringId);
                        var accountParentIds = Db.ChartField2ChartStringJoins.Where(join => join.ChartFieldId == empl.Salary.Account.Id).Select(join => join.ChartStringId);

                        int? csId = deptIdParentIds.Intersect(fundParentIds).Intersect(progParentIds).Intersect(accountParentIds).FirstOrDefault();

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


                    //Creating / Updating position records
                    foreach (var empl in employees)
                    {
                        //Retrieve the person record of this employee from the database
                        Person person = Db.Persons.Where(p => p.EmployeeId == empl.EmployeeId).FirstOrDefault();

                        //Retrieve the active position record with the given position number from the database
                        Position position = Db.Positions.Where(p => p.IsActive && p.Number == empl.PositionNumber).FirstOrDefault();

                        //If the position record exists, then it should belong to this employee. This data ingestion does not
                        //handle changing positions from one employee to another.
                        if (position != null && position.CurrentPerson.PersonId != person.Id)
                            throw new Exception("The Position Number " + empl.PositionNumber + " has already been assigned to another person (system ID: " + position.CurrentPerson.PersonId);

                        if (position == null)
                        {
                            //Creating a position record and attaching it to the current person record
                            //========================================================================

                            ////////    if(person.PersonPositions.)

                            ////////    position = new Position();
                            ////////    position.IsActive = true;
                            ////////    position.Number = empl.PositionNumber;


                            ////////    position.CurrentPerson = new PersonPosition();
                            ////////    position.CurrentPerson.PersonId = person.Id;
                            ////////    position.CurrentPerson.Positions.Add
                        }
                    }


                    //Updating employee salary information
                    foreach (var empl in employees)
                    {

                    }

                    //Updating 

                }
            }
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
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
