using System;
using System.Windows.Forms;

namespace WorldWind.GeoRSS
{
    public partial class GeoRSSFeedControl : UserControl
    {
        internal GeoRssFeeds m_feeds;

        internal bool m_updateNeeded;

        public GeoRSSFeedControl()
        {
            this.InitializeComponent();

            this.openFileDialog.InitialDirectory = Application.StartupPath + @"\Plugins\GeoRSS\";
            this.openFileDialog.FileName = "geoRSS-small.png";

            this.iconTextBox.Text = Application.StartupPath + @"\Plugins\GeoRSS\georss-small.png";
        }

        public GeoRSSFeedControl(GeoRssFeeds feeds)
        {
            this.InitializeComponent();

            this.m_feeds = feeds;

            this.UpdateDataGridView();
        }

        internal void UpdateDataGridView()
        {
            this.feedDataGridView.Rows.Clear();

            foreach (GeoRssFeed feed in this.m_feeds.Feeds)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewImageCell imageCell = new DataGridViewImageCell();
                row.Cells.Add(imageCell);

                DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
                nameCell.Value = feed.Name;
                row.Cells.Add(nameCell);

                DataGridViewTextBoxCell urlCell = new DataGridViewTextBoxCell();
                urlCell.Value = feed.Url;
                row.Cells.Add(urlCell);

                DataGridViewTextBoxCell refreshCell = new DataGridViewTextBoxCell();
                refreshCell.Value = feed.UpdateInterval.ToString();
                row.Cells.Add(refreshCell);

                DataGridViewTextBoxCell lastUpdateCell = new DataGridViewTextBoxCell();
                lastUpdateCell.Value = feed.LastUpdate.ToString();
                row.Cells.Add(lastUpdateCell);

                DataGridViewButtonCell buttonCell = new DataGridViewButtonCell();
                buttonCell.UseColumnTextForButtonValue = true;
                row.Cells.Add(buttonCell);

                this.feedDataGridView.Rows.Add(row);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            this.m_feeds.Add(this.nameTextBox.Text, this.urlTextBox.Text);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            this.openFileDialog.ShowDialog();

        }
    }
}
