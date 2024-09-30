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
    public partial class Form6 : Form
    {
        private const string ConnectionString = "Server=DESKTOP-KO8C234\\SQLEXPRESS;Database=TFMdb;Integrated Security=True;";
        public Form6()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string workerID = txtWorkerID.Text.Trim();
            string advanceAmountText = txtAdvanceAmount.Text.Trim();

            // Check if all fields are not null or empty
            if (string.IsNullOrEmpty(workerID) || string.IsNullOrEmpty(advanceAmountText))
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

            // Check if advance amount is a valid integer
            if (!int.TryParse(advanceAmountText, out int advanceAmount) || advanceAmount <= 0)
            {
                MessageBox.Show("Please enter a valid advance amount.");
                return;
            }

            if (HasTakenAdvanceToday(workerID))
            {
                MessageBox.Show("You have taken advance today.");
                return;
            }

            // Insert data into WorkerAdvance table
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "INSERT INTO WorkerAdvance (WorkerID, AdvanceDate, AdvanceAmount) VALUES (@WorkerID, @AdvanceDate, @AdvanceAmount)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@WorkerID", workerID);
                command.Parameters.AddWithValue("@AdvanceDate", DateTime.Today);
                command.Parameters.AddWithValue("@AdvanceAmount", advanceAmount);

                connection.Open();
                command.ExecuteNonQuery();
            }

            MessageBox.Show("Advance recorded successfully.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtWorkerID.Text = string.Empty;
            txtAdvanceAmount.Text = string.Empty;
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

        private bool HasTakenAdvanceToday(string workerID)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM WorkerAdvance WHERE workerID = @workerID AND AdvanceDate = @AdvanceDate";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@workerID", workerID);
                command.Parameters.AddWithValue("@AdvanceDate", DateTime.Today);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        private void Form6_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void txtWorkerID_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
