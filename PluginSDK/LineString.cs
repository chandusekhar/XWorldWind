using System.Drawing;

namespace WorldWind
{
	/// <summary>
	/// Summary description for LineString.
	/// </summary>
	public class LineString
	{
		public Point3d[] Coordinates = null;
		public Color Color = Color.Yellow;
		public float LineWidth = 1.0f;
		public bool Visible = true;
		public bool Remove = false;
		public RenderableObject ParentRenderable = null;

		public GeographicBoundingBox GetGeographicBoundingBox()
		{
			if(this.Coordinates == null || this.Coordinates.Length == 0)
				return null;

			double minX = this.Coordinates[0].X;
			double maxX = this.Coordinates[0].X;
			double minY = this.Coordinates[0].Y;
			double maxY = this.Coordinates[0].Y;
			double minZ = this.Coordinates[0].Z;
			double maxZ = this.Coordinates[0].Z;

			for(int i = 1; i < this.Coordinates.Length; i++)
			{
				if(this.Coordinates[i].X < minX)
					minX = this.Coordinates[i].X;
				if(this.Coordinates[i].X > maxX)
					maxX = this.Coordinates[i].X;

				if(this.Coordinates[i].Y < minY)
					minY = this.Coordinates[i].Y;
				if(this.Coordinates[i].Y > maxY)
					maxY = this.Coordinates[i].Y;

				if(this.Coordinates[i].Z < minZ)
					minZ = this.Coordinates[i].Z;
				if(this.Coordinates[i].Z > maxZ)
					maxZ = this.Coordinates[i].Z;
			}
			
			return new GeographicBoundingBox(
				maxY, minY, minX, maxX, minZ, maxZ);
		}
	}
}
