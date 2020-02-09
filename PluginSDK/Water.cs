/********************************************************************
 * Ocean/Water Rendering code obtained from MDXInfo Site
 * Code added by Tisham Dhar(aka what_nick) in hope of later inspiration
 * As this code matures specific water bodies can be added with given 
 * water level or global water data set constructed using SRTM water
 * boundaries data.
 * 
 * 
 * ******************************************************************/


using System;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

namespace WorldWind
{
	class Water : ModelFeature
	{
		#region Members
		private const string meshFilename = "Data/water.x";
		private const string textureFilename = "Data/water.dds";
        //The cloud file is missing
        private const string cloudFilename = "Data/cloud.dds";
		private string effectFilename = "Shaders/waterbump.fx";
		private CubeTexture texCube;
		private static float waterTime;
        protected Effect effect;
        //protected Mesh mesh;
        protected Texture texture;
        private bool isBumpmapped;
		#endregion

		#region Methods
		public Water(string name,World parentWorld,bool isBumpmapped,float lat,float lon,float alt,float scaleFactor) : 
            base(name,parentWorld,meshFilename,lat,lon,alt,scaleFactor,90.0f,0.0f,0.0f)
		{
            this.isBumpmapped = isBumpmapped;
            if (!isBumpmapped) this.effectFilename = @"Shaders/water.fx";
		}

        public override void Initialize(DrawArgs drawArgs)
        {
            base.Initialize(drawArgs);
            Device device = drawArgs.device;
            //load the mesh

            //Vertex elements that describe the new format of the mesh
            VertexElement[] elements = new VertexElement[]
			{
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
				new VertexElement(0, 24, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				new VertexElement(0, 36, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Tangent, 0),
				new VertexElement(0, 48, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Binormal, 0),
				VertexElement.VertexDeclarationEnd,
			};
            VertexDeclaration decl = new VertexDeclaration(device, elements);
            Mesh tempMesh = this.m_meshElems[0].mesh.Clone(MeshFlags.Managed, elements, device);
            this.m_meshElems[0].mesh.Dispose();
            this.m_meshElems[0].mesh = tempMesh;

            /* Performs tangent frame computations on a mesh. Tangent, binormal, and optionally normal vectors are generated. 
              * Singularities are handled as required by grouping edges and splitting vertices.
             */
            this.m_meshElems[0].mesh.ComputeTangentFrame(0);

            //load the effect
            this.effect = Effect.FromFile(device, this.effectFilename, null, null, ShaderFlags.Debug | ShaderFlags.PartialPrecision, null);

            //load the texture
            this.texCube = TextureLoader.FromCubeFile(device, cloudFilename);
            this.texture = TextureLoader.FromFile(device, textureFilename);
            this.isInitialized = true;
        }
        /*
        public override void Update(DrawArgs drawArgs)
        {
            if (!isInitialized)
                this.Initialize(drawArgs);
        }
         */ 
        /*
		public void Update(Matrix matView, Matrix matProj)
		{
            /*
			matWorld.Translate(0.0f, -80.0f, 0.0f);
			this.matView = matView;
			this.matProj = matProj;
            
		}
        */

        new protected bool IsVisible(CameraBase camera)
        {
            if(base.IsVisible(camera))
                //donot render at high altitudes
                if (camera.Altitude > 60000)
                    return false;
            return true;
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
                if (this.isInitialized) this.Dispose();
                return;
            }

            if (!this.isInitialized)
                return;
            
            //Donot render for other planets
            if (!drawArgs.CurrentWorld.IsEarth)
                return;


            
            drawArgs.device.SetRenderState(RenderState.CullMode, Cull.None);
            drawArgs.device.SetRenderState(RenderState.Lighting, true);
            drawArgs.device.SetRenderState(RenderState.Ambient, 0x808080);
            drawArgs.device.SetRenderState(RenderState.NormalizeNormals, true);

            Light lLight = new Light();
            lLight.Ambient = Color.FromArgb(255, 255, 255);
            lLight.Type = LightType.Directional;
            lLight.Direction = new Vector3(1f, 1f, 1f);
            // Put the light somewhere up in space
            lLight.Position = new Vector3((float) this.worldXyz.X * 2f, (float) this.worldXyz.Y * 1f, (float) this.worldXyz.Z * 1.5f);
            drawArgs.device.SetLight(0, ref lLight);
            
            drawArgs.device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);
            drawArgs.device.SetSamplerState(0, SamplerState.AddressV,TextureAddress.Wrap);

            drawArgs.device.SetRenderState(RenderState.AlphaBlendEnable, true);
            drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);

            
            
            Matrix currentWorld = drawArgs.device.GetTransform(TransformState.World);
            
