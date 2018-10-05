using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PD.Models.AppViewModels.Filters
{
    public class PositionFilter
    {
        [DisplayFormat(ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Now.Date;

        [Display(Name = "Category")]
        public Position.ePositionType PositionType { get; set; } = Position.ePositionType.Faculty;
        public List<SelectListItem> PositionTypes { get; set; } 
            = Enum.GetValues(typeof(Position.ePositionType))
            .OfType<Position.ePositionType>()
            .Select(t => new SelectListItem(t.ToString(), t.ToString()))
            .ToList();


        [Display(Name = "Active Positions")]
        public bool IsActive { get; set; } = true;

        public string GetFiscalYear()
        {
            if (Date.Month < 4)
                return (Date.Year - 1) + "/" + Date.Year;
            else
                return (Date.Year) + "/" + (Date.Year + 1);
        }

    }
}
