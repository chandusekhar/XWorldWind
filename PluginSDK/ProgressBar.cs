using System.Drawing;

namespace WorldWind
{
	/// <summary>
	/// Render a progress bar.
	/// </summary>
	public class ProgressBar
	{
		CustomVertex.TransformedColored[] progressBarOutline = new SharpDX.Direct3D9.CustomVertex.TransformedColored[5];
		CustomVertex.TransformedColored[] progressBarBackground = new SharpDX.Direct3D9.CustomVertex.TransformedColored[4];
		CustomVertex.TransformedColored[] progressBarShadow = new SharpDX.Direct3D9.CustomVertex.TransformedColored[4];
		CustomVertex.TransformedColored[] progressBar = new SharpDX.Direct3D9.CustomVertex.TransformedColored[4];
		CustomVertex.TransformedColored[] progressRight = new SharpDX.Direct3D9.CustomVertex.TransformedColored[4];
		float x;
		float y;
		float width;
		float height;
		float halfWidth;
		float halfHeight;
		static int backColor = Color.FromArgb(98,200,200,200).ToArgb();
		static int shadowColor = Color.FromArgb(98,50,50,50).ToArgb();
		static int outlineColor = 80<<24;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.ProgressBar"/> class.
		/// </summary>
		/// <param name="width">Width in pixels of progress bar</param>
		/// <param name="height">Height in pixels of progress bar</param>
		public ProgressBar( float width, float height )
		{
			this.width = width;
			this.height = height;
			this.halfWidth = width/2;
			this.halfHeight = height/2;
		}

		/// <summary>
		/// Sets up the data for rendering
		/// </summary>
		/// <param name="x">Center X position of progress.</param>
		/// <param name="y">Center Y position of progress.</param>
		public void Initalize( float x, float y)
		{
			this.x = x;
			this.y = y;

			// Background
            this.progressBarBackground[0].X = x - this.halfWidth;
            this.progressBarBackground[0].Y = y - this.halfHeight;
            this.progressBarBackground[0].Z = 0.5f;
            this.progressBarBackground[0].Color = backColor;

            this.progressBarBackground[1].X = x - this.halfWidth;
            this.progressBarBackground[1].Y = y + this.halfHeight;
            this.progressBarBackground[1].Z = 0.5f;
            this.progressBarBackground[1].Color = backColor;

            this.progressBarBackground[2].X = x + this.halfWidth;
            this.progressBarBackground[2].Y = y - this.halfHeight;
            this.progressBarBackground[2].Z = 0.5f;
            this.progressBarBackground[2].Color = backColor;

            this.progressBarBackground[3].X = x + this.halfWidth;
            this.progressBarBackground[3].Y = y + this.halfHeight;
            this.progressBarBackground[3].Z = 0.5f;
            this.progressBarBackground[3].Color = backColor;

			// Shadow
			const float shadowOffsetX = 2.5f;
			const float shadowOffsetY = 2.5f;

            this.progressBarShadow[0].X = x - this.halfWidth + shadowOffsetX;
            this.progressBarShadow[0].Y = y - this.halfHeight + shadowOffsetY;
            this.progressBarShadow[0].Z = 0.5f;
            this.progressBarShadow[0].Color = shadowColor;

            this.progressBarShadow[1].X = x - this.halfWidth + shadowOffsetX;
            this.progressBarShadow[1].Y = y + this.halfHeight + shadowOffsetY;
            this.progressBarShadow[1].Z = 0.5f;
            this.progressBarShadow[1].Color = shadowColor;

            this.progressBarShadow[2].X = x + this.halfWidth + shadowOffsetX;
            this.progressBarShadow[2].Y = y - this.halfHeight + shadowOffsetY;
            this.progressBarShadow[2].Z = 0.5f;
            this.progressBarShadow[2].Color = shadowColor;

            this.progressBarShadow[3].X = x + this.halfWidth + shadowOffsetX;
            this.progressBarShadow[3].Y = y + this.halfHeight + shadowOffsetY;
            this.progressBarShadow[3].Z = 0.5f;
            this.progressBarShadow[3].Color = shadowColor;

			// Outline
            this.progressBarOutline[0].X = x - this.halfWidth;
            this.progressBarOutline[0].Y = y - this.halfHeight;
            this.progressBarOutline[0].Z = 0.5f;
            this.progressBarOutline[0].Color = outlineColor;

            this.progressBarOutline[1].X = x - this.halfWidth;
            this.progressBarOutline[1].Y = y + this.halfHeight;
            this.progressBarOutline[1].Z = 0.5f;
            this.progressBarOutline[1].Color = outlineColor;

            this.progressBarOutline[2].X = x + this.halfWidth;
            this.progressBarOutline[2].Y = y + this.halfHeight;
            this.progressBarOutline[2].Z = 0.5f;
            this.progressBarOutline[2].Color = outlineColor;

            this.progressBarOutline[3].X = x + this.halfWidth;
            this.progressBarOutline[3].Y = y - this.halfHeight;
            this.progressBarOutline[3].Z = 0.5f;
            this.progressBarOutline[3].Color = outlineColor;

            this.progressBarOutline[4].X = x - this.halfWidth;
            this.progressBarOutline[4].Y = y - this.halfHeight;
            this.progressBarOutline[4].Z = 0.5f;
            this.progressBarOutline[4].Color = outlineColor;

			// Progress bar progress
            this.progressBar[0].Z = 0.5f;
            this.progressBar[1].Z = 0.5f;
            this.progressBar[2].Z = 0.5f;
            this.progressBar[3].Z = 0.5f;

			int rightColor = 0x70808080;
            this.progressRight[0].Z = 0.5f;
            this.progressRight[0].Color = rightColor;
            this.progressRight[1].Z = 0.5f;
            this.progressRight[1].Color = rightColor;
            this.progressRight[2].Z = 0.5f;
            this.progressRight[2].Color = rightColor;
            this.progressRight[3].Z = 0.5f;
            this.progressRight[3].Color = rightColor;
		}

