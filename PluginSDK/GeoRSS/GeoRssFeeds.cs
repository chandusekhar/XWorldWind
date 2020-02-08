using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using WorldWind.Renderable;
using System.Threading;


namespace WorldWind.GeoRSS
{
    public class GeoRssFeeds
    {

        public GeoRssForm ControlForm
        {
            get { return this.m_form; }
            set { this.m_form = value; }
        }
        GeoRssForm m_form;

        /// <summary>
        /// All the GeoRssFeeds
        /// </summary>
        public List<GeoRssFeed> Feeds
        {
            get { return this.m_feeds; }
            set { this.m_feeds = value; }
        }
        List<GeoRssFeed> m_feeds;

        /// <summary>
        /// The root layer to add all these feeds to.  
        /// Each feed gets its own layer.  
        /// If there are more than one channel then each channel gets a layer.
        /// </summary>
        public RenderableObjectList RootLayer
        {
            get { return this.m_rootLayer; }
            set { if (value != null) this.m_rootLayer = value; }
        }
        RenderableObjectList m_rootLayer;

        BackgroundWorker m_bw;

        /// <summary>
        /// Whether we should stop processing
        /// </summary>
        public bool Done
        {
            get { return this.m_done; }
            set { this.m_done = value; }
        }
        bool m_done;


        /// <summary>
        /// If we don't want to update a while but not quit completely
        /// </summary>
        public bool Idle
        {
            get { return this.m_idle; }
            set { this.m_idle = value; }
        }
        bool m_idle;

        /// <summary>
        /// The next time the background worker thread wakes up to get feeds
        /// </summary>
        public DateTime NextUpdate
        {
            get { return this.m_nextUpdate; }
            set { this.m_nextUpdate = value; }
        }
        DateTime m_nextUpdate;

        /// <summary>
        /// The default interval for all new feeds if no time is passed in.
        /// </summary>
        public TimeSpan DefaultInterval
        {
            get { return this.m_defaultInterval; }
            set { this.m_defaultInterval = value; }
        }
        TimeSpan m_defaultInterval = new TimeSpan(1, 0, 0);

        /// <summary>
        /// Constructor
        /// </summary>
        public GeoRssFeeds(RenderableObjectList rootLayer)
        {
            this.m_rootLayer = rootLayer;
            this.m_feeds = new List<GeoRssFeed>();

            this.InitializeBackgroundWorker();

            this.m_form = new GeoRssForm(this);
            //m_form.Show();

        }

        public void Dispose()
        {
            // TODO: clean up

            // close all feeds
            // clean up resources
        }

        /// <summary>
        /// Adds a pre-constructed feed to the list and its layer to the root layer
        /// </summary>
        /// <param name="feed">feed to add</param>
        /// <param name="addToRoot">sets whether or not the layer gets added to this root layer</param>
        public void Add(GeoRssFeed feed, bool addToRoot)
        {
            if (addToRoot) this.m_rootLayer.Add(feed.Layer);

            this.m_feeds.Add(feed);
            this.m_form.UpdateDataGridView();
        }

        /// <summary>
        /// Add a new geo rss feed
        /// </summary>
        /// <param name="name">name of feed</param>
        /// <param name="url">url for feed</param>
        public void Add(string name, string url)
        {
            this.Add(name, url, new TimeSpan (1, 0, 0));            
        }

        /// <summary>
        /// Add a new geo rss feed
        /// </summary>
        /// <param name="name">name of feed</param>
        /// <param name="url">url for feed</param>
        /// <param name="update">update interval.  If 0 then only gets it once</param>
        public void Add(string name, string url, TimeSpan update)
        {
            Icons layer = new Icons(name);

            this.Add(name, url, update, layer);
        }

        public void Add(string name, string url, TimeSpan update, string iconFileName)
        {
            Icons layer = new Icons(name);

            this.Add(name, url, update, layer, iconFileName);
        }

        /// <summary>
        /// Add a new geo rss feed
        /// </summary>
        /// <param name="name">name of feed</param>
        /// <param name="url">url for feed</param>
        /// <param name="update">update interval.  If 0 then only gets it once</param>
        /// <param name="layer">icon layer.  Added to rootlayer</param>
        public void Add(string name, string url, TimeSpan update, Icons layer)
        {
            this.m_rootLayer.Add(layer);

            this.m_feeds.Add(new GeoRssFeed(name, url, update, layer));
            this.m_form.UpdateDataGridView();
        }

        public void Add(string name, string url, TimeSpan update, Icons layer, string iconFileName)
        {
            this.m_rootLayer.Add(layer);

            this.m_feeds.Add(new GeoRssFeed(name, url, update, layer, iconFileName));
            this.m_form.UpdateDataGridView();
        }


        /// <summary>
        /// Removes specified feed
        /// </summary>
        /// <param name="name">name of feed to remove</param>
        public void RemoveByName(string name)
        {
            foreach (GeoRssFeed feed in this.m_feeds)
            {
                if (feed.Name == name) this.m_feeds.Remove(feed);
            }
        }

        /// <summary>
        /// Removed specified feed by URL.  Probably safer than removal by name
        /// </summary>
        /// <param name="url">url of feed to remove</param>
        public void RemoveByUrl(string url)
        {
            foreach (GeoRssFeed feed in this.m_feeds)
            {
                if (feed.Url == url) this.m_feeds.Remove(feed);
            }
        }

        # region Thread routines

        public void Start()
        {
            this.m_bw.RunWorkerAsync();
        }

        private void InitializeBackgroundWorker()
        {
            this.m_bw = new BackgroundWorker();

            this.m_bw.DoWork += new DoWorkEventHandler(this.bwDoWork);
            this.m_bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bwWorkerCompleted);
            this.m_bw.ProgressChanged += new ProgressChangedEventHandler(this.bwProgressChanged);
        }

        private void bwDoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                if (!this.Idle)
                {
                    this.m_nextUpdate = DateTime.MaxValue;
                    foreach (GeoRssFeed feed in this.m_feeds)
                    {
                        if (feed.NeedsUpdate ||
                            ((feed.UpdateInterval > TimeSpan.Zero) &&
                             (feed.LastUpdate + feed.UpdateInterval < DateTime.Now)))
                        {
                            feed.NeedsUpdate = true;
                            feed.Update();
                            feed.LastUpdate = DateTime.Now;
                        }

                        if (feed.UpdateInterval > TimeSpan.Zero)
                        {
                            if (feed.LastUpdate + feed.UpdateInterval < this.m_nextUpdate) this.m_nextUpdate = feed.LastUpdate + feed.UpdateInterval;
                        }
                    }
                }
                else
                {
                    this.m_nextUpdate = DateTime.Now;
                    this.m_nextUpdate.AddSeconds(1);
                }

                TimeSpan sleepTime = this.m_nextUpdate - DateTime.Now;
                if (sleepTime.Seconds < 1) sleepTime = new TimeSpan(0,0,0,1);

                Thread.Sleep(sleepTime);
            } 
            while (!this.m_done);
        }

        private void bwWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
                // resultLabel.Text = "Canceled";
            }
            else
            {
                // Finally, handle the case where the operation 
                // succeeded.
                // resultLabel.Text = e.Result.ToString();
            }

        }

        private void bwProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        #endregion

    }
}
