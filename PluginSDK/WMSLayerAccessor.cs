using System.Globalization;
using System.Xml;

namespace WorldWind
{
	/// <summary>
	/// Calculates URLs for WMS layers.
	/// </summary>
	public class WmsImageStore : ImageStore
	{
		#region Private Members
		
		string m_serverGetMapUrl;
		string m_wmsLayerName;
		string m_wmsLayerStyle;
		string m_imageFormat;
		string m_version;
        string m_username;
        string m_password;
		int	m_textureSizePixels = 512;

		#endregion	

		#region Properties
		
		public override bool IsDownloadableLayer
		{
			get
			{
				return true;
			}
		}

		public virtual string ServerGetMapUrl
		{
			get
			{
				return this.m_serverGetMapUrl;
			}
			set
			{
                this.m_serverGetMapUrl = value;
			}
		}


        public virtual string Username
        {
            get
            {
                return this.m_username;
            }
            set
            {
                this.m_username = value;
            }
        }


        public virtual string Password
        {
            get
            {
                return this.m_password;
            }
            set
            {
                this.m_password = value;
            }
        }

		public virtual string WMSLayerName
		{
			get
			{
				return this.m_wmsLayerName;
			}
			set
			{
                this.m_wmsLayerName = value;
			}
		}

		public virtual string WMSLayerStyle
		{
			get
			{
				return this.m_wmsLayerStyle;
			}
			set
			{
                this.m_wmsLayerStyle = value;
			}
		}

		public virtual string ImageFormat
		{
			get
			{
				return this.m_imageFormat;
			}
			set
			{
                this.m_imageFormat = value;
			}
		}

		public virtual string Version
		{
			get
			{
				return this.m_version;
			}
			set
			{
                this.m_version = value;
			}
		}

		/// <summary>
		/// Bitmap width/height
		/// </summary>
		public int TextureSizePixels
		{
			get
			{
				return this.m_textureSizePixels;
			}
			set
			{
                this.m_textureSizePixels = value;
			}
		}

		#endregion

		#region Public Methods

		protected override string GetDownloadUrl(QuadTile qt)
		{
			if(this.m_serverGetMapUrl.IndexOf('?')>=0)
			{
				// Allow custom format string url
				// http://server.net/path?imageformat=png&width={WIDTH}&north={NORTH}...
				string url = this.m_serverGetMapUrl;
				url = url.Replace("{WIDTH}", this.m_textureSizePixels.ToString(CultureInfo.InvariantCulture));
				url = url.Replace("{HEIGHT}", this.m_textureSizePixels.ToString(CultureInfo.InvariantCulture));
				url = url.Replace("{WEST}", qt.West.ToString(CultureInfo.InvariantCulture));
				url = url.Replace("{EAST}", qt.East.ToString(CultureInfo.InvariantCulture));
				url = url.Replace("{NORTH}", qt.North.ToString(CultureInfo.InvariantCulture));
				url = url.Replace("{SOUTH}", qt.South.ToString(CultureInfo.InvariantCulture));

				return url;
			}
			else
			{
				string url = string.Format(CultureInfo.InvariantCulture, 
					"{0}?request=GetMap&layers={1}&srs=EPSG:4326&width={2}&height={3}&bbox={4},{5},{6},{7}&format={8}&version={9}&styles={10}", this.m_serverGetMapUrl, this.m_wmsLayerName, this.m_textureSizePixels, this.m_textureSizePixels, 
					qt.West, qt.South, qt.East, qt.North, this.m_imageFormat, this.m_version, this.m_wmsLayerStyle );

				return url;
			}
		}

        public override XmlNode ToXml(XmlDocument worldDoc)
        {
            XmlNode iaNode = base.ToXml(worldDoc);


            XmlNode tileServiceNode = worldDoc.CreateElement("WMSAccessor");

            XmlNode UsernameNode = worldDoc.CreateElement("Username");
            UsernameNode.AppendChild(worldDoc.CreateTextNode(this.Username));
            tileServiceNode.AppendChild(UsernameNode);

            XmlNode PasswordNode = worldDoc.CreateElement("Password");
            PasswordNode.AppendChild(worldDoc.CreateTextNode(this.Password));
            tileServiceNode.AppendChild(PasswordNode);

            XmlNode imageFormatNode = worldDoc.CreateElement("ImageFormat");
            imageFormatNode.AppendChild(worldDoc.CreateTextNode(this.ImageFormat));
            tileServiceNode.AppendChild(imageFormatNode);

            XmlNode getMapUrlNode = worldDoc.CreateElement("ServerGetMapUrl");
            getMapUrlNode.AppendChild(worldDoc.CreateTextNode(this.ServerGetMapUrl));
            tileServiceNode.AppendChild(getMapUrlNode);

            XmlNode versionNode = worldDoc.CreateElement("Version");
            versionNode.AppendChild(worldDoc.CreateTextNode(this.Version));
            tileServiceNode.AppendChild(versionNode);

            XmlNode layerNameNode = worldDoc.CreateElement("WMSLayerName");
            layerNameNode.AppendChild(worldDoc.CreateTextNode(this.WMSLayerName));
            tileServiceNode.AppendChild(layerNameNode);

            XmlNode styleNode = worldDoc.CreateElement("WMSLayerStyle");
            styleNode.AppendChild(worldDoc.CreateTextNode(this.WMSLayerStyle));
            tileServiceNode.AppendChild(styleNode);

            XmlNode serverLogoNode = worldDoc.CreateElement("ServerLogoFilePath");
            serverLogoNode.AppendChild(worldDoc.CreateTextNode(this.ServerLogo));
            tileServiceNode.AppendChild(serverLogoNode);

            iaNode.AppendChild(tileServiceNode);

            return iaNode;
        }
		#endregion
	}
}
