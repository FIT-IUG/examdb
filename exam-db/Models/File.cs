using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace exam_db.Models
{


    public enum FileType
    {

        csv,
        txt,
        png,
        Jpg,
        Pdf,
        ppt,
        xls,
        docx,
        number,
        pages,
        key,
        rtf,
        oft,
        lba
    }
    public class File
    {
        public int Id { get; set; }
        public String path { get; set; }
        public FileType type { get; set; }
        public long size { get; set; }
        public int itemId { get; set; }
        public virtual Item item { get; set; }

    }
}