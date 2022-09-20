using System;
using System.Linq;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace UI
{
    partial class DB
    {
        private MySqlConnection conn;
        private void Initialize()
        {
            string s = "127.0.0.1";
            string db = "db";
            string u = "root";
            string p = "root";
            string c;
            c = "SERVER=" + s + ";" + "DATABASE=" + db + ";" + "USERNAME=" + u + ";" + "PASSWORD=" + p + ";";
            conn = new MySqlConnection(c);
            //Console.WriteLine("Database has connected!");
        }
        //deschide conxiunea la baza de date
        private bool OpenConnection()
        {
            Initialize();
            try
            {
                conn.Open();
                //Console.WriteLine("Server has connected");
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        //inchide conxiunea la baza de date
        private bool CloseConnection()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        //comanda sql select
        public List<string>[] Select(String table, List<String> fields)
        {
            string query = "SELECT * FROM " + table;
            List<string>[] list = new List<string>[fields.Count];
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = new List<string>();
            }
            if (OpenConnection() != true) return list;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                for (int i = 0; i < list.Length; i++)
                {
                    list[i].Add(dataReader[fields[i]].ToString());
                }
            }
            dataReader.Close();
            CloseConnection();
            return list;
        }
        //selecteza coloana
        public List<string> GetColumn(String table)
        {
            string query = "SELECT column_name FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = 'db' and table_name =  '" + table + "'";
            List<string> list = new List<string>();
            if (OpenConnection() != true) return list;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                list.Add(dataReader["COLUMN_NAME"].ToString());
            }
            dataReader.Close();
            CloseConnection();
            return list;
        }
        //adauga cheie primara daca nu exista
        public void AddPrimaryKey(String table)
        {
            List<string> list = GetPrKey(table);
            if (list.Count == 0)
            {
                AddPrKey(table);
                return;
            }
        }
        //verifica cheie primara, daca nu e numerica schimba
        public void CheckPrimaryKey(String table)
        {
            List<string> list = GetPrKey(table);
            if (list.Count == 1 && CheckPrKey(table, list[0].ToString()))
            {
                ChangePrKey(table, list[0].ToString());
                return;
            }
        }
        //verifica cheie primara, daca nu e singulara schimba
        public void ReducePrimaryKey(String table)
        {
            List<string> list = GetPrKey(table);
            if (list.Count > 1)
            {
                ChangePrKey(table, list[0].ToString());
                return;
            }
        }
        //reseteaza baza de date
        public void Reset(String table)
        {
            string query = "DROP TABLE db.T5;DROP TABLE db.T4;DROP TABLE db.T3;DROP TABLE db.T2;DROP TABLE db.T1;"
                + "CREATE TABLE db.T1 (UserName VARCHAR(255) NOT NULL,firstname VARCHAR(255) NOT NULL,lastname VARCHAR(255) NOT NULL,PRIMARY KEY(UserName));"
                + "CREATE TABLE db.T2 (UserName VARCHAR(255) NOT NULL,UserId1 INT NOT NULL,UserId2 INT NOT NULL,PRIMARY KEY(UserId1, UserId2),FOREIGN KEY (UserName) REFERENCES db.T1(UserName));"
                + "CREATE TABLE db.T3 (UserName VARCHAR(255) NOT NULL,UserId1 INT NOT NULL,UserId2 INT NOT NULL,PRIMARY KEY(UserId1, UserId2),FOREIGN KEY (UserName) REFERENCES db.T1(UserName));"
                + "CREATE TABLE db.T4 (UserId1 INT NOT NULL,UserId2 INT NOT NULL,message VARCHAR(255) NOT NULL,FOREIGN KEY (UserId1, UserId2) REFERENCES db.T2(UserId1, UserId2));"
                + "CREATE TABLE db.T5 (UserId1 INT NOT NULL,UserId2 INT NOT NULL,message VARCHAR(255) NOT NULL,FOREIGN KEY (UserId1, UserId2) REFERENCES db.T2(UserId1, UserId2));"
                + "INSERT INTO db.T1 VALUES ('UserA','UserA1','UserA2'),('UserB','UserB1','UserB2'),('UserC','UserC1','UserC2'),('UserD','UserD1','UserD2'),('UserE','UserE1','UserE2'),('UserF','UserF1','UserF2');"
                + "INSERT INTO db.T2 VALUES ('UserA',1,2),('UserB',2,3),('UserC',3,1);"
                + "INSERT INTO db.T3 VALUES ('UserD',4,5),('UserE',5,6),('UserF',7,4);"
                + "INSERT INTO db.T4 VALUES (1,2,'MsgA'),(2,3,'MsgB'),(3,1,'MsgC');"
                + "INSERT INTO db.T5 VALUES (1,2,'MsgD'),(2,3,'MsgE'),(3,1,'MsgF');";
            if (OpenConnection() != true) return;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }
    }
}
