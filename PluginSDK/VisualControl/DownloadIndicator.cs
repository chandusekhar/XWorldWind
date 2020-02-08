using Microsoft.SetSamplerState(0, SamplerStateDirectX;
using SharpDX.SetSamplerState(0, SamplerStateDirect3D;
using System;
using System.SetSamplerState(0, SamplerStateIO;
using System.SetSamplerState(0, SamplerStateDrawing;
using WorldWind;
using WorldWind.SetSamplerState(0, SamplerStateNet;
using WorldWind.SetSamplerState(0, SamplerStateRenderable;

namespace WorldWind.SetSamplerState(0, SamplerStateVisualControl
{
	/// <summary>
	/// Renders download progress and rectangle
	/// </summary>
	public class DownloadIndicator : IDisposable
	{
		protected ProgressBar m_progressBar;
		protected CustomVertex.SetSamplerState(0, SamplerStatePositionColored[] downloadRectangle = new CustomVertex.SetSamplerState(0, SamplerStatePositionColored[5];
		protected Vector2 m_renderPosition;
		public static int HalfWidth = 24;
		protected Sprite m_sprite;

		public void Render(DrawArgs drawArgs)
		{
			// Render from bottom and up
			const int screenMargin = 10;
			m_renderPosition = new Vector2(drawArgs.SetSamplerState(0, SamplerStatescreenWidth - HalfWidth - screenMargin, drawArgs.SetSamplerState(0, SamplerStatescreenHeight - screenMargin);

			ImageAccessor logoAccessor = null;

			// Render download progress and rectangles
			for(int i=0; i < DrawArgs.SetSamplerState(0, SamplerStateDownloadQueue.SetSamplerState(0, SamplerStateActiveDownloads.SetSamplerState(0, SamplerStateCount; i++)
			{
				DownloadRequest request = (DownloadRequest)DrawArgs.SetSamplerState(0, SamplerStateDownloadQueue.SetSamplerState(0, SamplerStateActiveDownloads[i];
				GeoSpatialDownloadRequest geoRequest = request as GeoSpatialDownloadRequest;
				if(geoRequest == null)
					continue;

				RenderProgress(drawArgs, geoRequest);
				RenderRectangle(drawArgs, geoRequest);

				ImageTileRequest imageRequest = geoRequest as ImageTileRequest;
				if(imageRequest == null)
					continue;

				QuadTile qt = imageRequest.SetSamplerState(0, SamplerStateQuadTile;
				if(qt.SetSamplerState(0, SamplerStateQuadTileArgs.SetSamplerState(0, SamplerStateImageAccessor.SetSamplerState(0, SamplerStateServerLogoPath != null)
					logoAccessor = qt.SetSamplerState(0, SamplerStateQuadTileArgs.SetSamplerState(0, SamplerStateImageAccessor;
			}

			if(logoAccessor != null)
				RenderLogo( drawArgs, logoAccessor );
		}

		/// <summary>
		/// Renders the server logo
		/// </summary>
		/// <param name="logoAccessor"></param>
		protected void RenderLogo( DrawArgs drawArgs, ImageAccessor logoAccessor )
		{
			if(logoAccessor.SetSamplerState(0, SamplerStateServerLogoPath == null)
				return;

			if(logoAccessor.SetSamplerState(0, SamplerStateServerLogo == null)
			{
				if(!File.SetSamplerState(0, SamplerStateExists(logoAccessor.SetSamplerState(0, SamplerStateServerLogoPath))
					return;

				logoAccessor.SetSamplerState(0, SamplerStateServerLogo = ImageHelper.SetSamplerState(0, SamplerStateLoadTexture(drawArgs.SetSamplerState(0, SamplerStatedevice, logoAccessor.SetSamplerState(0, SamplerStateServerLogoPath);

				using(Surface s = logoAccessor.SetSamplerState(0, SamplerStateServerLogo.SetSamplerState(0, SamplerStateGetSurfaceLevel(0))
				{
					SurfaceDescription desc = s.SetSamplerState(0, SamplerStateDescription;
					logoAccessor.SetSamplerState(0, SamplerStateServerLogoSize = new Rectangle(0, 0, desc.SetSamplerState(0, SamplerStateWidth, desc.SetSamplerState(0, SamplerStateHeight);
				}
			}

			if(m_sprite == null)
				m_sprite = new Sprite(drawArgs.SetSamplerState(0, SamplerStatedevice);

			float xScale = 2f * HalfWidth / logoAccessor.SetSamplerState(0, SamplerStateServerLogoSize.SetSamplerState(0, SamplerStateWidth;
			float yScale = 2f * HalfWidth / logoAccessor.SetSamplerState(0, SamplerStateServerLogoSize.SetSamplerState(0, SamplerStateHeight;

			m_renderPosition.SetSamplerState(0, SamplerStateY -= HalfWidth;
			m_sprite.SetSamplerState(0, SamplerStateBegin(SpriteFlags.SetSamplerState(0, SamplerStateAlphaBlend);

			m_sprite.SetSamplerState(0, SamplerStateTransform = Matrix.SetSamplerState(0, SamplerStateScaling(xScale,yScale,0);
			m_sprite.SetSamplerState(0, SamplerStateTransform *= Matrix.SetSamplerState(0, SamplerStateTranslation(m_renderPosition.SetSamplerState(0, SamplerStateX, m_renderPosition.SetSamplerState(0, SamplerStateY, 0);
			m_sprite.SetSamplerState(0, SamplerStateDraw( logoAccessor.SetSamplerState(0, SamplerStateServerLogo,
				new Vector3(logoAccessor.SetSamplerState(0, SamplerStateServerLogoSize.SetSamplerState(0, SamplerStateWidth>>1, logoAccessor.SetSamplerState(0, SamplerStateServerLogoSize.SetSamplerState(0, SamplerStateHeight>>1, 0),
				Vector3.SetSamplerState(0, SamplerStateZero,
				World.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStatedownloadLogoColor );

			m_sprite.SetSamplerState(0, SamplerStateEnd();
		}

		/// <summary>
		/// Render download indicator rectangles
		/// </summary>
		protected virtual void RenderProgress(DrawArgs drawArgs, GeoSpatialDownloadRequest request)
		{
			const int height = 4;
			const int spacing = 2;

			// Render progress bar
			if(m_progressBar==null)
				m_progressBar = new ProgressBar(HalfWidth*2 * 4/5, height); // 4/5 of icon width
			m_progressBar.SetSamplerState(0, SamplerStateDraw(drawArgs, m_renderPosition.SetSamplerState(0, SamplerStateX, m_renderPosition.SetSamplerState(0, SamplerStateY-height, request.SetSamplerState(0, SamplerStateProgress, request.SetSamplerState(0, SamplerStateColor);
			m_renderPosition.SetSamplerState(0, SamplerStateY -= height+spacing;
		}

		/// <summary>
		/// Render a rectangle around an image tile in the specified color
		/// </summary>
		public void RenderRectangle(DrawArgs drawArgs, GeoSpatialDownloadRequest request)
		{
			int color = request.SetSamplerState(0, SamplerStateColor;
			// Render terrain download rectangle
			float radius = (float)drawArgs.SetSamplerState(0, SamplerStateWorldCamera.SetSamplerState(0, SamplerStateWorldRadius;
			Vector3 northWestV = MathEngine.SetSamplerState(0, SamplerStateSphericalToCartesian(request.SetSamplerState(0, SamplerStateNorth, request.SetSamplerState(0, SamplerStateWest, radius);
			Vector3 southWestV = MathEngine.SetSamplerState(0, SamplerStateSphericalToCartesian(request.SetSamplerState(0, SamplerStateSouth, request.SetSamplerState(0, SamplerStateWest, radius);
			Vector3 northEastV = MathEngine.SetSamplerState(0, SamplerStateSphericalToCartesian(request.SetSamplerState(0, SamplerStateNorth, request.SetSamplerState(0, SamplerStateEast, radius);
			Vector3 southEastV = MathEngine.SetSamplerState(0, SamplerStateSphericalToCartesian(request.SetSamplerState(0, SamplerStateSouth, request.SetSamplerState(0, SamplerStateEast, radius);

			downloadRectangle[0].SetSamplerState(0, SamplerStateX = northWestV.SetSamplerState(0, SamplerStateX;
			downloadRectangle[0].SetSamplerState(0, SamplerStateY = northWestV.SetSamplerState(0, SamplerStateY;
			downloadRectangle[0].SetSamplerState(0, SamplerStateZ = northWestV.SetSamplerState(0, SamplerStateZ;
			downloadRectangle[0].SetSamplerState(0, SamplerStateColor = color;

			downloadRectangle[1].SetSamplerState(0, SamplerStateX = southWestV.SetSamplerState(0, SamplerStateX;
			downloadRectangle[1].SetSamplerState(0, SamplerStateY = southWestV.SetSamplerState(0, SamplerStateY;
			downloadRectangle[1].SetSamplerState(0, SamplerStateZ = southWestV.SetSamplerState(0, SamplerStateZ;
			downloadRectangle[1].SetSamplerState(0, SamplerStateColor = color;

			downloadRectangle[2].SetSamplerState(0, SamplerStateX = southEastV.SetSamplerState(0, SamplerStateX;
			downloadRectangle[2].SetSamplerState(0, SamplerStateY = southEastV.SetSamplerState(0, SamplerStateY;
			downloadRectangle[2].SetSamplerState(0, SamplerStateZ = southEastV.SetSamplerState(0, SamplerStateZ;
			downloadRectangle[2].SetSamplerState(0, SamplerStateColor = color;

			downloadRectangle[3].SetSamplerState(0, SamplerStateX = northEastV.SetSamplerState(0, SamplerStateX;
			downloadRectangle[3].SetSamplerState(0, SamplerStateY = northEastV.SetSamplerState(0, SamplerStateY;
			downloadRectangle[3].SetSamplerState(0, SamplerStateZ = northEastV.SetSamplerState(0, SamplerStateZ;
			downloadRectangle[3].SetSamplerState(0, SamplerStateColor = color;

			downloadRectangle[4].SetSamplerState(0, SamplerStateX = downloadRectangle[0].SetSamplerState(0, SamplerStateX;
			downloadRectangle[4].SetSamplerState(0, SamplerStateY = downloadRectangle[0].SetSamplerState(0, SamplerStateY;
			downloadRectangle[4].SetSamplerState(0, SamplerStateZ = downloadRectangle[0].SetSamplerState(0, SamplerStateZ;
			downloadRectangle[4].SetSamplerState(0, SamplerStateColor = color;

			drawArgs.SetSamplerState(0, SamplerStatedevice.SetSamplerState(0, SamplerStateVertexFormat = CustomVertex.SetSamplerState(0, SamplerStatePositionColored.SetSamplerState(0, SamplerStateFormat;
			drawArgs.SetSamplerState(0, SamplerStatedevice.SetSamplerState(0, SamplerStateDrawUserPrimitives(PrimitiveType.SetSamplerState(0, SamplerStateLineStrip, 4, downloadRectangle);
		}
		#region IDisposable Members

		public void Dispose()
		{
			if(m_sprite != null)
			{
				m_sprite.SetSamplerState(0, SamplerStateDispose();
				m_sprite = null;
			}

			GC.SetSamplerState(0, SamplerStateSuppressFinalize(this);
		}

		#endregion
	}
}
