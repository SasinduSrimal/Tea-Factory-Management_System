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
    public partial class Form3 : Form
    {
        

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Validate all fields
            if (string.IsNullOrWhiteSpace(txtNationalId.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtContactNo.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                cmbGender.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                dtpDateOfBirth.Value == DateTime.MinValue ||
                dtpDateOfJoin.Value == DateTime.MinValue)
            {
                MessageBox.Show("Please fill all the information.");
                return;
            }

            // Get the values from the form
            string nationalId = txtNationalId.Text;
            string name = txtName.Text;
            string contactNo = txtContactNo.Text;
            string email = txtEmail.Text;
            string gender = cmbGender.SelectedItem.ToString();
            string address = txtAddress.Text;
            DateTime dateOfBirth = dtpDateOfBirth.Value;
            DateTime dateOfJoin = dtpDateOfJoin.Value;

            // Connection string to  SQL Server database
            string connectionString = "Server=DESKTOP-KO8C234\\SQLEXPRESS;Database=TFMdb;Integrated Security=True;";

            // SQL query to check if the national id already exists
            string checkQuery = "SELECT COUNT(*) FROM workers WHERE national_id = @NationalId";

            // SQL query to insert a new worker record
            string insertQuery = "INSERT INTO workers (factory_id, national_id, name, contact_no, email, gender, address, date_of_birth, date_of_join) " +
                                 "VALUES (@FactoryId, @NationalId, @Name, @ContactNo, @Email, @Gender, @Address, @DateOfBirth, @DateOfJoin)";

            // Use SqlConnection and SqlCommand to check for existing national id
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@NationalId", nationalId);
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("This worker is already registered.");
                            return;
                        }
                    }

                    // Generate a unique ID for factory_id
                    string factoryId = GenerateUniqueId(connection);

                    // Use SqlCommand to insert data
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@FactoryId", factoryId);
                        insertCommand.Parameters.AddWithValue("@NationalId", nationalId);
                        insertCommand.Parameters.AddWithValue("@Name", name);
                        insertCommand.Parameters.AddWithValue("@ContactNo", contactNo);
                        insertCommand.Parameters.AddWithValue("@Email", email);
                        insertCommand.Parameters.AddWithValue("@Gender", gender);
                        insertCommand.Parameters.AddWithValue("@Address", address);
                        insertCommand.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                        insertCommand.Parameters.AddWithValue("@DateOfJoin", dateOfJoin);

                        insertCommand.ExecuteNonQuery();
                        MessageBox.Show($"Worker registered successfully! Factory ID: {factoryId}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private string GenerateUniqueId(SqlConnection connection)
        {
            string maxIdQuery = "SELECT MAX(factory_id) FROM workers WHERE factory_id LIKE 'CT/%'";
            using (SqlCommand command = new SqlCommand(maxIdQuery, connection))
            {
                var result = command.ExecuteScalar()?.ToString();
                if (string.IsNullOrEmpty(result))
                {
                    return "CT/0001";
                }

                // Extract the numeric part from the last generated ID
                string lastNumberPart = result.Substring(3); // Remove the 'CT/' part
                int lastNumber;
                if (int.TryParse(lastNumberPart, out lastNumber))
                {
                    return $"CT/{(lastNumber + 1).ToString("D4")}";
                }
                else
                {
                    throw new Exception("Invalid format of existing factory_id in the database.");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtNationalId.Clear();
            txtName.Clear();
            txtContactNo.Clear();
            txtEmail.Clear();
            cmbGender.SelectedIndex = -1;
            txtAddress.Clear();
            dtpDateOfBirth.Value = DateTime.Now;
            dtpDateOfJoin.Value = DateTime.Now;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}
