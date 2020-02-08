using System;
using System.Globalization;
using System.Text;
using System.Web;

namespace WorldWind
{
	/// <summary>
	/// worldwind:// URI class
	/// </summary>
	public class WorldWindUri 
	{		
		private string _world = "Earth";
		private Angle _bank = Angle.NaN;
		private Angle _latitude = Angle.NaN;
		private Angle _longitude = Angle.NaN;
		private Angle _viewRange = Angle.NaN;
		private Angle  _tilt = Angle.NaN;
		private Angle _direction = Angle.NaN;
		private double _altitude = double.NaN;
		private string _layer;
		private string _rawUrl = "";
        private string _preserveCase = "";
		
		public const string Scheme = "worldwind";

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.WorldWindUri"/> class.
		/// </summary>
		public WorldWindUri()
		{
		}

		/// <summary>
		/// Construct a new worldwind uri from camera position.
		/// </summary>
		public WorldWindUri(string worldName, CameraBase camera )
		{
            this._world = worldName;
            this._latitude=camera.Latitude;
            this._longitude=camera.Longitude;
            this._altitude=camera.Altitude;
			if (Math.Abs(camera.Heading.Degrees)>0.1) this._direction = camera.Heading;
			if (Math.Abs(camera.Tilt.Degrees)>0.1) this._tilt = camera.Tilt;
			if (Math.Abs(camera.Bank.Degrees)>0.1) this._bank = camera.Bank;
		}

		/// <summary>
		/// Parses a worldwind uri string
		/// Updated by CM = worldwind://goto/lat=51.41&amp;lon=5.479
        ///		   or	worldwind://goto/lat=51.41&amp;lon5.479&amp;view=0.25
		///			or	
		///	worldwind://wmsimage=displayname%3Dtestlayer%2526transparency%3D50%2526altitude%3D10000%2526link%3Dhttp%3A%2F%2Fviz.globe.gov%2Fviz-bin%2Fwmt.cgi%3Fservice%3DWMS%26version%3D1.1.1%26request%3DGetMap%26layers%3DRATMAX%26format%3Dimage%2Fpng%26width%3D512%26height%3D512%26bbox%3D-180%2C-90%2C180%2C90%26srs%3DEPSG%3A4326
		/// </summary>
		/// <param name="worldWindUri"></param>
		/// <returns></returns>
		public static WorldWindUri Parse( string worldWindUri )
		{
			try
			{
                WorldWindUri uri = new WorldWindUri();
                worldWindUri = worldWindUri.Trim();

                uri._preserveCase = worldWindUri; //set our case-sensitive one, hack for worldwind://install=

                //then continue as before
                worldWindUri = worldWindUri.ToLower(CultureInfo.InvariantCulture);
                uri._rawUrl = worldWindUri;
				if(!worldWindUri.StartsWith( Scheme + "://"))
					throw new UriFormatException("Invalid protocol, expected " + Scheme + "://");

				string url = worldWindUri.Replace( Scheme + "://", "");
				if(url.Length == 0)
					throw new UriFormatException("Incomplete URI");

				//url = url.Replace("///", "!");			// I wanted to allow a sure to split "functions" inside of the url by using "//"
				string[] functions = url.Split('!');
				
				foreach(string function in functions)
				{
					if(function.IndexOf("goto/") == 0)
					{
						string functionString = function.Replace("goto/", "").Trim();
				
						string[] functionParameters = functionString.Split('&');
					
						foreach(string curParam in functionParameters)
						{
							string[] nv = curParam.Split(new char[]{'='},2);
							if (nv.Length!=2)
								continue;
							string key = nv[0].ToLower();
							string value = nv[1];
							double doubleVal = double.NaN;
							string urlDecodedValue = HttpUtility.UrlDecode(value);
							double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleVal);

							switch(key.ToLower()) 
							{
								case "lat":
								case "latitude":
									uri.Latitude = Angle.FromDegrees(doubleVal);
									break;
								case "lon":
								case "longitude":
									uri.Longitude = Angle.FromDegrees(doubleVal);
									break;
								case "altitude":
								case "alt":
									uri.Altitude = doubleVal;
									break;
								case "view":
								case "viewrange":
									uri.ViewRange = Angle.FromDegrees(doubleVal);
									break;
									// TODO: Decide on URI parameter names
								case "bank":
									uri.Bank = Angle.FromDegrees(doubleVal);
									break;
								case "dir":
								case "direction":
									uri.Direction = Angle.FromDegrees(doubleVal);
									break;
								case "tilt":
									uri.Tilt = Angle.FromDegrees(doubleVal);
									break;
								case "world":
									uri.World = value;
									break;
								case "layer":
									uri.Layer = urlDecodedValue;
									break;
							}
						}
					}					
				}

				return uri;
			}
			catch(Exception caught)
			{
				throw new UriFormatException( "The worldwind:// URI could not be parsed. (The URI used was: "+worldWindUri+" and the error generated was: "+caught.Message+")"); 
			}
		}

