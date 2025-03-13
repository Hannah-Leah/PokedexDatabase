using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.DirectoryServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PokedexDatabase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
   
    public partial class MainWindow : Window
    {
        private Pokemon selectedItem;

        public ObservableCollection<Pokemon> PokemonsObsv { get; set; }
        private DatabaseHandler db;
        public MainWindow()
        {
            InitializeComponent();
            string connection = "Data Source=HannahLeah;Initial Catalog=PokeDex;Integrated Security=True;";

            PokemonsObsv = new ObservableCollection<Pokemon>();
            DataContext = this;

            db = new DatabaseHandler(connection);

            PokemonsObsv.Clear();
            var temp = db.ExecuteSQLQueryRead("SELECT * FROM Pokemon;");

            foreach (var item in temp)
            
            {
                PokemonsObsv.Add(item);
            }
        }

        private void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = (Pokemon)ListData.SelectedItem;

        }

        /// <summary>
        /// Create Pokemon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            CreatePokemon createPokemon = new CreatePokemon();
            createPokemon.Show();

            

        }

        public void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            PokemonsObsv.Clear();
            var temp = db.ExecuteSQLQueryRead("SELECT * FROM Pokemon;");

            foreach (var item in temp)

            {
                PokemonsObsv.Add(item);
            }
        }

        /// <summary>
        /// Edit Pokemon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null)
            {
                EditPokemon editPokemon = new EditPokemon(selectedItem);
                editPokemon.Show();
                
            }
            else
            {
                MessageBox.Show("Please select a pokemon.");
            }

            ListData.UnselectAll();
        }

        /// <summary>
        /// Show pokemon details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ListData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PokemonDetails detailsWindow = new PokemonDetails(selectedItem.PokIDPK);
            detailsWindow.Show();
        }

        /// <summary>
        /// Delete Pokemon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem == null)
            {
                MessageBox.Show("Please select an item to delete.");
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete: {selectedItem.PokName}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                int pokemonID = selectedItem.PokIDPK;

                string query = $"DELETE FROM PokAtt WHERE PokIDFK = {pokemonID};" + $"DELETE FROM PokemonTypes WHERE PoTyePokIDFK = {pokemonID};" + $"DELETE FROM Pokemon WHERE PokIDPK = {pokemonID};";
                  db.ExecuteSQLQueryWrite(query);

                PokemonsObsv.Remove(selectedItem);
            }
        }
    }
}