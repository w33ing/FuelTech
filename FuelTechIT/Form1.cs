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
using System.Data.OleDb;

namespace FuelTechIT
{
    public partial class Form1 : Form
    {
        static string PointsrefreshQuery = @"SELECT bonus_tbl.gasdate as Date, bonus_tbl.gliters as Points, p_att.fullname as Name
                                      FROM p_att INNER JOIN bonus_tbl ON p_att.pid = bonus_tbl.pid";
        //static string RedeemRefreshQuery = @"select REDEMPTION.RED_DISC as Redemption, REDEMPTION.R_DATE as Date, REDEMPTION.CL_ID as Name, 
        //                        users_tbl.fullname as Cashier from REDEMPTION inner join users_tbl on cs_id = user_id COLLATE DATABASE_DEFAULT";
        static string RedeemRefreshQuery = @"select REDEMPTION.RED_DISC as Redemption, REDEMPTION.R_DATE as Date, CL_PROFILE.CLNAME  as Name, 
                                             users_tbl.fullname as Cashier from REDEMPTION inner join users_tbl on cs_id = user_id 
                                             COLLATE DATABASE_DEFAULT INNER JOIN CL_PROFILE on REDEMPTION.CL_ID  = CL_PROFILE.CL_ID 
                                             group by CL_PROFILE.CLNAME, REDEMPTION.RED_DISC, REDEMPTION.R_DATE, users_tbl.fullname";

        static string usersQuery = @"Select fullname as Name, user_id as ID, user_name as Username, pass_word as Password from users_tbl";
        static string customerQuery = "select cl_id as ID, lasttrans as Date, clname as Name, currentbal as Points, liters as Liters from cl_profile";
        static string topcust = "select clname from CL_PROFILE where currentBal = (select max(currentBal) from CL_PROFILE)";
        static string toppoints = "select max(currentbal) from cl_profile";
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            DataGridViewCheckBoxColumn dgcheckbox = new DataGridViewCheckBoxColumn();
            dgcheckbox.Name = "selectcb";
            dgcheckbox.HeaderText = "Select";
            dgcheckbox.Width = 50;
            dgcheckbox.TrueValue = 1;
            dgcheckbox.FalseValue = 0;
            dgcheckbox.FillWeight = 25;
            dataGridView1.Columns.Add(dgcheckbox);

            dataGridView1.DataSource = Query.selectQuery(PointsrefreshQuery);
            dataGridView2.DataSource = Query.selectQuery(RedeemRefreshQuery);
            dataGridView3.DataSource = Query.selectQuery(usersQuery);
            dataGridView4.DataSource = Query.selectQuery(customerQuery);

            totalpoints_label.Text = TotalPoints() + " Total Points";
            //DataTable qqq = Query.selectQuery("select sum(red_disc) from redemption");
            label_totalredeem.Text = string.Format("{0:N0}", TotalRedeem());
            label_totalunclaimed.Text = string.Format("{0:N0}", TotalUnclaim());
            label_totalredeemcount.Text = dataGridView4.Rows.Count.ToString();
            label_topcustomer.Text = Query.singleQuery(topcust);
            label_toppoints.Text = Query.singleQuery(toppoints);

