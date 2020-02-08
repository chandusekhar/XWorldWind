using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using SharpDX;
using SharpDX.Direct3D9;
using WorldWind.Net;
using Color = System.Drawing.Color;

namespace WorldWind
{
    /// <summary>
    /// This class Loads and Renders at a specific lat,lon,alt a given
    /// Model(ie Textured Mesh) in Direct X or Other supported Format
    /// </summary>
    public class ModelFeature : RenderableObject
    {
        #region Public Variables
        #endregion

        #region Private Variables

        private float m_latitude;
        private float m_longitude;
        private float m_altitude;
        private float m_scale = 1;
        private float m_rotx, m_roty, m_rotz;
        private AltitudeMode m_altitudeMode = AltitudeMode.RelativeToGround;
        private bool m_isVertExaggerable = true;
        private bool m_isElevationRelativeToGround = true;
        private float storedAltitude; // to hold the old altitude value when AltitudeMode is switched to ClampedToGround
        private Color m_tintColor = Color.LightGray;

        private static Dictionary<string, MeshTableElem> m_meshTable = new Dictionary<string, MeshTableElem>();
        private object m_thisLock = new object();

        #endregion

        #region Protected variables
        protected double currentElevation;
        protected float vertExaggeration = 1;
        protected Vector3 worldXyz; // XYZ World coordinates
        protected string meshFileName;

        protected MeshTableElem m_meshTableElem;
        protected MeshElem[] m_meshElems;
        protected string errorMsg;
        private string m_refreshurl;
        private System.Timers.Timer refreshTimer;    //For mesh location polling

        protected Matrix m_lastView = Matrix.Identity;

        #endregion

        protected class MeshTableElem
        {
            public String meshFilePath;
            public long referenceCount;
            public MeshElem[] meshElems;
        }

        protected class MeshElem
        {
            public Mesh mesh;
            public Texture[] meshTextures;            // Textures for the mesh
            public Material[] meshMaterials;
        }

