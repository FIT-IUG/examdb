using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace exam_db.Models
{
    public class Constant
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int parentId { get; set; } // if record is parent this value null
        public int isParent { get; set; }// if parent 0 else 1(child)
    }
}