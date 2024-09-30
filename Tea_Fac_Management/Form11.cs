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
    public partial class Form11 : Form
    {
        private const string ConnectionString = "Server=DESKTOP-KO8C234\\SQLEXPRESS;Database=TFMdb;Integrated Security=True;";
        public Form11()
        {
            InitializeComponent();
        }

        private void Form11_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Validate all fields
            if (string.IsNullOrWhiteSpace(txtSupplierID.Text) ||
                string.IsNullOrWhiteSpace(txtTeaAmount.Text))
            {
                MessageBox.Show("Please fill all the information.");
                return;
            }

            // Get the values from the form
            string supplierId = txtSupplierID.Text.Trim();
            int teaAmount;

            // Validate tea amount
            if (!int.TryParse(txtTeaAmount.Text.Trim(), out teaAmount))
            {
                MessageBox.Show("Tea amount must be a valid integer.");
                return;
            }

            // Check if the supplier is registered
            if (!IsSupplierRegistered(supplierId))
            {
                MessageBox.Show("Supplier is not registered. Please register before marking supply.");
                return;
            }

            // Record the tea supply
            RecordTeaSupply(supplierId, teaAmount);

            MessageBox.Show("Tea supply recorded successfully!");
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

        private void RecordTeaSupply(string supplierId, int teaAmount)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "INSERT INTO TeaSupply (SupplierID, SupplyDate, TeaAmount) " +
                               "VALUES (@SupplierID, @SupplyDate, @TeaAmount)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierID", supplierId);
                command.Parameters.AddWithValue("@SupplyDate", DateTime.Today);
                command.Parameters.AddWithValue("@TeaAmount", teaAmount);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtSupplierID.Clear();
            txtTeaAmount.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void txtTeaAmount_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
