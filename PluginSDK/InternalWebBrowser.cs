using System;
using System.Windows.Forms;

namespace WorldWind
{
	public delegate void BrowserCloseDelegate();
	public delegate void BrowserNavigatedDelegate();

	/// <summary>
	/// Internal Web Browser panel with url bar and simple navigation buttons
	/// </summary>
	public class InternalWebBrowserPanel : Panel
	{
		private WebBrowser webBrowser;
		private ToolStrip webBrowserToolStrip;
		private ToolStripButton webBrowserBack;
		private ToolStripButton webBrowserForward;
		private ToolStripTextBox webBrowserURL;
		private ToolStripButton webBrowserGo;
		private ToolStripButton webBrowserClose;
		private ToolStripButton webBrowserStop;
		private Splitter splitter;
		
		public InternalWebBrowserPanel()
		{
            this.InitializeComponent();
		}

		public InternalWebBrowserPanel(string startUrl)
		{
            this.InitializeComponent();
			this.NavigateTo(startUrl);
		}

		#region Public Methods
		public void NavigateTo(string targetUrl)
		{
            this.webBrowser.Navigate(targetUrl);
            this.OnNavigate(targetUrl);
		}
		public bool IsTyping()
		{
			return this.webBrowserURL.Focused;
		}
		#endregion

		#region Events
		public delegate void BrowserCloseHandler();
		public delegate void BrowserNavigateHandler(string url);

		public event BrowserNavigateHandler Navigate;
		public event BrowserCloseHandler Close;

		protected void OnNavigate(string url)
		{
			if (this.Navigate != null)
			{
                this.Navigate(url);
			}

		}
		protected void OnClose()
		{
			if (this.Close != null)
			{
                this.Close();
			}
		}


		#endregion

