using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace exam_db.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int ConstantId { get; set; }
        public virtual Constant Constant { get; set; }
        public int itemId { get; set; }
        public virtual Item item { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}