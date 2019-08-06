using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace exam_db.Models
{
    public  class Connection
    {
        static string con = System.Configuration.ConfigurationManager.
    ConnectionStrings["DefaultConnection"].ConnectionString;
        public static SqlConnection Get()
        {
            SqlConnection sc = new SqlConnection(con);
            sc.Open();
            return sc;
        }
    }
}