		public string PreserveCase
		{
			get
			{
				return this._preserveCase;
			}
			set
			{
				this._preserveCase = value;
			}
		}

		public string RawUrl
		{
			get
			{
				return this._rawUrl;
			}
			set
			{
                this._rawUrl = value;
			}
		}

		public string World
		{
			get
			{
				return this._world;
			}
			set
			{
                this._world = value;
			}
		}

		public Angle Latitude
		{
			get
			{
				return this._latitude;
			}
			set
			{
                this._latitude = value;
			}
		}

		public Angle Longitude
		{
			get
			{
				return this._longitude;
			}
			set
			{
                this._longitude = value;
			}
		}

		public double Altitude
		{
			get
			{
				return this._altitude;
			}
			set
			{
                this._altitude = value;
			}
		}

		public Angle ViewRange
		{
			get
			{
				return this._viewRange;
			}
			set
			{
                this._viewRange = value;
			}
		}

		public Angle Direction
		{
			get
			{
				return this._direction;
			}
			set
			{
                this._direction = value;
			}
		}

		public Angle Tilt
		{
			get
			{
				return this._tilt;
			}
			set
			{
                this._tilt = value;
			}
		}

		/// <summary>
		/// Bank angle (rotation around camera eye->target axis)
		/// </summary>
		public Angle Bank
		{
			get
			{
				return this._bank;
			}
			set
			{
                this._bank = value;
			}
		}

		/// <summary>
		/// Layer name to enable for this uri
		/// </summary>
		public string Layer
		{
			get
			{
				return this._layer;
			}
			set
			{
                this._layer = value;
			}
		}		

		/// <summary>
		/// Generates a WW URI with filtering to keep the resulting string short and clean
		/// </summary>
		/// <returns>A filtered WW URI</returns>
		public override string ToString()
		{
			return this.ToString(true);
		}
 
		/// <summary>
		/// Generates a WW URI and optionally filters it
		/// </summary>
		/// <param name="bEnableFiltering">Forces a reduced string to be generated, enables filtering</param>
		/// <returns>A WW URI, filtered if bEnableFiltering is true</returns>
		public string ToString(bool bEnableFiltering) 
		{
			StringBuilder sb = new StringBuilder( Scheme + "://goto/" );
 
			sb.Append("world=");
			sb.Append(this.World);
			sb.Append("&lat=");
			sb.Append(this._latitude.Degrees.ToString("f5", CultureInfo.InvariantCulture));
			sb.Append("&lon=");
			sb.Append(this._longitude.Degrees.ToString("f5", CultureInfo.InvariantCulture));
			if (!Angle.IsNaN(this._viewRange))
				sb.Append("&view=" + this._viewRange.Degrees.ToString("f1", CultureInfo.InvariantCulture));
			else if (!double.IsNaN(this._altitude))
				sb.Append("&alt=" + this._altitude.ToString("f0", CultureInfo.InvariantCulture));
			// TODO: Decide on URI parameter names
			if (!Angle.IsNaN(this._direction) && (Math.Abs(this._direction.Degrees)>0.1 || !bEnableFiltering))
				sb.Append("&dir=" + this._direction.Degrees.ToString("f1", CultureInfo.InvariantCulture));
			if (!Angle.IsNaN(this._tilt) && (Math.Abs(this._tilt.Degrees)<89.5 || !bEnableFiltering))
				sb.Append("&tilt=" + this._tilt.Degrees.ToString("f1", CultureInfo.InvariantCulture));
			if (!Angle.IsNaN(this._bank) && (Math.Abs(this._bank.Degrees)>0.1 || !bEnableFiltering))
				sb.Append("&bank=" + this._bank.Degrees.ToString("f1", CultureInfo.InvariantCulture));
			if(this._layer != null)
				sb.Append("&layer=" + HttpUtility.UrlEncode(this._layer));
			return sb.ToString();
		}
	}
}
