using System;
using System.Windows.Forms;
using System.Drawing;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace WorldWind.Menu
{
    public abstract class MenuButton : IMenu
    {
        #region Private Members

        private string _iconTexturePath;
        private Texture m_iconTexture;
        private Size _iconTextureSize;
        string _description;
        float curSize;
        static int white = Color.White.ToArgb();
        static int black = Color.Black.ToArgb();
        static int transparent = Color.FromArgb(140, 255, 255, 255).ToArgb();
        int alpha;
        const int alphaStep = 30;
        const float zoomSpeed = 1.2f;

        public static float NormalSize;
        public static float SelectedSize;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Menu.MenuButton"/> class.
        /// </summary>
        protected MenuButton()
        {
        }

        #region Properties
        public string Description
        {
            get
            {
                if (this._description == null)
                    return "N/A";
                else
                    return this._description;
            }
            set
            {
                this._description = value;
            }
        }

        public Texture IconTexture
        {
            get
            {
                return this.m_iconTexture;
            }
        }

        public Size IconTextureSize
        {
            get
            {
                return this._iconTextureSize;
            }
        }
        #endregion

        public MenuButton(string iconTexturePath)
        {
            this._iconTexturePath = iconTexturePath;
        }

        public abstract bool IsPushed();
        public abstract void SetPushed(bool isPushed);

        public void InitializeTexture(Device device)
        {
            try
            {
                this.m_iconTexture = ImageHelper.LoadIconTexture(this._iconTexturePath);

                using (Surface s = this.m_iconTexture.GetSurfaceLevel(0))
                {
                    SurfaceDescription desc = s.Description;
                    this._iconTextureSize = new Size(desc.Width, desc.Height);
                }
            }
            catch
            {
            }
        }

        public void RenderLabel(DrawArgs drawArgs, int x, int y, int buttonHeight, bool selected, MenuAnchor anchor)
        {
            if (selected)
            {
                if (buttonHeight == this.curSize)
                {
                    this.alpha += alphaStep;
                    if (this.alpha > 255) this.alpha = 255;
                }
            }
            else
            {
                this.alpha -= alphaStep;
                if (this.alpha < 0)
                {
                    this.alpha = 0;
                    return;
                }
            }

            int halfWidth = (int)(SelectedSize * 0.75);
            int label_x = x - halfWidth + 1;
            int label_y = (int)(y + SelectedSize) + 1;

            FontDrawFlags format = FontDrawFlags.NoClip | FontDrawFlags.Center | FontDrawFlags.WordBreak;

            if (anchor == MenuAnchor.Bottom)
            {
                format |= FontDrawFlags.Bottom;
                label_y = y - 202;
            }

            Rectangle rect = new Rectangle(
                label_x,
                label_y,
                (int)halfWidth * 2,
                200);

            if (rect.Right > drawArgs.screenWidth)
            {
                rect = Rectangle.FromLTRB(rect.Left, rect.Top, drawArgs.screenWidth, rect.Bottom);
            }

            drawArgs.toolbarFont.DrawText(
                null, this.Description,
                rect,
                format,
                black & 0xffffff + (this.alpha << 24));

            rect.Offset(2, 0);

            drawArgs.toolbarFont.DrawText(
                null, this.Description,
                rect,
                format,
                black & 0xffffff + (this.alpha << 24));

            rect.Offset(0, 2);

            drawArgs.toolbarFont.DrawText(
                null, this.Description,
                rect,
                format,
                black & 0xffffff + (this.alpha << 24));

            rect.Offset(-2, 0);

            drawArgs.toolbarFont.DrawText(
                null, this.Description,
                rect,
                format,
                black & 0xffffff + (this.alpha << 24));

            rect.Offset(1, -1);

            drawArgs.toolbarFont.DrawText(
                null, this.Description,
                rect,
                format,
                white & 0xffffff + (this.alpha << 24));
        }

        public float CurrentSize
        {
            get { return this.curSize; }
        }

        [Obsolete]
        public void RenderEnabledIcon(Sprite sprite, DrawArgs drawArgs, float centerX, float topY,
                                      bool selected)
        {
            this.RenderEnabledIcon(sprite, drawArgs, centerX, topY, selected, MenuAnchor.Top);
        }

        public void RenderEnabledIcon(Sprite sprite, DrawArgs drawArgs, float centerX, float topY,
            bool selected, MenuAnchor anchor)
        {
            float width = selected ? SelectedSize : width = NormalSize;

            this.RenderLabel(drawArgs, (int)centerX, (int)topY, (int)width, selected, anchor);

            int color = selected ? white : transparent;

            float centerY = topY + this.curSize * 0.5f;
            this.RenderIcon(sprite, (int)centerX, (int)centerY, (int) this.curSize, (int) this.curSize, color, this.m_iconTexture);

            if (this.curSize == 0) this.curSize = width;
            if (width > this.curSize)
            {
                this.curSize = (int)(this.curSize * zoomSpeed);
                if (width < this.curSize) this.curSize = width;
            }
            else if (width < this.curSize)
            {
                this.curSize = (int)(this.curSize / zoomSpeed);
                if (width > this.curSize) this.curSize = width;
            }
        }

        private void RenderIcon(Sprite sprite, float centerX, float centerY,
            int buttonWidth, int buttonHeight, int color, Texture t)
        {
            int halfIconWidth = (int)(0.5f * buttonWidth);
            int halfIconHeight = (int)(0.5f * buttonHeight);

            float scaleWidth = (float)buttonWidth / this._iconTextureSize.Width;
            float scaleHeight = (float)buttonHeight / this._iconTextureSize.Height;

            sprite.Transform = Matrix.Transformation2D(
                Vector2.Zero, 0.0f,
                new Vector2(scaleWidth, scaleHeight),
                Vector2.Zero,
                0.0f,
                new Vector2(centerX, centerY));

            sprite.Draw(t,
                new Vector3(this._iconTextureSize.Width / 2.0f, this._iconTextureSize.Height / 2.0f, 0),
                Vector3.Zero,
                color);
        }

        public abstract void Update(DrawArgs drawArgs);
        public abstract void Render(DrawArgs drawArgs);
        public abstract bool OnMouseUp(MouseEventArgs e);
        public abstract bool OnMouseMove(MouseEventArgs e);
        public abstract bool OnMouseDown(MouseEventArgs e);
        public abstract bool OnMouseWheel(MouseEventArgs e);
        public abstract void OnKeyUp(KeyEventArgs keyEvent);
        public abstract void OnKeyDown(KeyEventArgs keyEvent);

        public virtual void Dispose()
        {
            if (this.m_iconTexture != null)
            {
                this.m_iconTexture.Dispose();
                this.m_iconTexture = null;
            }
        }
    }
}
