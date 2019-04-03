using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.Positions;
using PD.Models.SalaryScales;

namespace PD.Services.Projections.Rules.MeritComputations
{
    public class ComputeFullProfessorMerit : AbstractProjectionRule
    {
        public ComputeFullProfessorMerit(ApplicationDbContext db, IPdDataProtector dp, SalaryScaleService salaryScaleService)
            : base(db, dp, salaryScaleService, "Compute Full Professor Merit", "This rule computes the standard merit based on the merit decision, merit step and the position workload.")
        {

        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            if (!pa.IsPaidOn(targetDate))
                return true;

            //This merit calculation only applies to full professors.
            if (!(pa.Position.Title == Faculty.eRank.Professor1.ToString() || pa.Position.Title == Faculty.eRank.Professor2.ToString() || pa.Position.Title == Faculty.eRank.Professor3.ToString()))
                return false;

            pa.LogInfo("Computing merit for full professprs.", targetDate);

            if ((pa.Position as Faculty).ContractType == Position.eContractType.S)
                throw new Exception("Position contract status was set to \"S\". This individual should hold a pre-retirement or post-retirement rank, not a professor rank.");

            Salary pastSalary = pa.GetPastSalary(targetDate);

            //Finding the salary scale on which this individual sat in the previous year
            SalaryScale pastSalaryScale = _salaryScaleService._salaryScales
                .Where(sc => sc.Minimum <= pastSalary.Value 
                    && (sc.Maximum == 0 || sc.Maximum >= pastSalary.Value) //sc.Maximum == 0 means no max limit
                    && sc.StartDate <= pastSalary.StartDate 
                    && sc.EndDate >= pastSalary.EndDate
                    && (sc.Category == Faculty.eRank.Professor1.ToString()
                        || sc.Category == Faculty.eRank.Professor2.ToString()
                        || sc.Category == Faculty.eRank.Professor3.ToString())
                      )
                .FirstOrDefault();

            //Select the correct job title based on the salary scale held last year
            //Sometimes, this is incorrectly identified in the mannually entered data.
            string positionTitle = pastSalaryScale.Category;

            //Finding the salary scale for the currrent year for the rank that is held 
            //by this employee in as of now (i.e. before applying any applicable promotions.
            //We do this because we don't yet know whether the employee got promoted 
            //automatically.
            SalaryScale currentSalaryScale = _salaryScaleService.GetSalaryScale(positionTitle, targetDate);

            //Merit for the target year
            Merit merit = pa.GetCompensations<Merit>(targetDate).FirstOrDefault();
            if (merit == null)
            {
                DateTime salaryCycleStartDate = pa.GetSalaryCycleStartDate(targetDate);
                merit = new Merit()
                {
                    StartDate = salaryCycleStartDate,
                    EndDate = salaryCycleStartDate.AddYears(1).AddDays(-1),
                    MeritDecision = currentSalaryScale.DefaultMeritDecision,
                    IsProjection = true //set to true because we are using the DefaultMeritDecision
                };
                pa.Compensations.Add(merit);
            }

            if (positionTitle == Faculty.eRank.Professor3.ToString())
            {
                //The employee is already a Professor 3. Therefore, there is no 
                //max salary limit, thus we simply apply the merit directly based 
                //on the merit step of the scale for the target year
                merit.Value = merit.MeritDecision * currentSalaryScale.StepValue;
            }
            else
            {
                //In this case, we have to check whether the individual crosses the upper limit of the scale
                //and moves on to the next scale.

                //Locate on which step the individual sat on the current scale before the new merit 
                //decision is applied. We use the past year's salary and past year's salary scale
                //to calculate this step.
                //Note that this is the number of steps from the scale minimum and it can be different
                //from the actual salary step because of the transition steps at the begining of
                //Professor 1 salary scale.
                decimal numStepsForPastYearAboveScaleMinimum = (pastSalary.Value - pastSalaryScale.Minimum) / pastSalaryScale.StepValue;

                //New salary step after applying the merit decision
                decimal numStepsForCurrentYearAboveScaleMinimum = numStepsForPastYearAboveScaleMinimum + merit.MeritDecision;

                //Number of steps actually available within the CURRENT year's scale for 
                //this position, including any transition steps
                decimal numberOfStepsInScale = Math.Round((currentSalaryScale.Maximum - currentSalaryScale.Minimum) / currentSalaryScale.StepValue);

                if (numStepsForCurrentYearAboveScaleMinimum <= numberOfStepsInScale)
                {
                    //This individual stays within the current scale so we can simply apply the 
                    //same rules that we used for Professor3 calculation here.
                    //Note that, we need to do the calculation based on the scale for the CURRENT year.
                    merit.Value = merit.MeritDecision * currentSalaryScale.StepValue;
                }
                else
                {
                    //This individual crosses the boundary between the current scale and the next higher scale.
                    //Therefore, we calculate what portion of the merit decision which is used to reach the top
                    //of the current scale and then how much of merit is left to be carried over
                    //to the next higher scale.
                    decimal portionOfMeritUsedToFillThisScale = numberOfStepsInScale - numStepsForPastYearAboveScaleMinimum;
                    decimal portionOfMeritLeftForNextScale = merit.MeritDecision - portionOfMeritUsedToFillThisScale;

                    //Now calculate the dollar values for these two merit components from those 
                    //two scales for the target year
                    merit.Value = portionOfMeritUsedToFillThisScale * currentSalaryScale.StepValue;

                    PromotionScheme scheme = Db.PromotionSchemes.Where(sc => sc.CurrentTitle == positionTitle).FirstOrDefault();
                    if (scheme == null)
                        throw new Exception($"Promotion scheme for {positionTitle} not found");
                    SalaryScale nextScaleForTargetYear = _salaryScaleService.GetSalaryScale(scheme.PromotedTitle, targetDate);
                    merit.Value = merit.Value + portionOfMeritLeftForNextScale * nextScaleForTargetYear.StepValue;

                    //Since this individual got automatically promoted to the next scale we needs to 
                    //close the current position and open a new one.
                    //However, if the current position has a predecessor and if the past year's
                    //salary came from that predecessor, then we have already handled the 
                    //promotion in a previous computation cycle for this period. If this is
                    //not the case then we need to handle the promotion.

                    var alreadyPromoted = false;
                    if(pa.Predecessor != null || pa.PredecessorId != null)
                    {
                        PositionAssignment current = pa;
                        if (pa.Predecessor == null)
                            pa.Predecessor = pa.Position.PositionAssignments
                                .Where(p => p.Id == current.PredecessorId)
                                .FirstOrDefault();

                        alreadyPromoted = pa.Predecessor.Compensations
                            .Where(comp => comp.Id == pastSalary.Id)
                            .Any();
                    }

                    if(alreadyPromoted == false)
                        PromoteToFacultyPosition(pa.Person, pa, scheme.PromotedTitle, pa.GetSalaryCycleStartDate(targetDate));
                }
            }

            //Adjusting the merit value according to the workload
            merit.Value = Math.Round(merit.Value * pa.Position.Workload);
            pa.LogInfo("Merit: $" + merit.Value, targetDate);
            return true;
        }
    }
}
