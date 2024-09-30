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
    public partial class Form4 : Form
    {
        private Dictionary<String, DateTime> inTimes = new Dictionary<String, DateTime>();
        const string ConnectionString = "Server=DESKTOP-KO8C234\\SQLEXPRESS;Database=TFMdb;Integrated Security=True;";
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            timerCurrentTime.Start();
            UpdateCurrentTime();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateCurrentTime();
        }

        private void UpdateCurrentTime()
        {
            // Set the label text to the current time
            lblCurrentTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

       public void button1_Click(object sender, EventArgs e)
        {

            string workerID = txtWorkerID.Text.Trim();

            if (string.IsNullOrEmpty(workerID))
            {
                MessageBox.Show("Please enter a valid worker ID.");
                return;
            }

            // Check if worker is registered
            if (!IsWorkerRegistered(workerID))
            {
                MessageBox.Show("Worker is not registered. Please register before marking attendance.");
                return;
            }

            // Mark IN time with the current time
            inTimes[workerID] = DateTime.Now;
            if (HasMarkedAttendanceToday(workerID))
            {
                MessageBox.Show("You have already marked attendance today.");
                return;
            }
            else
            {
                MessageBox.Show("Worker marked IN successfully.");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string workerID = txtWorkerID.Text.Trim();

            if (string.IsNullOrEmpty(workerID))
            {
                MessageBox.Show("Please enter a valid worker ID.");
                return;
            }

            // Check if worker is registered
            if (!IsWorkerRegistered(workerID))
            {
                MessageBox.Show("Worker is not registered. Please register before marking attendance.");
                return;
            }

            // Check if worker has previously marked IN
            if (!inTimes.ContainsKey(workerID))
            {
                MessageBox.Show("Worker hasn't marked IN.");
                return;
            }

            DateTime inTime = inTimes[workerID];
            DateTime outTime = DateTime.Now;
            TimeSpan timeDifference = outTime - inTime;
            int workingHours = (int)Math.Round(timeDifference.TotalHours);

            // Store attendance in the database

            // Remove worker from dictionary as their attendance is now recorded
            inTimes.Remove(workerID);

            if (HasMarkedAttendanceToday(workerID))
            {
                MessageBox.Show("You have already marked attendance today.");
                return;
            }
            else
            {
                MarkAttendance(workerID, DateTime.Today, workingHours);

                MessageBox.Show("Worker marked OUT successfully.");
            }
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

        private bool HasMarkedAttendanceToday(string workerID)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM WorkerAttendance " +
                               "WHERE WorkerID = @WorkerID " +
                               "AND CurrentDate = @CurrentDate ";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@WorkerID", workerID);
                command.Parameters.AddWithValue("@CurrentDate", DateTime.Today);

                connection.Open();
                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }


        private void MarkAttendance(string workerID, DateTime currentDate, int workingHours)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "INSERT INTO WorkerAttendance (WorkerID, CurrentDate, WorkingHours) " +
                               "VALUES (@WorkerID, @CurrentDate, @WorkingHours)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@WorkerID", workerID);
                command.Parameters.AddWithValue("@CurrentDate", currentDate.Date);
                command.Parameters.AddWithValue("@WorkingHours", workingHours);

                connection.Open();
                command.ExecuteNonQuery();
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
