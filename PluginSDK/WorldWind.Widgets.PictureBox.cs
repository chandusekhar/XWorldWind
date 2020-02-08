using System;
using System.Diagnostics;
using System.Windows.Forms;
using Utility;

namespace WorldWind
{
	/// <summary>
	/// Summary description for PictureBox.
	/// </summary>
	public class PictureBox : IWidget, IInteractive
	{
		string m_Text = "";
		byte m_Opacity = 255;
		System.Drawing.Point m_Location = new System.Drawing.Point(0,0);
		System.Drawing.Size m_Size = new System.Drawing.Size(0,0);
		bool m_Visible = true;
		bool m_Enabled = true;
		IWidget m_ParentWidget;
		object m_Tag;
		string m_Name = "";
		string m_SaveFilePath;
		System.Drawing.Color m_ForeColor = System.Drawing.Color.White;

		double m_RefreshTime;
		System.Timers.Timer m_RefreshTimer = new System.Timers.Timer(100000);

		string m_ImageUri;
		string clickableUrl;

        public bool LoadAsUncompressed = false;

		public string ClickableUrl
		{
			get{ return this.clickableUrl; }
			set{ this.clickableUrl = value; }
		}

		public double RefreshTime
		{
			get
			{
				return this.m_RefreshTime;
			}
			set
			{
                this.m_RefreshTime = value;
				if(this.m_RefreshTime > 0) this.m_RefreshTimer.Interval = value;
			}
		}
		public byte Opacity
		{
			get
			{
				return this.m_Opacity;
			}
			set
			{
                this.m_Opacity = value;
			}
		}

		public PictureBox()
		{
			
		}
		
		#region Properties

		public string SaveFilePath
		{
			get
			{
				return this.m_SaveFilePath;
			}
			set
			{
                this.m_SaveFilePath = value;
			}
		}