            drawArgs.device.SetTransform(TransformState.World, Matrix.RotationX((float)MathEngine.DegreesToRadians(this.RotX)));
            drawArgs.device.SetTransform(TransformState.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(this.RotY));
            drawArgs.device.SetTransform(TransformState.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(this.RotZ));
            drawArgs.device.SetTransform(TransformState.World *= Matrix.Scaling(this.Scale, this.Scale, this.Scale);

            // Move the mesh to desired location on earth
            if (this.IsVertExaggerable)
                this.vertExaggeration = World.Settings.VerticalExaggeration;
            else
                this.vertExaggeration = 1;
            drawArgs.device.SetTransform(TransformState.World *= Matrix.Translation(0, 0, (float)drawArgs.WorldCamera.WorldRadius + this.Altitude * this.vertExaggeration);
            drawArgs.device.SetTransform(TransformState.World *= Matrix.RotationY((float)MathEngine.DegreesToRadians(90 - this.Latitude));
            drawArgs.device.SetTransform(TransformState.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(this.Longitude));


            drawArgs.device.SetTransform(TransformState.World *= Matrix.Translation(
                (float)-drawArgs.WorldCamera.ReferenceCenter.X,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Z
                );

            Device device = drawArgs.device;
            // Draw the mesh with effect
            if (this.isBumpmapped)
                this.setupBumpEffect(drawArgs);
            else
                this.setupReflectionEffect(drawArgs);

			//render the effect
            bool alphastate = device.GetRenderState<bool>(RenderState.AlphaBlendEnable);
            device.SetRenderState(RenderState.AlphaBlendEnable , true);
            this.effect.Begin(0);
            this.effect.BeginPass(0);
             
            //drawArgs.device.SetTexture(0, texture);
            this.m_meshElems[0].mesh.DrawSubset(0);

            this.effect.EndPass();
            this.effect.End();
			device.SetRenderState(RenderState.AlphaBlendEnable , alphastate);
            

            drawArgs.device.SetTransform(TransformState.World, currentWorld;
            drawArgs.device.SetRenderState(RenderState.Lighting , false);
		}

        private void setupBumpEffect(DrawArgs drawArgs)
        {
            float time = (float)(Environment.TickCount * 0.001);
            waterTime += 0.002f;
            //Calculate the matrices
            Matrix modelViewProj = drawArgs.device.SetTransform(TransformState.World * drawArgs.device.SetTransform(TransformState.View * drawArgs.device.SetTransform(TransformState.Projection;
            Matrix modelViewIT = drawArgs.device.SetTransform(TransformState.World * drawArgs.device.SetTransform(TransformState.View * drawArgs.device.SetTransform(TransformState.Projection;
            modelViewIT.Invert();
            modelViewIT = Matrix.TransposeMatrix(modelViewIT);
            //set the technique
            this.effect.Technique = "water";
            //set the texturs
            this.effect.SetValue("texture0", this.texture);
            this.effect.SetValue("texture1", this.texCube);
            //set the matrices
            this.effect.SetValue("ModelViewProj", modelViewProj);
            this.effect.SetValue("ModelWorld", drawArgs.device.GetTransform(TransformState.World));
            //set eye position
            this.effect.SetValue("eyePos", new Vector4(450.0f, 250.0f, 750.0f, 1.0f));
            //set the light position
            this.effect.SetValue("lightPos", new Vector4((float)(300 * Math.Sin(time)), 40.0f, (float)(300 * Math.Cos(time)), 1.0f));
            //set the time
            this.effect.SetValue("time", waterTime);

        }

        private void setupReflectionEffect(DrawArgs drawArgs)
        {
            //Calculate the matrices
            Matrix worldViewProj = drawArgs.device.SetTransform(TransformState.World * drawArgs.device.GetTransform(TransformState.View) * drawArgs.device.GetTransform(TransformState.Projection));
            Matrix worldIT = drawArgs.device.GetTransform(TransformState.World);
            worldIT.Invert();
            worldIT = Matrix.TransposeMatrix(worldIT);
            Matrix viewI = drawArgs.device.SetTransform(TransformState.View;
            viewI.Invert();
            //Point3d sunPosition = SunCalculator.GetGeocentricPosition(currentTime);

            //set the technique
            this.effect.Technique = "Textured";
            //set the texturs
            this.effect.SetValue("ColorTexture", this.texture);
            this.effect.SetValue("CubeEnvMap", this.texCube);
            //set the matrices
            this.effect.SetValue("WorldViewProj", worldViewProj);
            this.effect.SetValue("WorldIT", worldIT);
            this.effect.SetValue("World", drawArgs.device.GetTransform(TransformState.World);
            this.effect.SetValue("ViewI", viewI);
        }
        /*
		public void Render(Device device, Texture waterTexture)
		{
			float time = (float)(Environment.TickCount * 0.001);
			waterTime += 0.002f;
			//Calculate the matrices
			Matrix modelViewProj = matWorld * matView * matProj;
			Matrix modelViewIT = matWorld * matView * matProj;
			modelViewIT.Invert();
			modelViewIT = Matrix.TransposeMatrix(modelViewIT);
			//set the technique
			effect.Technique = "water";
			//set the texturs
			effect.SetValue("texture0", waterTexture);
			effect.SetValue("texture1", texCube);
			//set the matrices
			effect.SetValue("ModelViewProj", modelViewProj);
			effect.SetValue("ModelWorld", matWorld);
			//set eye position
			effect.SetValue("eyePos", new Vector4(450.0f, 250.0f, 750.0f, 1.0f));
			//set the light position
			effect.SetValue("lightPos", new Vector4((float)(300 * Math.Sin(time)), 40.0f, (float)(300 * Math.Cos(time)), 1.0f));
			//set the time
			effect.SetValue("time", waterTime);

			//render the effect
			//device.SetRenderState(RenderState.AlphaBlendEnable , true);
			effect.Begin(0);
			effect.BeginPass(0);
			mesh.DrawSubset(0);
			effect.EndPass();
			effect.End();
			//device.SetRenderState(RenderState.AlphaBlendEnable , false);
		}
        */
        public override void Dispose()
        {
            //set initialization state to false
            this.isInitialized = false;
            
            //Dispose Textures
            if (this.texCube != null)
            {
                this.texCube.Dispose();
                this.texCube = null;
            }
            if (this.texture != null)
            {
                this.texture.Dispose();
                this.texture = null;
            }
            //Dispose Effects
            if (this.effect != null)
            {
                this.effect.Dispose();
                this.effect = null;
            }
            //Dispose Mesh
            base.Dispose();
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }

		#endregion
	}
}
