using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokedexDatabase
{
    internal class DatabaseHandler
    {
        public string ConnectionString { get; set; }


        public DatabaseHandler(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public ObservableCollection<Pokemon> ExecuteSQLQueryRead(string query)
        {

            SqlConnection conn = new SqlConnection(ConnectionString);

            conn.Open();



            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = query;



            SqlDataReader reader = cmd.ExecuteReader();


            ObservableCollection<Pokemon> list = new ObservableCollection<Pokemon>();

            // Reading each line in the database
            // as long as there are lines to read, reader(read) returns true

            while (reader.Read())
            {
                Pokemon tempPokemon = new Pokemon();
                // Reading the ID
                tempPokemon.PokIDPK = reader.GetInt32(0);
                // reading PokID
                tempPokemon.PokDexID = reader.GetInt32(1);

                // Reading First name
                tempPokemon.PokName = reader.GetString(2);
                //Reading Description
                tempPokemon.PokDescription = reader.GetString(3);
                //Reading PokSize
                tempPokemon.PokSize = reader.GetDouble(4);
                // Reading PokWeight 
                tempPokemon.PokWeight = reader.GetDouble(5);
                // Reading PokRegIDFK
                tempPokemon.PokRegIDFK = reader.GetInt32(6);



                list.Add(tempPokemon);
            }


            conn.Close();

            return list;
        }

        // Pokemon Attacks

        public ObservableCollection<Attack> ExecuteSQLQueryReadAttack(string query, int? pokemonId = null)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = query;

            if (pokemonId.HasValue)
            {
                cmd.Parameters.AddWithValue("@PokIDFK", pokemonId.Value);
            }

            SqlDataReader reader = cmd.ExecuteReader();

            ObservableCollection<Attack> list = new ObservableCollection<Attack>();

            while (reader.Read())
            {
                Attack tempAttack = new Attack
                {
                    AttackID = reader.GetInt32(0),
                    AttTitel = reader.GetString(1),
                    AttDescription = reader.IsDBNull(2) ? "" : reader.GetString(2)
                };
                list.Add(tempAttack);
            }

            conn.Close();
            return list;
        }


        // Pokemon Types
        public ObservableCollection<Type> ExecuteSQLQueryReadType(string query)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = query;

            SqlDataReader reader = cmd.ExecuteReader();

            ObservableCollection<Type> list = new ObservableCollection<Type>();

            while (reader.Read())
            {
                Type tempType = new Type();
                tempType.TypeIDPK = reader.GetInt32(0);
                tempType.TypeDescription = reader.GetString(1);
                list.Add(tempType);
            }

            conn.Close();
            return list;
        }

        // Pokemon Regions

        public ObservableCollection<Region> ExecuteSQLQueryReadRegion(string query)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = query;

            SqlDataReader reader = cmd.ExecuteReader();

            ObservableCollection<Region> list = new ObservableCollection<Region>();

            while (reader.Read())
            {
                Region tempRegion = new Region();

                // Ensure RegIDPK exists in the result set before accessing it
                if (reader["RegIDPK"] != DBNull.Value)
                {
                    tempRegion.RegIDPK = Convert.ToInt32(reader["RegIDPK"]);
                }

                // Read the other columns
                tempRegion.RegDesignation = reader["RegDesignation"].ToString();
                tempRegion.RegDescription = reader["RegDescription"].ToString();

                list.Add(tempRegion);
            }

            conn.Close();
            return list;
        }

        public void ExecuteSQLQueryWrite(string query)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = query;

                // Executes the specified query
                cmd.ExecuteNonQuery();
            }
        }




    }
}
