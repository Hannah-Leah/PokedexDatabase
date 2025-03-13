using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PokedexDatabase
{
    /// <summary>
    /// Interaction logic for PokemonDetails.xaml
    /// </summary>
    public partial class PokemonDetails : Window
    {
        public ObservableCollection<Attack> Attacks { get; set; }
        public ObservableCollection<Type> Types { get; set; }

        public ObservableCollection<Region> Regions { get; set; }

        private DatabaseHandler db;
        public PokemonDetails(int pokemonId)
        {
            InitializeComponent();
            string connection = "Data Source=HannahLeah;Initial Catalog=PokeDex;Integrated Security=True;";
          
            Attacks = new ObservableCollection<Attack>();
            Types = new ObservableCollection<Type>();
            Regions = new ObservableCollection<Region>();

            db = new DatabaseHandler(connection);

            LoadAttacksForPokemon(pokemonId);
            LoadTypesForPokemon(pokemonId);
            LoadRegionForPokemon(pokemonId);

            AttackListView.ItemsSource = Attacks;
            TypeListView.ItemsSource = Types;
            RegionListView.ItemsSource = Regions;
        }

        // Attacks
        private void LoadAttacksForPokemon(int pokemonId)
        {
           
            string query = @"
                SELECT a.AttackID, a.AttTitel, a.AttDescription
                FROM Attack a
                JOIN PokAtt pa ON a.AttackID = pa.AttIDFK
                WHERE pa.PokIDFK = @PokIDFK";

           
            var attacks = db.ExecuteSQLQueryReadAttack(query, pokemonId);
            foreach (var attack in attacks)
            {
                Attacks.Add(attack);
            }
        }

        // Types

        private void LoadTypesForPokemon(int pokemonId)
        {
            string query = $@"
            SELECT t.TypeIDPK, t.TypeDescription
            FROM Types t
            JOIN PokemonTypes pt ON t.TypeIDPK = pt.PoTyTypeIDFK
            WHERE pt.PoTyePokIDFK = {pokemonId};";

            var types = db.ExecuteSQLQueryReadType(query);
            foreach (var type in types)
            {
                Types.Add(type);
            }
        }

        // Regions 

        private void LoadRegionForPokemon(int pokemonId)
        {
            // Query to fetch the region information for the selected Pokémon
            string query = $@"
        SELECT r.RegIDPK, r.RegDesignation, r.RegDescription
        FROM Region r
        JOIN Pokemon p ON r.RegIDPK = p.PokRegIDFK
        WHERE p.PokIDPK = {pokemonId};";

            try
            {
                var regions = db.ExecuteSQLQueryReadRegion(query);

                if (regions.Count > 0)
                {
                    foreach (var region in regions)
                    {
                        Regions.Add(region);
                    }
                }
                else
                {
                    // Handle case where no region is found (i.e., the Pokémon has no associated region)
                    MessageBox.Show("No region data found for this Pokémon.");
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur during the database query execution
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }


    }
}
