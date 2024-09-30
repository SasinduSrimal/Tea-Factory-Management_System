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
    public partial class Form7 : Form
    {
        private const string ConnectionString = "Server=DESKTOP-KO8C234\\SQLEXPRESS;Database=TFMdb;Integrated Security=True;";
        public Form7()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string workerID = txtWorkerID.Text.Trim();

            if (string.IsNullOrEmpty(workerID))
            {
                MessageBox.Show("Please enter a valid worker ID.");
                return;
            }

            if (!IsWorkerRegistered(workerID))
            {
                MessageBox.Show("Worker is not registered.");
                return;
            }

            int totalWorkingHours = GetTotalWorkingHours(workerID);
            int totalTeabagAmount = GetTotalTeabagAmount(workerID);
            int totalAdvanceAmount = GetTotalAdvanceAmount(workerID);

            int finalAmount_a = (totalWorkingHours * 150) - (totalAdvanceAmount + (totalTeabagAmount * 500));
            int finalAmount = finalAmount_a * (95 / 100);

            MessageBox.Show($"Worker ID: {workerID}\n" +
                            $"Total Working Hours: {totalWorkingHours}\n" +
                            $"Total Teabag Borrowing: {totalTeabagAmount}\n" +
                            $"Total Advance: {totalAdvanceAmount}\n" +
                            $"Final Amount: {finalAmount}");
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

        private int GetTotalWorkingHours(string workerID)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT SUM(WorkingHours) FROM WorkerAttendance WHERE WorkerID = @WorkerID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@WorkerID", workerID);

                connection.Open();
                object result = command.ExecuteScalar();

                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }

        private int GetTotalTeabagAmount(string workerID)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT SUM(TeabagAmount) FROM TeabagBorrowing WHERE WorkerID = @WorkerID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@WorkerID", workerID);

                connection.Open();
                object result = command.ExecuteScalar();

                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }

        private int GetTotalAdvanceAmount(string workerID)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT SUM(AdvanceAmount) FROM WorkerAdvance WHERE WorkerID = @WorkerID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@WorkerID", workerID);

                connection.Open();
                object result = command.ExecuteScalar();

                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
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
