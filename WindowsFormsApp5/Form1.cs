using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<Client> clients = new List<Client>();

        private void Form1_Load(object sender, EventArgs e)
        {
            SQLiteConnection connection = new SQLiteConnection("Integrated Security=SSPI;Data Source=C.db");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT o.CustomerID, o.LastName, SUM(o.UnitPrice * o.Quantity) AS OrderTotal FROM Orders o INNER JOIN Customers c ON o.CustomerID = c.CustomerID INNER JOIN Products p ON o.ProductID = p.ProductID GROUP BY o.CustomerID, o.LastName";

            using (var rd1 = command.ExecuteReader())
            {
                while (rd1.Read())
                {
                    string clientID = rd1.GetString(0);
                    string lastName = rd1.GetString(1);
                    decimal orderTotal = rd1.GetDecimal(2);

                    clients.Add(new Client(clientID, lastName, orderTotal));
                }
            }

            connection.Close();

            dataGridView1.DataSource = clients;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Items.Add("Сортировка по убыванию");
            comboBox1.Items.Add("Сортировка по возрастанию");
            comboBox1.SelectedIndex = 0; // Устанавливаем по умолчанию сортировку по убыванию

            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.Items.Add("Сумма заказа < 150");
            comboBox2.Items.Add("Сумма заказа > 150");
            comboBox2.SelectedIndex = 0; // Устанавливаем по умолчанию фильтрацию по сумме заказа < 150

            for (int j = 0; j < clients.Count; j++)
            {
                chart1.Series["Series1"].Points.AddXY(clients[j].ClientID, clients[j].OrderTotal);
            }

            chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            chart1.Series["Series1"].Label = "#VALX: #VALY";
            chart1.Series["Series1"].LegendText = "#VALX";
            chart1.Legends[0].Enabled = true;

            chart2.Series.Clear();
            chart2.Series.Add("OrderTotal");
            for (int j = 0; j < clients.Count; j++)
            {
                chart2.Series["OrderTotal"].Points.AddXY(clients[j].ClientID, clients[j].OrderTotal);
            }
            chart2.ChartAreas[0].AxisX.Interval = 1;
            chart2.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chart2.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 8);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                // Сортировка списка клиентов по убыванию суммы заказа и обновление данных в dataGridView2
                dataGridView2.DataSource = clients.OrderByDescending(c => c.OrderTotal).ToList();
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                // Сортировка списка клиентов по возрастанию суммы заказа и обновление данных в dataGridView2
                dataGridView2.DataSource = clients.OrderBy(c => c.OrderTotal).ToList();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedOption = comboBox2.SelectedItem.ToString();

            List<Client> filteredClients = new List<Client>();

            if (selectedOption == "Сумма заказа < 150")
            {
                // Фильтрация списка клиентов по сумме заказа меньше 150 и обновление данных в dataGridView3
                filteredClients = clients.Where(c => c.OrderTotal < 150).ToList();
            }
            else if (selectedOption == "Сумма заказа > 150")
            {
                // Фильтрация списка клиентов по сумме заказа больше 150 и обновление данных в dataGridView3
                filteredClients = clients.Where(c => c.OrderTotal > 150).ToList();
            }

            dataGridView3.DataSource = filteredClients;
        }

        class Client
        {
            string clientID;
            string lastName;
            decimal orderTotal;

            public Client(string clientID, string lastName, decimal orderTotal)
            {
                this.clientID = clientID;
                this.lastName = lastName;
                this.orderTotal = orderTotal;
            }

            public string ClientID { get => clientID; set => clientID = value; }
            public string LastName { get => lastName; set => lastName = value; }
            public decimal OrderTotal { get => orderTotal; set => orderTotal = value; }
        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string selectedOption = comboBox2.SelectedItem.ToString();

            List<Client> filteredClients = new List<Client>();

            if (selectedOption == "Сумма заказа < 150")
            {
                // Фильтрация списка клиентов по сумме заказа меньше 150 и обновление данных в dataGridView3
                filteredClients = clients.Where(c => c.OrderTotal < 150).ToList();
            }
            else if (selectedOption == "Сумма заказа > 150")
            {
                // Фильтрация списка клиентов по сумме заказа больше 150 и обновление данных в dataGridView3
                filteredClients = clients.Where(c => c.OrderTotal > 150).ToList();
            }

            dataGridView3.DataSource = filteredClients;
        }
    }
}
