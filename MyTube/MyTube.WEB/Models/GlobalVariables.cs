using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTube.WEB.Models
{
    public static class GlobalVariables
    {
        public static List<string> Categories
        {
            get
            {
                return new List<string>
                {
                    "Films & Animation",
                    "Cars & Vehicles",
                    "Music",
                    "Pets & Animals",
                    "Sports",
                    "Travel & Events",
                    "Gaming",
                    "People & Blogs",
                    "Comedy",
                    "Entertaiment",
                    "News & Politics",
                    "How to & Style",
                    "Education",
                    "Science & Technolegy",
                    "Non-profits & Activism",
                };
            }
        }
    }
}