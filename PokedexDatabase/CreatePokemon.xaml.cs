using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
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
    /// Interaction logic for CreatePokemon.xaml
    /// </summary>
    public partial class CreatePokemon : Window
    {
        private DatabaseHandler db;

        // Type values
        public ObservableCollection<Type> TypesObsv { get; set; }
    

        // Region values 

        public ObservableCollection<Region> RegionsObsv { get; set; }
        public int SelectedRegionID { get; set; } // Holds the selected region's ID

        // Attack values
        public ObservableCollection<Attack> AttacksObsv { get; set; }
        public List<int> SelectedAttackIDs { get; set; } = new List<int>(); // To store selected attack IDs

        // Pokemon values
        public int PokIDPK { get; set; }
        public int PokDexID { get; set; }

        public string PokName { get; set; }
        public string PokDescription { get; set; }
        public double PokSize { get; set; }
        public double PokWeight { get; set; }
        public int PokRegIDFK { get; set; }
        public CreatePokemon()
        {
            InitializeComponent();

            string connection = "Data Source=HannahLeah;Initial Catalog=PokeDex;Integrated Security=True;";

            TypesObsv = new ObservableCollection<Type>();
            RegionsObsv = new ObservableCollection<Region>();
            AttacksObsv = new ObservableCollection<Attack>();
            DataContext = this;

            db = new DatabaseHandler(connection);

            // Load pokemon types

            var temp = db.ExecuteSQLQueryReadType("SELECT * FROM Types;");

            foreach (var item in temp)

            {
                TypesObsv.Add(item);
            }

            // load pokemon regions

            var tempRegions = db.ExecuteSQLQueryReadRegion("SELECT * FROM Region;");
            foreach (var region in tempRegions)
            {
                RegionsObsv.Add(region);
            }

            // Load Pokemon Attacks

            var tempAttacks = db.ExecuteSQLQueryReadAttack("SELECT * FROM Attack;", -1);
            foreach (var attack in tempAttacks)
            {
                AttacksObsv.Add(attack);
            }
        }

        private void BtnCreatePokemon_Click(object sender, RoutedEventArgs e)
        {

             // ensure correct number formats
            if (!int.TryParse(TextBox_PokDexID.Text, out int pokDexID) || !double.TryParse(TextBox_PokSize.Text, out double pokSize) || !double.TryParse(TextBox_PokWeight.Text, out double pokWeight))
            {
                MessageBox.Show("Error: Pokemon ID, Size and Weight have to be numbers!",
                                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validation: Required fields cannot be empty
            if (PokDexID <= 0 || string.IsNullOrWhiteSpace(PokName) ||
                string.IsNullOrWhiteSpace(PokDescription) || SelectedRegionID <= 0)
            {
                MessageBox.Show("Error: PokDexID, PokName, PokDescription, and Region are required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; 
            }

            Pokemon tempPokemon = new Pokemon();
            tempPokemon.PokIDPK = PokIDPK;
            tempPokemon.PokDexID = PokDexID;
            tempPokemon.PokName = PokName;
            tempPokemon.PokDescription = PokDescription;
            tempPokemon.PokSize = PokSize;
            tempPokemon.PokWeight = PokWeight;

            // Use selected region
            tempPokemon.PokRegIDFK = SelectedRegionID;

            // Get selected attacks
            SelectedAttackIDs.Clear();
            foreach (Attack selectedAttack in LstAttacks.SelectedItems)
            {
                SelectedAttackIDs.Add(selectedAttack.AttackID);
            }

            // Ensure correct decimal format for SQL (replace ',' with '.')
            string pokSizeValue = PokSize > 0 ? PokSize.ToString(System.Globalization.CultureInfo.InvariantCulture) : "NULL";
            string pokWeightValue = PokWeight > 0 ? PokWeight.ToString(System.Globalization.CultureInfo.InvariantCulture) : "NULL";


            string query = $"INSERT INTO Pokemon (PokDexID, PokName, PokDescription, PokSize, PokWeight, PokRegIDFK) " +
                           $"VALUES ('{PokDexID}', '{PokName}', '{PokDescription}', {pokSizeValue}, {pokWeightValue}, '{SelectedRegionID}'); " +
                           "SELECT SCOPE_IDENTITY();"; // Gets the last inserted ID

            int newPokemonID;
            using (SqlConnection conn = new SqlConnection(db.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                newPokemonID = Convert.ToInt32(cmd.ExecuteScalar()); // Retrieve new Pokémon ID
            }

            // Insert selected attacks into PokAtt table
            foreach (int attackID in SelectedAttackIDs)
            {
                string attackQuery = $"INSERT INTO PokAtt (PokIDFK, AttIDFK) VALUES ('{newPokemonID}', '{attackID}')";
                db.ExecuteSQLQueryWrite(attackQuery);
            }

            // Insert Pokemon Types
            foreach (Type selectedType in ListBox_Type.SelectedItems)
            {
                string insertTypeQuery = $"INSERT INTO PokemonTypes (PoTyePokIDFK, PoTyTypeIDFK) VALUES ('{newPokemonID}', '{selectedType.TypeIDPK}');";
                db.ExecuteSQLQueryWrite(insertTypeQuery);
            }

            MessageBox.Show("Pokemon added successfully!");

            Close();
        }
    }
}
