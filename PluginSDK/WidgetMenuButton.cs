using WorldWind.Menu;

namespace WorldWind
{
    public class WidgetMenuButton : MenuButton
    {
        IWidget m_widget;

        public WidgetMenuButton(
			string name,
			string iconFilePath,
			IWidget widget) : base(iconFilePath)
		{
			this.Description = name;
            this.m_widget = widget;
		}

        public override void Update(DrawArgs drawArgs)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override bool IsPushed()
        {
            return this.m_widget.Visible;
        }

        public override void SetPushed(bool isPushed)
        {
            this.m_widget.Visible = isPushed;
        }

        public override bool OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            return false;
        }

        public override void OnKeyDown(System.Windows.Forms.KeyEventArgs keyEvent)
        {
        }

        public override void OnKeyUp(System.Windows.Forms.KeyEventArgs keyEvent)
        {
        }

        public override bool OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            return false;
        }

        public override bool OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            return false;
        }

        public override bool OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            return false;
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (!this.m_widget.Visible)
            {
                this.SetPushed(false);
            }
        }
    }
}
