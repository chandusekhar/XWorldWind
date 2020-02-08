using System.Windows.Forms;

namespace WorldWind.GeoRSS
{
    public partial class GeoRssForm : Form
    {
        public GeoRssForm(GeoRssFeeds feed)
        {
            this.InitializeComponent();

            this.geoRSSFeedControl1.m_feeds = feed;
            this.geoRSSFeedControl1.UpdateDataGridView();
        }

        internal void UpdateDataGridView()
        {
            this.geoRSSFeedControl1.UpdateDataGridView();
        }

        private void GeoRssForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            } 
        }
    }
}