		public string ImageUri
		{
			get
			{
				return this.m_ImageUri;
			}
			set
			{
                this.m_ImageUri = value;
			}
		}

		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
                this.m_Name = value;
			}
		}
		public System.Drawing.Color ForeColor
		{
			get
			{
				return this.m_ForeColor;
			}
			set
			{
                this.m_ForeColor = value;
			}
		}
		public string Text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
                this.m_Text = value;
			}
		}
		#endregion

		#region IWidget Members

		public IWidget ParentWidget
		{
			get
			{
				return this.m_ParentWidget;
			}
			set
			{
                this.m_ParentWidget = value;
			}
		}

		public bool Visible
		{
			get
			{
				return this.m_Visible;
			}
			set
			{
                this.m_Visible = value;
			}
		}

		public object Tag
		{
			get
			{
				return this.m_Tag;
			}
			set
			{
                this.m_Tag = value;
			}
		}

		public IWidgetCollection ChildWidgets
		{
			get
			{
				// TODO:  Add TextLabel.ChildWidgets getter implementation
				return null;
			}
			set
			{
				// TODO:  Add TextLabel.ChildWidgets setter implementation
			}
		}

		public System.Drawing.Size ClientSize
		{
			get
			{
				return this.m_Size;
			}
			set
			{
                this.m_Size = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
                this.m_Enabled = value;
			}
		}

		public System.Drawing.Point ClientLocation
		{
			get
			{
				return this.m_Location;
			}
			set
			{
                this.m_Location = value;
			}
		}

		public System.Drawing.Point AbsoluteLocation
		{
			get
			{
				if(this.m_ParentWidget != null)
				{
					return new System.Drawing.Point(this.m_Location.X + this.m_ParentWidget.AbsoluteLocation.X, this.m_Location.Y + this.m_ParentWidget.AbsoluteLocation.Y);
					
				}
				else
				{
					return this.m_Location;
				}
			}
		}

		Texture m_ImageTexture = null;
		string displayText;

		bool isLoading = false;
		Sprite m_sprite = null;
		SurfaceDescription m_surfaceDescription;
		public bool IsLoaded;
        string m_currentImageUri;
        bool m_isMouseInside;

        public event EventHandler OnMouseEnterEvent;
        public event EventHandler OnMouseLeaveEvent;
        public event MouseEventHandler OnMouseUpEvent;

		public void Render(DrawArgs drawArgs)
		{
			if(this.m_Visible)
			{
				if(this.m_ImageTexture == null)
				{
					if(!this.m_RefreshTimer.Enabled)
					{
                        this.displayText = "Loading Image...";
                        if (this.m_RefreshTime > 0)
                        {
                            this.m_RefreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.m_RefreshTimer_Elapsed);
                            this.m_RefreshTimer.Start();
                        }
                        else
                        {
                            this.m_RefreshTimer_Elapsed(null, null);
                        }
					}
				}

                if (DrawArgs.LastMousePosition.X > this.AbsoluteLocation.X + this.clickBuffer &&
                    DrawArgs.LastMousePosition.X < this.AbsoluteLocation.X + this.ClientSize.Width - this.clickBuffer &&
                        DrawArgs.LastMousePosition.Y > this.AbsoluteLocation.Y + this.clickBuffer &&
                        DrawArgs.LastMousePosition.Y < this.AbsoluteLocation.Y + this.ClientSize.Height - this.clickBuffer)
                {
                    if (!this.m_isMouseInside)
                    {
                        this.m_isMouseInside = true;
                        if (this.OnMouseEnterEvent != null)
                        {
                            this.OnMouseEnterEvent(this, null);
                        }
                    }
                }
                else
                {
                    if (this.m_isMouseInside)
                    {
                        this.m_isMouseInside = false;
                        if (this.OnMouseLeaveEvent != null)
                        {
                            this.OnMouseLeaveEvent(this, null);
                        }
                    }
                }

                if (this.m_ImageTexture != null && this.m_currentImageUri != this.m_ImageUri)
                {
                    this.m_RefreshTimer_Elapsed(null, null);
                }

				if(this.displayText != null)
				{
					drawArgs.defaultDrawingFont.DrawText(
						null, this.displayText,
						new System.Drawing.Rectangle(this.AbsoluteLocation.X, this.AbsoluteLocation.Y, this.m_Size.Width, this.m_Size.Height),
						DrawTextFormat.None, this.m_ForeColor);
				}

				if(this.m_ImageTexture != null && !this.isLoading)
				{
					drawArgs.device.SetTexture(0, this.m_ImageTexture);
							
					drawArgs.device.SetRenderState(RenderState.ZBufferEnable = false;

					System.Drawing.Point ul = new System.Drawing.Point(this.AbsoluteLocation.X, this.AbsoluteLocation.Y);
					System.Drawing.Point ur = new System.Drawing.Point(this.AbsoluteLocation.X + this.m_Size.Width, this.AbsoluteLocation.Y);
					System.Drawing.Point ll = new System.Drawing.Point(this.AbsoluteLocation.X, this.AbsoluteLocation.Y + this.m_Size.Height);
					System.Drawing.Point lr = new System.Drawing.Point(this.AbsoluteLocation.X + this.m_Size.Width, this.AbsoluteLocation.Y + this.m_Size.Height);
									
					if(this.m_sprite == null) this.m_sprite = new Sprite(drawArgs.device);

                    this.m_sprite.Begin(SpriteFlags.AlphaBlend);

					float xscale = (float)(ur.X - ul.X) / (float) this.m_surfaceDescription.Width;
					float yscale = (float)(lr.Y - ur.Y) / (float) this.m_surfaceDescription.Height;
                    this.m_sprite.Transform = Matrix.Scaling(xscale,yscale,0);
                    this.m_sprite.Transform *= Matrix.Translation(0.5f * (ul.X + ur.X), 0.5f * (ur.Y + lr.Y), 0);
                    this.m_sprite.Draw(this.m_ImageTexture,
						new Vector3(this.m_surfaceDescription.Width / 2, this.m_surfaceDescription.Height / 2,0),
						Vector3.Zero,
						System.Drawing.Color.FromArgb(this.m_Opacity, 255, 255, 255).ToArgb()
						);
				
					// Reset transform to prepare for text rendering later
                    this.m_sprite.Transform = Matrix.Identity;
                    this.m_sprite.End();
				}
			}	
		}
		#endregion

        
		bool isUpdating;
		private void m_RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
            try
            {
                if (this.isUpdating)
                    return;

                this.isUpdating = true;

                if (this.m_ImageUri == null)
                    return;

                if (this.m_ImageUri.ToLower().StartsWith("http://"))
                {
                    bool forceDownload = false;
                    if (this.m_SaveFilePath == null)
                    {
                        // TODO: hack, need to get the correct cache directory
                        this.m_SaveFilePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Cache\\PictureBoxImages\\temp";
                        forceDownload = true;
                    }
                    System.IO.FileInfo saveFile = new System.IO.FileInfo(this.m_SaveFilePath);
                    
                    // Offline check added
                    if (!World.Settings.WorkOffline && (forceDownload || !saveFile.Exists || 
                        (this.m_RefreshTime > 0 && saveFile.LastWriteTime.Subtract(DateTime.Now) > TimeSpan.FromSeconds(this.m_RefreshTime))))
                    {
                        //download it
                        try
                        {
                            Net.WebDownload webDownload = new Net.WebDownload(this.m_ImageUri);
                            webDownload.DownloadType = Net.DownloadType.Unspecified;

                            if (!saveFile.Directory.Exists)
                                saveFile.Directory.Create();

                            webDownload.DownloadFile(this.m_SaveFilePath);
                        }
                        catch { }
                    }
                }
                else
                {
                    this.m_SaveFilePath = this.m_ImageUri;
                }

                if (this.m_ImageTexture != null && !this.m_ImageTexture.Disposed)
                {
                    this.m_ImageTexture.Dispose();
                    this.m_ImageTexture = null;
                }

                if (!System.IO.File.Exists(this.m_SaveFilePath))
                {
                    this.displayText = "Image Not Found";
                    return;
                }

                if (this.LoadAsUncompressed)
                {
                    this.m_ImageTexture = TextureLoader.FromFile(DrawArgs.Device, this.m_SaveFilePath, 0, 0, 1, 0, Format.Unknown, Pool.Managed, Filter.None, Filter.None, 0);
                }
                else
                {
                    this.m_ImageTexture = ImageHelper.LoadTexture(this.m_SaveFilePath);
                }

                this.m_surfaceDescription = this.m_ImageTexture.GetLevelDescription(0);

                int width = this.ClientSize.Width;
                int height = this.ClientSize.Height;

                if (this.ClientSize.Width <= 0)
                {
                    width = this.m_surfaceDescription.Width;
                }
                if (this.ClientSize.Height <= 0)
                {
                    height = this.m_surfaceDescription.Height;
                }

                if (this.ParentWidget is Form && this.SizeParentToImage)
                {
                    Form parentForm = (Form) this.ParentWidget;
                    parentForm.ClientSize = new System.Drawing.Size(width, height + parentForm.HeaderHeight);
                }
                else if(this.SizeParentToImage)
                {
                    this.ParentWidget.ClientSize = new System.Drawing.Size(width, height);
                }


                this.ClientSize = new System.Drawing.Size(width, height);
                this.m_currentImageUri = this.m_ImageUri;

                this.IsLoaded = true;
                this.isUpdating = false;
                this.displayText = null;
                if (this.m_RefreshTime == 0 && this.m_RefreshTimer.Enabled) this.m_RefreshTimer.Stop();

            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
		}

        public bool SizeParentToImage = false;
		#region IInteractive Members

		public bool OnKeyDown(KeyEventArgs e)
		{
			// TODO:  Add PictureBox.OnKeyDown implementation
			return false;
		}

		public bool OnKeyUp(KeyEventArgs e)
		{
			// TODO:  Add PictureBox.OnKeyUp implementation
			return false;
		}

		public bool OnKeyPress(KeyPressEventArgs e)
		{
			// TODO:  Add PictureBox.OnKeyPress implementation
			return false;
		}

		public bool OnMouseDown(MouseEventArgs e)
		{
			// TODO:  Add PictureBox.OnMouseDown implementation
			return false;
		}

		public bool OnMouseEnter(EventArgs e)
		{
			// TODO:  Add PictureBox.OnMouseEnter implementation
			return false;
		}

		public bool OnMouseLeave(EventArgs e)
		{
			// TODO:  Add PictureBox.OnMouseLeave implementation
			return false;
		}

		public bool OnMouseMove(MouseEventArgs e)
		{
			// TODO:  Add PictureBox.OnMouseMove implementation
			return false;
		}

		private int clickBuffer = 5;

		public bool OnMouseUp(MouseEventArgs e)
		{
            if (!this.Visible)
                return false;

            if (e.X > this.AbsoluteLocation.X + this.clickBuffer && e.X < this.AbsoluteLocation.X + this.ClientSize.Width - this.clickBuffer &&
                        e.Y > this.AbsoluteLocation.Y + this.clickBuffer && e.Y < this.AbsoluteLocation.Y + this.ClientSize.Height - this.clickBuffer)
            {
                if (this.OnMouseUpEvent != null)
                {
                    this.OnMouseUpEvent(this, e);
                }
            }

			if(this.ClickableUrl != null && e.X > this.AbsoluteLocation.X + this.clickBuffer && e.X < this.AbsoluteLocation.X + this.ClientSize.Width - this.clickBuffer &&
               e.Y > this.AbsoluteLocation.Y + this.clickBuffer && e.Y < this.AbsoluteLocation.Y + this.ClientSize.Height - this.clickBuffer)
			{
				if (World.Settings.UseInternalBrowser || this.clickableUrl.StartsWith(@"worldwind://"))
				{
					SplitContainer sc = (SplitContainer)DrawArgs.ParentControl.Parent.Parent;
					InternalWebBrowserPanel browser = (InternalWebBrowserPanel)sc.Panel1.Controls[0];
					browser.NavigateTo(this.clickableUrl);
				}
				else
				{
					ProcessStartInfo psi = new ProcessStartInfo();
					psi.FileName = this.clickableUrl;
					psi.Verb = "open";
					psi.UseShellExecute = true;

					psi.CreateNoWindow = true;
					Process.Start(psi);
				}

                return true;
			}
			return false;
		}

		public bool OnMouseWheel(MouseEventArgs e)
		{
			// TODO:  Add PictureBox.OnMouseWheel implementation
			return false;
		}

		#endregion
	}
}