using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyTube.WEB.Models.Video
{
    public class VideoUploadViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Video { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase Poster { get; set; }

        [Required]
        public SelectList Category { get; set; } = new SelectList(GlobalVariables.Categories);

        public string SelectedCategory { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(3000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Description { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Tags { get; set; }

        public string[] TagsList
        {
            get
            {
                string[] separators = new string[] { " ", ",", ";", ".", "_", "#", "/", @"\", "|", ":" };
                return Tags.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}