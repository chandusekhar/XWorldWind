using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using SharpDX.Direct3D9;

namespace WorldWind.Menu
{

    /// <summary>
    /// WorldWind Toolbar
    /// </summary>
    public class MenuBar : IMenu
    {
        #region Private Members
        protected ArrayList m_toolsMenuButtons = new ArrayList();
        protected ArrayList m_layersMenuButtons = new ArrayList();
        protected VisibleState _visibleState = VisibleState.Visible;
        protected DateTime _lastVisibleChange = DateTime.Now;
        protected float _outerPadding = 5;
        protected int x;
        protected int y;
        protected int hideTimeMilliseconds = 100;
        protected bool _isHideable;
        protected const float padRatio = 1 / 9.0f;
        protected CursorType mouseCursor;
        protected int chevronColor = Color.Black.ToArgb();
        protected CustomVertex.TransformedColored[] enabledChevron = new CustomVertex.TransformedColored[3];
        protected Sprite m_sprite;


        #endregion

        #region Properties

        /// <summary>
        /// Where the menubar is anchored.
        /// </summary>
        public MenuAnchor Anchor
        {
            get { return this.m_anchor; }
            set { this.m_anchor = value; }
        }
        private MenuAnchor m_anchor = MenuAnchor.Top;

        /// <summary>
        /// Indicates whether the menu is "open". (user activity)
        /// </summary>
        public bool IsActive
        {
            get
            {
                return (this._curSelection >= 0);
            }
        }

        public ArrayList LayersMenuButtons
        {
            get
            {
                return this.m_layersMenuButtons;
            }
            set
            {
                this.m_layersMenuButtons = value;
            }
        }

