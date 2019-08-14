using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Database
{
    public partial class BizContacts : Form
    {
        public BizContacts()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            cboSearch.SelectedIndex = 0; //first item in combo box is selected when the form loads
            dataGridView1.DataSource = bindingSource1; //sets the source of the data to be displayed in the grid view;

            GetData("Select * from BizContacts"); 
        }

        private void GetData(string v)
        {
            throw new NotImplementedException();
        }

        private void BizContacts_Load(object sender, EventArgs e)
        {

        }
    }
}
