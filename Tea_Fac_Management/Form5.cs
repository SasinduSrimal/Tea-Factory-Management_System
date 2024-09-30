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
    public partial class Form5 : Form
    {
        private const string ConnectionString = "Server=DESKTOP-KO8C234\\SQLEXPRESS;Database=TFMdb;Integrated Security=True;";
        public Form5()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string workerID = txtWorkerID.Text.Trim();
            string teabagAmountText = txtTeabagAmount.Text.Trim();

            // Check if all fields are not null or empty
            if (string.IsNullOrEmpty(workerID) || string.IsNullOrEmpty(teabagAmountText))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            // Check if worker is registered
            if (!IsWorkerRegistered(workerID))
            {
                MessageBox.Show("Worker is not registered.");
                return;
            }

            // Check if teabag amount is a valid integer
            if (!int.TryParse(teabagAmountText, out int teabagAmount) || teabagAmount <= 0)
            {
                MessageBox.Show("Please enter a valid teabag amount.");
                return;
            }

            if (HasTakenteabagsToday(workerID))
            {
                MessageBox.Show("You have taken teabags today.");
                return;
            }

            // Insert data into TeabagBorrowing table
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "INSERT INTO TeabagBorrowing (WorkerID, BorrowDate, TeabagAmount) VALUES (@WorkerID, @BorrowDate, @TeabagAmount)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@WorkerID", workerID);
                command.Parameters.AddWithValue("@BorrowDate", DateTime.Today);
                command.Parameters.AddWithValue("@TeabagAmount", teabagAmount);

                connection.Open();
                command.ExecuteNonQuery();
            }

            MessageBox.Show("Teabag borrowing recorded successfully.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtWorkerID.Text = string.Empty;
            txtTeabagAmount.Text = string.Empty;
        }

        private bool IsWorkerRegistered(string workerID)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM workers WHERE Factory_ID = @WorkerID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@WorkerID", workerID);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        private bool HasTakenteabagsToday(string workerID)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM TeabagBorrowing WHERE workerID = @workerID AND BorrowDate = @BorrowDate";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@workerID", workerID);
                command.Parameters.AddWithValue("@BorrowDate", DateTime.Today);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
    
}
