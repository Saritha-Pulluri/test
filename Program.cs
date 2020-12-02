using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace FAS125_Upload
{
    class Program
    {
        static void Main(string[] args)
        {
            localCode();
        }
        public static void localCode()
        {

            int counter = 0;
            string line;

            // Read the file and display it line by line.  
            System.IO.StreamReader file =
            //new System.IO.StreamReader(args[0]);
            new System.IO.StreamReader(@"C:\Users\Public\Sanitized_FAS125___INS.TAPEIN.VAI16101.HINES.ORIG.txt");

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("{", "0");
            dic.Add("A", "1");
            dic.Add("B", "2");
            dic.Add("C", "3");
            dic.Add("D", "4");
            dic.Add("E", "5");
            dic.Add("F", "6");
            dic.Add("G", "7");
            dic.Add("H", "8");
            dic.Add("I", "9");

            StringBuilder strBuilder = new StringBuilder();
            //update table and columns names
            strBuilder.Append("INSERT INTO DFBFAS125_TBL (OFFICECD , FUNDCODE ,FILENUMBER ,INSURANCENUMBER, STUBSURNAME,FIRSTINTIALFIRSTNAME, " +
                "FIRSTINTIALMIDDLENAME ,INSURANCECHANGE ,ACTIONEFFECTIVEDATE , DEDUCTIONAMOUNT, ESTABLSHEDAMOUNT, DISTRIBUTIONCODE ) VALUES ");

            //read line by line and extract info
            while ((line = file.ReadLine()) != null)
            {
                string officeCode = "";
                string fundCode = "";
                int FileNumber = 0;
                int insuranceNumber = 0;
                string SurName = "";
                string firstInitial = "";
                string middleInitial = "";
                string actionType = "";
                string date = "";
                int deductionAmount = 0;
                int establishedAmount = 0;
                string distributionCode = "";

                officeCode = line.Substring(0, 1);
                fundCode = line.Substring(1, 1);
                FileNumber = Convert.ToInt32(updateString(line.Substring(2, 9), dic));
                insuranceNumber = Convert.ToInt32(updateString(line.Substring(11, 9), dic));
                SurName = line.Substring(20, 5);
                firstInitial = String.IsNullOrWhiteSpace(line.Substring(35, 1)) ? " " : line.Substring(35, 1);
                middleInitial = String.IsNullOrWhiteSpace(line.Substring(41, 1)) ? " " : line.Substring(41, 1);
                actionType = line.Substring(42, 1);
                date = line.Substring(43, 3);
                deductionAmount = String.IsNullOrWhiteSpace(line.Substring(47, 6)) ? 0 : Convert.ToInt32(updateString(line.Substring(47, 6), dic));
                establishedAmount = String.IsNullOrWhiteSpace(line.Substring(61, 5)) ? 0 : Convert.ToInt32(updateString(line.Substring(61, 5), dic));
                distributionCode = String.IsNullOrWhiteSpace(line.Substring(67, 1)) ? " " : updateString(line.Substring(67, 1), dic);

                if (Convert.ToInt32(updateString(line.Substring(3, 9), dic)) != 99999999)
                {

                    strBuilder.Append("(" + officeCode + "," + fundCode + ", " + FileNumber + ", " + insuranceNumber + ", '" + SurName
                            + "', '" + firstInitial + "', '" + middleInitial + "', " + actionType + ", '" + date + "', " + deductionAmount + ", " + establishedAmount + ", '" + distributionCode + "'),");

                    System.Console.WriteLine(line);
                    counter++;
                }
                //insert every 999 records into DB
                if (counter % 999 == 0)
                {
                    strBuilder.Length--;
                    try
                    {
                        Console.WriteLine("Openning Connection ...");

                        //ToDo: Provide the database details below
                        string connString = @"Data Source=DESKTOP-NS5UOPR\SQLEXPRESS02;Initial Catalog=DFB;Integrated Security=true;";
                        SqlConnection conn = new SqlConnection(connString);

                        //open connection
                        conn.Open();

                        string sqlQuery = strBuilder.ToString();
                        using (SqlCommand command = new SqlCommand(sqlQuery, conn)) //pass SQL query created above and connection
                        {
                            command.ExecuteNonQuery(); //execute the Query
                                                       //Console.WriteLine("Query Executed.");
                        }

                        //Console.WriteLine("Connection successful!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                    strBuilder = new StringBuilder();
                    //update table and columns names
                    strBuilder.Append("INSERT INTO DFBFAS125_TBL (OFFICECODE , FUNDCODE ,FILENUMBER ,INSURANCENUMBER, STUBSURNAME,FIRSTINTIALFIRSTNAME, " +
      "FIRSTINTIALMIDDLENAME ,INSURANCECHANGE ,ACTIONEFFECTIVEDATE , DEDUCTIONAMOUNT, ESTABLSHEDAMOUNT, DISTRIBUTIONCODE ) VALUES ");
                }
            }
            file.Close();
            strBuilder.Length--;
            //insert remaining records into db
            try
            {
                Console.WriteLine("Openning Connection ...");

                string connString = @"Data Source=DESKTOP-NS5UOPR\SQLEXPRESS02;Initial Catalog=DFB;Integrated Security=true;";
                SqlConnection conn = new SqlConnection(connString);

                //open connection
                conn.Open();

                string sqlQuery = strBuilder.ToString();
                using (SqlCommand command = new SqlCommand(sqlQuery, conn)) //pass SQL query created above and connection
                {
                    command.ExecuteNonQuery(); //execute the Query
                                               //Console.WriteLine("Query Executed.");
                }

                //Console.WriteLine("Connection successful!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.  
            //System.Console.ReadLine();
            // }
        }
        public static String updateString(string baseValue, Dictionary<string, string> dic)
        {

            foreach (KeyValuePair<string, string> entry in dic)
            {
                baseValue = baseValue.Replace(entry.Key, entry.Value);
            }

            return baseValue;

        }
    }
}
