using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Tea_Fac_Management
{
    public partial class Form10 : Form
    {
        private const string ConnectionString = "Server=DESKTOP-KO8C234\\SQLEXPRESS;Database=TFMdb;Integrated Security=True;";
        public Form10()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Validate all fields
            if (string.IsNullOrWhiteSpace(txtNationalId.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtContactNo.Text) ||
                string.IsNullOrWhiteSpace(txtLocation.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please fill all the information.");
                return;
            }

            // Get the values from the form
            string nationalId = txtNationalId.Text.Trim();
            string name = txtName.Text.Trim();
            string contactNo = txtContactNo.Text.Trim();
            string location = txtLocation.Text.Trim();
            string email = txtEmail.Text.Trim();

            // SQL query to check if the national_id already exists
            string checkQuery = "SELECT COUNT(*) FROM Suppliers WHERE NationalID = @NationalID";

            // SQL query to insert a new supplier record
            string insertQuery = "INSERT INTO Suppliers (SupplierID, NationalID, Name, ContactNo, Location, Email) " +
                                 "VALUES (@SupplierID, @NationalID, @Name, @ContactNo, @Location, @Email)";

            // Use SqlConnection and SqlCommand to check for existing NationalID
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Check if the national_id already exists
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@NationalID", nationalId);
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("This Supplier is already registered.");
                            return;
                        }
                    }

                    // Generate a unique SupplierID starting from CT/S/00001
                    string supplierId = GenerateSupplierId(connection);

                    // Insert new supplier record
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@SupplierID", supplierId);
                        insertCommand.Parameters.AddWithValue("@NationalID", nationalId);
                        insertCommand.Parameters.AddWithValue("@Name", name);
                        insertCommand.Parameters.AddWithValue("@ContactNo", contactNo);
                        insertCommand.Parameters.AddWithValue("@Location", location);
                        insertCommand.Parameters.AddWithValue("@Email", email);

                        insertCommand.ExecuteNonQuery();
                        MessageBox.Show($"Supplier registered successfully! Supplier ID: {supplierId}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Clear all input fields
            txtNationalId.Clear();
            txtName.Clear();
            txtContactNo.Clear();
            txtLocation.Clear();
            txtEmail.Clear();
        }

        private string GenerateSupplierId(SqlConnection connection)
        {
            string maxIdQuery = "SELECT MAX(SupplierID) FROM Suppliers WHERE SupplierID LIKE 'CT/S/%'";
            using (SqlCommand command = new SqlCommand(maxIdQuery, connection))
            {
                var result = command.ExecuteScalar()?.ToString();
                if (string.IsNullOrEmpty(result))
                {
                    return "CT/S/00001";
                }

                // Extract the numeric part from the last generated ID
                string lastNumberPart = result.Substring(6); // Remove the 'CT/S/' part
                int lastNumber;
                if (int.TryParse(lastNumberPart, out lastNumber))
                {
                    return $"CT/S/{(lastNumber + 1).ToString("D4")}";
                }
                else
                {
                    throw new Exception("Invalid format of existing SupplierID in the database.");
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}
