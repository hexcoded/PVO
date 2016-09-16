using System;
using System.Data;
using System.Data.SqlClient;
namespace ExploitFilter
{
    public static class InitializeArrays
    {
        public static string[] InitStringArray(string[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = "";
            }
            return arr;
        }

        public static int[] InitIntArray(int[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = 0;
            }
            return arr;
        }

        public static bool[] InitBoolArray(bool[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = false;
            }
            return arr;
        }
    }

    public static class sqlCon
    {
        public static iniFile cfg = new iniFile("config/settings.ini");
        public static SqlConnection con = new SqlConnection(@"Server=" + cfg.IniReadValue("GENERAL", "HOST") + ";Database=" + cfg.IniReadValue("GENERAL", "SUP_DB") + ";User Id=" + cfg.IniReadValue("GENERAL", "USER") + ";Password=" + cfg.IniReadValue("GENERAL", "PASS") + ";MultipleActiveResultSets=True");
        public static void exec(string query)
        {
            try
            {
                if (sqlCon.con.State == ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(query, con);
                    com.ExecuteNonQuery();
                }
                else
                {
                    sqlCon.con.Open();
                    sqlCon.exec(query);
                }
            }
            catch (SqlException Ex)
            {
                // Exception handler
                if (FilterMain.EXCEPTION_LOG)
                {
                    Console.WriteLine("MSSQL Error -> Function exec(" + query + "), exception catched : " + Ex.ToString());
                }
            }
        }
        //read string
        public static string ReadString(string query)
        {
            string strResult = null;
            try
            {
                if (sqlCon.con.State == ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;
                    strResult = (string)com.ExecuteScalar();
                }
                else
                {
                    sqlCon.con.Open();
                    sqlCon.ReadString(query);
                }
            }
            catch (SqlException Ex)
            {
                // Exception handler
                if (FilterMain.EXCEPTION_LOG)
                {
                    Console.WriteLine("MSSQL Error -> Function exec(" + query + "), exception catched : " + Ex.ToString());
                }
            }
            return strResult;
        }

        public static SqlDataReader Return(string query, params SqlParameter[] args)
        {
            if (sqlCon.con.State == ConnectionState.Open)
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(args);
                        return cmd.ExecuteReader();
                    }

                    catch (Exception Ex)
                    {
                        // Exception handler
                        if (FilterMain.EXCEPTION_LOG)
                        {
                            Console.WriteLine("MSSQL Error -> Function exec(" + query + "), exception catched : " + Ex.ToString());
                        }
                    }
                }
            }
            else
            {
                sqlCon.con.Open();
            }
            return null;
        }

        public static short ReadShort(string query)
        {
            short nResult = 0;
            try
            {
                if (sqlCon.con.State == ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;
                    nResult = (short)com.ExecuteScalar();
                }
                else
                {
                    sqlCon.con.Open();
                    sqlCon.ReadShort(query);
                }
            }
            catch (SqlException Ex)
            {
                // Exception handler
                if (FilterMain.EXCEPTION_LOG)
                {
                    Console.WriteLine("MSSQL Error -> Function exec(" + query + "), exception catched : " + Ex.ToString());
                }
            }
            return nResult;
        }

        public static int ReadInt(string query)
        {
            int nResult = 0;
            try
            {
                if (sqlCon.con.State == ConnectionState.Open)
                {
                    SqlCommand com = new SqlCommand(query, con);
                    com.CommandType = CommandType.Text;
                    nResult = (int)com.ExecuteScalar();
                }
                else
                {
                    sqlCon.con.Open();
                    sqlCon.ReadInt(query);
                }
            }
            catch (SqlException Ex)
            {
                // Exception handler
                if (FilterMain.EXCEPTION_LOG)
                {
                    Console.WriteLine("MSSQL Error -> Function exec(" + query + "), exception catched : " + Ex.ToString());
                }
            }
            return nResult;
        }
        public static string[] getSingleArray(string query)
        {
            try
            {
                if (sqlCon.con.State == ConnectionState.Open)
                {
                    SqlDataAdapter SqlAD = new SqlDataAdapter();
                    SqlAD.SelectCommand = new SqlCommand(query, con);
                    DataSet ds = new DataSet();
                    SqlAD.Fill(ds);

                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count != 0)
                    {
                        string[] arr = new string[dt.Rows[0].ItemArray.Length];

                        arr = InitializeArrays.InitStringArray(arr);


                        DataRow row = dt.Rows[0]; //first array

                        for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++)
                        {
                            arr[i] = row[i].ToString();

                        }
                        return arr;
                    }
                }
                else
                {
                    sqlCon.con.Open();
                    sqlCon.getSingleArray(query);
                }
            }
            catch (SqlException Ex)
            {
                // Exception handler
                if (FilterMain.EXCEPTION_LOG)
                {
                    Console.WriteLine("MSSQL Error -> Function exec(" + query + "), exception catched : " + Ex.ToString());
                }
            }
            return null;
        }
    }


}