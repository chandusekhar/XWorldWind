using System;
using SharpDX;
using SharpDX.Direct3D9;
using Utility;
using WorldWind.Extensions;
using Color = System.Drawing.Color;

namespace WorldWind
{
	/// <summary>
	/// Summary description for MeshLayer.
	/// </summary>
	public class MeshLayer : RenderableObject
	{
		Mesh mesh;
		string meshFilePath;
		ExtendedMaterial[] materials;
		Material[] meshMaterials;

		float lat;
		float lon;
		float layerRadius;
		float scaleFactor;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.MeshLayer"/> class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="latitude"></param>
		/// <param name="longitude"></param>
		/// <param name="layerRadius"></param>
		/// <param name="scaleFactor"></param>
		/// <param name="meshFilePath"></param>
		/// <param name="orientation"></param>
		public MeshLayer(string name, float latitude, float longitude, float layerRadius, float scaleFactor, string meshFilePath, Quaternion orientation) : base(name, MathEngine.SphericalToCartesian(latitude, longitude, layerRadius), orientation) 
		{
			this.meshFilePath = meshFilePath;
			this.lat = latitude;
			this.lon = longitude;
			this.layerRadius = layerRadius;
			this.scaleFactor = scaleFactor;
		}

		public override void Initialize(DrawArgs drawArgs)
		{
			try
			{
				GraphicsStream adj;
				this.mesh = Mesh.FromFile(this.meshFilePath, MeshFlags.Managed, drawArgs.device, out adj, out this.materials );
				this.meshMaterials = new Material[this.materials.Length];
				//using(StreamWriter sw = new StreamWriter("mat.txt", true, System.Text.Encoding.ASCII))
			{
				//sw.WriteLine(this.meshMaterials.Length.ToString());
				for(int i = 0; i < this.materials.Length; i++)
				{
					this.meshMaterials[i] = this.materials[i].Material3D;
					this.meshMaterials[i].Ambient = this.meshMaterials[i].Diffuse;
				}
			}

				//this.mesh.ComputeNormals();
			}
			catch(Exception caught)
			{
				Log.Write( caught );
			}
			this.isInitialized = true;
		}

		public override void Dispose()
		{
			// Don't dispose the mesh!
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

		public override void Update(DrawArgs drawArgs)
		{
			if(!this.isInitialized)
				this.Initialize(drawArgs);

		}

		public override void Render(DrawArgs drawArgs)
		{
//			Vector3 here = MathEngine.SphericalToCartesian(drawArgs.WorldCamera.Latitude, drawArgs.WorldCamera.Longitude, this.layerRadius);
			Matrix currentWorld = drawArgs.device.GetTransform(TransformState.World);
			drawArgs.device.SetRenderState(RenderState.Lighting , true);
			drawArgs.device.SetRenderState(RenderState.ZEnable , true);


            Light lLight = new Light
            {
                Diffuse = Color.White.ToRawColor4(),
                Type = LightType.Point,
                Range = 100000,
                Position = new Vector3(this.layerRadius, 0, 0)
            };
            drawArgs.device.SetLight(0, ref lLight);

			drawArgs.device.SetRenderState(RenderState.CullMode , Cull.None);
            Matrix lWorldTransform = Matrix.Identity;
            lWorldTransform = Matrix.Multiply(lWorldTransform, Matrix.Scaling(this.scaleFactor, this.scaleFactor, this.scaleFactor));
            lWorldTransform = Matrix.Multiply(lWorldTransform, Matrix.Translation(0,0,-this.layerRadius));
            lWorldTransform = Matrix.Multiply(lWorldTransform, Matrix.RotationY((float)MathEngine.DegreesToRadians(90-this.lat)));
            lWorldTransform = Matrix.Multiply(lWorldTransform, Matrix.RotationZ((float)MathEngine.DegreesToRadians(180+this.lon)));
	        drawArgs.device.SetTransform(TransformState.World, lWorldTransform);
			drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);
			drawArgs.device.SetRenderState(RenderState.NormalizeNormals , true);
		for( int i = 0; i < this.meshMaterials.Length; i++ )
			{
				drawArgs.device.Material = this.meshMaterials[i];
				this.mesh.DrawSubset(i);
			}
			
			drawArgs.device.SetTransform(TransformState.World, currentWorld);
			drawArgs.device.SetRenderState(RenderState.CullMode , Cull.Clockwise);
			drawArgs.device.SetRenderState(RenderState.Lighting , false);
		}
	}
}
