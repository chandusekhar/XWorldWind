using System;
using Rss;
using WorldWind.Renderable;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WorldWind.GeoRSS
{
    public class GeoRssFeed
    {
        /// <summary>
        /// The name of this feed
        /// </summary>
        public string Name
        {
            get { return this.m_name; }
            set { this.m_name = value; }
        }
        string m_name;

        /// <summary>
        /// The RSS Feed object
        /// </summary>
        public RssFeed Feed
        {
            get { return this.m_feed; }
            set { this.m_feed = value; }
        }
        private RssFeed m_feed;

        /// <summary>
        /// The layer that all Geo objects appear on.
        /// If more than one channel then each channel is its own layer
        /// </summary>
        public Icons Layer
        {
            get { return this.m_layer; }
            set { this.m_layer = value; }
        }
        Icons m_layer;

        /// <summary>
        /// Whether this feed had changes in the last update
        /// </summary>
        public bool HasChanges
        {
            get { return this.m_hasChanges; }
            set { this.m_hasChanges = value; }
        }
        private bool m_hasChanges;

        /// <summary>
        /// When the last update was
        /// </summary>
        public DateTime LastUpdate
        {
            get { return this.m_lastUpdate; }
            set { this.m_lastUpdate = value; }
        }
        DateTime m_lastUpdate;

        /// <summary>
        /// How often we update this feed
        /// </summary>
        public TimeSpan UpdateInterval
        {
            get { return this.m_updateInterval; }
            set { this.m_updateInterval = value; }
        }
        TimeSpan m_updateInterval;

        /// <summary>
        /// The filename of the icon for this feed
        /// </summary>
        public string IconFileName
        {
            get { return this.m_iconFileName; }
            set { this.m_iconFileName = value; }
        }
        private string m_iconFileName;

        public string Url
        {
            get { return this.m_url; }
            set { this.m_url = value; }
        }
        string m_url;

        /// <summary>
        /// Whether or not this feed needs an update based on the
        /// last update time.  Set to true by GroRssFeeds background worker
        /// </summary>
        internal bool NeedsUpdate
        {
            get { return this.m_needsUpdate; }
            set { this.m_needsUpdate = value; }
        }
        private bool m_needsUpdate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url"></param>
        /// <param name="update"></param>
        public GeoRssFeed(string name, string url, TimeSpan update, Icons layer)
        {
            this.m_name = name;
            this.m_url = url;
            this.m_updateInterval = update;
            this.m_layer = layer;
            this.m_needsUpdate = true;
            this.m_iconFileName = Application.StartupPath + @"\Plugins\GeoRSS\georss-small.png";
        }

        public GeoRssFeed(string name, string url, TimeSpan update, Icons layer, string iconName)
        {
            this.m_name = name;
            this.m_url = url;
            this.m_updateInterval = update;
            this.m_layer = layer;
            this.m_needsUpdate = true;
            this.m_iconFileName = iconName;
        }

        /// <summary>
        /// Clean up
        /// </summary>
        public void Dispose()
        {
            // TODO: clean up

            // close all feeds
            // clean up resources
        }

        public void Update()
        {
            if (this.m_needsUpdate)
            {
                try
                {
                    DateTime lastModified = new DateTime(); 
                    if (this.m_feed == null)
                    {
                        this.m_feed = RssFeed.Read(this.m_url);
                    }
                    else
                    {
                        lastModified = this.m_feed.LastModified;
                        this.m_feed = RssFeed.Read(this.m_feed);
                    }

                    // if the page was modified and there was stuff update everything.
                    if (lastModified.CompareTo(this.m_feed.LastModified) != 0 )
                    {
                        this.m_layer.RemoveAll();

                        if (this.m_feed.Channels.Count > 0)
                        {
                            if (this.m_feed.Channels.Count == 1)
                            {
                                foreach (RssItem item in this.m_feed.Channels[0].Items)
                                {
                                    if ((item.GeoPoint != null) && (item.GeoPoint.IsValid))
                                    {
                                        Icon icon = new Icon(item.Title, this.StripTags(item.Description), item.GeoPoint.Lat, item.GeoPoint.Lon, item.GeoPoint.Alt, this.m_iconFileName, 0, 0, item.Link.ToString());
                                        icon.isSelectable = true;
                                        icon.AutoScaleIcon = true;
                                        this.m_layer.Add(icon);
                                    }
                                }
                            }
                            else
                            {
                                foreach (RssChannel channel in this.m_feed.Channels)
                                {
                                    string name = channel.Title;
                                    if (name.Trim() == "")
                                    {
                                        name = "Unknown Channel";
                                    }
                                    Icons channelLayer = new Icons(name);
                                    this.m_layer.Add(channelLayer);

                                    foreach (RssItem item in channel.Items)
                                    {
                                        if ((item.GeoPoint != null) && (item.GeoPoint.IsValid))
                                        {
                                            Icon icon = new Icon(item.Title, this.StripTags(item.Description), item.GeoPoint.Lat, item.GeoPoint.Lon, item.GeoPoint.Alt, this.m_iconFileName, 0, 0, item.Link.ToString());
                                            icon.isSelectable = true;
                                            icon.AutoScaleIcon = true;
                                            channelLayer.Add(icon);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    this.m_needsUpdate = false;
                }
                catch (Exception)
                {
                    // TODO decide if we want to dump the channel if there was an error
                }
            }
        }

        /// <summary>
        /// Helper function that strips out HTML tags
        /// </summary>
        /// <param name="text">string to strip</param>
        /// <returns>stripped string</returns>
        public string StripTags(string text)
        {
            string stripped = Regex.Replace(text, Regex.Escape("<br>"), "\n", RegexOptions.IgnoreCase);
            return Regex.Replace(stripped, "<.*?>", string.Empty, RegexOptions.Compiled);
        }
    }
}
