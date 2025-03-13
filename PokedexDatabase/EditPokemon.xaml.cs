using Microsoft.VisualBasic;
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
    /// Interaction logic for EditPokemon.xaml
    /// </summary>
    public partial class EditPokemon : Window
    {
        private DatabaseHandler db;
        private Pokemon _selectedItem;

        // Type values
        public ObservableCollection<Type> TypesObsv { get; set; }


        // Region values 

        public ObservableCollection<Region> RegionsObsv { get; set; }
        public int SelectedRegionID { get; set; } 

        // Attack values
        public ObservableCollection<Attack> AttacksObsv { get; set; }
        public List<int> SelectedAttackIDs { get; set; } = new List<int>(); 

        public EditPokemon(Pokemon selectedItem )
        {
            InitializeComponent();
            
            _selectedItem = selectedItem;
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

            // Changing the textboxes to match the pokemon that is being edited

            TextBox_PokDexID.Text = selectedItem.PokDexID.ToString();
            TextBox_PokName.Text = selectedItem.PokName;
            TextBox_PokDescription.Text = selectedItem.PokDescription;
            TextBox_PokSize.Text = selectedItem.PokSize.ToString();
            TextBox_PokWeight.Text = selectedItem.PokWeight.ToString();

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

            // ensure correct number formats
            if (!int.TryParse(TextBox_PokDexID.Text, out int pokDexID) || !double.TryParse(TextBox_PokSize.Text, out double pokSize) || !double.TryParse(TextBox_PokWeight.Text, out double pokWeight))
            {
                MessageBox.Show("Error: Pokemon ID, Size and Weight have to be numbers!",
                                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // make sure the important fields are not empty
            if (string.IsNullOrWhiteSpace(TextBox_PokDexID.Text) || string.IsNullOrWhiteSpace(TextBox_PokName.Text) ||
                string.IsNullOrWhiteSpace(TextBox_PokDescription.Text) || SelectedRegionID <= 0)
            {
                MessageBox.Show("Error: PokDexID, PokName, PokDescription, and Region are required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _selectedItem.PokDexID = int.Parse(TextBox_PokDexID.Text);
            _selectedItem.PokName = TextBox_PokName.Text;
            _selectedItem.PokDescription = TextBox_PokDescription.Text;
            _selectedItem.PokSize = double.Parse(TextBox_PokSize.Text);
            _selectedItem.PokWeight = double.Parse(TextBox_PokWeight.Text);
            _selectedItem.PokRegIDFK = SelectedRegionID;

            // Get selected attacks
            SelectedAttackIDs.Clear();
            foreach (Attack selectedAttack in LstAttacks.SelectedItems)
            {
                SelectedAttackIDs.Add(selectedAttack.AttackID);
            }

            // Ensure correct decimal format for SQL (replace ',' with '.')
            string pokSizeValue = _selectedItem.PokSize > 0 ? _selectedItem.PokSize.ToString(System.Globalization.CultureInfo.InvariantCulture) : "NULL";
            string pokWeightValue = _selectedItem.PokWeight > 0 ? _selectedItem.PokWeight.ToString(System.Globalization.CultureInfo.InvariantCulture) : "NULL";

            var query = $"UPDATE Pokemon SET PokDexID = '{_selectedItem.PokDexID}', PokName = '{_selectedItem.PokName}', PokDescription = '{_selectedItem.PokDescription}', PokSize = '{pokSizeValue}', PokWeight = '{pokWeightValue}', PokRegIDFK = '{_selectedItem.PokRegIDFK}'  WHERE PokIDPK = {_selectedItem.PokIDPK};";
            int newPokemonID;
            using (SqlConnection conn = new SqlConnection(db.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                newPokemonID = Convert.ToInt32(cmd.ExecuteScalar()); // Retrieve new Pokémon ID
            }

            // Remove existing attacks for the pokemon
            string deleteAttacksQuery = $"DELETE FROM PokAtt WHERE PokIDFK = {_selectedItem.PokIDPK};";
            db.ExecuteSQLQueryWrite(deleteAttacksQuery);

            // Insert new attacks
            foreach (int attackID in SelectedAttackIDs)
            {
                string insertAttackQuery = $"INSERT INTO PokAtt (PokIDFK, AttIDFK) VALUES ({_selectedItem.PokIDPK}, {attackID});";
                db.ExecuteSQLQueryWrite(insertAttackQuery);
            }

            // Remove existing types for the pokemon
            string deleteTypesQuery = $"DELETE FROM PokemonTypes WHERE PoTyePokIDFK = {_selectedItem.PokIDPK};";
            db.ExecuteSQLQueryWrite(deleteTypesQuery);

            // Insert new types
            foreach (Type selectedType in ListBox_Type.SelectedItems)
            {
                string insertTypeQuery = $"INSERT INTO PokemonTypes (PoTyePokIDFK, PoTyTypeIDFK) VALUES ({_selectedItem.PokIDPK}, {selectedType.TypeIDPK});";
                db.ExecuteSQLQueryWrite(insertTypeQuery);
            }

            MessageBox.Show("Pokemon edited!");

            Close();


        }
    }
}
