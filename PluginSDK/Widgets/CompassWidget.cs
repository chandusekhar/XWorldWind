//========================= (UNCLASSIFIED) ==============================
// Copyright © 2005-2006 The Johns Hopkins University /
// Applied Physics Laboratory.  All rights reserved.
//
// WorldWind Source Code - Copyright 2005 NASA World Wind 
// Modified under the NOSA License
//
//========================= (UNCLASSIFIED) ==============================
//
// LICENSE AND DISCLAIMER 
//
// Copyright (c) 2006 The Johns Hopkins University. 
//
// This software was developed at The Johns Hopkins University/Applied 
// Physics Laboratory (“JHU/APL”) that is the author thereof under the 
// “work made for hire” provisions of the copyright law.  Permission is 
// hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation (the “Software”), to use the 
// Software without restriction, including without limitation the rights 
// to copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit others to do so, subject to the 
// following conditions: 
//
// 1.  This LICENSE AND DISCLAIMER, including the copyright notice, shall 
//     be included in all copies of the Software, including copies of 
//     substantial portions of the Software; 
//
// 2.  JHU/APL assumes no obligation to provide support of any kind with 
//     regard to the Software.  This includes no obligation to provide 
//     assistance in using the Software nor to provide updated versions of 
//     the Software; and 
//
// 3.  THE SOFTWARE AND ITS DOCUMENTATION ARE PROVIDED AS IS AND WITHOUT 
//     ANY EXPRESS OR IMPLIED WARRANTIES WHATSOEVER.  ALL WARRANTIES 
//     INCLUDING, BUT NOT LIMITED TO, PERFORMANCE, MERCHANTABILITY, FITNESS
//     FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT ARE HEREBY DISCLAIMED.  
//     USERS ASSUME THE ENTIRE RISK AND LIABILITY OF USING THE SOFTWARE.  
//     USERS ARE ADVISED TO TEST THE SOFTWARE THOROUGHLY BEFORE RELYING ON 
//     IT.  IN NO EVENT SHALL THE JOHNS HOPKINS UNIVERSITY BE LIABLE FOR 
//     ANY DAMAGES WHATSOEVER, INCLUDING, WITHOUT LIMITATION, ANY LOST 
//     PROFITS, LOST SAVINGS OR OTHER INCIDENTAL OR CONSEQUENTIAL DAMAGES, 
//     ARISING OUT OF THE USE OR INABILITY TO USE THE SOFTWARE. 
//

