using System;
using System.Linq;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace UI
{
    partial class DB
    {
        //selecteza chei primare
        public List<string> GetPrKey(String table)
        {
            string query = "SELECT column_name FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = 'db'"
                + "and table_name='" + table + "' AND CONSTRAINT_NAME='PRIMARY'";
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
        //adauga cheie primara
        private void AddPrKey(String table)
        {
            string query = "ALTER TABLE db." + table + " add column id_" + table + " INT auto_increment not null, add primary key(id_" + table + ")";
            if (OpenConnection() != true) return;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }
        //verifica tipul cheii primare
        private bool CheckPrKey(String table, String column)
        {
            String[] dt = new String[12] { "bit", "tinyint", "smallint", "mediumint", "int", "integer", "bigint", "float", "double", "double precision", "decimal", "dec" };
            string query = "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE table_schema='db' and table_name = '" + table + "' AND COLUMN_NAME = '" + column + "';";
            if (OpenConnection() != true) { return false; }
            List<string> list = new List<string>();
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                list.Add(dataReader["DATA_TYPE"].ToString());
            }
            dataReader.Close();
            CloseConnection();
            if (dt.Contains(list[0].ToString())) { return false; }
            return true;
        }
        //schimba cheia primara
        public void ChangePrKey(String table, String pk)
        {
            //search FK
            string query = "SELECT TABLE_NAME, CONSTRAINT_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = 'db'"
                + "AND REFERENCED_TABLE_NAME = '" + table + "'" + "AND REFERENCED_COLUMN_NAME = '" + pk + "'";
            if (OpenConnection() != true) return;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            List<string>[] list = new List<string>[2];
            list[0] = new List<string>();
            list[1] = new List<string>();
            while (dataReader.Read())
            {
                list[0].Add(dataReader["TABLE_NAME"].ToString());
                list[1].Add(dataReader["CONSTRAINT_NAME"].ToString());
            }
            dataReader.Close();
            for (int i = 0; i < list[0].Count; i++)
            {
                //drop FK and add tempFK
                query = "ALTER TABLE db." + list[0][i] + " drop foreign key " + list[1][i] + ", add column id_" + list[0][i] + "_" + table + " INT not null";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            //drop old PK and add new PK
            query = "ALTER TABLE db." + table + " drop primary key, add column id_" + table + " INT auto_increment not null, add primary key(id_" + table + ")";
            //Console.WriteLine(query);
            cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            for (int i = 0; i < list[0].Count; i++)
            {
                //set tempFK
                query = "update db." + list[0][i] + " t1 inner join db." + table + " t2 on t1." + pk + "=t2." + pk + " set t1.id_" + list[0][i] + "_" + table + "=t2.id_" + table;
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                //add FK constraint
                query = "ALTER TABLE db." + list[0][i] + " add foreign key(id_" + list[0][i] + "_" + table + ") REFERENCES db." + table + "(id_" + table+")";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            CloseConnection();
        }
    }
}
