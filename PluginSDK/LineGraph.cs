using SharpDX.Direct3D9;

namespace WorldWind
{
	/// <summary>
	/// Summary description for LineGraph.
	/// </summary>
	public class LineGraph
	{
		float m_Min = 0f;
		float m_Max = 50.0f;

		float[] m_Values = new float[0];

		System.Drawing.Point m_Location = new System.Drawing.Point(100,100);
		System.Drawing.Size m_Size = new System.Drawing.Size(300,100);
		System.Drawing.Color m_BackgroundColor = System.Drawing.Color.FromArgb(100, 0, 0, 0);
		System.Drawing.Color m_LineColor = System.Drawing.Color.Red;

		bool m_Visible;
		bool m_ResetVerts = true;

		public bool Visible
		{
			get { return this.m_Visible; }
			set { this.m_Visible = value; }
		}

		public float[] Values
		{
			get
			{
				return this.m_Values;
			}
			set
			{
                this.m_Values = value;
                this.m_ResetVerts = true;
			}
		}

		public System.Drawing.Color BackgroundColor
		{
			get
			{
				return this.m_BackgroundColor;
			}
			set
			{
                this.m_BackgroundColor = value;
			}
		}

		public System.Drawing.Color LineColor
		{
			get
			{
				return this.m_LineColor;
			}
			set
			{
                this.m_LineColor = value;
                this.m_ResetVerts = true;
			}
		}

		public System.Drawing.Point Location
		{
			get{ return this.m_Location; }
			set
			{
				if(this.m_Location != value)
				{
                    this.m_Location = value;
                    this.m_ResetVerts = true;
				}
			 }
		}

		public System.Drawing.Size Size
		{
			get{ return this.m_Size; }
			set
			{
				if(this.m_Size != value)
				{
                    this.m_Size = value;
                    this.m_ResetVerts = true;
				}
			}
		}

		public LineGraph()
		{
			
		}

		CustomVertex.TransformedColored[] m_Verts = new SharpDX.Direct3D9.CustomVertex.TransformedColored[0];

		public void Render(DrawArgs drawArgs)
		{
			if(!this.m_Visible)
				return;

			MenuUtils.DrawBox(this.m_Location.X, this.m_Location.Y, this.m_Size.Width, this.m_Size.Height,
				0.0f, this.m_BackgroundColor.ToArgb(),
				drawArgs.device);

			if(this.m_Values == null || this.m_Values.Length == 0)
				return;

			float xIncr = (float) this.m_Size.Width / (float) this.m_Values.Length;

            this.m_Verts = new CustomVertex.TransformedColored[this.m_Values.Length];

			if(this.m_ResetVerts)
			{
				for(int i = 0; i < this.m_Values.Length; i++)
				{
					if(this.m_Values[i] < this.m_Min)
					{
                        this.m_Verts[i].Y = this.m_Location.Y + this.m_Size.Height;
					}
					else if(this.m_Values[i] > this.m_Max)
					{
                        this.m_Verts[i].Y = this.m_Location.Y;
					}
					else
					{
						float p = (this.m_Values[i] - this.m_Min) / (this.m_Max - this.m_Min);
                        this.m_Verts[i].Y = this.m_Location.Y + this.m_Size.Height - (float) this.m_Size.Height * p;
					}

                    this.m_Verts[i].X = this.m_Location.X + i * xIncr;
                    this.m_Verts[i].Z = 0.0f;
                    this.m_Verts[i].Color = this.m_LineColor.ToArgb();
				}
			}

			drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);
			drawArgs.device.VertexFormat = CustomVertex.TransformedColored.Format;

			drawArgs.device.VertexFormat = CustomVertex.TransformedColored.Format;
			drawArgs.device.DrawUserPrimitives(
				PrimitiveType.LineStrip, this.m_Verts.Length - 1, this.m_Verts);

		}
	}
}
