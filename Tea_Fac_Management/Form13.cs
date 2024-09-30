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

namespace Tea_Fac_Management
{
    public partial class Form13 : Form
    {
        private const string ConnectionString = "Server=DESKTOP-KO8C234\\SQLEXPRESS;Database=TFMdb;Integrated Security=True;";
        public Form13()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string SupplierID = txtSupplierID.Text.Trim();
            string advanceAmountText = txtAdvanceAmount.Text.Trim();

            // Check if all fields are not null or empty
            if (string.IsNullOrEmpty(SupplierID) || string.IsNullOrEmpty(advanceAmountText))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            // Check if worker is registered
            if (!IsSupplierRegistered(SupplierID))
            {
                MessageBox.Show("Supplier is not registered.");
                return;
            }

            // Check if advance amount is a valid integer
            if (!int.TryParse(advanceAmountText, out int advanceAmount) || advanceAmount <= 0)
            {
                MessageBox.Show("Please enter a valid advance amount.");
                return;
            }

            if (HasTakenAdvanceToday(SupplierID))
            {
                MessageBox.Show("You have taken advance today.");
                return;
            }

            // Insert data into WorkerAdvance table
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "INSERT INTO SupplierAdvanced (SupplierID, AdvanceDate, AdvanceAmount) VALUES (@SupplierID, @AdvanceDate, @AdvanceAmount)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierID", SupplierID);
                command.Parameters.AddWithValue("@AdvanceDate", DateTime.Today);
                command.Parameters.AddWithValue("@AdvanceAmount", advanceAmount);

                connection.Open();
                command.ExecuteNonQuery();
            }

            MessageBox.Show("Advance recorded successfully.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtSupplierID.Text = string.Empty;
            txtAdvanceAmount.Text = string.Empty;
        }

        private bool IsSupplierRegistered(string supplierId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM Suppliers WHERE SupplierID = @SupplierID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierID", supplierId);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        private bool HasTakenAdvanceToday(string supplierId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM SupplierAdvanced WHERE SupplierID = @SupplierID AND AdvanceDate = @AdvanceDate";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierID", supplierId);
                command.Parameters.AddWithValue("@AdvanceDate", DateTime.Today);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }





        private void Form13_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            txtSupplierID.Text = string.Empty;
            txtAdvanceAmount.Text = string.Empty;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
    }

