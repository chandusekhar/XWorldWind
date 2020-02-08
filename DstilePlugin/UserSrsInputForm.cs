using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DstileGUI
{
    public partial class UserSrsInputForm : Form
    {
        public UserSrsInputForm()
        {
            this.InitializeComponent();
        }

        public string Projection
        {
            get { return this.cbInputSrs.Text; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}