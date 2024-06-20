using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System;

namespace Employee_Information
{
    public partial class Form1 : Form
    {
        //DataSet dataSet;
        private SqlConnection _connection;

        private string connectionString = "";
        public Form1()
        {
            InitializeComponent();
            LoadSettings();

            this.Text = "Employs";

            comboBox1.Text = "Select....";
            comboBox1.Items.Add("id");
            comboBox1.Items.Add("firs name");
            comboBox1.Items.Add("second name");
            comboBox1.Items.Add("last name");
            comboBox1.Items.Add("date employ");
            comboBox1.Items.Add("date unemploy");
            comboBox1.Items.Add("status");
            comboBox1.Items.Add("dep");
            comboBox1.Items.Add("post");

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.ColumnCount = 9;
            dataGridView1.Columns[0].Name = "id";
            dataGridView1.Columns[1].Name = "firs name";
            dataGridView1.Columns[2].Name = "second name";
            dataGridView1.Columns[3].Name = "last name";
            dataGridView1.Columns[4].Name = "date employ";
            dataGridView1.Columns[5].Name = "date unemploy";
            dataGridView1.Columns[6].Name = "status";
            dataGridView1.Columns[7].Name = "dep";
            dataGridView1.Columns[8].Name = "post";
        }

        private void LoadSettings()
        {
            try
            {
                connectionString = File.ReadAllText("connection.ini");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке файла настроек: " + ex.Message);
            }
        }

        private void PrintOnDataGridView(List<List<object>> list)
        {
            dataGridView1.Rows.Clear();
            if (list is null)
                return;
            foreach (var item in list)
            {
                object statusT = ReadDBT($"SELECT name FROM Status WHERE id = {item[6]}");
                object id_depT = ReadDBT($"SELECT name FROM Deps WHERE id = {item[7]}");
                object id_postT = ReadDBT($"SELECT name FROM Post WHERE id = {item[8]}");
                item[4] = item[4].ToString() == "" ? "null" : ((DateTime)item[4]).ToString("dd.MM.yyyy");
                item[5] = item[5].ToString() == "" ? "null" : ((DateTime)item[5]).ToString("dd.MM.yyyy");
                item[6] = statusT;
                item[7] = id_depT;
                item[8] = id_postT;
                dataGridView1.Rows.Add(item.ToArray());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            List<List<object>> list = new List<List<object>>();

            string sqlExpression1 = "SELECT * FROM Persons";
            list = ReadDBTAll(sqlExpression1);
            PrintOnDataGridView(list);
        }

        private object ReadDBT(string nameDBT)
        {
            object result = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(nameDBT, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    object name = reader.GetValue(0);
                    result = name;
                }
            }
            return result;
        }
        private List<List<object>> ReadDBTAll(string sqlExpression)
        {
            List<List<object>> list = new List<List<object>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlExpression, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        while (reader.Read())
                        {
                            object id = reader.GetValue(0);
                            object name1 = reader.GetValue(1);
                            object name2 = reader.GetValue(2);
                            object name3 = reader.GetValue(3);

                            object date_employ = reader.GetValue(4);
                            object date_uneploy = reader.GetValue(5);
                            object status = reader.GetValue(6);

                            object id_dep = reader.GetValue(7);
                            object id_post = reader.GetValue(8);

                            list.Add(new List<object>() { id, name1, name2, name3, date_employ, date_uneploy, status, id_dep, id_post });
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }
            return list;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PrintOnDataGridView(Select());
        }
        private List<List<object>> Select()
        {
            string text = textBox1.Text;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    int id = 0;
                    if (!int.TryParse(text, out id))
                        return null;
                    return ReadDBTAll($"SELECT * FROM Persons WHERE id = {id}");
                case 1:
                    return ReadDBTAll($"SELECT * FROM Persons WHERE first_name = '{text}'");
                case 2:
                    return ReadDBTAll($"SELECT * FROM Persons WHERE second_name = '{text}'");
                case 3:
                    return ReadDBTAll($"SELECT * FROM Persons WHERE last_name = '{text}'");
                case 4:
                    return ReadDBTAll($"SELECT * FROM Persons WHERE date_employ = '{text}'");
                case 5:
                    return ReadDBTAll($"SELECT * FROM Persons WHERE date_uneploy = '{text}'");
                case 6:
                    var st = ReadDBT($"SELECT id FROM Status WHERE name = '{text}'");
                    if (st == null)
                        return null;
                    return ReadDBTAll($"SELECT * FROM Persons WHERE id = {st}");
                case 7:
                    var dep = ReadDBT($"SELECT id FROM Deps WHERE name = '{text}'");
                    if (dep == null)
                        return null;
                    return ReadDBTAll($"SELECT * FROM Persons WHERE id = {dep}");
                case 8:
                    var post = ReadDBT($"SELECT id FROM Post WHERE name = '{text}'");
                    if (post == null)
                        return null;
                    return ReadDBTAll($"SELECT * FROM Persons WHERE id = {post}");
            }
            return null;
        }
    }
}
