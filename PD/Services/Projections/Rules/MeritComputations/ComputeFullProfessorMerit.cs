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

            Salary pastSalary = GetPastSalary(pa, targetDate);

            DateTime salaryCycleStartDate = pa.GetSalaryCycleStartDate(targetDate);

            //Finding the salary scale on which this individual sat in the previous year
            var scales = Db.SalaryScales
                .Where(sc => sc.Minimum <= pastSalary.Value 
                    && (sc.Maximum == 0 || sc.Maximum >= pastSalary.Value) //sc.Maximum == 0 means no max limit
                    && sc.StartDate <= pastSalary.StartDate 
                    && sc.EndDate >= pastSalary.EndDate
                    && (sc.Category == Faculty.eRank.Professor1.ToString()
                        || sc.Category == Faculty.eRank.Professor2.ToString()
                        || sc.Category == Faculty.eRank.Professor3.ToString())
                      )
                .ToList();

            SalaryScale scale = scales.FirstOrDefault();

            //Merit for the target year
            Merit merit = pa.GetCompensations<Merit>(targetDate).FirstOrDefault();
            if (merit == null)
            {
                merit = new Merit()
                {
                    StartDate = salaryCycleStartDate,
                    EndDate = salaryCycleStartDate.AddYears(1).AddDays(-1),
                    MeritDecision = scale.DefaultMeritDecision,
                    IsProjection = true //set to true because we are using the DefaultMeritDecision
                };
                pa.Compensations.Add(merit);
            }

            if (scale.Category == Faculty.eRank.Professor3.ToString())
            {
                //There is no maximum limit for this scale, so simply apply the merit directly based on the 
                //merit step of the scale for the target year
                scale = _salaryScaleService.GetSalaryScale(scale.Category, targetDate);
                merit.Value = merit.MeritDecision * scale.StepValue;
            }
            else
            {
                //In this case, we have to check whether the individual crosses the upper limit of the scale
                //and moves on to the next scale.

                //Locate on which step the individual sat on the current scale in the past year
                //Note that this is the number of steps from the scale minimum and it can be different
                //from the actual salary step because of the transition steps at the begining of
                //Professor 1 salary scale.
                decimal numStepsForPastYearAboveScaleMinimum = (pastSalary.Value - scale.Minimum) / scale.StepValue;

                //New salary step after applying the merit decision
                decimal numStepsForTargetYearAboveScaleMinimum = numStepsForPastYearAboveScaleMinimum + merit.MeritDecision;

                //Number of steps actually available within the scale, including any transition steps
                decimal numberOfStepsInScale = Math.Round((scale.Maximum - scale.Minimum) / scale.StepValue);

                if (numStepsForTargetYearAboveScaleMinimum <= numberOfStepsInScale)
                {
                    //This individual stays within the current scale so we can simply apply the 
                    //same rules that we used for Professor3 calculation here.
                    //Note that, we need to do the calculation based on the scale for the target year.
                    scale = _salaryScaleService.GetSalaryScale(scale.Category, targetDate);
                    merit.Value = merit.MeritDecision * scale.StepValue;
                }
                else
                {
                    //This individual crosses the boundary between the current scale and the next higher scale.
                    //Therefore, we calculate what portion of the merit decision is used to reach the top of the
                    //current scale and what portion of it is carried over to the next higher scale.
                    decimal portionOfMeritUsedToFillThisScale = numberOfStepsInScale - numStepsForPastYearAboveScaleMinimum;
                    decimal portionOfMeritLeftForNextScale = merit.MeritDecision - portionOfMeritUsedToFillThisScale;

                    //Now calculate the dollar values for these two merit components from those two scales for the 
                    //target year
                    SalaryScale currentScaleForTargetYear = _salaryScaleService.GetSalaryScale(scale.Category, targetDate);
                    merit.Value = portionOfMeritUsedToFillThisScale * currentScaleForTargetYear.StepValue;

                    PromotionScheme scheme = Db.PromotionSchemes.Where(sc => sc.CurrentTitle == scale.Category).FirstOrDefault();
                    if (scheme == null)
                        throw new Exception(string.Format("Promotion scheme for {0} not found", pa.Position.Title));
                    SalaryScale nextScaleForTargetYear = _salaryScaleService.GetSalaryScale(scheme.PromotedTitle, targetDate);
                    merit.Value = merit.Value + portionOfMeritLeftForNextScale * nextScaleForTargetYear.StepValue;


                    //Updating the Position Details
                    //=============================
                    //
                    //Since this individual got automatically promoted as the upper limit of 
                    //the previous scale was passed, we needs to close the current position and open a new one

                    bool status = PromoteToFacultyPosition(ref pa, scheme.PromotedTitle, pa.GetSalaryCycleStartDate(targetDate));
                    if (!status)
                        throw new Exception(string.Format("Promoting to position {0} failed.", scheme.PromotedTitle));

                    //Since a new position and position assignment created, we save the
                    //Db changes right away so that we get Ids for newly created objects, in case they
                    //were needed for rest of the computations.
                    Db.SaveChanges();
                }
            }

            //Adjusting the merit value according to the workload
            merit.Value = Math.Round(merit.Value * pa.Position.Workload);
            pa.LogInfo("Merit: $" + merit.Value, targetDate);
            return true;
        }
    }
}
