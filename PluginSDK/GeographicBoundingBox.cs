namespace WorldWind
{
	/// <summary>
	/// Summary description for GeographicBoundingBox.
	/// </summary>
	public class GeographicBoundingBox
	{
		public double North;
		public double South;
		public double West;
		public double East;
		public double MinimumAltitude;
		public double MaximumAltitude;

		public GeographicBoundingBox()
		{
            this.North = 90;
            this.South = -90;
            this.West = -180;
            this.East = 180;
		}

		public GeographicBoundingBox(double north, double south, double west, double east)
		{
            this.North = north;
            this.South = south;
            this.West = west;
            this.East = east;
		}

		public GeographicBoundingBox(double north, double south, double west, double east, double minAltitude, double maxAltitude)
		{
            this.North = north;
            this.South = south;
            this.West = west;
            this.East = east;
            this.MinimumAltitude = minAltitude;
            this.MaximumAltitude = maxAltitude;
		}

		public bool Intersects(GeographicBoundingBox boundingBox)
		{
			if(this.North <= boundingBox.South || this.South >= boundingBox.North || this.West >= boundingBox.East || this.East <= boundingBox.West)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

        public bool Contains(Point3d p)
        {
            if (this.North < p.Y || this.South > p.Y || this.West > p.X || this.East < p.X || this.MaximumAltitude < p.Z || this.MinimumAltitude > p.Z)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
	}
}
