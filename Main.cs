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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void BusinessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BizContacts form = new BizContacts(); //new business contacts form
            form.MdiParent = this; // set the main form as the parent of each business form
            form.Show(); // show the business contact form
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade); //cascade the child form within the main form
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical); //apply vertical layout to child forms within parent forms
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal); //apply horizontal layout to children elements
        }
    }
}
