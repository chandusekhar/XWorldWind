using System.Drawing;

namespace WorldWind
{
	/// <summary>
	/// Polygon geometry used to rasterize vectors
	/// </summary>
	public class Polygon
	{
		public LinearRing outerBoundary = null;
		public LinearRing[] innerBoundaries = null;

		public Color PolgonColor = Color.Red;
		public Color OutlineColor = Color.Black;
		public float LineWidth = 1.0f;
		public bool Outline = true;
		public bool Fill = true;
		public bool Visible = true;
		public bool Remove = false;
		public RenderableObject ParentRenderable = null;

		public GeographicBoundingBox GetGeographicBoundingBox()
		{
			if(this.outerBoundary == null || this.outerBoundary.Points == null || this.outerBoundary.Points.Length == 0)
				return null;

			double minX = this.outerBoundary.Points[0].X;
			double maxX = this.outerBoundary.Points[0].X;
			double minY = this.outerBoundary.Points[0].Y;
			double maxY = this.outerBoundary.Points[0].Y;
			double minZ = this.outerBoundary.Points[0].Z;
			double maxZ = this.outerBoundary.Points[0].Z;

			for(int i = 1; i < this.outerBoundary.Points.Length; i++)
			{
				if(this.outerBoundary.Points[i].X < minX)
					minX = this.outerBoundary.Points[i].X;
				if(this.outerBoundary.Points[i].X > maxX)
					maxX = this.outerBoundary.Points[i].X;
				if(this.outerBoundary.Points[i].Y < minY)
					minY = this.outerBoundary.Points[i].Y;
				if(this.outerBoundary.Points[i].Y > maxY)
					maxY = this.outerBoundary.Points[i].Y;
				if(this.outerBoundary.Points[i].Z < minZ)
					minZ = this.outerBoundary.Points[i].Y;
				if(this.outerBoundary.Points[i].Z > maxZ)
					maxZ = this.outerBoundary.Points[i].Y;
			}
			
			return new GeographicBoundingBox(
				maxY, minY, minX, maxX, minZ, maxZ);
		}
	}
}
