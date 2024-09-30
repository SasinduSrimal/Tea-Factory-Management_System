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
    public partial class Form12 : Form
    {
        private const string ConnectionString = "Server=DESKTOP-KO8C234\\SQLEXPRESS;Database=TFMdb;Integrated Security=True;";
        public Form12()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string SupplierID = txtSupplierID.Text.Trim();
            string teabagAmountText = txtTeabagAmount.Text.Trim();

            // Check if all fields are not null or empty
            if (string.IsNullOrEmpty(SupplierID) || string.IsNullOrEmpty(teabagAmountText))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            // Check if worker is registered
            if (!IsWorkerRegistered(SupplierID))
            {
                MessageBox.Show("SupplierID is not registered.");
                return;
            }

            // Check if teabag amount is a valid integer
            if (!int.TryParse(teabagAmountText, out int teabagAmount) || teabagAmount <= 0)
            {
                MessageBox.Show("Please enter a valid teabag amount.");
                return;
            }

            if (HasTakenteabagsToday(SupplierID))
            {
                MessageBox.Show("You have taken teabags today.");
                return;
            }

            // Insert data into TeabagBorrowing table
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "INSERT INTO TeaBagBorrowingsforsuppl (SupplierID, BorrowingDate, TeaBagAmount) VALUES (@SupplierID, @BorrowingDate, @TeabagAmount)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierID", SupplierID);
                command.Parameters.AddWithValue("@BorrowingDate", DateTime.Today);
                command.Parameters.AddWithValue("@TeaBagAmount", teabagAmount);

                connection.Open();
                command.ExecuteNonQuery();
            }

            MessageBox.Show("Teabag borrowing recorded successfully.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtSupplierID.Text = string.Empty;
            txtTeabagAmount.Text = string.Empty;
        }

        private bool IsWorkerRegistered(string SupplierID)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM Suppliers WHERE SupplierID = @SupplierID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierID", SupplierID);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        private bool HasTakenteabagsToday(string SupplierID)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM TeaBagBorrowingsforsuppl WHERE SupplierID = @SupplierID AND BorrowingDate = @BorrowingDate";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierID", SupplierID);
                command.Parameters.AddWithValue("@BorrowingDate", DateTime.Today);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                return count > 0;
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
