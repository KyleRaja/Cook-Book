using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration; // added reference
using System.Data.SqlClient; // added reference to work with SQL database(classes and objects).

namespace Cookbook

    // added database and connected it to data set. 
       
    
{
    public partial class frmMain : Form
    {
        SqlConnection connection; // open connection to sql server database.
        string connectionString; // string tells us: info on where the data base is, how to connect to it, what credentials need to be used.
        public frmMain()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["Cookbook.Properties.Settings.CookBookConnectionString"].ConnectionString; // From the app.config file
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            PopulateRecipes(); // calls method 
            PopulateAllIngredients(); // calls method
        }

        private void PopulateRecipes()
        {
            using (connection = new SqlConnection(connectionString)) // object connection = open connection to sql server database. using statement will auto close connection. IDisposable.
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Recipe", connection)) // (Parameter: sql query, connection). SqlDataAdapter auto opens connection. Also IDisposable.
            
            {
                // both using statements use this code block.
                DataTable recipeTable = new DataTable(); // DataTable is C# object to hold the data returned by sql query above.
                adapter.Fill(recipeTable); // Adds or refreshes rows in Data table to match data in source.
                lstRecipes.DisplayMember = "Name"; // Displays name
                lstRecipes.ValueMember = "Id"; // Displays ID. Used for reference.
                lstRecipes.DataSource = recipeTable; // linking list box to data table.

            }
        }

        private void PopulateAllIngredients() // method to handle list for all ingredients
        {
            using (connection = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Ingredient", connection)) // now selecting from ingredients

            {
                DataTable ingredientTable = new DataTable();
                adapter.Fill(ingredientTable); // filling ingredient table 
                lstAllIngredients.DisplayMember = "Name"; 
                lstAllIngredients.ValueMember = "Id";
                lstAllIngredients.DataSource = ingredientTable; // linking list box to ingredient table

            }
        }

        private void PopulateIngredients()
        {
           
            string query = "SELECT a.Name FROM Ingredient a " + // selecting name from Ingredient, a is the alias.             
                "INNER JOIN RecipeIngredient b ON a.Id = b.IngredientId " + // Joining Recipe ingredient with Id.
                "WHERE b.RecipeId = @RecipeId"; // @RecipeId(parameter) means each time query is run we can pass any value with @RecipeId.

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection)) // query is passed in. Command handles select query and support parameters.
            using (SqlDataAdapter adapter = new SqlDataAdapter(command)) // Now I just have to pass in command rather than (query, connection). 

            {
               
                //Defining parameters
                command.Parameters.AddWithValue("@RecipeId", lstRecipes.SelectedValue); // Parameter name: @RecipeId, Value: recipe selected in the list box(ID) ->passed to query -> returns ingredients tied to recipe.
                DataTable ingredientTable = new DataTable();
                adapter.Fill(ingredientTable); // filling table
                lstIngredients.DisplayMember = "Name";
                lstIngredients.ValueMember = "Id";
                lstIngredients.DataSource = ingredientTable; // linking list box to table

            }
        }

        private void lstRecipes_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateIngredients(); // method is called in here because this needs to happen when the selected index has changed. Changed everytime the query is repopulated with a different value.
        }


        private void btnAddRecipe_Click_1(object sender, EventArgs e) // button on form to add recipe.
        {
            string query = "INSERT INTO Recipe VALUES(@REcipeName,80,'blah blah')"; // Insert query. Need name(parameter) Time:80 min, instuctions: irrelevant.

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection)) // command also used for inserts and updates
            {
                //Not using SqlAdapter so must open and close the sql connection manually
                connection.Open();
                command.Parameters.AddWithValue("@RecipeName", txtRecipeName.Text); // get value from txtRecipeName
                command.ExecuteNonQuery(); // execute is a sql statement against the connection and returns the number of rows affected.
            }
            PopulateRecipes();
        }

        private void btnUpdateRecipeName_Click_1(object sender, EventArgs e) // button to update recipe name
        {
            string query = "UPDATE Recipe SET Name = @RecipeName WHERE Id = @RecipeId"; // Update query. 

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Not using SqlAdapter so must open and close the sql connection manually
                connection.Open();
                command.Parameters.AddWithValue("@RecipeName", txtRecipeName.Text);
                command.Parameters.AddWithValue("@RecipeId", lstRecipes.SelectedValue); // same thing I did above on line 80
                command.ExecuteNonQuery();
                //change mdf file in properties to "copy if newer"
            }
            PopulateRecipes(); // repopulate automatically so the recipies are saved.
        }

        private void btnAddToRecipe_Click_1(object sender, EventArgs e) // button to add to recipe
        {
            string query = "INSERT INTO RecipeIngredient VALUES(@RecipeId,@IngredientId)";

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                //Not using SqlAdapter so must open and close the sql connection manually
                connection.Open();
                command.Parameters.AddWithValue("@RecipeId", lstRecipes.SelectedValue);
                command.Parameters.AddWithValue("@IngredientId", lstAllIngredients.SelectedValue);
                command.ExecuteNonQuery();
            }
            PopulateRecipes();
        }
    }
}