using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data;

namespace Database
{
    public partial class BizContacts : Form
    {
        string connString = @"Data Source=SUSMITABHOWC65E\SQLEXPRESS;Initial Catalog=AddressBook;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlDataAdapter dataAdapter; //allows us to build the connection between the program and the database
        System.Data.DataTable table; //table to hold information so we can fill the datagrid view
        SqlCommandBuilder commandBuilder; //declare a new sql command builder object
        SqlConnection conn; //declares variable to hold sql connection
        string selectionStatement = "Select * from BizContacts";
        public BizContacts()
        {
            InitializeComponent();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentCell.OwningRow; //grab a reference to the current row
            string value = row.Cells["ID"].Value.ToString(); //grab the value from the id field of the selected row
            string fname = row.Cells["First_Name"].Value.ToString();
            string lname = row.Cells["Last_Name"].Value.ToString();
            DialogResult result = MessageBox.Show("Do you really want to delete " + fname + " " + lname + ",record" + value,"Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            string deleteState = @"Delete from BizContacts where id = '"+value+"'"; //sql to delete the records from the table

            if (result == DialogResult.Yes)
            {
                using (conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand(deleteState, conn);
                        comm.ExecuteNonQuery();
                        GetData(selectionStatement); //get the data
                        dataGridView1.Update();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void GetData(string selectCommand)
        {
            try
            {
                dataAdapter = new SqlDataAdapter(selectCommand, connString);
                table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table); //fill the data table
                bindingSource1.DataSource = table; //bind bindingsource to table
                dataGridView1.Columns[0].ReadOnly = true; //prevents id field from being change
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BizContacts_Load(object sender, EventArgs e)
        {
            cboSearch.SelectedIndex = 0; //First item and combo box is selected when form loads
            dataGridView1.DataSource = bindingSource1;

            GetData(selectionStatement);
            // TODO: This line of code loads data into the 'addressBookDataSet1.BizContacts' table. You can move, or remove it, as needed.
            this.bizContactsTableAdapter1.Fill(this.addressBookDataSet1.BizContacts);
            // TODO: This line of code loads data into the 'addressBookDataSet.BizContacts' table. You can move, or remove it, as needed.
            this.bizContactsTableAdapter.Fill(this.addressBookDataSet.BizContacts);

        }

        private void BindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            SqlCommand command; //declares a new sql command object
            //field names in the table
            string insert = @"insert into BizContacts(Date_Added, Company, Website, Title, First_Name, Last_Name, Address,
                                                       City,State,Postal_Code,Phone,Notes)
                                                        values(@Date_Added, @Company, @Website, @Title, @First_Name, @Last_Name, @Address,
                                                       @City,@State,@Postal_Code,@Phone,@Notes)"; //parameter names
            using (conn = new SqlConnection(connString)) //using allows disposing of low level resources
            {
                try
                {
                    conn.Open(); //open connection
                    command = new SqlCommand(insert, conn); //create new sql command object 
                    command.Parameters.AddWithValue(@"Date_Added", dateTimePicker1.Value.Date); //read values and save to table
                    command.Parameters.AddWithValue(@"Company", txtCompany.Text);
                    command.Parameters.AddWithValue(@"Website", txtWebsite.Text);
                    command.Parameters.AddWithValue(@"Title", txtTitle.Text);
                    command.Parameters.AddWithValue(@"First_Name", txtFirstName.Text);
                    command.Parameters.AddWithValue(@"Last_Name", txtLastName.Text);
                    command.Parameters.AddWithValue(@"Address", txtAddress.Text);
                    command.Parameters.AddWithValue(@"City", txtCity.Text);
                    command.Parameters.AddWithValue(@"State", txtState.Text);
                    command.Parameters.AddWithValue(@"Postal_Code", txtPostalCode.Text);
                    command.Parameters.AddWithValue(@"Phone", txtMobile.Text);
                    command.Parameters.AddWithValue(@"Notes", txtNotes.Text);
                    command.ExecuteNonQuery(); //push to table
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            GetData(selectionStatement);
            dataGridView1.Update(); //draws data grid view so new record is visible on the bottom
        }

        private void TxtCity_TextChanged(object sender, EventArgs e)
        {

        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //code that will run once we finish editing a cell in the datagridview
            commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand(); //get the update command
            try
            {
                bindingSource1.EndEdit(); // updates the table in memory in our program
                dataAdapter.Update(table); // updates the database
                MessageBox.Show("update successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            switch(cboSearch.SelectedItem.ToString()) //because we have a combo box
            {
                case "First Name":
                    GetData("select * from bizcontacts where lower(first_name) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;
                case "Last Name":
                    GetData("select * from bizcontacts where lower(last_name) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;
                case "Company":
                    GetData("select * from bizcontacts where lower(company) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;
            }
        }
    }
}