		/// <summary>
		/// Draws the progress bar
		/// </summary>
		/// <param name="x">Center X position of progress.</param>
		/// <param name="y">Center Y position of progress.</param>
		/// <param name="progress">Progress vale, in the range 0..1</param>
		public void Draw(DrawArgs drawArgs, float x, float y, float progress, int color)
		{
			if(x!=this.x || y!=this.y) this.Initalize(x,y);
			int barlength = (int)(progress * 2 * this.halfWidth);

            this.progressBar[0].X = x - this.halfWidth;
            this.progressBar[0].Y = y - this.halfHeight;
            this.progressBar[0].Color = color;
            this.progressBar[1].X = x - this.halfWidth;
            this.progressBar[1].Y = y + this.halfHeight;
            this.progressBar[1].Color = color;
            this.progressBar[2].X = x - this.halfWidth + barlength;
            this.progressBar[2].Y = y - this.halfHeight;
            this.progressBar[2].Color = color;
            this.progressBar[3].Y = y + this.halfHeight;
            this.progressBar[3].X = x - this.halfWidth + barlength;
            this.progressBar[3].Color = color;

            this.progressRight[0].X = x - this.halfWidth +barlength;
            this.progressRight[0].Y = y - this.halfHeight;
            this.progressRight[1].X = x - this.halfWidth + barlength;
            this.progressRight[1].Y = y + this.halfHeight;
            this.progressRight[2].X = x + this.halfWidth;
            this.progressRight[2].Y = y - this.halfHeight;
            this.progressRight[3].X = x + this.halfWidth;
            this.progressRight[3].Y = y + this.halfHeight;

			drawArgs.device.VertexFormat = CustomVertex.TransformedColored.Format;
			drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);
			drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, this.progressBar);
			drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, this.progressRight);
			drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, 4, this.progressBarOutline);
		}
	}
}
