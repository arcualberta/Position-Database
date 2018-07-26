using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PD.Models.AppViewModels
{
    public class FileUploadViewModel
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
