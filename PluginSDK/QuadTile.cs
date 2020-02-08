using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SharpDX;
using SharpDX.Direct3D9;
using Utility;
using WorldWind.Terrain;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace WorldWind
{
    public class QuadTile : IDisposable
    {
        /// <summary>
        /// Child tile location
        /// </summary>
        public enum ChildLocation
        {
            NorthWest,
            SouthWest,
            NorthEast,
            SouthEast
        }

        public QuadTileSet QuadTileSet;
        public double West;
        public double East;
        public double North;
        public double South;
        public Angle CenterLatitude;
        public Angle CenterLongitude;
        public double LatitudeSpan;
        public double LongitudeSpan;
        public int Level;
        public int Row;
        public int Col;

        public bool isInitialized;
        public BoundingBox BoundingBox;
        public List<global::GeoSpatialDownloadRequest> DownloadRequests;
        //public GeoSpatialDownloadRequest DownloadRequest;

        protected Texture[] textures;

        /// <summary>
        /// Number of points in child flat mesh grid (times 2)
        /// </summary>
        protected static int vertexCount = 40;

        /// <summary>
        /// Number of points in child terrain mesh grid (times 2)
        /// </summary>
        protected static int vertexCountElevated = 40;

        protected QuadTile northWestChild;
        protected QuadTile southWestChild;
        protected QuadTile northEastChild;
        protected QuadTile southEastChild;

        protected CustomVertex.PositionNormalTextured[] northWestVertices;
        protected CustomVertex.PositionNormalTextured[] southWestVertices;
        protected CustomVertex.PositionNormalTextured[] northEastVertices;
        protected CustomVertex.PositionNormalTextured[] southEastVertices;
        protected short[] vertexIndexes;
        protected Point3d localOrigin; // Add this offset to get world coordinates

        protected bool m_isResetingCache;

        /// <summary>
        /// The vertical exaggeration the tile mesh was computed for
        /// </summary>
        protected float verticalExaggeration;

        protected bool isDownloadingTerrain;

        private string key;

        /// New Cache idea
        /// 
        internal static Dictionary<string, CacheEntry> VerticeCache
            = new Dictionary<string, CacheEntry>();

        internal static int CacheSize = 256;

        internal class CacheEntry
        {
            public CustomVertex.PositionNormalTextured[] northWestVertices;
            public CustomVertex.PositionNormalTextured[] southWestVertices;
            public CustomVertex.PositionNormalTextured[] northEastVertices;
            public CustomVertex.PositionNormalTextured[] southEastVertices;
            public short[] vertexIndexes;

            public DateTime EntryTime;
        }

        // End New Cache

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.QuadTile"/> class.
        /// </summary>
        /// <param name="south"></param>
        /// <param name="north"></param>
        /// <param name="west"></param>
        /// <param name="east"></param>
        /// <param name="level"></param>
        /// <param name="quadTileSet"></param>
        public QuadTile(double south, double north, double west, double east, int level, QuadTileSet quadTileSet)
        {
            this.South = south;
            this.North = north;
            this.West = west;
            this.East = east;
            this.CenterLatitude = Angle.FromDegrees(0.5f*(this.North + this.South));
            this.CenterLongitude = Angle.FromDegrees(0.5f*(this.West + this.East));
            this.LatitudeSpan = Math.Abs(this.North - this.South);
            this.LongitudeSpan = Math.Abs(this.East - this.West);

            this.Level = level;
            this.QuadTileSet = quadTileSet;

            this.BoundingBox = new BoundingBox((float) south, (float) north, (float) west, (float) east,
                                          (float) quadTileSet.LayerRadius, (float) quadTileSet.LayerRadius + 300000f);
            //localOrigin = BoundingBox.CalculateCenter();
            this.localOrigin = MathEngine.SphericalToCartesianD(this.CenterLatitude, this.CenterLongitude, quadTileSet.LayerRadius);

            // To avoid gaps between neighbouring tiles truncate the origin to 
            // a number that doesn't get rounded. (nearest 10km)
            this.localOrigin.X = (float) (Math.Round(this.localOrigin.X/10000)*10000);
            this.localOrigin.Y = (float) (Math.Round(this.localOrigin.Y/10000)*10000);
            this.localOrigin.Z = (float) (Math.Round(this.localOrigin.Z/10000)*10000);

            this.Row = MathEngine.GetRowFromLatitude(this.South, this.North - this.South);
            this.Col = MathEngine.GetColFromLongitude(this.West, this.North - this.South);

            this.DownloadRequests = new List<global::GeoSpatialDownloadRequest>();

            this.key = string.Format("{0,4}", this.Level)
                       + "_"
                       + string.Format("{0,4}", this.Col)
                       + string.Format("{0,4}", this.Row)
                       + this.QuadTileSet.Name
                       + this.QuadTileSet.ParentList.Name;
        }

        public override string ToString()
        {
            return String.Format("QuadTile:Set={0} Level={1} X={2} Y={3}", this.QuadTileSet.Name, this.Level, this.Col, this.Row);
        }

        public virtual void ResetCache()
        {
            try
            {
                this.m_isResetingCache = true;
                this.isInitialized = false;

                if (this.northEastChild != null)
                {
                    this.northEastChild.ResetCache();
                }

                if (this.northWestChild != null)
                {
                    this.northWestChild.ResetCache();
                }

                if (this.southEastChild != null)
                {
                    this.southEastChild.ResetCache();
                }

                if (this.southWestChild != null)
                {
                    this.southWestChild.ResetCache();
                }

                this.Dispose();

                for (int i = 0; i < this.QuadTileSet.ImageStores.Length; i++)
                {
                    if ((this.QuadTileSet.ImageStores[i] != null) && this.QuadTileSet.ImageStores[i].IsDownloadableLayer) this.QuadTileSet.ImageStores[i].DeleteLocalCopy(this);
                }

                this.m_isResetingCache = false;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Returns the QuadTile for specified location if available.
        /// Tries to queue a download if not available.
        /// </summary>
        /// <returns>Initialized QuadTile if available locally, else null.</returns>
        private QuadTile ComputeChild(double childSouth, double childNorth, double childWest, double childEast)
        {
            QuadTile child = new QuadTile(
                childSouth,
                childNorth,
                childWest,
                childEast,
                this.Level + 1, this.QuadTileSet);

            return child;
        }

        public virtual void ComputeChildren(DrawArgs drawArgs)
        {
            if (this.Level + 1 >= this.QuadTileSet.ImageStores[0].LevelCount)
                return;

            double CenterLat = 0.5f*(this.South + this.North);
            double CenterLon = 0.5f*(this.East + this.West);
            if (this.northWestChild == null) this.northWestChild = this.ComputeChild(CenterLat, this.North, this.West, CenterLon);

            if (this.northEastChild == null) this.northEastChild = this.ComputeChild(CenterLat, this.North, CenterLon, this.East);

            if (this.southWestChild == null) this.southWestChild = this.ComputeChild(this.South, CenterLat, this.West, CenterLon);

            if (this.southEastChild == null) this.southEastChild = this.ComputeChild(this.South, CenterLat, CenterLon, this.East);
        }

        public virtual void Dispose()
        {
            try
            {
                this.isInitialized = false;
                if (this.textures != null)
                {
                    for (int i = 0; i < this.textures.Length; i++)
                    {
                        if (this.textures[i] != null && !this.textures[i].Disposed)
                        {
                            this.textures[i].Dispose();
                            this.textures[i] = null;
                        }
                    }

                    this.textures = null;
                }
                if (this.northWestChild != null)
                {
                    this.northWestChild.Dispose();
                    this.northWestChild = null;
                }
                if (this.southWestChild != null)
                {
                    this.southWestChild.Dispose();
                    this.southWestChild = null;
                }
                if (this.northEastChild != null)
                {
                    this.northEastChild.Dispose();
                    this.northEastChild = null;
                }
                if (this.southEastChild != null)
                {
                    this.southEastChild.Dispose();
                    this.southEastChild = null;
                }
                if (this.DownloadRequests != null)
                {
                    foreach (global::GeoSpatialDownloadRequest request in this.DownloadRequests)
                    {
                        this.QuadTileSet.RemoveFromDownloadQueue(request);
                        request.Dispose();
                    }

                    this.DownloadRequests.Clear();
                }
            }
            catch
            {
            }
        }

        public bool WaitingForDownload;
        public bool IsDownloadingImage;

        public virtual void Initialize()
        {
            if (this.m_isResetingCache)
                return;

            try
            {
                if (this.DownloadRequests.Count > 0)
                {
                    // Waiting for download
                    return;
                }
                if (this.textures == null)
                {
                    this.textures = new Texture[this.QuadTileSet.ImageStores.Length];

                    // not strictly necessary
                    for (int i = 0; i < this.textures.Length; i++) this.textures[i] = null;
                }

                // assume we're finished.
                this.WaitingForDownload = false;

                // check for missing textures.
                for (int i = 0; i < this.textures.Length; i++)
                {
                    Texture newTexture = this.QuadTileSet.ImageStores[i].LoadFile(this);
                    if (newTexture == null)
                    {
                        // At least one texture missing, wait for download
                        this.WaitingForDownload = true;
                    }

                    // not entirely sure if this is a good idea...
                    if (this.textures[i] != null) this.textures[i].Dispose();

                    this.textures[i] = newTexture;
                }
                if (this.WaitingForDownload)
                    return;

                this.IsDownloadingImage = false;
                this.CreateTileMesh();
                this.isInitialized = true;
            }
                //catch (SharpDX.Direct3D9.Direct3DXException)
            catch (Exception)
            {
                //Log.Write(ex);
                // Texture load failed.
            }
        }

        /// <summary>
        /// Updates this layer (background)
        /// </summary>
        public virtual void Update(DrawArgs drawArgs)
        {
            if (this.m_isResetingCache)
                return;

            try
            {
                double tileSize = this.North - this.South;

                if (!this.isInitialized)
                {
                    if (DrawArgs.Camera.ViewRange*0.5f < Angle.FromDegrees(this.QuadTileSet.TileDrawDistance*tileSize)
                        && MathEngine.SphericalDistance(this.CenterLatitude, this.CenterLongitude,
                                                        DrawArgs.Camera.Latitude, DrawArgs.Camera.Longitude) <
                           Angle.FromDegrees(this.QuadTileSet.TileDrawSpread*tileSize*1.25f)
                        && DrawArgs.Camera.ViewFrustum.Intersects(this.BoundingBox)
                        )
                        this.Initialize();
                }

                if (this.isInitialized && World.Settings.VerticalExaggeration != this.verticalExaggeration || this.m_CurrentOpacity != this.QuadTileSet.Opacity || this.QuadTileSet.RenderStruts != this.renderStruts)
                {
                    this.CreateTileMesh();
                }

                if (this.isInitialized)
                {
                    if (DrawArgs.Camera.ViewRange < Angle.FromDegrees(this.QuadTileSet.TileDrawDistance*tileSize)
                        && MathEngine.SphericalDistance(this.CenterLatitude, this.CenterLongitude,
                                                        DrawArgs.Camera.Latitude, DrawArgs.Camera.Longitude) <
                           Angle.FromDegrees(this.QuadTileSet.TileDrawSpread*tileSize)
                        && DrawArgs.Camera.ViewFrustum.Intersects(this.BoundingBox)
                        )
                    {
                        if (this.northEastChild == null || this.northWestChild == null || this.southEastChild == null || this.southWestChild == null)
                        {
                            this.ComputeChildren(drawArgs);
                        }

                        if (this.northEastChild != null)
                        {
                            this.northEastChild.Update(drawArgs);
                        }

                        if (this.northWestChild != null)
                        {
                            this.northWestChild.Update(drawArgs);
                        }

                        if (this.southEastChild != null)
                        {
                            this.southEastChild.Update(drawArgs);
                        }

                        if (this.southWestChild != null)
                        {
                            this.southWestChild.Update(drawArgs);
                        }
                    }
                    else
                    {
                        if (this.northWestChild != null)
                        {
                            this.northWestChild.Dispose();
                            this.northWestChild = null;
                        }

                        if (this.northEastChild != null)
                        {
                            this.northEastChild.Dispose();
                            this.northEastChild = null;
                        }

                        if (this.southEastChild != null)
                        {
                            this.southEastChild.Dispose();
                            this.southEastChild = null;
                        }

                        if (this.southWestChild != null)
                        {
                            this.southWestChild.Dispose();
                            this.southWestChild = null;
                        }
                    }
                }

                if (this.isInitialized)
                {
                    if (DrawArgs.Camera.ViewRange/2 > Angle.FromDegrees(this.QuadTileSet.TileDrawDistance*tileSize*1.5f)
                        ||
                        MathEngine.SphericalDistance(this.CenterLatitude, this.CenterLongitude, DrawArgs.Camera.Latitude,
                                                     DrawArgs.Camera.Longitude) >
                        Angle.FromDegrees(this.QuadTileSet.TileDrawSpread*tileSize*1.5f))
                    {
                        if (this.Level != 0 || (this.Level == 0 && !this.QuadTileSet.AlwaysRenderBaseTiles))
                            this.Dispose();
                    }
                }
            }
            catch
            {
            }
        }

        private bool renderStruts = true;

        /// <summary>
        /// Builds flat or terrain mesh for current tile
        /// </summary>
        //public virtual void CreateTileMesh()
        //{
        //    verticalExaggeration = World.Settings.VerticalExaggeration;
        //    m_CurrentOpacity = QuadTileSet.Opacity;
        //    renderStruts = QuadTileSet.RenderStruts;
        //    if (QuadTileSet.TerrainMapped && Math.Abs(verticalExaggeration) > 1e-3)
        //        CreateElevatedMesh();
        //    else
        //        CreateFlatMesh();
        //}
        public virtual void CreateTileMesh()
        {
            if (VerticeCache.ContainsKey(this.key) && World.Settings.VerticalExaggeration == this.verticalExaggeration)
            {
                this.northWestVertices = VerticeCache[this.key].northWestVertices;
                this.southWestVertices = VerticeCache[this.key].southWestVertices;
                this.northEastVertices = VerticeCache[this.key].northEastVertices;
                this.southEastVertices = VerticeCache[this.key].southEastVertices;
                this.vertexIndexes = VerticeCache[this.key].vertexIndexes;

                VerticeCache[this.key].EntryTime = DateTime.Now;

                return;
            }

            this.verticalExaggeration = World.Settings.VerticalExaggeration;
            this.m_CurrentOpacity = this.QuadTileSet.Opacity;
            this.renderStruts = this.QuadTileSet.RenderStruts;

            if (this.QuadTileSet.TerrainMapped && Math.Abs(this.verticalExaggeration) > 1e-3)
                this.CreateElevatedMesh();
            else
                this.CreateFlatMesh();

            this.AddToCache();
        }

        private void AddToCache()
        {
            if (!VerticeCache.ContainsKey(this.key))
            {
                if (VerticeCache.Count >= CacheSize)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        // Remove least recently used tile
                        CacheEntry oldestTile = null;
                        string k = "";
                        foreach (KeyValuePair<string, CacheEntry> curEntry in VerticeCache)
                        {
                            if (oldestTile == null)
                                oldestTile = curEntry.Value;
                            else
                            {
                                if (curEntry.Value.EntryTime < oldestTile.EntryTime)
                                {
                                    oldestTile = curEntry.Value;
                                    k = curEntry.Key;
                                }
                            }
                        }
                        VerticeCache.Remove(k);
                    }
                }

                CacheEntry c = new CacheEntry();
                c.EntryTime = DateTime.Now;
                c.northEastVertices = this.northEastVertices;
                c.northWestVertices = this.northWestVertices;
                c.southEastVertices = this.southEastVertices;
                c.southWestVertices = this.southWestVertices;
                c.vertexIndexes = this.vertexIndexes;

                VerticeCache.Add(this.key, c);
            }
        }


        // Edits by Patrick Murris : fixing mesh sides normals (2006-11-18)

        /// <summary>
        /// Builds a flat mesh (no terrain)
        /// </summary>
        protected virtual void CreateFlatMesh()
        {
            double layerRadius = (double) this.QuadTileSet.LayerRadius;
            double scaleFactor = 1.0/(double) vertexCount;
            int thisVertexCount = vertexCount/2 + (vertexCount%2);
            int thisVertexCountPlus1 = thisVertexCount + 1;

            int totalVertexCount = thisVertexCountPlus1*thisVertexCountPlus1;
            this.northWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            this.southWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            this.northEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            this.southEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];

            const double Degrees2Radians = Math.PI/180.0;

            // Cache western sin/cos of longitude values
            double[] sinLon = new double[thisVertexCountPlus1];
            double[] cosLon = new double[thisVertexCountPlus1];

            int baseIndex;
            double angle = this.West*Degrees2Radians;
            double angleConst;
            double deltaAngle = scaleFactor* this.LongitudeSpan*Degrees2Radians;

            angleConst = this.West*Degrees2Radians;
            for (int i = 0; i < thisVertexCountPlus1; i++)
            {
                angle = angleConst + i*deltaAngle;
                sinLon[i] = Math.Sin(angle);
                cosLon[i] = Math.Cos(angle);
                //angle += deltaAngle;
            }

            baseIndex = 0;
            angleConst = this.North*Degrees2Radians;
            deltaAngle = -scaleFactor* this.LatitudeSpan*Degrees2Radians;
            for (int i = 0; i < thisVertexCountPlus1; i++)
            {
                angle = angleConst + i*deltaAngle;
                double sinLat = Math.Sin(angle);
                double radCosLat = Math.Cos(angle)*layerRadius;

                for (int j = 0; j < thisVertexCountPlus1; j++)
                {
                    this.northWestVertices[baseIndex].X = (float) (radCosLat*cosLon[j] - this.localOrigin.X);
                    this.northWestVertices[baseIndex].Y = (float) (radCosLat*sinLon[j] - this.localOrigin.Y);
                    this.northWestVertices[baseIndex].Z = (float) (layerRadius*sinLat - this.localOrigin.Z);
                    this.northWestVertices[baseIndex].Tu = (float) (j*scaleFactor);
                    this.northWestVertices[baseIndex].Tv = (float) (i*scaleFactor);
                    this.northWestVertices[baseIndex].Normal =
                        new Vector3(this.northWestVertices[baseIndex].X + (float) this.localOrigin.X, this.northWestVertices[baseIndex].Y + (float) this.localOrigin.Y, this.northWestVertices[baseIndex].Z + (float) this.localOrigin.Z);
                    this.northWestVertices[baseIndex].Normal.Normalize();

                    baseIndex += 1;
                }
                //angle += deltaAngle;
            }

            baseIndex = 0;
            angleConst = 0.5*(this.North + this.South)*Degrees2Radians;
            for (int i = 0; i < thisVertexCountPlus1; i++)
            {
                angle = angleConst + i*deltaAngle;
                double sinLat = Math.Sin(angle);
                double radCosLat = Math.Cos(angle)*layerRadius;

                for (int j = 0; j < thisVertexCountPlus1; j++)
                {
                    this.southWestVertices[baseIndex].X = (float) (radCosLat*cosLon[j] - this.localOrigin.X);
                    this.southWestVertices[baseIndex].Y = (float) (radCosLat*sinLon[j] - this.localOrigin.Y);
                    this.southWestVertices[baseIndex].Z = (float) (layerRadius*sinLat - this.localOrigin.Z);
                    this.southWestVertices[baseIndex].Tu = (float) (j*scaleFactor);
                    this.southWestVertices[baseIndex].Tv = (float) ((i + thisVertexCount)*scaleFactor);
                    this.southWestVertices[baseIndex].Normal =
                        new Vector3(this.southWestVertices[baseIndex].X + (float) this.localOrigin.X, this.southWestVertices[baseIndex].Y + (float) this.localOrigin.Y, this.southWestVertices[baseIndex].Z + (float) this.localOrigin.Z);
                    this.southWestVertices[baseIndex].Normal.Normalize();

                    baseIndex += 1;
                }
                //angle += deltaAngle;
            }

            // Cache eastern sin/cos of longitude values
            angleConst = 0.5*(this.West + this.East)*Degrees2Radians;
            deltaAngle = scaleFactor* this.LongitudeSpan*Degrees2Radians;
            for (int i = 0; i < thisVertexCountPlus1; i++)
            {
                angle = angleConst + i*deltaAngle;
                sinLon[i] = Math.Sin(angle);
                cosLon[i] = Math.Cos(angle);
                //angle += deltaAngle;
            }

            baseIndex = 0;
            angleConst = this.North*Degrees2Radians;
            deltaAngle = -scaleFactor* this.LatitudeSpan*Degrees2Radians;
            for (int i = 0; i < thisVertexCountPlus1; i++)
            {
                angle = angleConst + i*deltaAngle;
                double sinLat = Math.Sin(angle);
                double radCosLat = Math.Cos(angle)*layerRadius;

                for (int j = 0; j < thisVertexCountPlus1; j++)
                {
                    this.northEastVertices[baseIndex].X = (float) (radCosLat*cosLon[j] - this.localOrigin.X);
                    this.northEastVertices[baseIndex].Y = (float) (radCosLat*sinLon[j] - this.localOrigin.Y);
                    this.northEastVertices[baseIndex].Z = (float) (layerRadius*sinLat - this.localOrigin.Z);
                    this.northEastVertices[baseIndex].Tu = (float) ((j + thisVertexCount)*scaleFactor);
                    this.northEastVertices[baseIndex].Tv = (float) (i*scaleFactor);
                    this.northEastVertices[baseIndex].Normal =
                        new Vector3(this.northEastVertices[baseIndex].X + (float) this.localOrigin.X, this.northEastVertices[baseIndex].Y + (float) this.localOrigin.Y, this.northEastVertices[baseIndex].Z + (float) this.localOrigin.Z);
                    this.northEastVertices[baseIndex].Normal.Normalize();

                    baseIndex += 1;
                }
                //angle += deltaAngle;
            }

            baseIndex = 0;
            angleConst = 0.5f*(this.North + this.South)*Degrees2Radians;
            for (int i = 0; i < thisVertexCountPlus1; i++)
            {
                angle = angleConst + i*deltaAngle;
                double sinLat = Math.Sin(angle);
                double radCosLat = Math.Cos(angle)*layerRadius;

                for (int j = 0; j < thisVertexCountPlus1; j++)
                {
                    this.southEastVertices[baseIndex].X = (float) (radCosLat*cosLon[j] - this.localOrigin.X);
                    this.southEastVertices[baseIndex].Y = (float) (radCosLat*sinLon[j] - this.localOrigin.Y);
                    this.southEastVertices[baseIndex].Z = (float) (layerRadius*sinLat - this.localOrigin.Z);
                    this.southEastVertices[baseIndex].Tu = (float) ((j + thisVertexCount)*scaleFactor);
                    this.southEastVertices[baseIndex].Tv = (float) ((i + thisVertexCount)*scaleFactor);
                    this.southEastVertices[baseIndex].Normal =
                        new Vector3(this.southEastVertices[baseIndex].X + (float) this.localOrigin.X, this.southEastVertices[baseIndex].Y + (float) this.localOrigin.Y, this.southEastVertices[baseIndex].Z + (float) this.localOrigin.Z);
                    this.southEastVertices[baseIndex].Normal.Normalize();

                    baseIndex += 1;
                }
                //angle += deltaAngle;
            }

            this.vertexIndexes = new short[2*thisVertexCount*thisVertexCount*3];

            for (int i = 0; i < thisVertexCount; i++)
            {
                baseIndex = (2*3*i*thisVertexCount);

                for (int j = 0; j < thisVertexCount; j++)
                {
                    this.vertexIndexes[baseIndex] = (short) (i*thisVertexCountPlus1 + j);
                    this.vertexIndexes[baseIndex + 1] = (short) ((i + 1)*thisVertexCountPlus1 + j);
                    this.vertexIndexes[baseIndex + 2] = (short) (i*thisVertexCountPlus1 + j + 1);

                    this.vertexIndexes[baseIndex + 3] = (short) (i*thisVertexCountPlus1 + j + 1);
                    this.vertexIndexes[baseIndex + 4] = (short) ((i + 1)*thisVertexCountPlus1 + j);
                    this.vertexIndexes[baseIndex + 5] = (short) ((i + 1)*thisVertexCountPlus1 + j + 1);

                    baseIndex += 6;
                }
            }
        }

        private double meshBaseRadius;

        /// <summary>
        /// Build the elevated terrain mesh
        /// </summary>
        protected virtual void CreateElevatedMesh()
        {
            this.isDownloadingTerrain = true;
            // Get height data with one extra sample around the tile
            double degreePerSample = this.LatitudeSpan/vertexCountElevated;
            TerrainTile tile = this.QuadTileSet.World.TerrainAccessor.GetElevationArray(this.North + degreePerSample, this.South - degreePerSample, this.West - degreePerSample, this.East + degreePerSample,
                                                                    vertexCountElevated + 3);
            float[,] heightData = tile.ElevationData;

            int vertexCountElevatedPlus3 = vertexCountElevated/2 + 3;
            int totalVertexCount = vertexCountElevatedPlus3*vertexCountElevatedPlus3;
            this.northWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            this.southWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            this.northEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            this.southEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            double layerRadius = (double) this.QuadTileSet.LayerRadius;

            // Calculate mesh base radius (bottom vertices)
            // Find minimum elevation to account for possible bathymetry
            float minimumElevation = float.MaxValue;
            float maximumElevation = float.MinValue;
            foreach (float height in heightData)
            {
                if (height < minimumElevation)
                    minimumElevation = height;
                if (height > maximumElevation)
                    maximumElevation = height;
            }
            minimumElevation *= this.verticalExaggeration;
            maximumElevation *= this.verticalExaggeration;

            if (minimumElevation > maximumElevation)
            {
                // Compensate for negative vertical exaggeration
                minimumElevation = maximumElevation;
                maximumElevation = minimumElevation;
            }

            double overlap = 500* this.verticalExaggeration; // 500m high tiles

            // Radius of mesh bottom grid
            this.meshBaseRadius = layerRadius + minimumElevation - overlap;

            this.CreateElevatedMesh(ChildLocation.NorthWest, this.northWestVertices, this.meshBaseRadius, heightData);
            this.CreateElevatedMesh(ChildLocation.SouthWest, this.southWestVertices, this.meshBaseRadius, heightData);
            this.CreateElevatedMesh(ChildLocation.NorthEast, this.northEastVertices, this.meshBaseRadius, heightData);
            this.CreateElevatedMesh(ChildLocation.SouthEast, this.southEastVertices, this.meshBaseRadius, heightData);

            this.BoundingBox = new BoundingBox((float) this.South, (float) this.North, (float) this.West, (float) this.East,
                                          (float) layerRadius, (float) layerRadius + 10000*this.verticalExaggeration);

            this.QuadTileSet.IsDownloadingElevation = false;

            // Build common set of indexes for the 4 child meshes	
            int vertexCountElevatedPlus2 = vertexCountElevated/2 + 2;
            this.vertexIndexes = new short[2*vertexCountElevatedPlus2*vertexCountElevatedPlus2*3];

            int elevated_idx = 0;
            for (int i = 0; i < vertexCountElevatedPlus2; i++)
            {
                for (int j = 0; j < vertexCountElevatedPlus2; j++)
                {
                    this.vertexIndexes[elevated_idx++] = (short) (i*vertexCountElevatedPlus3 + j);
                    this.vertexIndexes[elevated_idx++] = (short) ((i + 1)*vertexCountElevatedPlus3 + j);
                    this.vertexIndexes[elevated_idx++] = (short) (i*vertexCountElevatedPlus3 + j + 1);

                    this.vertexIndexes[elevated_idx++] = (short) (i*vertexCountElevatedPlus3 + j + 1);
                    this.vertexIndexes[elevated_idx++] = (short) ((i + 1)*vertexCountElevatedPlus3 + j);
                    this.vertexIndexes[elevated_idx++] = (short) ((i + 1)*vertexCountElevatedPlus3 + j + 1);
                }
            }

            this.calculate_normals(ref this.northWestVertices, this.vertexIndexes);
            this.calculate_normals(ref this.southWestVertices, this.vertexIndexes);
            this.calculate_normals(ref this.northEastVertices, this.vertexIndexes);
            this.calculate_normals(ref this.southEastVertices, this.vertexIndexes);

            this.isDownloadingTerrain = false;
        }

        protected byte m_CurrentOpacity = 255;

        /// <summary>
        /// Create child tile terrain mesh
        /// Build the mesh with one extra vertice all around for proper normals calculations later on.
        /// Use the struts vertices to that effect. Struts are properly folded after normals calculations.
        /// </summary>
        protected void CreateElevatedMesh(ChildLocation corner, CustomVertex.PositionNormalTextured[] vertices,
                                          double meshBaseRadius, float[,] heightData)
        {
            // Figure out child lat/lon boundaries (radians)
            double north = MathEngine.DegreesToRadians(this.North);
            double west = MathEngine.DegreesToRadians(this.West);

            // Texture coordinate offsets
            float TuOffset = 0;
            float TvOffset = 0;

            switch (corner)
            {
                case ChildLocation.NorthWest:
                    // defaults are all good
                    break;
                case ChildLocation.NorthEast:
                    west = MathEngine.DegreesToRadians(0.5*(this.West + this.East));
                    TuOffset = 0.5f;
                    break;
                case ChildLocation.SouthWest:
                    north = MathEngine.DegreesToRadians(0.5*(this.North + this.South));
                    TvOffset = 0.5f;
                    break;
                case ChildLocation.SouthEast:
                    north = MathEngine.DegreesToRadians(0.5*(this.North + this.South));
                    west = MathEngine.DegreesToRadians(0.5*(this.West + this.East));
                    TuOffset = 0.5f;
                    TvOffset = 0.5f;
                    break;
            }

            double latitudeRadianSpan = MathEngine.DegreesToRadians(this.LatitudeSpan);
            double longitudeRadianSpan = MathEngine.DegreesToRadians(this.LongitudeSpan);

            double layerRadius = (double) this.QuadTileSet.LayerRadius;
            double scaleFactor = 1.0/vertexCountElevated;
            int terrainLongitudeIndex = (int) (TuOffset*vertexCountElevated) + 1;
            int terrainLatitudeIndex = (int) (TvOffset*vertexCountElevated) + 1;

            int vertexCountElevatedPlus1 = vertexCountElevated/2 + 1;

            double radius = 0;
            int vertexIndex = 0;
            for (int latitudeIndex = -1; latitudeIndex <= vertexCountElevatedPlus1; latitudeIndex++)
            {
                double latitudeFactor = latitudeIndex*scaleFactor;
                double latitude = north - latitudeFactor*latitudeRadianSpan;

                // Cache trigonometric values
                double cosLat = Math.Cos(latitude);
                double sinLat = Math.Sin(latitude);

                for (int longitudeIndex = -1; longitudeIndex <= vertexCountElevatedPlus1; longitudeIndex++)
                {
                    // Top of mesh for all (real terrain + struts)
                    radius = layerRadius +
                             heightData[terrainLatitudeIndex + latitudeIndex, terrainLongitudeIndex + longitudeIndex]
                             * this.verticalExaggeration;

                    double longitudeFactor = longitudeIndex*scaleFactor;

                    // Texture coordinates
                    vertices[vertexIndex].Tu = TuOffset + (float) longitudeFactor;
                    vertices[vertexIndex].Tv = TvOffset + (float) latitudeFactor;

                    // Convert from spherical (radians) to cartesian
                    double longitude = west + longitudeFactor*longitudeRadianSpan;
                    double radCosLat = radius*cosLat;
                    vertices[vertexIndex].X = (float) (radCosLat*Math.Cos(longitude) - this.localOrigin.X);
                    vertices[vertexIndex].Y = (float) (radCosLat*Math.Sin(longitude) - this.localOrigin.Y);
                    vertices[vertexIndex].Z = (float) (radius*sinLat - this.localOrigin.Z);

                    vertexIndex++;
                }
            }
        }

        private void calculate_normals(ref CustomVertex.PositionNormalTextured[] vertices, short[] indices)
        {
            ArrayList[] normal_buffer = new ArrayList[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                normal_buffer[i] = new ArrayList();
            }
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3 p1 = vertices[indices[i + 0]].Position;
                Vector3 p2 = vertices[indices[i + 1]].Position;
                Vector3 p3 = vertices[indices[i + 2]].Position;

                Vector3 v1 = p2 - p1;
                Vector3 v2 = p3 - p1;
                Vector3 normal = Vector3.Cross(v1, v2);

                normal.Normalize();

                // Store the face's normal for each of the vertices that make up the face.
                normal_buffer[indices[i + 0]].Add(normal);
                normal_buffer[indices[i + 1]].Add(normal);
                normal_buffer[indices[i + 2]].Add(normal);
            }

            // Now loop through each vertex vector, and avarage out all the normals stored.
            for (int i = 0; i < vertices.Length; ++i)
            {
                for (int j = 0; j < normal_buffer[i].Count; ++j)
                {
                    Vector3 curNormal = (Vector3) normal_buffer[i][j];

                    if (vertices[i].Normal == Vector3.Zero)
                        vertices[i].Normal = curNormal;
                    else
                        vertices[i].Normal += curNormal;
                }

                vertices[i].Normal.Multiply(1.0f/normal_buffer[i].Count);
            }

            // Adjust/Fold struts vertices using terrain border vertices positions
            short vertexDensity = (short) Math.Sqrt(vertices.Length);
            for (int i = 0; i < vertexDensity; i++)
            {
                if (i == 0 || i == vertexDensity - 1)
                {
                    for (int j = 0; j < vertexDensity; j++)
                    {
                        int offset = (i == 0) ? vertexDensity : -vertexDensity;
                        if (j == 0) offset++;
                        if (j == vertexDensity - 1) offset--;
                        Point3d p =
                            new Point3d(vertices[i*vertexDensity + j + offset].Position.X,
                                        vertices[i*vertexDensity + j + offset].Position.Y,
                                        vertices[i*vertexDensity + j + offset].Position.Z);
                        if (this.renderStruts && this.m_CurrentOpacity == 255) p = this.ProjectOnMeshBase(p);
                        vertices[i*vertexDensity + j].Position = new Vector3((float) p.X, (float) p.Y, (float) p.Z);
                    }
                }
                else
                {
                    Point3d p =
                        new Point3d(vertices[i*vertexDensity + 1].Position.X, vertices[i*vertexDensity + 1].Position.Y,
                                    vertices[i*vertexDensity + 1].Position.Z);
                    if (this.renderStruts && this.m_CurrentOpacity == 255) p = this.ProjectOnMeshBase(p);
                    vertices[i*vertexDensity].Position = new Vector3((float) p.X, (float) p.Y, (float) p.Z);

                    p =
                        new Point3d(vertices[i*vertexDensity + vertexDensity - 2].Position.X,
                                    vertices[i*vertexDensity + vertexDensity - 2].Position.Y,
                                    vertices[i*vertexDensity + vertexDensity - 2].Position.Z);
                    if (this.renderStruts && this.m_CurrentOpacity == 255) p = this.ProjectOnMeshBase(p);
                    vertices[i*vertexDensity + vertexDensity - 1].Position =
                        new Vector3((float) p.X, (float) p.Y, (float) p.Z);
                }
            }
        }

        // Project an elevated mesh point to the mesh base
        private Point3d ProjectOnMeshBase(Point3d p)
        {
            p = p + this.localOrigin;
            p = p.normalize();
            p = p* this.meshBaseRadius - this.localOrigin;
            return p;
        }

        // End edits by Patrick Murris : fixing mesh sides normals (2006-11-18)


        public string ImageFilePath = null;

        public virtual bool Render(DrawArgs drawArgs)
        {
            this.m_CurrentOpacity = this.QuadTileSet.Opacity;
            try
            {
                if (!this.isInitialized ||
                    this.northWestVertices == null ||
                    this.northEastVertices == null ||
                    this.southEastVertices == null ||
                    this.southWestVertices == null)
                    return false;

                if (!DrawArgs.Camera.ViewFrustum.Intersects(this.BoundingBox))
                    return false;

                bool northWestChildRendered = false;
                bool northEastChildRendered = false;
                bool southWestChildRendered = false;
                bool southEastChildRendered = false;

                if (this.northWestChild != null)
                    if (this.northWestChild.Render(drawArgs))
                        northWestChildRendered = true;

                if (this.southWestChild != null)
                    if (this.southWestChild.Render(drawArgs))
                        southWestChildRendered = true;

                if (this.northEastChild != null)
                    if (this.northEastChild.Render(drawArgs))
                        northEastChildRendered = true;

                if (this.southEastChild != null)
                    if (this.southEastChild.Render(drawArgs))
                        southEastChildRendered = true;

                if (this.QuadTileSet.RenderFileNames &&
                    (!northWestChildRendered || !northEastChildRendered || !southWestChildRendered ||
                     !southEastChildRendered))
                {
                    Vector3 referenceCenter = new Vector3(
                        (float) drawArgs.WorldCamera.ReferenceCenter.X,
                        (float) drawArgs.WorldCamera.ReferenceCenter.Y,
                        (float) drawArgs.WorldCamera.ReferenceCenter.Z);

                    this.RenderDownloadRectangle(drawArgs, Color.FromArgb(255, 0, 0).ToArgb(), referenceCenter);

                    Vector3 cartesianPoint = MathEngine.SphericalToCartesian(this.CenterLatitude.Degrees, this.CenterLongitude.Degrees,
                        drawArgs.WorldCamera.WorldRadius + drawArgs.WorldCamera.TerrainElevation);

                    if (this.ImageFilePath != null && drawArgs.WorldCamera.ViewFrustum.ContainsPoint(cartesianPoint))
                    {
                        Vector3 projectedPoint = drawArgs.WorldCamera.Project(cartesianPoint - referenceCenter);

                        Rectangle rect = new Rectangle(
                            (int) projectedPoint.X - 100,
                            (int) projectedPoint.Y,
                            200,
                            200);

                        drawArgs.defaultDrawingFont.DrawText(
                            null, this.ImageFilePath,
                            rect,
                            DrawTextFormat.WordBreak,
                            Color.Red);
                    }
                }

                if (northWestChildRendered && northEastChildRendered && southWestChildRendered && southEastChildRendered)
                {
                    return true;
                }

                Device device = DrawArgs.Device;

                for (int i = 0; i < this.textures.Length; i++)
                {
                    if (this.textures[i] == null || this.textures[i].Disposed)
                        return false;

                    device.SetTexture(i, this.textures[i]);
                }

                drawArgs.numberTilesDrawn++;

                int numpasses = 1;
                int pass;

                DrawArgs.device.SetTransform(TransformState.World, Matrix.Translation(
                    (float) (this.localOrigin.X - drawArgs.WorldCamera.ReferenceCenter.X),
                    (float) (this.localOrigin.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                    (float) (this.localOrigin.Z - drawArgs.WorldCamera.ReferenceCenter.Z)
                    );

                for (pass = 0; pass < numpasses; pass++)
                {
                    if (!northWestChildRendered) this.Render(device, this.northWestVertices, this.northWestChild);
                    if (!southWestChildRendered) this.Render(device, this.southWestVertices, this.southWestChild);
                    if (!northEastChildRendered) this.Render(device, this.northEastVertices, this.northEastChild);
                    if (!southEastChildRendered) this.Render(device, this.southEastVertices, this.southEastChild);
                }

                DrawArgs.device.SetTransform(TransformState.World, DrawArgs.Camera.WorldMatrix;

                return true;
            }
            catch (DirectXException)
            {
            }
            return false;
        }

        protected CustomVertex.PositionColored[] downloadRectangle = new CustomVertex.PositionColored[5];

        /// <summary>
        /// Render a rectangle around an image tile in the specified color
        /// </summary>
        public void RenderDownloadRectangle(DrawArgs drawArgs, int color, Vector3 referenceCenter)
        {
            // Render terrain download rectangle
            Vector3 northWestV = MathEngine.SphericalToCartesian((float) this.North, (float) this.West, this.QuadTileSet.LayerRadius) -
                                 referenceCenter;
            Vector3 southWestV = MathEngine.SphericalToCartesian((float) this.South, (float) this.West, this.QuadTileSet.LayerRadius) -
                                 referenceCenter;
            Vector3 northEastV = MathEngine.SphericalToCartesian((float) this.North, (float) this.East, this.QuadTileSet.LayerRadius) -
                                 referenceCenter;
            Vector3 southEastV = MathEngine.SphericalToCartesian((float) this.South, (float) this.East, this.QuadTileSet.LayerRadius) -
                                 referenceCenter;

            this.downloadRectangle[0].X = northWestV.X;
            this.downloadRectangle[0].Y = northWestV.Y;
            this.downloadRectangle[0].Z = northWestV.Z;
            this.downloadRectangle[0].Color = color;

            this.downloadRectangle[1].X = southWestV.X;
            this.downloadRectangle[1].Y = southWestV.Y;
            this.downloadRectangle[1].Z = southWestV.Z;
            this.downloadRectangle[1].Color = color;

            this.downloadRectangle[2].X = southEastV.X;
            this.downloadRectangle[2].Y = southEastV.Y;
            this.downloadRectangle[2].Z = southEastV.Z;
            this.downloadRectangle[2].Color = color;

            this.downloadRectangle[3].X = northEastV.X;
            this.downloadRectangle[3].Y = northEastV.Y;
            this.downloadRectangle[3].Z = northEastV.Z;
            this.downloadRectangle[3].Color = color;

            this.downloadRectangle[4].X = this.downloadRectangle[0].X;
            this.downloadRectangle[4].Y = this.downloadRectangle[0].Y;
            this.downloadRectangle[4].Z = this.downloadRectangle[0].Z;
            this.downloadRectangle[4].Color = color;

            drawArgs.device.SetRenderState(RenderState.ZBufferEnable = false;
            drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation = TextureOperation.Disable;
            drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, 4, this.downloadRectangle);
            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation = TextureOperation.SelectArg1;
            drawArgs.device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            drawArgs.device.SetRenderState(RenderState.ZBufferEnable = true;
        }

        private static Effect grayscaleEffect = null;

        /// <summary>
        /// Render one of the 4 quadrants with optional download indicator
        /// </summary>
        private void Render(Device device, CustomVertex.PositionNormalTextured[] verts, QuadTile child)
        {
            bool isMultitexturing = false;

            if (!World.Settings.EnableSunShading)
            {
                if (World.Settings.ShowDownloadIndicator && child != null)
                {
                    // Check/display download activity
                    //GeoSpatialDownloadRequest request = child.DownloadRequest;
                    if (child.isDownloadingTerrain)
                    {
                        device.SetTexture(1, QuadTileSet.DownloadTerrainTexture);
                        isMultitexturing = true;
                    }
                        //else if (request != null)
                    else if (child.WaitingForDownload)
                    {
                        if (child.IsDownloadingImage)
                            device.SetTexture(1, QuadTileSet.DownloadInProgressTexture);
                        else
                            device.SetTexture(1, QuadTileSet.DownloadQueuedTexture);
                        isMultitexturing = true;
                    }
                }
            }

            if (isMultitexturing)
                device.SetTextureStageState(1, TextureStageStates.ColorOperation,
                                            (int) TextureOperation.BlendTextureAlpha);

            if (verts != null && this.vertexIndexes != null)
            {
                if (this.QuadTileSet.Effect != null)
                {
                    Effect effect = this.QuadTileSet.Effect;

                    int tc1 = device.GetTextureStageStateInt32(1, TextureStageStates.TextureCoordinateIndex);
                    device.SetTextureStageState(1, TextureStageStates.TextureCoordinateIndex, 1);


                    // FIXME: just use the first technique for now
                    effect.Technique = effect.GetTechnique(0);
                    EffectHandle param;
                    param = (EffectHandle) this.QuadTileSet.EffectParameters["WorldViewProj"];
                    if (param != null)
                        effect.SetValue(param,
                                        Matrix.Multiply(device.Transform.World,
                                                        Matrix.Multiply(device.Transform.View,
                                                                        device.Transform.Projection)));
                    try
                    {
                        param = (EffectHandle) this.QuadTileSet.EffectParameters["World"];
                        if (param != null)
                            effect.SetValue(param, device.Transform.World);
                        param = (EffectHandle) this.QuadTileSet.EffectParameters["ViewInverse"];
                        if (param != null)
                        {
                            Matrix viewInv = Matrix.Invert(device.Transform.View);
                            effect.SetValue(param, viewInv);
                        }

                        // set textures as available
                        for (int i = 0; i < this.textures.Length; i++)
                        {
                            string name = string.Format("Tex{0}", i);
                            param = (EffectHandle) this.QuadTileSet.EffectParameters[name];
                            if (param != null)
                            {
                                SurfaceDescription sd = this.textures[i].GetLevelDescription(0);
                                effect.SetValue(param, this.textures[i]);
                            }
                        }

                        // brightness & opacity values
                        param = (EffectHandle) this.QuadTileSet.EffectParameters["Brightness"];
                        if (param != null)
                            effect.SetValue(param, this.QuadTileSet.GrayscaleBrightness);

                        param = (EffectHandle) this.QuadTileSet.EffectParameters["Opacity"];
                        if (param != null)
                        {
                            float opacity = (float) this.QuadTileSet.Opacity/255.0f;
                            effect.SetValue(param, opacity);
                        }

                        param = (EffectHandle) this.QuadTileSet.EffectParameters["LayerRadius"];
                        if (param != null)
                        {
                            effect.SetValue(param, (float) this.QuadTileSet.LayerRadius);
                        }

                        param = (EffectHandle) this.QuadTileSet.EffectParameters["TileLevel"];
                        if (param != null)
                        {
                            effect.SetValue(param, this.Level);
                        }

                        param = (EffectHandle) this.QuadTileSet.EffectParameters["LocalOrigin"];
                        if (param != null)
                        {
                            effect.SetValue(param, this.localOrigin.Vector4);
                        }

                        // sun position
                        param = (EffectHandle) this.QuadTileSet.EffectParameters["LightDirection"];
                        if (param != null)
                        {
                            Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
                            sunPosition = sunPosition.normalize();
                            Vector4 sunVector = new Vector4(
                                (float)sunPosition.X,
                                (float)sunPosition.Y,
                                (float)sunPosition.Z,
                                0.0f);
                            effect.SetValue(param, sunVector);
                        }

                        // local origin
                        param = (EffectHandle) this.QuadTileSet.EffectParameters["LocalFrameOrigin"];
                        if (param != null)
                        {
                            //localOrigin = BoundingBox.CalculateCenter();
                            Point3d centerPoint =
                                MathEngine.SphericalToCartesianD(this.CenterLatitude, this.CenterLongitude, this.QuadTileSet.LayerRadius);
                            Point3d northHalf =
                                MathEngine.SphericalToCartesianD(Angle.FromDegrees(this.North), this.CenterLongitude, this.QuadTileSet.LayerRadius);
                            Point3d eastHalf =
                                MathEngine.SphericalToCartesianD(this.CenterLatitude, Angle.FromDegrees(this.East), this.QuadTileSet.LayerRadius);

                            Vector4 xdir = 2*(eastHalf - centerPoint).Vector4;
                            Vector4 ydir = 2*(northHalf - centerPoint).Vector4;
                            // up vector is radius at center point, normalized
                            Vector4 zdir = centerPoint.normalize().Vector4;
                            // local frame origin at SW corner, relative to local origin
                            Point3d localFrameOrigin = northHalf + eastHalf - centerPoint - this.localOrigin;
                            Vector4 lfoW = localFrameOrigin.Vector4;
                            lfoW.W = 1;
                            lfoW.Transform(device.Transform.World);
                            effect.SetValue(param, localFrameOrigin.Vector4);

                            param = (EffectHandle) this.QuadTileSet.EffectParameters["LocalFrameXAxis"];
                            if (param != null) effect.SetValue(param, xdir);
                            param = (EffectHandle) this.QuadTileSet.EffectParameters["LocalFrameYAxis"];
                            if (param != null) effect.SetValue(param, ydir);
                            param = (EffectHandle) this.QuadTileSet.EffectParameters["LocalFrameZAxis"];
                            if (param != null) effect.SetValue(param, zdir);
                        }
                    }
                    catch
                    {
                    }

                    int numPasses = effect.Begin(0);
                    for (int i = 0; i < numPasses; i++)
                    {
                        effect.BeginPass(i);
                        device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
                                                         verts.Length, this.vertexIndexes.Length/3, this.vertexIndexes, true,
                                                         verts);

                        effect.EndPass();
                    }

                    effect.End();
                    device.SetTextureStageState(1, TextureStageStates.TextureCoordinateIndex, tc1);
                }
                else if (!this.QuadTileSet.RenderGrayscale || (device.Capabilities.PixelShaderVersion.Major < 1))
                {
                    if (World.Settings.EnableSunShading)
                    {
                        Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
                        Vector3 sunVector = new Vector3(
                            (float) sunPosition.X,
                            (float) sunPosition.Y,
                            (float) sunPosition.Z);

                        device.SetRenderState(RenderState.Lighting = true;
                        Material material = new Material();
                        material.Diffuse = Color.White;
                        material.Ambient = Color.White;

                        device.Material = material;
                        device.SetRenderState(RenderState.AmbientColor = World.Settings.ShadingAmbientColor.ToArgb();
                        device.SetRenderState(RenderState.NormalizeNormals = true;
                        device.SetRenderState(RenderState.AlphaBlendEnable = true;

                        device.Lights[0].Enabled = true;
                        device.Lights[0].Type = LightType.Directional;
                        device.Lights[0].Diffuse = World.Settings.LightColor;
                        device.Lights[0].Direction = sunVector;

                        device.SetTextureStageState(0, TextureStage.ColorOperation = TextureOperation.Modulate;
                        device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse;
                        device.SetTextureStageState(0, TextureStage.ColorArg2,TextureArgument.TextureColor;
                    }
                    else
                    {
                        device.SetRenderState(RenderState.Lighting = false;
                        device.SetRenderState(RenderState.Ambient = World.Settings.StandardAmbientColor;
                    }

                    device.SetRenderState(RenderState.TextureFactor = Color.FromArgb(this.m_CurrentOpacity, 255, 255, 255).ToArgb();
                    device.SetTextureStageState(0, TextureStage.AlphaOperation,  TextureOperation.Modulate;
                    device.SetTextureStageState(0, TextureStage.AlphaArg1,TextureArgument.TextureColor;
                    device.SetTextureStageState(0, TextureStage.AlphaArg2,TextureArgument.TFactor;

                    device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, verts.Length, this.vertexIndexes.Length/3, this.vertexIndexes, true, verts);
                }
                else
                {
                    if (grayscaleEffect == null)
                    {
                        device.DeviceReset += new EventHandler(this.device_DeviceReset);
                        device_DeviceReset(device, null);
                    }

                    grayscaleEffect.Technique = "RenderGrayscaleBrightness";
                    grayscaleEffect.SetValue("WorldViewProj",
                                             Matrix.Multiply(device.Transform.World,
                                                             Matrix.Multiply(device.Transform.View,
                                                                             device.Transform.Projection)));
                    grayscaleEffect.SetValue("Tex0", this.textures[0]);
                    grayscaleEffect.SetValue("Brightness", this.QuadTileSet.GrayscaleBrightness);
                    float opacity = (float) this.QuadTileSet.Opacity/255.0f;
                    grayscaleEffect.SetValue("Opacity", opacity);

                    int numPasses = grayscaleEffect.Begin(0);
                    for (int i = 0; i < numPasses; i++)
                    {
                        grayscaleEffect.BeginPass(i);
                        device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
                                                         verts.Length, this.vertexIndexes.Length/3, this.vertexIndexes, true,
                                                         verts);

                        grayscaleEffect.EndPass();
                    }

                    grayscaleEffect.End();
                }
            }
            if (isMultitexturing)
                device.SetTextureStageState(1, TextureStageStates.ColorOperation, (int) TextureOperation.Disable);
        }

        private void device_DeviceReset(object sender, EventArgs e)
        {
            Device device = (Device) sender;

            string outerrors = "";

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream("WorldWind.Shaders.grayscale.fx");

                grayscaleEffect =
                    Effect.FromStream(
                        device,
                        stream,
                        null,
                        null,
                        ShaderFlags.None,
                        null,
                        out outerrors);

                if (outerrors != null && outerrors.Length > 0)
                    Log.Write(Log.Levels.Error, outerrors);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
