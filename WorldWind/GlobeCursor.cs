//----------------------------------------------------------------------------
// NAME: Globe cursor
// VERSION: 1.0
// DESCRIPTION: Displays a floating cursor on the globe showing where a mouse click will land (and illustrating any raytracing error that may be present)
// DEVELOPER: Erik Newman
//----------------------------------------------------------------------------
//
// This file is in the Public Domain, and comes with no warranty. 
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using WorldWind;
using WorldWind.Renderable;
using WorldWind.PluginEngine;


namespace Withak.Plugins
{
	public class GlobeCursorPlugin : WorldWind.PluginEngine.Plugin
	{

		KMLIcon ic;
		Icons ics;
		Bitmap cursBmp = new Bitmap("Plugins\\cursorIcon.png");

		public override void Load()
		{

			ics = new Icons("Globe cursor");

			ic = new KMLIcon("", 0f, 0f, "", 0f);
			ic.Image = cursBmp;
			ic.Width = 16;
			ic.Height = 16;
			
			ics.Add(ic);
			
			ParentApplication.WorldWindow.CurrentWorld.RenderableObjects.Add(ics);

			ParentApplication.WorldWindow.MouseMove += new MouseEventHandler(MouseMove);

			base.Load();

		}

		public override void Unload()
		{
			ParentApplication.WorldWindow.CurrentWorld.RenderableObjects.Remove(ics);
			ParentApplication.WorldWindow.MouseMove -= new MouseEventHandler(MouseMove);


			base.Unload();
		}

		public void MouseMove(object sender, MouseEventArgs e)
		{
			Angle lat,lon = Angle.NaN;
			ParentApplication.WorldWindow.DrawArgs.WorldCamera.PickingRayIntersection(
				e.X,e.Y,out lat, out lon);
			ic.SetPosition((float)lat.Degrees, (float)lon.Degrees, 0f);
		}
		
	}
}
