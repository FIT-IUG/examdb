using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace exam_db.ViewModels
{
    public class RecommendViewModel
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string college { get; set; }
        public string department { get; set; }
        public string user { get; set; }
        public int likes { get; set; }
        public int views { get; set; }
        public int download { get; set; }
        public string year { get; set; }
    }
}