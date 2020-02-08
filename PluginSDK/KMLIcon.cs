//
// Copyright © 2005 NASA.  Available under the NOSA License
//
// Portions copied from Icon - Copyright © 2005-2006 The Johns Hopkins University 
// Applied Physics Laboratory.  Available under the JHU/APL Open Source Agreement.
//

using System.Drawing;

namespace WorldWind
{
	/// <summary>
	/// One icon in an icon layer
	/// </summary>
	public class KMLIcon : Icon
    {
        public new string Description
        {
            get
            {
                return this.m_description;
            }
            set
            {
                this.isSelectable = value != null;
                this.m_description = value;
            }
        }

        public string NormalIcon;

        public bool HasBeenUpdated = true;

        public bool IsDescriptionVisible;

        public KMLDialog DescriptionBubble;

        /// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Icon"/> class  
        /// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
        /// <param name="normalicon"></param>
        /// <param name="heightAboveSurface">Altitude</param>
        public KMLIcon(string name, double latitude, double longitude, string normalicon, double heightAboveSurface)
            : base(name, latitude, longitude, heightAboveSurface)
		{
            this.NormalIcon = normalicon;
            this.AutoScaleIcon = true;
            this.Declutter = true;
            this.IsAGL = false;
		}

		#region Overrriden methods

        /// <summary>
        /// Disposes the icon (when disabled)
        /// </summary>
        public override void Dispose()
        {
            // Nothing to dispose
            // Ashish Datta - make sure the description bubble is destroyed and not visisble.
            try
            {
                if (this.DescriptionBubble != null)
                    this.DescriptionBubble.Dispose();
            }
            finally
            {
                base.Dispose();
            }
        }

        protected override bool PerformLMBAction(DrawArgs drawArgs)
        {
            if (this.DescriptionBubble != null)
            {
                this.IsDescriptionVisible = false;
                this.DescriptionBubble.isVisible = false;
                this.DescriptionBubble.Dispose();
            }

            this.DescriptionBubble = new KMLDialog();
            this.DescriptionBubble.Owner = (System.Windows.Forms.Form)drawArgs.parentControl.Parent.Parent.Parent;

            if (this.IsDescriptionVisible == false)
            {
                this.IsDescriptionVisible = true;
            }
            else
            {
                this.DescriptionBubble.Dispose();
                this.IsDescriptionVisible = false;
            }

            return true;
        }

        protected override void RenderDescription(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color)
        {
            // Ashish Datta - Show/Hide the description bubble.
            if (this.IsDescriptionVisible)
            {
                if (this.DescriptionBubble.isVisible == true)
                {
                    this.DescriptionBubble.Location = new Point((int)projectedPoint.X + (this.Width / 4), (int)projectedPoint.Y);
                    this.DescriptionBubble.Show();

                    if (this.DescriptionBubble.HTMLIsSet == false) this.DescriptionBubble.SetHTML(this.Description + "<br>URL: <A HREF=" + this.ClickableActionURL + ">" + this.ClickableActionURL + "</A>");

                    this.DescriptionBubble.BringToFront();
                }
            }
        }

        public override void NoRender(DrawArgs drawArgs)
        {
            base.NoRender(drawArgs);

            if (this.IsDescriptionVisible)
            {
                this.DescriptionBubble.Hide();
                this.IsDescriptionVisible = false;
            }
        }

		#endregion
	}
}
