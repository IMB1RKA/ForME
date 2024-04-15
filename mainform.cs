using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace test_DataBase
{
    enum RoWState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }
    public partial class Form1 : Form
    {
        DataBase dataBase = new DataBase();
        int selectedRow;
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }
        private void CreateColomns()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("type_of", "Тип товара");
            dataGridView1.Columns.Add("count_of", "Количество");
            dataGridView1.Columns.Add("postavka", "Поставщик");
            dataGridView1.Columns.Add("price", "Цена");
            dataGridView1.Columns.Add("IsNew", String.Empty);
        }
        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(
                record.GetInt32(0),
                record.GetString(1),
                record.GetInt32(2),
                record.GetString(3),
                record.GetInt32(4),
                RoWState.ModifiedNew
                );
        }
        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select * from help_db";
            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnction();
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateColomns();
            RefreshDataGrid(dataGridView1);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;
            if(e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                tb_id.Text = row.Cells[0].Value.ToString();
                tb_type_of.Text = row.Cells[1].Value.ToString();
                tb_count_of.Text = row.Cells[2].Value.ToString();
                tb_postavka.Text = row.Cells[3].Value.ToString();
                tb_price.Text = row.Cells[4].Value.ToString();
            }
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            dataBase.openConnction();
            var type_of = tb_type_of.Text;
            int count_of;
            var postavka = tb_postavka.Text;
            int price;
            if(int.TryParse(tb_price.Text, out price) && int.TryParse(tb_count_of.Text, out count_of))
            {
                var addQuery = $"insert into help_db (type_of, count_of, postavka, price) values('{type_of}', '{count_of}', '{postavka}', '{price}')";
                var command = new SqlCommand(addQuery, dataBase.getConnection());
                command.ExecuteNonQuery();
                MessageBox.Show("SUCCESS", "OK", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("ERROR", "NOT GOOD :(", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            tb_count_of.Text = "";
            tb_price.Text = "";
            tb_postavka.Text = "";
            tb_type_of.Text = "";
            dataBase.closeConnction();
        }
        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string searchString = $"select * from help_db where concat (id, type_of, count_of, postavka, price) like '%" + tb_search.Text + "%'";
            SqlCommand com = new SqlCommand(searchString, dataBase.getConnection());
            dataBase.openConnction();
            SqlDataReader read = com.ExecuteReader();
            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
        }
        private void tb_search_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }
        private void DeleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows[index].Visible = false;
            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[5].Value = RoWState.Deleted;
                return;
            }
            dataGridView1.Rows[index].Cells[5].Value = RoWState.Deleted;
        }
        private void Update()
        {
            dataBase.openConnction();
            for(int index = 0; index <= dataGridView1.Rows.Count; index++)
            {
                var rowState = (RoWState)dataGridView1.Rows[index].Cells[5].Value;
                if(rowState == RoWState.Existed)
                {
                    continue;
                }
                if(rowState == RoWState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = $"delete from help_db where id = {id}";
                    var command = new SqlCommand(deleteQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();
                }
                if(rowState == RoWState.Modified)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var type_of = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var count_of = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var postavka = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var price = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var changeQuery = $"update help_db set type_of = '{type_of}', count_of = '{count_of}', postavka = '{postavka}', price = '{price}' where id = {id}";
                    var command = new SqlCommand(changeQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();
                }
            }
            dataBase.closeConnction();
        }
        private void btn_delete_Click(object sender, EventArgs e)
        {
            DeleteRow();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            Update();
        }
        private void Change()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;
            var id = tb_id.Text;
            var type_of = tb_type_of.Text;
            int cout_of;
            var postavka = tb_postavka.Text;
            int price;
            if(dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if(int.TryParse(tb_price.Text, out price) && int.TryParse(tb_count_of.Text, out cout_of))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(id, type_of, cout_of, postavka, price);
                    dataGridView1.Rows[selectedRowIndex].Cells[5].Value = RoWState.Modified;
                }
                else
                {
                    MessageBox.Show("ERROR", "NOT GOOF :(");
                }
            }
        }
        private void btn_change_Click(object sender, EventArgs e)
        {
            Change();
        }
    }
}
