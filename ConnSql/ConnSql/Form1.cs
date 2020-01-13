using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnSql
{
    public partial class Form1 : Form
    {
        private string connectionStr;
        public Form1()
        {
            connectionStr = ConfigurationManager.ConnectionStrings["P213Db"].ConnectionString;
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            #region Scala   
            //using(SqlConnection conn = new SqlConnection(connectionStr))
            //{
            //    conn.Open();
            //    string query = "Select Name from Students where Id=1";
            //    using(SqlCommand comm = new SqlCommand(query, conn))
            //    {
            //        object result =await comm.ExecuteScalarAsync();
            //        MessageBox.Show(result.ToString());
            //    }
            //}
            #endregion

            string queryList = "Select Name from Types";
            GetData(queryList, cmbGroupType);

            string dgvQuery = "select * from StuDeteail";
            using(SqlConnection conn=new SqlConnection(connectionStr))
            {
                conn.Open();
                using(SqlCommand comm=new SqlCommand(dgvQuery,conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(comm);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dgv.DataSource = table;
                }
            }
        }

        private async void cmbGroupType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbGroup.Items.Clear();
            cmbGroup.Text = "";
            string type = cmbGroupType.Text.Trim();
            string query = "Select Name from Groups" +
                    " where TypeId = " +
                    $"(select Id from Types where Name = '{type}')";
            GetData(query, cmbGroup);
        }

        private async void GetData(string query,ComboBox cmb)
        {
            using (SqlConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = await comm.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            cmb.Items.Add(reader.GetValue(0));
                        }
                    }
                }
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtStuName.Text.Trim();
            string group = cmbGroup.Text.Trim();

            if (name == "" || group == "")
            {
                MessageBox.Show("Fill all input", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "insert into Students values" +
                $"('{name}', (select Id from Groups where Name = '{group}'))";
            using(SqlConnection conn=new SqlConnection(connectionStr))
            {
                conn.Open();
                using(SqlCommand comm=new SqlCommand(query, conn))
                {
                    int result =await comm.ExecuteNonQueryAsync();
                    if (result != 0)
                    {
                        MessageBox.Show("Successfully added");
                    }
                }
            }

        }
    }
}
