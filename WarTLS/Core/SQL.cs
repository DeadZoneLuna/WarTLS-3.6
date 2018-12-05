using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WARTLS
{
    class SQL
    {
        internal static SqlConnection Handler = new SqlConnection();
        internal SQL()
        {

            Handler.ConnectionString = "Server=localhost;Database=wfdb;User Id=root;Trusted_Connection=True;MultipleActiveResultSets=true;";
            Handler.Open();

        }
    }
}
