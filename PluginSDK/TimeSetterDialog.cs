using System;
using System.Windows.Forms;

namespace WorldWind
{
    public partial class TimeSetterDialog : Form
    {
        public DateTime DateTimeUtc
        {
            get 
            {
                if (this.checkBoxUTC.Checked)
                {
                    return this.dateTimePicker1.Value;
                }
                else
                {
                    return this.dateTimePicker1.Value.ToUniversalTime();
                }
            }
            set 
            {
                if (this.checkBoxUTC.Checked)
                {
                    this.dateTimePicker1.Value = value;
                }
                else
                {
                    this.dateTimePicker1.Value = value.ToLocalTime();
                }
            }
        }

        public TimeSetterDialog()
        {
            this.InitializeComponent();
        }

        private void checkBoxUTC_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxUTC.Checked)
            {
                this.dateTimePicker1.Value = this.dateTimePicker1.Value.ToUniversalTime();
            }
            else
            {
                this.dateTimePicker1.Value = this.dateTimePicker1.Value.ToLocalTime();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (this.checkBoxUTC.Checked)
            {
                TimeKeeper.CurrentTimeUtc = this.dateTimePicker1.Value;
            }
            else
            {
                TimeKeeper.CurrentTimeUtc = this.dateTimePicker1.Value.ToUniversalTime();
            }
        }
    }
}