		private void InitializeComponent()
		{
			this.webBrowser = new WebBrowser();
			this.webBrowserToolStrip = new ToolStrip();
			this.webBrowserBack = new ToolStripButton();
			this.webBrowserForward = new ToolStripButton();
			this.webBrowserURL = new ToolStripTextBox();
			this.webBrowserGo = new ToolStripButton();
			this.webBrowserStop = new ToolStripButton();
			this.webBrowserClose = new ToolStripButton();
			this.splitter = new Splitter();
			this.webBrowserToolStrip.SuspendLayout();
			this.SuspendLayout();
		
			this.SizeChanged += new EventHandler(this.Resized);

			// 
			// webBrowser
			// 
			this.webBrowser.Location = new System.Drawing.Point(0, 26);
			this.webBrowser.Margin = new Padding(5);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.Height = this.Height - this.webBrowserToolStrip.Height;
			this.webBrowser.Width = this.Width;
			this.webBrowser.ScriptErrorsSuppressed = true;
			this.webBrowser.TabIndex = 3;
			this.webBrowser.Url = new Uri("http://worldwind.arc.nasa.gov/", UriKind.Absolute);
			this.webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
			
			// 
			// webBrowserToolStrip
			// 
			this.webBrowserToolStrip.ImageScalingSize = new System.Drawing.Size(18, 18);
			this.webBrowserToolStrip.Items.AddRange(new ToolStripItem[] {
				this.webBrowserBack,
				this.webBrowserForward,
				this.webBrowserStop,
				this.webBrowserURL,
				this.webBrowserGo,
				this.webBrowserClose});
			this.webBrowserToolStrip.Dock = DockStyle.Top;
			this.webBrowserToolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			//this.webBrowserToolStrip.LayoutStyle = ToolStripLayoutStyle.Table;
			this.webBrowserToolStrip.Location = new System.Drawing.Point(0, 0);
			this.webBrowserToolStrip.Name = "webBrowserToolStrip";
			this.webBrowserToolStrip.Padding = new Padding(0, 0, 1, 1);
			this.webBrowserToolStrip.Height = 26;
			this.webBrowserToolStrip.Width = this.Width;
			this.webBrowserToolStrip.TabIndex = 4;
			this.webBrowserToolStrip.Text = "toolStrip1";
			// 
			// webBrowserBack
			// 
			this.webBrowserBack.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.webBrowserBack.Image = Properties.Resources.back;
			this.webBrowserBack.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.webBrowserBack.Name = "webBrowserBack";
			this.webBrowserBack.Size = new System.Drawing.Size(23, 22);
			this.webBrowserBack.Text = "Back";
			this.webBrowserBack.Click += new EventHandler(this.webBrowserBack_Click);
			// 
			// webBrowserForward
			// 
			this.webBrowserForward.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.webBrowserForward.Image = Properties.Resources.forward;
			this.webBrowserForward.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.webBrowserForward.Name = "webBrowserForward";
			this.webBrowserForward.Size = new System.Drawing.Size(23, 22);
			this.webBrowserForward.Text = "Forward";
			this.webBrowserForward.Click += new EventHandler(this.webBrowserForward_Click);
			// 
			// webBrowserURL
			// 
			this.webBrowserURL.AcceptsReturn = true;
			this.webBrowserURL.Name = "webBrowserURL";
			this.webBrowserURL.Size = new System.Drawing.Size(200, 25);
			this.webBrowserURL.KeyPress += new KeyPressEventHandler(this.webBrowserURL_KeyPress);
			// 
			// webBrowserGo
			// 
			this.webBrowserGo.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.webBrowserGo.Image = Properties.Resources.go;
			this.webBrowserGo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.webBrowserGo.Name = "webBrowserGo";
			this.webBrowserGo.Size = new System.Drawing.Size(23, 22);
			this.webBrowserGo.Text = "Go";
			this.webBrowserGo.Click += new EventHandler(this.webBrowserGo_Click);
			// 
			// webBrowserStop
			// 
			this.webBrowserStop.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.webBrowserStop.Image = Properties.Resources.stop;
			this.webBrowserStop.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.webBrowserStop.Name = "webBrowserStop";
			this.webBrowserStop.Size = new System.Drawing.Size(23, 22);
			this.webBrowserStop.Text = "Stop";
			this.webBrowserStop.Click += new EventHandler(this.webBrowserStop_Click);
			// 
			// webBrowserClose
			// 
			this.webBrowserClose.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.webBrowserClose.Image = Properties.Resources.close;
			this.webBrowserClose.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.webBrowserClose.Name = "webBrowserClose";
			this.webBrowserClose.Size = new System.Drawing.Size(23, 22);
			this.webBrowserClose.Text = "Close";
			this.webBrowserClose.Alignment = ToolStripItemAlignment.Right;
			this.webBrowserClose.Click += new EventHandler(this.webBrowserClose_Click);


			this.Controls.Add(this.webBrowserToolStrip);
			this.Controls.Add(this.webBrowser);
			this.webBrowserToolStrip.ResumeLayout(false);
			this.webBrowserToolStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#region Browser control methods
		/// <summary>
		/// Following methods handle navigation buttons for the web browser.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void webBrowserBack_Click(object sender, EventArgs e)
		{
            this.webBrowser.GoBack();
		}

		private void webBrowserForward_Click(object sender, EventArgs e)
		{
            this.webBrowser.GoForward();
		}

		private void webBrowserGo_Click(object sender, EventArgs e)
		{
            this.webBrowser.Navigate(this.webBrowserURL.Text);
		}

		private void webBrowserStop_Click(object sender, EventArgs e)
		{
            this.webBrowser.Stop();
		}

		private void webBrowserClose_Click(object sender, EventArgs e)
		{
            this.OnClose();
		}

		private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
            this.webBrowserURL.Text = this.webBrowser.Url.ToString();
		}

		private void webBrowserURL_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)13)
			{
                this.webBrowser.Navigate(this.webBrowserURL.Text);
			}
		}

		private void Resized(object sender, EventArgs e)
		{
			this.webBrowser.Width = this.Width;
			this.webBrowser.Height = this.Height - this.webBrowserToolStrip.Height;
		}
		#endregion

		public class BrowserEventArgs : EventArgs
		{
		}



	}
}

