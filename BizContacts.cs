﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; //needed for file use
using System.Diagnostics; //allows us to open excel using process.start
using System.Data;
using Microsoft.Office.Interop.Excel; //allows us to make excel objects

namespace Database
{
    public partial class BizContacts : Form
    {
        string connString = @"Data Source=SUSMITABHOWC65E\SQLEXPRESS;Initial Catalog=AddressBook;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlDataAdapter dataAdapter; //allows us to build the connection between the program and the database
        System.Data.DataTable table; //table to hold information so we can fill the datagrid view
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
                table = new System.Data.DataTable();
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
                                                       City,State,Postal_Code,Phone,Notes,Image)
                                                        values(@Date_Added, @Company, @Website, @Title, @First_Name, @Last_Name, @Address,
                                                       @City,@State,@Postal_Code,@Phone,@Notes,@Image)"; //parameter names
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
                    if (dlgOpenImage.FileName != "")
                    {
                        command.Parameters.AddWithValue(@"Image", File.ReadAllBytes(dlgOpenImage.FileName)); //convert image into array of bytes, get filename through dialog box
                    }
                    else
                    {
                        command.Parameters.Add("@Image", SqlDbType.VarBinary).Value = DBNull.Value; //save null to database with no image
                    }
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
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
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

        private void BtnGetImage_Click(object sender, EventArgs e)
        {
            if (dlgOpenImage.ShowDialog() == DialogResult.OK)
            { //show box for selecting image
                pictureBox1.Load(dlgOpenImage.FileName); //loads image from drive using the filename property of the dialog box
            }
        }

        private void PictureBox1_DoubleClick(object sender, EventArgs e)
        {
            Form frm = new Form();
            frm.BackgroundImage = pictureBox1.Image; //set background image of new preview form of image
            frm.Size = pictureBox1.Image.Size; //sets the size of the form to the size of the image so the image is wholly visible
            frm.Show();
        }

        private void BtnExportOpen_Click(object sender, EventArgs e)
        {
            _Application excel = new Microsoft.Office.Interop.Excel.Application(); //new excel object
            _Workbook workbook = excel.Workbooks.Add(Type.Missing); //make a workbook
            _Worksheet worksheet = null; //make a worksheet and for now set it to null
            try
            {
                worksheet = workbook.ActiveSheet; //set active sheet to workbook
                worksheet.Name = "Business Contacts";
                //because both data grids and excel sheets are tabular, use nested loops to write from one to the other
                for (int rowIndex = 0; rowIndex < dataGridView1.Rows.Count - 1; rowIndex++) //this loop controls the row number
                {
                    for (int colIndex = 0; colIndex < dataGridView1.Columns.Count; colIndex++) //this is needed to go over the columns of each row
                    {
                        if (rowIndex == 0) //the first row at i 0 is the header row 
                        {
                            //in Excel, row and column indexes begin at 1,1 and not 0,0
                            //write out the header texts from the grid view to excel sheet
                            worksheet.Cells[rowIndex+1,colIndex+1] = dataGridView1.Columns[colIndex].HeaderText;
                        }
                        else
                        {
                            worksheet.Cells[rowIndex + 1, colIndex + 1] = dataGridView1.Rows[rowIndex].Cells[colIndex].Value.ToString();
                            //fix the row index at 1, then change the column index over its possible value from 0 - 5
                        }
                    }
                }
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveFileDialog1.FileName); //save file to drive
                    Process.Start("excel.exe", saveFileDialog1.FileName);//load excel with the exported file
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally //this code always runs
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
