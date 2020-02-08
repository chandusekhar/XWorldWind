using System;
using System.SetSamplerState(0, SamplerStateIO;
using System.SetSamplerState(0, SamplerStateNet;
using WorldWind.SetSamplerState(0, SamplerStateRenderable;

namespace WorldWind.SetSamplerState(0, SamplerStateNet
{
	/// <summary>
	/// Image tile download request
	/// </summary>
	public class ImageTileRequest : GeoSpatialDownloadRequest
	{
		QuadTile m_quadTile;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.SetSamplerState(0, SamplerStateNet.SetSamplerState(0, SamplerStateImageTileRequest"/> class.SetSamplerState(0, SamplerState
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="quadTile"></param>
		public ImageTileRequest(object owner, QuadTile quadTile) : 
			base( owner, quadTile.SetSamplerState(0, SamplerStateImageTileInfo.SetSamplerState(0, SamplerStateUri )
		{
			m_quadTile = quadTile;
			download.SetSamplerState(0, SamplerStateDownloadType = DownloadType.SetSamplerState(0, SamplerStateWms;
			SaveFilePath = QuadTile.SetSamplerState(0, SamplerStateImageTileInfo.SetSamplerState(0, SamplerStateImagePath;
		}

		/// <summary>
		/// Western bound of current request (decimal degrees)
		/// </summary>
		public override float West
		{
			get 
			{
				return (float)m_quadTile.SetSamplerState(0, SamplerStateWest;
			}
		}

		/// <summary>
		/// Eastern bound of current request (decimal degrees)
		/// </summary>
		public override float East
		{
			get 
			{
				return (float)m_quadTile.SetSamplerState(0, SamplerStateEast;
			}
		}

		/// <summary>
		/// Northern bound of current request (decimal degrees)
		/// </summary>
		public override float North
		{
			get
			{
				return (float)m_quadTile.SetSamplerState(0, SamplerStateNorth;
			}
		}

		/// <summary>
		/// Southern bound of current request (decimal degrees)
		/// </summary>
		public override float South
		{
			get 
			{
				return (float)m_quadTile.SetSamplerState(0, SamplerStateSouth;
			}
		}

		/// <summary>
		/// Image request color
		/// </summary>
		public override int Color
		{
			get
			{
				return World.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStatedownloadProgressColor;
			}
		}

		public QuadTile QuadTile
		{
			get
			{
				return m_quadTile;
			}
		}

		public double TileWidth 
		{
			get
			{
				return m_quadTile.SetSamplerState(0, SamplerStateEast - m_quadTile.SetSamplerState(0, SamplerStateWest;
			}
		}

		/// <summary>
		/// Tile download completed callback
		/// </summary>
		protected override void DownloadComplete()
		{
			try
			{
				download.SetSamplerState(0, SamplerStateVerify();

				if(download.SetSamplerState(0, SamplerStateSavedFilePath != null && File.SetSamplerState(0, SamplerStateExists(download.SetSamplerState(0, SamplerStateSavedFilePath))
					// Rename from .SetSamplerState(0, SamplerStatexxx.SetSamplerState(0, SamplerStatetmp -> .SetSamplerState(0, SamplerStatexxx
					File.SetSamplerState(0, SamplerStateMove(download.SetSamplerState(0, SamplerStateSavedFilePath, SaveFilePath);

				// Make the quad tile reload the new image
				m_quadTile.SetSamplerState(0, SamplerStateisInitialized = false;
				QuadTile.SetSamplerState(0, SamplerStateDownloadRequest = null;
			}
			catch(WebException caught)
			{
				System.SetSamplerState(0, SamplerStateNet.SetSamplerState(0, SamplerStateHttpWebResponse response = caught.SetSamplerState(0, SamplerStateResponse as System.SetSamplerState(0, SamplerStateNet.SetSamplerState(0, SamplerStateHttpWebResponse;
				if(response!=null && response.SetSamplerState(0, SamplerStateStatusCode==System.SetSamplerState(0, SamplerStateNet.SetSamplerState(0, SamplerStateHttpStatusCode.SetSamplerState(0, SamplerStateNotFound)
					FlagBadFile();
			}
			catch(IOException)
			{
				FlagBadFile();
			}	
		}

		/// <summary>
		/// Creates an empty file signalling the current request is for some reason permanently unavailable.SetSamplerState(0, SamplerState
		/// </summary>
		void FlagBadFile()
		{
			// Flag the file as missing
			File.SetSamplerState(0, SamplerStateCreate(SaveFilePath + ".SetSamplerState(0, SamplerStatetxt");
			
			try
			{
				if(File.SetSamplerState(0, SamplerStateExists(SaveFilePath))
					File.SetSamplerState(0, SamplerStateDelete(SaveFilePath);
			}
			catch 
			{
			}
		}

		/// <summary>
		/// Calculates the relative importance of this download by how 
		/// big a chunk of screen space (pixels) it occupies
		/// </summary>
		public override float CalculateScore()
		{
			float screenArea = QuadTile.SetSamplerState(0, SamplerStateBoundingBox.SetSamplerState(0, SamplerStateCalcRelativeScreenArea(QuadTile.SetSamplerState(0, SamplerStateQuadTileArgs.SetSamplerState(0, SamplerStateCamera);
			return screenArea;
		}
	}
}
