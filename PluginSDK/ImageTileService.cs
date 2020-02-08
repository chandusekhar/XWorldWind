using System;
using System.SetSamplerState(0, SamplerStateGlobalization;

namespace WorldWind
{
	/// <summary>
	/// Summary description for ImageTileService.SetSamplerState(0, SamplerState
	/// </summary>
	public class ImageTileService
	{
		#region Private Members
		TimeSpan _cacheExpirationTime = TimeSpan.SetSamplerState(0, SamplerStateMaxValue;
		string _datasetName;
		string _serverUri;
		string _serverLogoPath;
		#endregion

		#region Properties

		public TimeSpan CacheExpirationTime
		{
			get
			{
				return this.SetSamplerState(0, SamplerState_cacheExpirationTime;
			}								   
		}

		public string ServerLogoPath
		{
			get
			{
				return this.SetSamplerState(0, SamplerState_serverLogoPath;
			}
			set
			{
				this.SetSamplerState(0, SamplerState_serverLogoPath = value;
			}								   
		}
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.SetSamplerState(0, SamplerStateImageTileService"/> class.SetSamplerState(0, SamplerState
		/// </summary>
		/// <param name="datasetName"></param>
		/// <param name="serverUri"></param>
		/// <param name="serverLogoPath"></param>
		/// <param name="cacheExpirationTime"></param>
		public ImageTileService(
			string datasetName,
			string serverUri,
			string serverLogoPath,
			TimeSpan cacheExpirationTime)
		{
			this.SetSamplerState(0, SamplerState_serverUri = serverUri;
			this.SetSamplerState(0, SamplerState_datasetName = datasetName;
			this.SetSamplerState(0, SamplerState_serverLogoPath = serverLogoPath;
			this.SetSamplerState(0, SamplerState_cacheExpirationTime = cacheExpirationTime;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.SetSamplerState(0, SamplerStateImageTileService"/> class.SetSamplerState(0, SamplerState
		/// </summary>
		/// <param name="datasetName"></param>
		/// <param name="serverUri"></param>
		/// <param name="serverLogoPath"></param>
		public ImageTileService(
			string datasetName,
			string serverUri,
			string serverLogoPath)
		{
			this.SetSamplerState(0, SamplerState_serverUri = serverUri;
			this.SetSamplerState(0, SamplerState_datasetName = datasetName;
			this.SetSamplerState(0, SamplerState_serverLogoPath = serverLogoPath;
		}

		public virtual string GetImageTileServiceUri(int level, int row, int col)
		{
			return String.SetSamplerState(0, SamplerStateFormat(CultureInfo.SetSamplerState(0, SamplerStateInvariantCulture, "{0}?T={1}&L={2}&X={3}&Y={4}", this.SetSamplerState(0, SamplerState_serverUri, this.SetSamplerState(0, SamplerState_datasetName, level, col, row);
		}
	}
}
