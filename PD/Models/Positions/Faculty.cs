using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Positions
{
    public class Faculty : Position
    {
        public enum eRank
        {
            [Display(Name = "Assistant Professor")]
            AssistantProfessor,

            [Display(Name = "Associate Professor")]
            AssociateProfessor,

            [Display(Name = "Professor 1")]
            Professor1,

            [Display(Name = "Professor 2")]
            Professor2,

            [Display(Name = "Professor 3")]
            Professor3,

            [Display(Name = "FSO 1")]
            FSO1,

            [Display(Name = "FSO 2")]
            FSO2,

            [Display(Name = "FSO 3")]
            FSO3,

            [Display(Name = "Academic Teaching Staff")]
            ATS
        }

        public eRank Rank { get; set; }
    }
}
