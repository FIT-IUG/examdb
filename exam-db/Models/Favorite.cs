using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace exam_db.Models
{
    public class Favorite
    {
        //public Favorite(int itemId, string userId)
        //{
        //    this.itemId = itemId;
        //    UserId = userId;
        //}

        public int Id { get; set; }

        public int itemId { get; set; }

        public virtual Item item { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}