        public ArrayList ToolsMenuButtons
        {
            get
            {
                return this.m_toolsMenuButtons;
            }
            set
            {
                this.m_toolsMenuButtons = value;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Menu.MenuBar"/> class.
        /// </summary>
        /// <param name="anchor"></param>
        /// <param name="iconSize"></param>
        public MenuBar(MenuAnchor anchor, int iconSize)
        {
            this.m_anchor = anchor;
            MenuButton.SelectedSize = iconSize;
        }

        /// <summary>
        /// Adds a tool button to the bar
        /// </summary>
        public void AddToolsMenuButton(MenuButton button)
        {
            lock (this.m_toolsMenuButtons.SyncRoot)
            {
                this.m_toolsMenuButtons.Add(button);
            }
        }

        /// <summary>
        /// Adds a tool button to the bar
        /// </summary>
        public void AddToolsMenuButton(MenuButton button, int index)
        {
            lock (this.m_toolsMenuButtons.SyncRoot)
            {
                if (index < 0)
                    this.m_toolsMenuButtons.Insert(0, button);
                else if (index >= this.m_toolsMenuButtons.Count)
                    this.m_toolsMenuButtons.Add(button);
                else
                    this.m_toolsMenuButtons.Insert(index, button);
            }
        }

        /// <summary>
        /// Removes a layer button from the bar if it is found.
        /// </summary>
        public void RemoveToolsMenuButton(MenuButton button)
        {
            lock (this.m_toolsMenuButtons.SyncRoot)
            {
                this.m_toolsMenuButtons.Remove(button);
            }
        }

        /// <summary>
        /// Adds a layer button to the bar
        /// </summary>
        public void AddLayersMenuButton(MenuButton button)
        {
            lock (this.m_layersMenuButtons.SyncRoot)
            {
                this.m_layersMenuButtons.Add(button);
            }
        }

        /// <summary>
        /// Adds a layer button to the bar
        /// </summary>
        public void AddLayersMenuButton(MenuButton button, int index)
        {
            lock (this.m_layersMenuButtons.SyncRoot)
            {
                if (index < this.m_layersMenuButtons.Count)
                    this.m_layersMenuButtons.Insert(0, button);
                else if (index >= this.m_layersMenuButtons.Count)
                    this.m_layersMenuButtons.Add(button);
                else
                    this.m_layersMenuButtons.Insert(index, button);
            }
        }

        /// <summary>
        /// Removes a layer button from the bar if it is found.
        /// </summary>
        public void RemoveLayersMenuButton(MenuButton button)
        {
            lock (this.m_layersMenuButtons.SyncRoot)
            {
                this.m_layersMenuButtons.Remove(button);
            }
        }

        #region IMenu Members

        public void OnKeyUp(KeyEventArgs keyEvent)
        {
            // TODO:  Add ToolsMenuBar.OnKeyUp implementation
        }

        public void OnKeyDown(KeyEventArgs keyEvent)
        {
            // TODO:  Add ToolsMenuBar.OnKeyDown implementation
        }

        public bool OnMouseUp(MouseEventArgs e)
        {
            if (World.Settings.showToolbar)
            {
                if (this._curSelection != -1 && e.Button == MouseButtons.Left)
                {
                    if (this._curSelection < this.m_toolsMenuButtons.Count)
                    {
                        MenuButton button = (MenuButton) this.m_toolsMenuButtons[this._curSelection];
                        button.SetPushed(!button.IsPushed());
                    }
                    else
                    {
                        MenuButton button = (MenuButton) this.m_layersMenuButtons[this._curSelection - this.m_toolsMenuButtons.Count];
                        button.SetPushed(!button.IsPushed());
                    }

                    return true;
                }
            }

            // Pass message on to the "tools"
            foreach (MenuButton button in this.m_toolsMenuButtons)
                if (button.IsPushed())
                    if (button.OnMouseUp(e))
                        return true;

            return false;
        }

        public bool OnMouseDown(MouseEventArgs e)
        {
            // Trigger "tool" update
            foreach (MenuButton button in this.m_toolsMenuButtons)
                if (button.IsPushed())
                    if (button.OnMouseDown(e))
                        return true;

            return false;
        }

        int _curSelection = -1;

        public bool OnMouseMove(MouseEventArgs e)
        {
            // Default to arrow cursor every time mouse moves
            this.mouseCursor = CursorType.Arrow;

            // Trigger "tools" update
            foreach (MenuButton button in this.m_toolsMenuButtons)
                if (button.IsPushed())
                    if (button.OnMouseMove(e))
                        return true;

            if (!World.Settings.showToolbar)
                return false;

            if (this._visibleState == VisibleState.Visible)
            {
                float width, height;

                int buttonCount;

                int sel = -1;

                switch (this.m_anchor)
                {
                    case MenuAnchor.Top:
                        buttonCount = this.m_toolsMenuButtons.Count + this.m_layersMenuButtons.Count;
                        width = buttonCount * (this._outerPadding + MenuButton.NormalSize) + this._outerPadding;
                        height = this._outerPadding * 2 + MenuButton.NormalSize;

                        if (e.Y >= this.y && e.Y <= this.y + height + 2 * this._outerPadding)
                        {
                            sel = (int)((e.X - this._outerPadding) / (MenuButton.NormalSize + this._outerPadding));
                            if (sel < buttonCount)
                                this.mouseCursor = CursorType.Hand;
                            else
                                sel = -1;
                        }

                        this._curSelection = sel;

                        break;

                    case MenuAnchor.Bottom:
                        buttonCount = this.m_toolsMenuButtons.Count + this.m_layersMenuButtons.Count;
                        width = buttonCount * (this._outerPadding + MenuButton.NormalSize) + this._outerPadding;
                        height = this._outerPadding * 2 + MenuButton.NormalSize;

                        if (e.Y >= this.y && e.Y <= this.y + (height + 2 * this._outerPadding))
                        {
                            sel = (int)((e.X - this._outerPadding) / (MenuButton.NormalSize + this._outerPadding));
                            if (sel < buttonCount)
                                this.mouseCursor = CursorType.Hand;
                            else
                                sel = -1;
                        }

                        this._curSelection = sel;

                        break;

                    case MenuAnchor.Right:
                        width = this._outerPadding * 2 + MenuButton.SelectedSize;
                        height = this._outerPadding * 2 + (this.m_toolsMenuButtons.Count * this.m_layersMenuButtons.Count) * MenuButton.SelectedSize;

                        if (e.X >= this.x + this._outerPadding && e.X <= this.x + width + this._outerPadding &&
                            e.Y >= this.y + this._outerPadding && e.Y <= this.y + height + this._outerPadding)
                        {
                            int dx = (int)(e.Y - (this.y + this._outerPadding));
                            this._curSelection = (int)(dx / MenuButton.SelectedSize);
                        }
                        else
                        {
                            this._curSelection = -1;
                        }
                        break;
                }
            }

            return false;
        }

        public bool OnMouseWheel(MouseEventArgs e)
        {
            // Trigger "tool" update
            foreach (MenuButton button in this.m_toolsMenuButtons)
                if (button.IsPushed())
                    if (button.OnMouseWheel(e))
                        return true;

            return false;
        }

        public void Render(DrawArgs drawArgs)
        {
            if (this.m_sprite == null) this.m_sprite = new Sprite(drawArgs.device);

            if (this.mouseCursor != CursorType.Arrow)
                DrawArgs.MouseCursor = this.mouseCursor;


            foreach (MenuButton button in this.m_toolsMenuButtons)
                if (button.IsPushed())
                    // Does not render the button, but the functionality behind the button
                    button.Render(drawArgs);

            foreach (MenuButton button in this.m_toolsMenuButtons)
                button.Update(drawArgs);

            foreach (MenuButton button in this.m_layersMenuButtons)
                button.Update(drawArgs);

            if (!World.Settings.showToolbar)
                return;

            if (this._isHideable)
            {
                if (this._visibleState == VisibleState.NotVisible)
                {
                    if (
                        (this.m_anchor == MenuAnchor.Top && DrawArgs.LastMousePosition.Y < MenuButton.NormalSize) ||
                        (this.m_anchor == MenuAnchor.Bottom && DrawArgs.LastMousePosition.Y > drawArgs.screenHeight - MenuButton.NormalSize) ||
                        (this.m_anchor == MenuAnchor.Right && DrawArgs.LastMousePosition.X > drawArgs.screenWidth - MenuButton.NormalSize)
                        )
                    {
                        this._visibleState = VisibleState.Ascending;
                        this._lastVisibleChange = DateTime.Now;
                    }
                }
                else if (
                    (this.m_anchor == MenuAnchor.Top && DrawArgs.LastMousePosition.Y > 2 * this._outerPadding + MenuButton.NormalSize) ||
                    (this.m_anchor == MenuAnchor.Bottom && DrawArgs.LastMousePosition.Y < drawArgs.screenHeight - 2 * this._outerPadding - MenuButton.NormalSize) ||
                    (this.m_anchor == MenuAnchor.Right && DrawArgs.LastMousePosition.X < drawArgs.screenWidth - MenuButton.NormalSize)
                    )
                {
                    if (this._visibleState == VisibleState.Visible)
                    {
                        this._visibleState = VisibleState.Descending;
                        this._lastVisibleChange = DateTime.Now;
                    }
                    else if (this._visibleState == VisibleState.Descending)
                    {
                        if (DateTime.Now.Subtract(this._lastVisibleChange) > TimeSpan.FromMilliseconds(this.hideTimeMilliseconds))
                        {
                            this._visibleState = VisibleState.NotVisible;
                            this._lastVisibleChange = DateTime.Now;
                        }
                    }
                }
                else if (this._visibleState == VisibleState.Ascending)
                {
                    if (DateTime.Now.Subtract(this._lastVisibleChange) > TimeSpan.FromMilliseconds(this.hideTimeMilliseconds))
                    {
                        this._visibleState = VisibleState.Visible;
                        this._lastVisibleChange = DateTime.Now;
                    }
                }
                else if (this._visibleState == VisibleState.Descending)
                {
                    if (DateTime.Now.Subtract(this._lastVisibleChange) > TimeSpan.FromMilliseconds(this.hideTimeMilliseconds))
                    {
                        this._visibleState = VisibleState.NotVisible;
                        this._lastVisibleChange = DateTime.Now;
                    }
                }
            }
            else
            {
                this._visibleState = VisibleState.Visible;
            }

            int totalNumberButtons = this.m_toolsMenuButtons.Count + this.m_layersMenuButtons.Count;
            MenuButton.NormalSize = MenuButton.SelectedSize / 2;
            this._outerPadding = MenuButton.NormalSize * padRatio;

            float menuWidth = (MenuButton.NormalSize + this._outerPadding) * totalNumberButtons + this._outerPadding;
            if (menuWidth > drawArgs.screenWidth)
            {
                MenuButton.NormalSize = (drawArgs.screenWidth) / ((padRatio + 1) * totalNumberButtons + padRatio);
                this._outerPadding = MenuButton.NormalSize * padRatio;

                // recalc menuWidth if we want to center the toolbar
                menuWidth = (MenuButton.NormalSize + this._outerPadding) * totalNumberButtons + this._outerPadding;
            }

            if (this.m_anchor == MenuAnchor.Left)
            {
                this.x = 0;
                this.y = (int)MenuButton.NormalSize;
            }
            else if (this.m_anchor == MenuAnchor.Right)
            {
                this.x = (int)(drawArgs.screenWidth - 2 * this._outerPadding - MenuButton.NormalSize);
                this.y = (int)MenuButton.NormalSize;
            }
            else if (this.m_anchor == MenuAnchor.Top)
            {
                this.x = (int)(drawArgs.screenWidth / 2 - totalNumberButtons * MenuButton.NormalSize / 2 - this._outerPadding);
                this.y = 0;
            }
            else if (this.m_anchor == MenuAnchor.Bottom)
            {
                this.x = (int)(drawArgs.screenWidth / 2 - totalNumberButtons * MenuButton.NormalSize / 2 - this._outerPadding);
                this.y = (int)(drawArgs.screenHeight - 2 * this._outerPadding - MenuButton.NormalSize);
            }

            if (this._visibleState == VisibleState.Ascending)
            {
                TimeSpan t = DateTime.Now.Subtract(this._lastVisibleChange);
                if (t.Milliseconds < this.hideTimeMilliseconds)
                {
                    double percent = (double)t.Milliseconds / this.hideTimeMilliseconds;
                    int dx = (int)((MenuButton.NormalSize + 5) - (percent * (MenuButton.NormalSize + 5)));

                    if (this.m_anchor == MenuAnchor.Left)
                    {
                        this.x -= dx;
                    }
                    else if (this.m_anchor == MenuAnchor.Right)
                    {
                        this.x += dx;
                    }
                    else if (this.m_anchor == MenuAnchor.Top)
                    {
                        this.y -= dx;

                    }
                    else if (this.m_anchor == MenuAnchor.Bottom)
                    {
                        this.y += dx;
                    }
                }
            }
            else if (this._visibleState == VisibleState.Descending)
            {
                TimeSpan t = DateTime.Now.Subtract(this._lastVisibleChange);
                if (t.Milliseconds < this.hideTimeMilliseconds)
                {
                    double percent = (double)t.Milliseconds / this.hideTimeMilliseconds;
                    int dx = (int)((percent * (MenuButton.NormalSize + 5)));

                    if (this.m_anchor == MenuAnchor.Left)
                    {
                        this.x -= dx;
                    }
                    else if (this.m_anchor == MenuAnchor.Right)
                    {
                        this.x += dx;
                    }
                    else if (this.m_anchor == MenuAnchor.Top)
                    {
                        this.y -= dx;
                    }
                    else if (this.m_anchor == MenuAnchor.Bottom)
                    {
                        this.y += dx;
                    }
                }
            }

            lock (this.m_toolsMenuButtons.SyncRoot)
            {
                MenuButton selectedButton = null;
                if (this._curSelection >= 0 & this._curSelection < totalNumberButtons)
                {
                    if (this._curSelection < this.m_toolsMenuButtons.Count)
                        selectedButton = (MenuButton) this.m_toolsMenuButtons[this._curSelection];
                    else
                        selectedButton = (MenuButton) this.m_layersMenuButtons[this._curSelection - this.m_toolsMenuButtons.Count];
                }

                //_outerPadding = MenuButton.NormalSize*padRatio;
                //float menuWidth = (MenuButton.NormalSize+_outerPadding)*totalNumberButtons+_outerPadding;
                //if(menuWidth>drawArgs.screenWidth)
                //{
                //    //MessageBox.Show(drawArgs.screenWidth.ToString());
                //    MenuButton.NormalSize = (drawArgs.screenWidth)/((padRatio+1)*totalNumberButtons+padRatio);
                //    //MessageBox.Show(MenuButton.NormalSize.ToString());
                //    _outerPadding = MenuButton.NormalSize*padRatio;
                //}

                if (this._visibleState != VisibleState.NotVisible)
                {
                    if (this.m_anchor == MenuAnchor.Top)
                    {
                        MenuUtils.DrawBox(0, 0, drawArgs.screenWidth, (int)(MenuButton.NormalSize + 2 * this._outerPadding), 0.0f,
                            World.Settings.toolBarBackColor, drawArgs.device);
                    }
                    else if (this.m_anchor == MenuAnchor.Bottom)
                    {
                        MenuUtils.DrawBox(0, (int)(this.y - this._outerPadding), drawArgs.screenWidth, (int)(MenuButton.NormalSize + 4 * this._outerPadding), 0.0f,
                            World.Settings.toolBarBackColor, drawArgs.device);
                    }
                }

                float total = 0;
                float extra = 0;
                for (int i = 0; i < totalNumberButtons; i++)
                {
                    MenuButton button;
                    if (i < this.m_toolsMenuButtons.Count)
                        button = (MenuButton) this.m_toolsMenuButtons[i];
                    else
                        button = (MenuButton) this.m_layersMenuButtons[i - this.m_toolsMenuButtons.Count];
                    total += button.CurrentSize;
                    extra += button.CurrentSize - MenuButton.NormalSize;
                }

                float pad = ((float) this._outerPadding * (totalNumberButtons + 1) - extra) / (totalNumberButtons + 1);
                float buttonX = pad;

                // TODO - to center the menubar set the buttonX to center-half toolbar width
                // float buttonX = (drawArgs.screenWidth - menuWidth) / 2; 

                this.m_sprite.Begin(SpriteFlags.AlphaBlend);
                for (int i = 0; i < totalNumberButtons; i++)
                {
                    MenuButton button;
                    if (i < this.m_toolsMenuButtons.Count)
                        button = (MenuButton) this.m_toolsMenuButtons[i];
                    else
                        button = (MenuButton) this.m_layersMenuButtons[i - this.m_toolsMenuButtons.Count];

                    if (button.IconTexture == null)
                        button.InitializeTexture(drawArgs.device);

                    if (this._visibleState != VisibleState.NotVisible)
                    {
                        int centerX = (int)(buttonX + button.CurrentSize * 0.5f);
                        buttonX += button.CurrentSize + pad;
                        float buttonTopY = this.y + this._outerPadding;

                        if (this.m_anchor == MenuAnchor.Bottom)
                            buttonTopY = (int)(drawArgs.screenHeight - this._outerPadding - button.CurrentSize);

                        if (button.IsPushed())
                        {
                            // Draw the chevron
                            float chevronSize = button.CurrentSize * padRatio;

                            this.enabledChevron[0].Color = this.chevronColor;
                            this.enabledChevron[1].Color = this.chevronColor;
                            this.enabledChevron[2].Color = this.chevronColor;

                            if (this.m_anchor == MenuAnchor.Bottom)
                            {
                                this.enabledChevron[2].X = centerX - chevronSize;
                                this.enabledChevron[2].Y = this.y - 2;
                                this.enabledChevron[2].Z = 0.0f;

                                this.enabledChevron[0].X = centerX;
                                this.enabledChevron[0].Y = this.y - 2 + chevronSize;
                                this.enabledChevron[0].Z = 0.0f;

                                this.enabledChevron[1].X = centerX + chevronSize;
                                this.enabledChevron[1].Y = this.y - 2;
                                this.enabledChevron[1].Z = 0.0f;
                            }
                            else
                            {
                                this.enabledChevron[2].X = centerX - chevronSize;
                                this.enabledChevron[2].Y = this.y + 2;
                                this.enabledChevron[2].Z = 0.0f;

                                this.enabledChevron[0].X = centerX;
                                this.enabledChevron[0].Y = this.y + 2 + chevronSize;
                                this.enabledChevron[0].Z = 0.0f;

                                this.enabledChevron[1].X = centerX + chevronSize;
                                this.enabledChevron[1].Y = this.y + 2;
                                this.enabledChevron[1].Z = 0.0f;
                            }

                            drawArgs.device.VertexFormat = CustomVertex.TransformedColored.Format;
                            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation = TextureOperation.Disable;
                            drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleList, 1, this.enabledChevron);
                            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation = TextureOperation.SelectArg1;
                        }

                        button.RenderEnabledIcon(this.m_sprite,
                            drawArgs,
                            centerX,
                            buttonTopY,
                            i == this._curSelection, this.m_anchor);
                    }
                }

                this.m_sprite.End();

            }
        }

        public void Dispose()
        {
            foreach (MenuButton button in this.m_toolsMenuButtons)
                button.Dispose();

            if (this.m_sprite != null)
            {
                this.m_sprite.Dispose();
                this.m_sprite = null;
            }
        }

        #endregion

        protected enum VisibleState
        {
            NotVisible,
            Descending,
            Ascending,
            Visible
        }
    }
}