using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WorldWind.Widgets
{
	/// <summary>
	/// CompassWidget. This is a specialized widget that displays
	/// a compass image based on the direction of the camera.
	/// 
	/// Inspired by CompassRose written by Patrick Murris.
	/// http://www.worldwindcentral.com/wiki/Add-on:Compass_Rose
	/// </summary>
	public class CompassWidget : IWidget
	{
		#region IWidget support variables

		/// <summary>
		/// Name property value
		/// </summary>
		protected string m_name = "";

		/// <summary>
		/// Location property value
		/// </summary>
		protected Point m_location = new Point(0,0);

		/// <summary>
		/// WidgetSize property value
		/// </summary>
		protected Size m_size = new Size(0,0);

		/// <summary>
		/// Visible property value
		/// </summary>
		protected bool m_visible = true;

		/// <summary>
		/// Enabled property value
		/// </summary>
		protected bool m_enabled = true;

		/// <summary>
		/// CountHeight property value
		/// </summary>
		protected bool m_countHeight = true;

		/// <summary>
		/// CountWidth property value
		/// </summary>
		protected bool m_countWidth;

		/// <summary>
		/// Parent widget property value
		/// </summary>
		protected IWidget m_parentWidget;

		/// <summary>
		/// Tag property
		/// </summary>
		protected object m_tag;

		/// <summary>
		/// Flag indicating if initialization is required
		/// </summary>
		protected bool m_isInitialized;

		#endregion

		/// <summary>
		/// Sprite used to draw texture and text
		/// </summary>
		protected Sprite m_sprite;

		/// <summary>
		/// The texture for this widget
		/// </summary>
		protected IconTexture m_iconTexture;

		/// <summary>
		/// Name of texture image
		/// </summary>
		protected string m_imageName = "compass.png";

		/// <summary>
		/// Normal color (not highlighted/mouseover etc.)
		/// </summary>
		protected static int m_normalColor = Color.White.ToArgb();

		/// <summary>
		///  Icon X scaling computed by dividing icon width by texture width
		/// </summary>
		protected float XScale;
 
		/// <summary>
		///  Icon Y scaling computed by dividing icon height by texture height 
		/// </summary>
		protected float YScale;

		/// <summary>
		/// Default Constructor
		/// </summary>
		public CompassWidget()
		{
            this.m_location.X = 5;
            this.m_location.Y = 100;
            this.m_size.Height = 79;
            this.m_size.Width = 79;
            this.m_isInitialized = false;
		}

		#region Properties

		/// <summary>
		/// Filename of button graphic
		/// </summary>
		public string ImageName
		{
			get { return this.m_imageName; }
			set 
			{
                this.m_imageName = value;
                this.m_isInitialized = false;
			}
		}

		#endregion

		#region IWidget Members

		#region Properties

		/// <summary>
		/// Name of this widget
		/// </summary>
		public string Name
		{
			get { return this.m_name; }
			set { this.m_name = value; }
		}


		/// <summary>
		/// Location of this widget relative to the client area of the parent
		/// </summary>
		public Point Location
		{
			get { return this.m_location; }
			set { this.m_location = value; }
		}


		/// <summary>
		/// Where this widget is on the window
		/// </summary>
		public Point AbsoluteLocation
		{
			get
			{
				if(this.m_parentWidget != null)
				{
					return new Point(this.m_location.X + this.m_parentWidget.ClientLocation.X, this.m_location.Y + this.m_parentWidget.ClientLocation.Y);
				}
				else
				{
					return this.m_location;
				}
			}
		}


		/// <summary>
		/// The top left corner of this widget's client area
		/// </summary>
		public Point ClientLocation
		{
			get { return this.AbsoluteLocation; }
		}


		/// <summary>
		/// Size of widget in pixels
		/// </summary>
		public Size WidgetSize
		{
			get { return this.m_size; }
			set { this.m_size = value; }
		}


		/// <summary>
		/// Size of the client area in pixels - Same as widget size.
		/// </summary>
		public Size ClientSize
		{
			get { return this.m_size; }
			set { this.m_size = value; }
		}


		/// <summary>
		/// Whether this widget is enabled
		/// </summary>
		public bool Enabled
		{
			get { return this.m_enabled; }
			set { this.m_enabled = value; }
		}


		/// <summary>
		/// Whether this widget is visible
		/// </summary>
		public bool Visible
		{
			get { return this.m_visible; }
			set { this.m_visible = value; }
		}


		/// <summary>
		/// Whether this widget should count for height calculations - HACK until we do real layout
		/// </summary>
		public bool CountHeight
		{
			get { return this.m_countHeight; }
			set { this.m_countHeight = value; }
		}


		/// <summary>
		/// Whether this widget should count for width calculations - HACK until we do real layout
		/// </summary>
		public bool CountWidth		
		{
			get { return this.m_countWidth; }
			set { this.m_countWidth = value; }
		}


		/// <summary>
		/// The parent widget of this widget.
		/// </summary>
		public IWidget ParentWidget
		{
			get { return this.m_parentWidget; }
			set { this.m_parentWidget = value; }
		}


		/// <summary>
		/// List of children widgets - None in the case of button widgets.
		/// </summary>
		public IWidgetCollection ChildWidgets
		{
			get { return null; }
			set { }
		}


		/// <summary>
		/// A link to an object.
		/// </summary>
		public object Tag
		{
			get { return this.m_tag; }
			set { this.m_tag = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Initializes the button by loading the texture, creating the sprite and figure out the scaling.
		/// 
		/// Called on the GUI thread.
		/// </summary>
		/// <param name="drawArgs">The drawing arguments passed from the WW GUI thread.</param>
		public void Initialize (DrawArgs drawArgs)
		{
			object key = null;

            this.m_imageName = Path.GetDirectoryName(Application.ExecutablePath)+ @"\Plugins\Navigator\compass.png";

			// Icon image from file
            this.m_iconTexture = (IconTexture)DrawArgs.Textures[this.m_imageName];
			if(this.m_iconTexture==null)
			{
				key = this.m_imageName;
                this.m_iconTexture = new IconTexture( drawArgs.device, this.m_imageName );
			}

			if(this.m_iconTexture!=null)
			{
                this.m_iconTexture.ReferenceCount++;

				if(key!=null)
				{
					// New texture, cache it
					DrawArgs.Textures.Add(key, this.m_iconTexture);
				}

				if (this.m_size.Width == 0) this.m_size.Width = this.m_iconTexture.Width;

				if (this.m_size.Height == 0) this.m_size.Height = this.m_iconTexture.Height;

				this.XScale = (float) this.m_size.Width / this.m_iconTexture.Width;
				this.YScale = (float) this.m_size.Height / this.m_iconTexture.Height;
			}

			if (this.m_sprite == null) this.m_sprite = new Sprite(drawArgs.device);

            this.m_isInitialized = true;
		}


		/// <summary>
		/// The render method to draw this widget on the screen.
		/// 
		/// Called on the GUI thread.
		/// </summary>
		/// <param name="drawArgs">The drawing arguments passed from the WW GUI thread.</param>
		public void Render(DrawArgs drawArgs)
		{

			if (!this.m_isInitialized)
				this.Initialize(drawArgs);

			if ((this.m_visible) && (this.m_enabled))
			{
                this.m_sprite.Begin(SpriteFlags.AlphaBlend);
                this.m_sprite.Transform = Matrix.Scaling(this.XScale,this.YScale,0);
                this.m_sprite.Transform *= Matrix.RotationZ((float)drawArgs.WorldCamera.Heading.Radians*-1);
                this.m_sprite.Transform *= Matrix.Translation(this.AbsoluteLocation.X+ this.m_size.Width/2, this.AbsoluteLocation.Y+ this.m_size.Height/2, 0);
                this.m_sprite.Draw(this.m_iconTexture.Texture,
					new Vector3(this.m_iconTexture.Width>>1, this.m_iconTexture.Height>>1,0),
					Vector3.Zero,
					m_normalColor );
                this.m_sprite.Transform = Matrix.Identity;
                this.m_sprite.End();
			}
				
		}

		#endregion

		#endregion
	}
}
