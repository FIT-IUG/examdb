using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace exam_db.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string title { get; set; }
        public int downloadNumber { get; set; }
        public int viewNumber { get; set; }
        public string yearOfPublish { get; set; }
        public string semester { get; set; }
        public string examType { get; set; }
        public string category { get; set; }//file category (quiz , exam,others,Summaries);
        public int likeNumber { get; set; }
        public DateTime uploadTime { get; set; }
       
        public virtual ICollection<File> listOfFile { get; set; }

        public int CourseId { get; set; }

        
        public virtual Course Course { get; set; }

        public string UserId { get; set; }

      
        public virtual ApplicationUser User { get; set; }


        [NotMapped]
        public string CourseName { get; set; }
        [NotMapped]
        public string DepartmentName { get; set; }
    }
}