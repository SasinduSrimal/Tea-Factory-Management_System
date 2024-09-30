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
    public partial class Form14 : Form
    {
        private const string ConnectionString = "Server=DESKTOP-KO8C234\\SQLEXPRESS;Database=TFMdb;Integrated Security=True;";
        public Form14()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string supplierID = txtSupplierID.Text.Trim();

            // Validate Supplier ID input
            if (string.IsNullOrEmpty(supplierID))
            {
                MessageBox.Show("Please enter a valid Supplier ID.");
                return;
            }

            // Check if supplier is registered
            if (!IsSupplierRegistered(supplierID))
            {
                MessageBox.Show("Supplier is not registered.");
                return;
            }

            // Calculate total tea supply
            decimal totalTeaSupply = CalculateTotalTeaSupply(supplierID);

            // Calculate total tea bag borrowing
            int totalTeaBagBorrowing = CalculateTotalTeaBagBorrowing(supplierID);

            // Calculate total advanced amount
            int totalAdvanced = CalculateTotalAdvanced(supplierID);

            // Calculate final amount
            decimal finalAmount = CalculateFinalAmount(totalTeaSupply, totalTeaBagBorrowing, totalAdvanced);

            // Display results
            MessageBox.Show($"Total Tea Supply: {totalTeaSupply}\n" +
                            $"Total Tea Bag Borrowing: {totalTeaBagBorrowing}\n" +
                            $"Final Amount: {finalAmount}");
        }

        private decimal CalculateTotalTeaSupply(string supplierID)
        {
            string query = "SELECT ISNULL(SUM(TeaAmount), 0) AS TotalTeaSupply " +
                           "FROM TeaSupply " +
                           "WHERE SupplierID = @SupplierID;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierID", supplierID);

                connection.Open();
                object result = command.ExecuteScalar();
                connection.Close();

                return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            }
        }

        private int CalculateTotalTeaBagBorrowing(string supplierID)
        {
            string query = "SELECT ISNULL(SUM(TeaBagAmount), 0) AS TotalTeaBagBorrowing " +
                           "FROM TeaBagBorrowingsforsuppl " +
                           "WHERE SupplierID = @SupplierID;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierID", supplierID);

                connection.Open();
                object result = command.ExecuteScalar();
                connection.Close();

                return result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
        }

        private int CalculateTotalAdvanced(string supplierID)
        {
            string query = "SELECT ISNULL(SUM(AdvanceAmount), 0) AS TotalAdvanced " +
                           "FROM SupplierAdvanced " +
                           "WHERE SupplierID = @SupplierID;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierID", supplierID);

                connection.Open();
                object result = command.ExecuteScalar();
                connection.Close();

                return result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
        }

        private decimal CalculateFinalAmount(decimal totalTeaSupply, int totalTeaBagBorrowing, int totalAdvanced)
        {
            // Formula: (total tea supply * 250) - (total advanced + total tea bag amount * 500)
            decimal finalAmount = (totalTeaSupply * 250) - (totalAdvanced + totalTeaBagBorrowing * 500);
            return finalAmount;
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


        private void Form14_Load(object sender, EventArgs e)
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
