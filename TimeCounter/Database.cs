

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace TimeCounter
{
    class Database
    {
       
        static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db3");
        static string dbpath = string.Format("DataSource={0};", dbPath);
        static SqliteConnection con = new SqliteConnection(dbpath);
         static SqliteCommand command = new SqliteCommand();
        static SqliteCommand testcommand = new SqliteCommand();
        public static void SendData(Dictionary<string, long[]> Dic)
        {
            con.Open();
           foreach(KeyValuePair<string,long[]> item in Dic)
            {
                long time = item.Value[1] - item.Value[0];
                command.Parameters.Clear();
                command.Connection = con;

                try {

                    command.CommandText = string.Format("SELECT * FROM {0}", item.Key);
                    command.Prepare();
                }
                catch(SqliteException ex)
                {
                    Console.WriteLine(ex);
                    CreateTable(item.Key);
                }
                command.Parameters.Clear();
                //command.CommandText =string.Format( "CREATE TABLE {0} (Day DATE,UsageTime INTEGER)",item.Key);
                int inRecord = GetRecordByDay(item.Key, DateTime.Now.ToString("MM/dd/yyyy"));
                if(inRecord == -1)
                {
                    command.CommandText = string.Format("INSERT INTO {0} (Day,UsageTime) VALUES(@day,@usagetime)", item.Key);
                    command.Parameters.AddWithValue("@day", DateTime.Now.ToString("MM/dd/yyyy"));
                    command.Parameters.AddWithValue("@usagetime", time);
                }
                else
                {
                    command.CommandText = string.Format("UPDATE {0} SET UsageTime=@usagetime WHERE Day=@day", item.Key);
                    command.Parameters.AddWithValue("@day", DateTime.Now.ToString("MM/dd/yyyy"));
                    command.Parameters.AddWithValue("@usagetime", time + inRecord);
                }

                

                Console.WriteLine(command.CommandText);
                Console.WriteLine(command.Parameters);
                command.Prepare();
                command.ExecuteNonQuery();
             }
            

        }
        
        public static int GetRecordByDay(string AppName, string day)
        {
        
            command.CommandText = string.Format("SELECT * FROM {0} WHERE Day='{1}'",AppName,day);
            command.Connection = con;
            command.Prepare();
            SqliteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    
                    var i=  reader.GetInt32(1);
                    reader.Close();
                    return i;
                }
            }
            reader.Close();
            return -1;
        }
       public static  int GetAppTime(string appname,bool sum = true,string from="", string to="")
        {
            con.Open();
            command.Parameters.Clear();
            command.Connection = con;
            int UsageTime=0;
            if (sum)
            {
                command.CommandText = string.Format("SELECT SUM(UsageTime) FROM {0}", appname);
            }
            else
            {
                command.CommandText = string.Format("SELECT SUM(UsageTime) FROM {0} WHERE Day BETWEEM #{1}# AND #{2}", appname, from, to);
            }
            try { command.Prepare(); } catch { return 0; }
            
            SqliteDataReader Resoult = command.ExecuteReader();
            try
            {
                while (Resoult.Read())
                {
                    UsageTime = Resoult.GetInt16(0);
                }
            }
            finally
            {
                Resoult.Close();
                con.Close();
                
            }

            return UsageTime;
        }
        private static void CreateTable(string tablename)
        {
            command.Parameters.Clear();
            command.Connection = con;
            command.CommandText = string.Format("CREATE TABLE {0} (Day DATE,UsageTime INTEGER)", tablename);
            command.Prepare();
            command.ExecuteNonQuery();

        }
        public static void CreateDatabase()
        {
            string dbpath = string.Format( "DataSource={0};", Path.Combine(Environment.CurrentDirectory, "database.db3"));
            SqliteConnection con = new SqliteConnection(dbpath);




        }
    }
}