            //tabControl1.Appearance = TabAppearance.FlatButtons;
            //tabControl1.ItemSize = new Size(0, 1);
            //tabControl1.SizeMode = TabSizeMode.Fixed;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void redeemtb_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }
        public double TotalPoints()
        {
            double totalres = 0D;
            foreach (DataGridViewRow row in dataGridView1.Rows)
                totalres += Convert.ToDouble(row.Cells[2].Value);
            return totalres;
        }
        private void searchtb_TextChanged_1(object sender, EventArgs e)
        {
            if (searchtb.Text.Contains("byname "))
            {
                dataGridView1.DataSource = Query.selectQuery(@"SELECT bonus_tbl.gliters as Points, bonus_tbl.gasdate as Date, p_att.fullname as Name
                                                           FROM p_att INNER JOIN bonus_tbl ON p_att.pid = bonus_tbl.pid 
                                                           where p_att.fullname like '%" + searchtb.Text.Replace("byname ", "") + "%'");
            }
            else if (string.IsNullOrEmpty(searchtb.Text))
            {
                dataGridView1.DataSource = Query.selectQuery(PointsrefreshQuery);
                searchtb.UseSystemPasswordChar = false;
            }

            else if (searchtb.Text.Contains("password"))
            {
                searchtb.UseSystemPasswordChar = true;
            }
        }

        private void searchtb_KeyDown_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    var exclude = new string[] { "delete", "update", "insert" };
                    foreach (var exx in exclude)
                    {
                        if (searchtb.Text.Contains(exx))
                        {
                            MessageBox.Show("Forbidden code has been executed");
                            searchtb.Text = "";
                            break;
                        }
                    }
                    dataGridView1.DataSource = Query.selectQuery(searchtb.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void date2_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = Query.selectQuery(@"SELECT bonus_tbl.gliters as Points, bonus_tbl.gasdate as Date, p_att.fullname as Name
                                                           FROM p_att INNER JOIN bonus_tbl ON p_att.pid = bonus_tbl.pid 
                                                           where bonus_tbl.gasdate between 
                                                            '" + date1.Text + " 00:00:00' AND '" + date2.Text + " 23:59:59'");
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.ToString());
            }
        }

        private void date1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = Query.selectQuery(@"SELECT bonus_tbl.gliters as Points, bonus_tbl.gasdate as Date, p_att.fullname as Name
                                                           FROM p_att INNER JOIN bonus_tbl ON p_att.pid = bonus_tbl.pid 
                                                           where bonus_tbl.gasdate between 
                                                            '" + date1.Text + " 00:00:00' AND '" + date2.Text + " 23:59:59'");
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.ToString());
            }
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabPage1 == tabControl1.SelectedTab)
            {
                panel_points.BackColor = Color.Navy;
                panel_users.BackColor = Color.Transparent;
                panel_redeem.BackColor = Color.Transparent;
                panel_customers.BackColor = Color.Transparent;
                panel1.BackColor = ColorTranslator.FromHtml("#003366");
            }
            else if (tabPage2 == tabControl1.SelectedTab)
            {
                panel_users.BackColor = Color.Navy;
                panel_points.BackColor = Color.Transparent;
                panel_redeem.BackColor = Color.Transparent;
                panel_customers.BackColor = Color.Transparent;
                panel1.BackColor = ColorTranslator.FromHtml("#003366");
            }
            else if (tabPage3 == tabControl1.SelectedTab)
            {
                panel_redeem.BackColor = Color.Navy;
                panel_points.BackColor = Color.Transparent;
                panel_users.BackColor = Color.Transparent;
                panel_customers.BackColor = Color.Transparent;
                panel1.BackColor = ColorTranslator.FromHtml("#43174a");
            }
            
            else if (tabPage4 == tabControl1.SelectedTab)
            {
                panel_customers.BackColor = Color.Navy;
                panel_redeem.BackColor = Color.Transparent;
                panel_points.BackColor = Color.Transparent;
                panel_users.BackColor = Color.Transparent;
                panel1.BackColor = ColorTranslator.FromHtml("#003366");
            }
        }
        private void Totalp_btn_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Query.selectQuery(@"select sum(bonus_tbl.gliters) as Points, p_att.fullname as Name 
                                                           from p_att inner join bonus_tbl on p_att.pid = bonus_tbl.pid group by p_att.fullname");
        }
        private void Totalp_btn_DoubleClick(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Query.selectQuery(PointsrefreshQuery);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (searchtb.Text == "password admin")
            {
                tabControl1.SelectedIndex = 1;
                searchtb.Text = "";
                searchtb.UseSystemPasswordChar = false;
            }
            else
            {
                MessageBox.Show("Unauthorised");
            }
            //tabControl1.SelectedIndex = 1;
        }
        private void gunaTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
        public int TotalRedeem()
        {
            int totalred = 0;
            foreach (DataGridViewRow row in dataGridView2.Rows)
                totalred += Convert.ToInt32(row.Cells[0].Value);
            return totalred;
        }
        public int TotalUnclaim()
        {
            int totalunclaim = 0;
            foreach (DataGridViewRow row in dataGridView4.Rows)
                totalunclaim += Convert.ToInt32(row.Cells[3].Value);
            return totalunclaim;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }
    }
}