        #region Accessor Methods
        public string RefreshURL
        {
            set { this.m_refreshurl = value; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets/sets model latitude
        /// </summary>
        [Category("Model"), Description("Latitude of model")]
        public float Latitude
        {
            get { return this.m_latitude; }
            set { this.m_latitude = value; }
        }

        /// <summary>
        /// Gets/sets model longitude
        /// </summary>
        [Category("Model"), Description("Longitude of model")]
        public float Longitude
        {
            get { return this.m_longitude; }
            set { this.m_longitude = value; }
        }

        /// <summary>
        /// Gets/sets model altitude
        /// </summary>
        [Category("Model"), Description("Altitude of model")]
        public float Altitude
        {
            get { return this.m_altitude; }
            set
            {
                this.m_altitude = value;
                this.storedAltitude = this.m_altitude;
            }
        }

        /// <summary>
        /// Gets/sets model scale
        /// </summary>
        [Category("Model"), Description("Scale of model")]
        public float Scale
        {
            get { return this.m_scale; }
            set { this.m_scale = value; }
        }

        /// <summary>
        /// Gets/sets X rotation
        /// </summary>
        [Category("Model"), Description("Rotation about model X-axis")]
        public float RotX
        {
            get { return this.m_rotx; }
            set { this.m_rotx = value; }
        }

        /// <summary>
        /// Gets/sets Y rotation
        /// </summary>
        [Category("Model"), Description("Rotation about model Y-axis")]
        public float RotY
        {
            get { return this.m_roty; }
            set { this.m_roty = value; }
        }

        /// <summary>
        /// Gets/sets Z rotation
        /// </summary>
        [Category("Model"), Description("Rotation about model Z-axis")]
        public float RotZ
        {
            get { return this.m_rotz; }
            set { this.m_rotz = value; }
        }

        /// <summary>
        /// Gets/sets whether altitude obeys vert. exaggeration value from settings
        /// </summary>
        [Category("Model"), Description("Whether model altitude scales with vertical exaggeration")]
        public bool IsVertExaggerable
        {
            get { return this.m_isVertExaggerable; }
            set { this.m_isVertExaggerable = value; }
        }

        /// <summary>
        /// Gets/sets whether model altitude is relative to ground
        /// </summary>
        [Category("Model"), Description("Whether model altitude is relative to ground")]
        public bool IsElevationRelativeToGround
        {
            get { return this.m_isElevationRelativeToGround; }
            set { this.m_isElevationRelativeToGround = value; }
        }

        /// <summary>
        /// Gets/sets altitude mode
        /// </summary>
        [Category("Model")]
        public AltitudeMode AltitudeMode
        {
            get { return this.m_altitudeMode; }
            set
            {
                this.m_altitudeMode = value;

                if (this.m_altitudeMode == AltitudeMode.Absolute)
                {
                    this.m_isElevationRelativeToGround = false;
                    this.m_altitude = this.storedAltitude;
                }
                if (this.m_altitudeMode == AltitudeMode.RelativeToGround)
                {
                    this.m_isElevationRelativeToGround = true;
                    this.m_altitude = this.storedAltitude;
                }
                if (this.m_altitudeMode == AltitudeMode.ClampedToGround) this.m_altitude = 0;

            }
        }

        /// <summary>
        /// Get sets the color to tint the model with.  Used by ambient lighting.
        /// </summary>
        [Category("Model"), Description("Color to tint the model.")]
        public Color TintColor
        {
            get { return this.m_tintColor; }
            set { this.m_tintColor = value; }
        }

        #endregion


        public ModelFeature(string name, World parentWorld, string fileName, float Latitude,
            float Longitude, float Altitude, float Scale, float rotX, float rotY, float rotZ)
            : base(name, parentWorld)
        {
            this.meshFileName = fileName;
            this.m_latitude = Latitude;
            this.m_longitude = Longitude;
            this.m_altitude = Altitude;
            this.m_scale = Scale;
            this.m_rotx = rotX;
            this.m_roty = rotY;
            this.m_rotz = rotZ;
            this.storedAltitude = Altitude;
            this.m_meshElems = null;
        }

        /// <summary>
        /// Determine if the object is visible
        /// </summary>
        protected bool IsVisible(CameraBase camera)
        {
            if (this.IsVertExaggerable == true)
                this.vertExaggeration = World.Settings.VerticalExaggeration;
            else
                this.vertExaggeration = 1;

            //if (worldXyz == Vector3.Zero)
            this.worldXyz = MathEngine.SphericalToCartesian(this.Latitude, this.Longitude, this.World.EquatorialRadius + ((this.currentElevation + this.Altitude) * this.vertExaggeration));
            return camera.ViewFrustum.ContainsPoint(this.worldXyz);
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (this.errorMsg != null)
            {
                //System.Windows.Forms.MessageBox.Show( errorMsg, "Model failed to load.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.errorMsg = null;
                this.IsOn = false;
                this.isInitialized = false;
                return;
            }

            if (!this.IsVisible(drawArgs.WorldCamera))
            {
                // Mesh is not in view, unload it to save memory
                //if (isInitialized)
                    //Dispose();
                return;
            }

            if (!this.isInitialized)
            {
                this.Update(drawArgs);
            }

            // save current state
            Matrix currentWorld = drawArgs.device.Transform.World;
            Cull cullMode = drawArgs.device.SetRenderState(RenderState.CullMode;
            bool lighting = drawArgs.device.SetRenderState(RenderState.Lighting;
            int ambientColor = drawArgs.device.SetRenderState(RenderState.AmbientColor;
            bool normalizeNormals = drawArgs.device.SetRenderState(RenderState.NormalizeNormals;

            drawArgs.device.SetRenderState(RenderState.CullMode = Cull.None;
            drawArgs.device.SetRenderState(RenderState.Lighting = true;
            drawArgs.device.SetRenderState(RenderState.AmbientColor = 0xCCCCCC; //  0x808080;
            drawArgs.device.SetRenderState(RenderState.NormalizeNormals = true;

            drawArgs.device.Lights[0].Diffuse = Color.FromArgb(255, 255, 255);
            drawArgs.device.Lights[0].Ambient = this.TintColor;
            drawArgs.device.Lights[0].Type = LightType.Directional;
            drawArgs.device.Lights[0].Direction = new Vector3(1f, 1f, 1f);
            drawArgs.device.Lights[0].Enabled = true;

            drawArgs.device.SetSamplerState(0, SamplerState.AddressU = TextureAddress.Wrap;
            drawArgs.device.SetSamplerState(0, SamplerState.AddressV = TextureAddress.Wrap;

            drawArgs.device.SetRenderState(RenderState.AlphaBlendEnable = true;
            drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.TextureColor;
            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation = TextureOperation.SelectArg1;

            // Put the light somewhere up in space
            drawArgs.device.Lights[0].Position = new Vector3(
                (float) this.worldXyz.X * 2f,
                (float) this.worldXyz.Y * 1f,
                (float) this.worldXyz.Z * 1.5f);

            drawArgs.device.SetTransform(TransformState.World, Matrix.RotationX((float)MathEngine.DegreesToRadians(this.RotX));
            drawArgs.device.Transform.World *= Matrix.RotationY((float)MathEngine.DegreesToRadians(this.RotY));
            drawArgs.device.Transform.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(this.RotZ));
            drawArgs.device.Transform.World *= Matrix.Scaling(this.Scale, this.Scale, this.Scale);

            // Move the mesh to desired location on earth
            if (this.IsVertExaggerable == true)
                this.vertExaggeration = World.Settings.VerticalExaggeration;
            else
                this.vertExaggeration = 1;
            drawArgs.device.Transform.World *= Matrix.Translation(0, 0, (float)(drawArgs.WorldCamera.WorldRadius + ((this.currentElevation + this.Altitude) * this.vertExaggeration)));
            drawArgs.device.Transform.World *= Matrix.RotationY((float)MathEngine.DegreesToRadians(90 - this.Latitude));
            drawArgs.device.Transform.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(this.Longitude));


            drawArgs.device.Transform.World *= Matrix.Translation(
                (float)-drawArgs.WorldCamera.ReferenceCenter.X,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Z
                );

            foreach (MeshElem me in this.m_meshElems)
            {
                try
                {
                    for (int i = 0; i < me.meshMaterials.Length; i++)
                    {
                        // Set the material and texture for this subset
                        drawArgs.device.Material = me.meshMaterials[i];
                        drawArgs.device.SetTexture(0, me.meshTextures[i]);

                        // Draw the mesh subset
                        me.mesh.DrawSubset(i);
                    }
                }
                catch (Exception)
                {
                    // Utility.Log.Write(name + caught);
                }
            }

            drawArgs.device.SetTransform(TransformState.World, currentWorld;
            drawArgs.device.SetRenderState(RenderState.Lighting = lighting;
            drawArgs.device.SetRenderState(RenderState.CullMode = cullMode;
            drawArgs.device.SetRenderState(RenderState.AmbientColor = ambientColor;
            drawArgs.device.SetRenderState(RenderState.NormalizeNormals = normalizeNormals;
        }


        /// <summary>
        /// RenderableObject abstract member (needed) 
        /// OBS: Worker thread (don't update UI directly from this thread)
        /// </summary>
        public override void Initialize(DrawArgs drawArgs)
        {
            if (!this.IsVisible(drawArgs.WorldCamera))
                return;
            if (this.meshFileName.StartsWith("http"))
            {
                Uri meshuri = new Uri(this.meshFileName);
                string meshpath = meshuri.AbsolutePath;
                string extension = Path.GetExtension(meshpath);
                //download online mesh files to cache and
                //update meshfilename to new name
                if (meshuri.Scheme == Uri.UriSchemeHttp
                    || meshuri.Scheme == Uri.UriSchemeHttps)
                {
                    try
                    {
                        // Offline check
                        if (World.Settings.WorkOffline)
                            throw new Exception("Offline mode active.");

                        WebDownload request = new WebDownload(this.meshFileName);

                        string cachefilename = request.GetHashCode() + extension;
                        //HACK: Hard Coded Path
                        cachefilename =
                            Directory.GetParent(System.Windows.Forms.Application.ExecutablePath) +
                            "//Cache//Models//" + cachefilename;
                        if (!File.Exists(cachefilename))
                            request.DownloadFile(cachefilename);
                        this.meshFileName = cachefilename;
                    }
                    catch (Exception caught)
                    {
                        Utility.Log.Write(caught);
                        this.errorMsg = "Failed to download mesh from " + this.meshFileName;
                    }
                }
            }
            string ext = Path.GetExtension(this.meshFileName);
            try
            {
                lock (this.m_thisLock)
                {
                    if (m_meshTable.ContainsKey(this.meshFileName))
                    {
                        this.m_meshTableElem = m_meshTable[this.meshFileName];
                        this.m_meshTableElem.referenceCount++;
                        this.m_meshElems = this.m_meshTableElem.meshElems;
                    }
                    else
                    {
                        if (ext.Equals(".x"))
                            this.LoadDirectXMesh(drawArgs);
                        else if (ext.Equals(".dae") || ext.Equals(".xml")) this.LoadColladaMesh(drawArgs);

                        // if mesh loaded then add to the mesh table
                        if (this.m_meshElems != null)
                        {
                            this.m_meshTableElem = new MeshTableElem();
                            this.m_meshTableElem.meshFilePath = this.meshFileName;
                            this.m_meshTableElem.meshElems = this.m_meshElems;
                            this.m_meshTableElem.referenceCount = 1;

                            m_meshTable.Add(this.meshFileName, this.m_meshTableElem);
                        }
                    }
                }

                if (this.m_meshElems == null)
                    throw new InvalidMeshException();

                //vertExaggeration = World.Settings.VerticalExaggeration;
                //if (m_isElevationRelativeToGround == true)
                //    currentElevation = World.TerrainAccessor.GetElevationAt(Latitude, Longitude);

                if (this.refreshTimer == null && this.m_refreshurl != null)
                {
                    this.refreshTimer = new System.Timers.Timer(60000);
                    this.refreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.refreshTimer_Elapsed);
                    this.refreshTimer.Start();
                }

                this.isInitialized = true;
            }
            catch (Exception caught)
            {
                Utility.Log.Write(caught);
                this.errorMsg = "Failed to read mesh from " + this.meshFileName;
            }
        }

        /// <summary>
        /// Method to load Native Direct X Meshes
        /// </summary>
        private void LoadDirectXMesh(DrawArgs drawArgs)
        {
            this.m_meshElems = new MeshElem[1];

            ExtendedMaterial[] materials;
            GraphicsStream adj;
            this.m_meshElems[0] = new MeshElem();
            this.m_meshElems[0].mesh = Mesh.FromFile(this.meshFileName, MeshFlags.Managed, drawArgs.device, out adj, out materials);

            // Extract the material properties and texture names.
            this.m_meshElems[0].meshTextures = new Texture[materials.Length];
            this.m_meshElems[0].meshMaterials = new Material[materials.Length];
            string xFilePath = Path.GetDirectoryName(this.meshFileName);
            for (int i = 0; i < materials.Length; i++)
            {
                this.m_meshElems[0].meshMaterials[i] = materials[i].Material3D;
                // Set the ambient color for the material (D3DX does not do this)
                this.m_meshElems[0].meshMaterials[i].Ambient = this.m_meshElems[0].meshMaterials[i].Diffuse;

                // Create the texture.
                if (materials[i].TextureFilename != null)
                {
                    string textureFilePath = Path.Combine(xFilePath, materials[i].TextureFilename);
                    this.m_meshElems[0].meshTextures[i] = TextureLoader.FromFile(drawArgs.device, textureFilePath);
                }
            }
        }

        private void LoadColladaMesh(DrawArgs drawArgs)
        {
            XmlDocument colladaDoc = new XmlDocument();
            colladaDoc.Load(this.meshFileName);
            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(colladaDoc.NameTable);
            xmlnsManager.AddNamespace("c", colladaDoc.GetElementsByTagName("COLLADA")[0].NamespaceURI);

            int meshCount =
                    colladaDoc.SelectNodes("//c:COLLADA/c:library_geometries/c:geometry/c:mesh",
                    xmlnsManager).Count;
            if (meshCount == 0)
                return;
            if (this.m_meshElems != null)
            {
                foreach (MeshElem me in this.m_meshElems)
                {
                    if (me.mesh != null)
                        me.mesh.Dispose();
                    if (me.meshTextures != null)
                        foreach (Texture t in me.meshTextures)
                            if (t != null)
                                t.Dispose();
                }
            }

            this.m_meshElems = new MeshElem[meshCount];

            for (int j = 0; j < meshCount; j++)
            {
                XmlNode meshNode = colladaDoc.SelectNodes(
                        "//c:COLLADA/c:library_geometries/c:geometry/c:mesh",
                        xmlnsManager)[j];

                Matrix transform = Matrix.Identity;
                string sGeometryId = meshNode.SelectNodes("../@id", xmlnsManager)[0].Value;
                if (colladaDoc.SelectNodes("//c:COLLADA/c:library_visual_scenes/c:visual_scene/c:node" +
                        "[c:instance_geometry/@url='#" + sGeometryId + "']", xmlnsManager).Count == 1)
                {
                    XmlNode sceneNode =
                            colladaDoc.SelectNodes("//c:COLLADA/c:library_visual_scenes/c:visual_scene/c:node" +
                            "[c:instance_geometry/@url='#" + sGeometryId + "']", xmlnsManager)[0];

                    foreach (XmlNode childNode in sceneNode.ChildNodes)
                    {
                        Matrix m;
                        if (childNode.Name == "translate")
                        {
                            string[] translateParams = childNode.InnerText.Trim().Split(' ');
                            m = Matrix.Translation(new Vector3(
                                    Decimal.ToSingle(Decimal.Parse(
                                        translateParams[0],
                            System.Globalization.NumberStyles.Any)),
                                    Decimal.ToSingle(Decimal.Parse(
                                        translateParams[1],
                            System.Globalization.NumberStyles.Any)),
                                    Decimal.ToSingle(Decimal.Parse(
                                        translateParams[2],
                                        System.Globalization.NumberStyles.Any))));

                        }
                        else if (childNode.Name == "rotate")
                        {
                            string[] rotateParams = childNode.InnerText.Trim().Split(' ');
                            m = Matrix.RotationAxis(
                                    new Vector3(
                                        Decimal.ToSingle(Decimal.Parse(
                                            rotateParams[0],
                            System.Globalization.NumberStyles.Any)),
                                        Decimal.ToSingle(Decimal.Parse(
                                            rotateParams[1],
                            System.Globalization.NumberStyles.Any)),
                                        Decimal.ToSingle(Decimal.Parse(
                                            rotateParams[2],
                                            System.Globalization.NumberStyles.Any))),
                                    (float)Math.PI * Decimal.ToSingle(Decimal.Parse(
                                        rotateParams[3],
                                        System.Globalization.NumberStyles.Any)) / 180.0f);


                        }
                        else if (childNode.Name == "scale")
                        {
                            string[] scaleParams = childNode.InnerText.Trim().Split(' ');
                            m = Matrix.Scaling(new Vector3(
                                    Decimal.ToSingle(Decimal.Parse(
                                        scaleParams[0],
                                        System.Globalization.NumberStyles.Any)),
                                    Decimal.ToSingle(Decimal.Parse(
                                        scaleParams[1],
                                        System.Globalization.NumberStyles.Any)),
                                    Decimal.ToSingle(Decimal.Parse(
                                        scaleParams[2],
                                        System.Globalization.NumberStyles.Any))));
                        }
                        else
                        {
                            continue;
                        }
                        transform = Matrix.Multiply(m, transform);
                    }

                }



                string sVertSource = meshNode.SelectNodes(
                        "c:vertices/c:input[@semantic='POSITION']/@source",
                        xmlnsManager)[0].Value;
                int iVertCount = Decimal.ToInt32(Decimal.Parse(
                        meshNode.SelectNodes(
                                "c:source[@id='" +
                                sVertSource.Substring(1) +
                                "']/c:float_array/@count", xmlnsManager)[0].Value)) / 3;
                string[] vertCoords = meshNode.SelectNodes(
                        "c:source[@id='" +
                        sVertSource.Substring(1) +
                        "']/c:float_array", xmlnsManager)[0].InnerText.Trim().Split(' ');
                CustomVertex.PositionNormalTextured[] vertices =
                        new CustomVertex.PositionNormalTextured[iVertCount];
                Vector3 v = new Vector3();
                for (int i = 0; i < iVertCount; i++)
                {
                    v.X = Decimal.ToSingle(Decimal.Parse(vertCoords[i * 3 + 0],
                                System.Globalization.NumberStyles.Any));
                    v.Y = Decimal.ToSingle(Decimal.Parse(vertCoords[i * 3 + 1],
                                System.Globalization.NumberStyles.Any));
                    v.Z = Decimal.ToSingle(Decimal.Parse(vertCoords[i * 3 + 2],
                                System.Globalization.NumberStyles.Any));
                    v.TransformCoordinate(transform);

                    vertices[i] = new CustomVertex.PositionNormalTextured(
                            v.X, v.Y, v.Z, 0.0f, 0.0f, 0.0f, v.X, v.Y);
                }
                int iFaceCount = Decimal.ToInt32(Decimal.Parse(
                            meshNode.SelectNodes(
                                    "c:triangles/@count",
                                xmlnsManager)[0].Value));

                string[] triVertIndicesStr = meshNode.SelectNodes(
                        "c:triangles/c:p",
                    xmlnsManager)[0].InnerText.Trim().Split(' '); ;
                short[] triVertIndices = new short[triVertIndicesStr.Length];
                for (int i = 0; i < triVertIndicesStr.Length; i++)
                {
                    triVertIndices[i] = Decimal.ToInt16(Decimal.Parse(triVertIndicesStr[i]));
                }

                this.m_meshElems[j] = new MeshElem();
                this.m_meshElems[j].mesh = new Mesh(iFaceCount, iVertCount, 0, CustomVertex.PositionNormalTextured.Format, drawArgs.device);
                this.m_meshElems[j].mesh.SetVertexBufferData(vertices, LockFlags.None);
                this.m_meshElems[j].mesh.SetIndexBufferData(triVertIndices, LockFlags.None);

                int[] adjacency = new int[this.m_meshElems[j].mesh.NumberFaces * 3];
                this.m_meshElems[j].mesh.GenerateAdjacency(0.000000001F, adjacency);
                this.m_meshElems[j].mesh.OptimizeInPlace(MeshFlags.OptimizeVertexCache, adjacency);
                this.m_meshElems[j].mesh.ComputeNormals();

                int numSubSets = this.m_meshElems[j].mesh.GetAttributeTable().Length;
                this.m_meshElems[j].meshTextures = new Texture[numSubSets];
                this.m_meshElems[j].meshMaterials = new Material[numSubSets];

                for (int i = 0; i < numSubSets; i++)
                {
                    this.m_meshElems[j].meshMaterials[i].Ambient = Color.FromArgb(255, 255, 43, 48);
                    this.m_meshElems[j].meshMaterials[i].Diffuse = Color.FromArgb(255, 155, 113, 148);
                    this.m_meshElems[j].meshMaterials[i].Emissive = Color.FromArgb(255, 255, 143, 98);
                    this.m_meshElems[j].meshMaterials[i].Specular = Color.FromArgb(255, 155, 243, 48);
                }
            }

        }

        /// <summary>
        /// RenderableObject abstract member (needed)
        /// OBS: Worker thread (d
        /// on't update UI directly from this thread)
        /// </summary>
        public override void Update(DrawArgs drawArgs)
        {
            if (!this.isInitialized) this.Initialize(drawArgs);

            if ( (this.vertExaggeration != World.Settings.VerticalExaggeration) && (this.IsVertExaggerable == true) ||
                 (drawArgs.WorldCamera.ViewMatrix != this.m_lastView))
            {
                this.currentElevation = 0;

                if (this.m_isElevationRelativeToGround && drawArgs.CurrentWorld.TerrainAccessor != null && drawArgs.WorldCamera.Altitude < 300000 )
                {
                    double samplesPerDegree = 50.0 / drawArgs.WorldCamera.ViewRange.Degrees;
                    this.currentElevation += this.World.TerrainAccessor.GetElevationAt(this.Latitude, this.Longitude, samplesPerDegree);
                }

                this.m_lastView = drawArgs.WorldCamera.ViewMatrix;
                this.vertExaggeration = World.Settings.VerticalExaggeration;
            }
        }

        bool isUpdating;
        private void refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.isUpdating)
                return;
            // refresh object position, check that not offline 
            if (this.m_refreshurl != null && !World.Settings.WorkOffline)
            {
                WebDownload dl = new WebDownload(this.m_refreshurl);
                dl.SavedFilePath = this.meshFileName.Replace(".x", ".txt");
                dl.CompleteCallback += new DownloadCompleteHandler(this.positionupdateComplete);
                dl.BackgroundDownloadFile();
            }

            this.isUpdating = true;
        }

        private void positionupdateComplete(WebDownload wd)
        {
            //TODO: Update Object Position
            try
            {
                wd.Verify();
                StreamReader reader = File.OpenText(wd.SavedFilePath);
                string[] location = reader.ReadLine().Split(new char[] { ',' });
                this.Latitude = Convert.ToSingle(location[0]);
                this.Longitude = Convert.ToSingle(location[1]);
                this.Altitude = Convert.ToSingle(location[2]);
            }
            finally
            {
                this.isUpdating = false;
            }
        }

        /// <summary>
        /// RenderableObject abstract member (needed)
        /// OBS: Worker thread (don't update UI directly from this thread)
        /// </summary>
        public override void Dispose()
        {
            this.isInitialized = false;

            if (this.m_meshTableElem != null)
            {
                this.m_meshTableElem.referenceCount--;

                if (this.m_meshTableElem.referenceCount <= 0)
                {
                    m_meshTable.Remove(this.m_meshTableElem.meshFilePath);

                    // remove mesh elements
                    if (this.m_meshElems != null)
                    {
                        foreach (MeshElem me in this.m_meshElems)
                        {
                            if (me.mesh != null)
                                me.mesh.Dispose();
                            if (me.meshTextures != null)
                                foreach (Texture t in me.meshTextures)
                                    if (t != null)
                                        t.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets called when user left clicks.
        /// RenderableObject abstract member (needed)
        /// Called from UI thread = UI code safe in this function
        /// </summary>
        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            // Note: The following assignment to m is
            // to match the assignment to 
            // drawArgs.device.Transform.World in Render().
            // Changes to that matrix should be mirrored here
            // until the assignment is centralized.

            Matrix m = Matrix.RotationX((float)MathEngine.DegreesToRadians(this.RotX));
            m *= Matrix.RotationY((float)MathEngine.DegreesToRadians(this.RotY));
            m *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(this.RotZ));
            m *= Matrix.Scaling(this.Scale, this.Scale, this.Scale);

            if (this.IsVertExaggerable == true)
                this.vertExaggeration = World.Settings.VerticalExaggeration;
            else
                this.vertExaggeration = 1;
            m *= Matrix.Translation(0, 0, (float)(drawArgs.WorldCamera.WorldRadius + ((this.currentElevation + this.Altitude) * this.vertExaggeration)));
            m *= Matrix.RotationY((float)MathEngine.DegreesToRadians(90 - this.Latitude));
            m *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(this.Longitude));

            m *= Matrix.Translation(
                (float)-drawArgs.WorldCamera.ReferenceCenter.X,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Z
                );

            m = Matrix.Invert(m);

            Vector3 v1 = new Vector3();
            v1.X = DrawArgs.LastMousePosition.X;
            v1.Y = DrawArgs.LastMousePosition.Y;
            v1.Z = drawArgs.WorldCamera.Viewport.MinZ;
            v1.Unproject(drawArgs.WorldCamera.Viewport,
                    drawArgs.WorldCamera.ProjectionMatrix,
                    drawArgs.WorldCamera.ViewMatrix,
                    drawArgs.WorldCamera.WorldMatrix);

            Vector3 v2 = new Vector3();
            v2.X = DrawArgs.LastMousePosition.X;
            v2.Y = DrawArgs.LastMousePosition.Y;
            v2.Z = drawArgs.WorldCamera.Viewport.MaxZ;
            v2.Unproject(drawArgs.WorldCamera.Viewport,
                    drawArgs.WorldCamera.ProjectionMatrix,
                    drawArgs.WorldCamera.ViewMatrix,
                    drawArgs.WorldCamera.WorldMatrix);

            v1.TransformCoordinate(m);
            v2.TransformCoordinate(m);

            bool sel = false;
            foreach (MeshElem me in this.m_meshElems)
            {
                sel |= (me.mesh.Intersect(v1, v2 - v1));
            }
            if (sel)
            {
            }

            return sel;
        }

    }
}
