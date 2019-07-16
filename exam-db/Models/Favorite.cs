using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace exam_db.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int itemId { get; set; }

        public virtual Item item { get; set; }

        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}