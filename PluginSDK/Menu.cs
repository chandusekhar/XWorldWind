using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using Utility;
using WorldWind.Menu;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace WorldWind
{
	public interface IMenu
	{
		void OnKeyUp(KeyEventArgs keyEvent);
		void OnKeyDown(KeyEventArgs keyEvent);
		bool OnMouseUp(MouseEventArgs e);
		bool OnMouseDown(MouseEventArgs e);
		bool OnMouseMove(MouseEventArgs e);
		bool OnMouseWheel(MouseEventArgs e);
		void Render(DrawArgs drawArgs);
		void Dispose();
	}


	public class MenuCollection : IMenu
	{
		ArrayList _menus = new ArrayList();

		#region IMenu Members

		public void OnKeyUp(KeyEventArgs keyEvent)
		{
			foreach(IMenu m in this._menus)
				m.OnKeyUp(keyEvent);
		}

		public void OnKeyDown(KeyEventArgs keyEvent)
		{
			foreach(IMenu m in this._menus)
				m.OnKeyDown(keyEvent);
		}

		public bool OnMouseUp(MouseEventArgs e)
		{
			foreach(IMenu m in this._menus)
			{
				if(m.OnMouseUp(e))
					return true;
			}
			return false;
		}

		public bool OnMouseDown(MouseEventArgs e)
		{
			foreach(IMenu m in this._menus)
			{
				if(m.OnMouseDown(e))
					return true;
			}
			return false;
		}

		public bool OnMouseMove(MouseEventArgs e)
		{
			foreach(IMenu m in this._menus)
			{
				if(m.OnMouseMove(e))
					return true;
			}
			return false;
		}

		public bool OnMouseWheel(MouseEventArgs e)
		{
			foreach(IMenu m in this._menus)
			{
				if(m.OnMouseWheel(e))
					return true;
			}
			return false;
		}

		public void Render(DrawArgs drawArgs)
		{
			foreach(IMenu m in this._menus)
				m.Render(drawArgs);
		}

		public void Dispose()
		{
			foreach(IMenu m in this._menus)
				m.Dispose();
		}

		#endregion

		public void AddMenu(IMenu menu)
		{
			lock(this._menus.SyncRoot)
			{
				this._menus.Add(menu);
			}
		}

		public void RemoveMenu(IMenu menu)
		{
			lock(this._menus.SyncRoot)
			{
				this._menus.Remove(menu);
			}
		}
	}

	public abstract class SideBarMenu : IMenu
	{
		public long Id;

		public readonly int Left;
		public int Top = 120;
		public int Right = World.Settings.layerManagerWidth;
		public int Bottom;
		public readonly float HeightPercent = 0.9f;
		private Vector2[] outlineVerts = new Vector2[5];

		public int Width
		{
			get { return this.Right - this.Left; }
			set { this.Right = this.Left + value; }
		}

		public int Height
		{
			get { return this.Bottom - this.Top; }
			set { this.Bottom = this.Top + value; }
		}

		#region IMenu Members

		public abstract void OnKeyUp(KeyEventArgs keyEvent);
		public abstract void OnKeyDown(KeyEventArgs keyEvent);
		public abstract bool OnMouseUp(MouseEventArgs e);
		public abstract bool OnMouseDown(MouseEventArgs e);
		public abstract bool OnMouseMove(MouseEventArgs e);
		public abstract bool OnMouseWheel(MouseEventArgs e);
		public void Render(DrawArgs drawArgs)
		{
            if ((DrawArgs.WorldWindow != null) && (DrawArgs.WorldWindow.MenuBar.Anchor == MenuAnchor.Bottom))
            {
                this.Top = 0;
                this.Bottom = drawArgs.screenHeight - 120;
            }
            else
            {
                this.Top = 120;
                this.Bottom = drawArgs.screenHeight - 1;
            }

			MenuUtils.DrawBox(this.Left, this.Top, this.Right- this.Left, this.Bottom- this.Top, 0.0f,
				World.Settings.menuBackColor, drawArgs.device);

            this.RenderContents(drawArgs);

            this.outlineVerts[0].X = this.Left;
            this.outlineVerts[0].Y = this.Top;

            this.outlineVerts[1].X = this.Right;
            this.outlineVerts[1].Y = this.Top;

            this.outlineVerts[2].X = this.Right;
            this.outlineVerts[2].Y = this.Bottom;

            this.outlineVerts[3].X = this.Left;
            this.outlineVerts[3].Y = this.Bottom;

            this.outlineVerts[4].X = this.Left;
            this.outlineVerts[4].Y = this.Top;

			MenuUtils.DrawLine(this.outlineVerts, World.Settings.menuOutlineColor, drawArgs.device);
		}

		public abstract void RenderContents(DrawArgs drawArgs);
		public abstract void Dispose();

		#endregion

	}

	public class LayerManagerButton : MenuButton
	{
		World _parentWorld;
		LayerManagerMenu lmm;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.LayerManagerButton"/> class.
		/// </summary>
		/// <param name="iconImagePath"></param>
		/// <param name="parentWorld"></param>
		public LayerManagerButton(
			string iconImagePath,
			World parentWorld)
			: base(iconImagePath)
		{
			this._parentWorld = parentWorld;
			this.Description = "Layer Manager";
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override bool IsPushed()
		{
			return World.Settings.showLayerManager;
		}

		public override void Update(DrawArgs drawArgs)
		{
		}

		public override void OnKeyDown(KeyEventArgs keyEvent)
		{
		}

		public override void OnKeyUp(KeyEventArgs keyEvent)
		{
		}

		public override bool OnMouseDown(MouseEventArgs e)
		{
			if(this.IsPushed())
				return this.lmm.OnMouseDown(e);
			else 
				return false;
		}

		public override bool OnMouseMove(MouseEventArgs e)
		{
			if(this.lmm!=null && this.IsPushed())
				return this.lmm.OnMouseMove(e);
			else	
				return false;
		}

		public override bool OnMouseUp(MouseEventArgs e)
		{
			if(this.IsPushed())
				return this.lmm.OnMouseUp(e);
			else
				return false;
		}

		public override bool OnMouseWheel(MouseEventArgs e)
		{
			if(this.IsPushed())
				return this.lmm.OnMouseWheel(e);
			else
				return false;
		}

		public override void Render(DrawArgs drawArgs)
		{
			if(this.IsPushed())
			{
				if(this.lmm == null) this.lmm = new LayerManagerMenu(this._parentWorld, this);

                this.lmm.Render(drawArgs);
			}
		}

		public override void SetPushed(bool isPushed)
		{
			World.Settings.showLayerManager = isPushed;
		}
	}

	public class LayerManagerMenu : SideBarMenu
	{
		public int DialogColor = Color.Gray.ToArgb();
		public int TextColor = Color.White.ToArgb();
		public LayerMenuItem MouseOverItem;
		public int ScrollBarSize = 20;
		public int ItemHeight = 20;
		World _parentWorld;
		MenuButton _parentButton;
		bool showScrollbar;
		int scrollBarPosition;
		float scrollSmoothPosition; // Current position of scroll when smooth scrolling (scrollBarPosition=target)
		int scrollGrabPositionY; // Location mouse grabbed scroll
		bool isResizing;
		bool isScrolling;
		int leftBorder=2;
		int rightBorder=1;
		int topBorder=25;
		int bottomBorder=1;
		SharpDX.Direct3D9.Font headerFont;
		SharpDX.Direct3D9.Font itemFont;
		SharpDX.Direct3D9.Font wingdingsFont;
		SharpDX.Direct3D9.Font worldwinddingsFont;
		ArrayList _itemList = new ArrayList();
		SharpDX.Vector2[] scrollbarLine = new Vector2[2];	
		public ContextMenu ContextMenu;

		/// <summary>
		/// Client area X position of left side
		/// </summary>
		public int ClientLeft
		{
			get
			{
				return this.Left + this.leftBorder;
			}
		}

		/// <summary>
		/// Client area X position of right side
		/// </summary>
		public int ClientRight
		{
			get
			{
				int res = this.Right - this.rightBorder;
				if(this.showScrollbar)
					res -= this.ScrollBarSize;
				return res;
			}
		}

		/// <summary>
		/// Client area Y position of top side
		/// </summary>
		public int ClientTop
		{
			get
			{
				return this.Top + this.topBorder + 1;
			}
		}

		/// <summary>
		/// Client area Y position of bottom side
		/// </summary>
		public int ClientBottom
		{
			get
			{
				return this.Bottom - this.bottomBorder;
			}
		}

		/// <summary>
		/// Client area width
		/// </summary>
		public int ClientWidth
		{
			get
			{
				int res = this.Right - this.rightBorder - this.Left - this.leftBorder;
				if(this.showScrollbar)
					res -= this.ScrollBarSize;
				return res;
			}
		}

		/// <summary>
		/// Client area height
		/// </summary>
		public int ClientHeight
		{
			get
			{
				int res = this.Bottom - this.bottomBorder - this.Top - this.topBorder - 1;
				return res;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.LayerManagerMenu"/> class.
		/// </summary>
		/// <param name="parentWorld"></param>
		/// <param name="parentButton"></param>
		public LayerManagerMenu(World parentWorld, MenuButton parentButton)
		{
			this._parentWorld = parentWorld;
			this._parentButton = parentButton;			
		}

		public override void OnKeyDown(KeyEventArgs keyEvent)
		{
		}

		public override void Dispose()
		{
		}

		public override void OnKeyUp(KeyEventArgs keyEvent)
		{
		}

		public override bool OnMouseWheel(MouseEventArgs e)
		{
			if(e.X > this.Right || e.X < this.Left  || e.Y < this.Top || e.Y > this.Bottom)
				// Outside
				return false;

			// Mouse wheel scroll
			this.scrollBarPosition -= (e.Delta/6);
			return true;
		}

		public override bool OnMouseDown(MouseEventArgs e)
		{
			if(e.X > this.Right || e.X < this.Left  || e.Y < this.Top || e.Y > this.Bottom)
				// Outside
				return false;
			
			if(e.X > this.Right - 5 && e.X < this.Right + 5)
			{
				this.isResizing = true;
				return true;
			}

			if(e.Y < this.ClientTop)
				return false;

			if(e.X > this.Right - this.ScrollBarSize)
			{
				int numItems = this.GetNumberOfUncollapsedItems();
				int totalHeight = this.GetItemsHeight(this.m_DrawArgs);
				if(totalHeight > this.ClientHeight)
				{
					//int totalHeight = numItems * ItemHeight;
					double percentHeight = (double) this.ClientHeight / totalHeight;
					if(percentHeight > 1)
						percentHeight = 1;

					double scrollItemHeight = (double)percentHeight * this.ClientHeight;
					int scrollPosition = this.ClientTop + (int)(this.scrollBarPosition * percentHeight);
					if(e.Y < scrollPosition)
                        this.scrollBarPosition -= this.ClientHeight;
					else if(e.Y > scrollPosition + scrollItemHeight)
                        this.scrollBarPosition += this.ClientHeight;
					else
					{
                        this.scrollGrabPositionY = e.Y - scrollPosition;
                        this.isScrolling = true;
					}
				}
			}

			return true;
		}

		DrawArgs m_DrawArgs;

		public override bool OnMouseMove(MouseEventArgs e)
		{
			// Reset mouse over effect since mouse moved.
            this.MouseOverItem = null;

			if(this.isResizing)
			{
				if(e.X > 140 && e.X < 800)
					this.Width = e.X;
				
				return true;
			}

			if(this.isScrolling)
			{
				int totalHeight = this.GetItemsHeight(this.m_DrawArgs);//GetNumberOfUncollapsedItems() * ItemHeight;
				double percent = (double)totalHeight/ this.ClientHeight;
                this.scrollBarPosition = (int)((e.Y - this.scrollGrabPositionY - this.ClientTop) * percent);
				return true;
			}
			
			if(e.X > this.Right || e.X < this.Left  || e.Y < this.Top || e.Y > this.Bottom)
				// Outside
				return false;
			
			if(Math.Abs(e.X - this.Right ) < 5 )
			{
				DrawArgs.MouseCursor = CursorType.SizeWE;
				return true;
			}

			if(e.X > this.ClientRight)
				return true;

			foreach(LayerMenuItem lmi in this._itemList)
				if(lmi.OnMouseMove(e))
					return true;
			
			// Handled
			return true;
		}

		public override bool OnMouseUp(MouseEventArgs e)
		{
			if(this.isResizing)
			{
				this.isResizing = false;
				return true;
			}

			if(this.isScrolling)
			{
				this.isScrolling = false;
				return true;
			}

			foreach(LayerMenuItem lmi in this._itemList)
			{
				if(lmi.OnMouseUp(e))
					return true;
			}
			
			if(e.X > this.Right - 20 && e.X < this.Right &&
				e.Y > this.Top && e.Y < this.Top + this.topBorder)
			{
				this._parentButton.SetPushed(false);
				return true;
			}
			else if(e.X > 0 && e.X < this.Right && e.Y > 0 && e.Y < this.Bottom)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Displays the layer manager context menu for an item.
		/// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
		/// <param name="item"></param>
		public void ShowContextMenu( int x, int y, LayerMenuItem item )
		{
			if(this.ContextMenu!=null)
			{
                this.ContextMenu.Dispose();
                this.ContextMenu = null;
			}

            this.ContextMenu = new ContextMenu();
			item.RenderableObject.BuildContextMenu(this.ContextMenu);
            this.ContextMenu.Show(item.ParentControl, new Point(x,y));
		}

		/// <summary>
		/// Calculate the number of un-collapsed items in the tree.
		/// </summary>
		public int GetNumberOfUncollapsedItems()
		{
			int numItems = 1;
			foreach(LayerMenuItem subItem in this._itemList)
				numItems += subItem.GetNumberOfUncollapsedItems();

			return numItems;
		}

		public int GetItemsHeight(DrawArgs drawArgs)
		{
			int height = 20;
			foreach(LayerMenuItem subItem in this._itemList)
				height += subItem.GetItemsHeight(drawArgs);

			return height;
		}

		private void updateList()
		{
			if(this._parentWorld != null && this._parentWorld.RenderableObjects != null)
			{
				for(int i = 0; i < this._parentWorld.RenderableObjects.ChildObjects.Count; i++)
				{
					RenderableObject curObject = (RenderableObject)this._parentWorld.RenderableObjects.ChildObjects[i];
				
					if(i >= this._itemList.Count)
					{
						LayerMenuItem newItem = new LayerMenuItem(this, curObject);
						this._itemList.Add(newItem);
					}
					else
					{
						LayerMenuItem curItem = (LayerMenuItem)this._itemList[i];
						if(!curItem.RenderableObject.Name.Equals(curObject.Name))
						{
							this._itemList.Insert(i, new LayerMenuItem(this, curObject));
						}
					}
				}

				int extraItems = this._itemList.Count - this._parentWorld.RenderableObjects.ChildObjects.Count;
				this._itemList.RemoveRange(this._parentWorld.RenderableObjects.ChildObjects.Count, extraItems);
			}
			else
			{
				this._itemList.Clear();
			}
		}

		public override void RenderContents(DrawArgs drawArgs)
		{
            this.m_DrawArgs = drawArgs;
			try
			{
				if(this.itemFont == null)
				{
                    this.itemFont = drawArgs.CreateFont( World.Settings.LayerManagerFontName, 
						World.Settings.LayerManagerFontSize, World.Settings.LayerManagerFontStyle );

					// TODO: Fix wingdings menu problems
					Font localHeaderFont = new Font("Arial", 12.0f, FontStyle.Italic | FontStyle.Bold);
                    this.headerFont = new SharpDX.Direct3D9.Font(drawArgs.device, localHeaderFont);

					Font wingdings = new Font("Wingdings", 12.0f);
                    this.wingdingsFont = new SharpDX.Direct3D9.Font(drawArgs.device, wingdings);

					AddFontResource(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
					System.Drawing.Text.PrivateFontCollection fpc = new System.Drawing.Text.PrivateFontCollection();
					fpc.AddFontFile(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
					Font worldwinddings = new Font(fpc.Families[0], 12.0f);
                    this.worldwinddingsFont = new SharpDX.Direct3D9.Font(drawArgs.device, worldwinddings);
				}

				this.updateList();
				
				this.worldwinddingsFont.DrawText(
					null,
					"E",
					new Rectangle(this.Right - 16, this.Top + 2, 20, this.topBorder),
                    FontDrawFlags.None, this.TextColor);

				int numItems = this.GetNumberOfUncollapsedItems();
				int totalHeight = this.GetItemsHeight(drawArgs);//numItems * ItemHeight;
                this.showScrollbar = totalHeight > this.ClientHeight;
				if(this.showScrollbar)
				{
					double percentHeight = (double) this.ClientHeight / totalHeight;
					int scrollbarHeight = (int)(this.ClientHeight * percentHeight);

					int maxScroll = totalHeight- this.ClientHeight;

					if(this.scrollBarPosition < 0)
                        this.scrollBarPosition = 0;
					else if(this.scrollBarPosition > maxScroll) this.scrollBarPosition = maxScroll;

					// Smooth scroll
					const float scrollSpeed = 0.3f;
					float smoothScrollDelta = (this.scrollBarPosition - this.scrollSmoothPosition)*scrollSpeed;
					float absDelta = Math.Abs(smoothScrollDelta);
					if(absDelta > 100f || absDelta < 3f) 
						// Scroll > 100 pixels and < 1.5 pixels faster
						smoothScrollDelta = (this.scrollBarPosition - this.scrollSmoothPosition)*(float)Math.Sqrt(scrollSpeed);

                    this.scrollSmoothPosition += smoothScrollDelta;

					if(this.scrollSmoothPosition > maxScroll) this.scrollSmoothPosition = maxScroll;

					int scrollPos = (int)((float)percentHeight * this.scrollBarPosition );

					int color = this.isScrolling ? World.Settings.scrollbarHotColor : World.Settings.scrollbarColor;
					MenuUtils.DrawBox(this.Right - this.ScrollBarSize + 2, this.ClientTop + scrollPos, this.ScrollBarSize - 3,
						scrollbarHeight + 1,
						0.0f,
						color,
						drawArgs.device);

                    this.scrollbarLine[0].X = this.Right - this.ScrollBarSize;
                    this.scrollbarLine[0].Y = this.ClientTop;
                    this.scrollbarLine[1].X = this.Right - this.ScrollBarSize;
                    this.scrollbarLine[1].Y = this.Bottom;
					MenuUtils.DrawLine(this.scrollbarLine, this.DialogColor,
						drawArgs.device);
				}

				this.headerFont.DrawText(
					null, "Layer Manager",
					new Rectangle(this.Left+5, this.Top+1, this.Width, this.topBorder-2 ),
                    FontDrawFlags.VerticalCenter, this.TextColor);

				SharpDX.Vector2[] headerLinePoints = new SharpDX.Vector2[2];
				headerLinePoints[0].X = this.Left;
				headerLinePoints[0].Y = this.Top + this.topBorder - 1;

				headerLinePoints[1].X = this.Right;
				headerLinePoints[1].Y = this.Top + this.topBorder - 1;

				MenuUtils.DrawLine(headerLinePoints, this.DialogColor, drawArgs.device);

				int runningItemHeight = 0;
				if(this.showScrollbar )
					runningItemHeight = -(int)Math.Round(this.scrollSmoothPosition);

				// Set the Direct3D viewport to match the layer manager client area
				// to clip the text to the window when scrolling
				Viewport lmClientAreaViewPort = new Viewport();
				lmClientAreaViewPort.X = this.ClientLeft;
				lmClientAreaViewPort.Y = this.ClientTop;
				lmClientAreaViewPort.Width = this.ClientWidth;
				lmClientAreaViewPort.Height = this.ClientHeight;
				Viewport defaultViewPort = drawArgs.device.Viewport;
				drawArgs.device.Viewport = lmClientAreaViewPort;
				for(int i = 0; i < this._itemList.Count; i++)
				{
					if(runningItemHeight > this.ClientHeight)
						// No more space for items
						break;
					LayerMenuItem lmi = (LayerMenuItem) this._itemList[i];
					runningItemHeight += lmi.Render(
						drawArgs, this.ClientLeft, this.ClientTop,
						runningItemHeight, this.ClientWidth, this.ClientBottom, this.itemFont, this.wingdingsFont, this.worldwinddingsFont, this.MouseOverItem);
				}
				drawArgs.device.Viewport = defaultViewPort;
			}
			catch(Exception caught)
			{
				Log.Write( caught );
			}
		}

		[DllImport("gdi32.dll")]
		static extern int AddFontResource(string lpszFilename);
	}

		public class LayerMenuItem
		{
			RenderableObject m_renderableObject;
			ArrayList m_subItems = new ArrayList();
			private int _x;
			private int _y;
			private int _width;

			private int _itemXOffset = 5;
			private int _expandArrowXSize = 15;
			private int _checkBoxXOffset = 15;
			private int _subItemXIndent = 15;

			int itemOnColor = Color.White.ToArgb();
			int itemOffColor = Color.Gray.ToArgb();

			private bool isExpanded;
			public Control ParentControl;
			LayerManagerMenu m_parent; // menu this item belongs in

			public RenderableObject RenderableObject
			{
				get
				{
					return this.m_renderableObject;
				}
			}

			/// <summary>
			/// Calculate the number of un-collapsed items in the tree.
			/// </summary>
			public int GetNumberOfUncollapsedItems()
			{
				int numItems = 1;
				if(this.isExpanded)
				{
					foreach(LayerMenuItem subItem in this.m_subItems)
						numItems += subItem.GetNumberOfUncollapsedItems();
				}

				return numItems;
			}

			public int GetItemsHeight(DrawArgs drawArgs)
			{
				Rectangle rect = drawArgs.defaultDrawingFont.MeasureString(
					null,
					this.m_renderableObject.Name, FontDrawFlags.None, Color.White.ToArgb());

				int height = rect.Height;
				
				if(this.m_renderableObject.Description != null && this.m_renderableObject.Description.Length > 0)
				{
					SizeF rectF = DrawArgs.Graphics.MeasureString(this.m_renderableObject.Description,
						drawArgs.defaultSubTitleFont, this._width - (this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset)
						);

					height += (int)rectF.Height + 15;
				}

				if(height < this.lastConsumedHeight)
					height = this.lastConsumedHeight;

				if(this.isExpanded)
				{
					foreach(LayerMenuItem subItem in this.m_subItems)
						height += subItem.GetItemsHeight(drawArgs);
				}
				
				return height;
			}


			private string getFullRenderableObjectName(RenderableObject ro, string name)
			{
				if(ro.ParentList == null)
					return "/" + name;
				else
				{
					if(name == null)
						return this.getFullRenderableObjectName(ro.ParentList, ro.Name);
					else
						return this.getFullRenderableObjectName(ro.ParentList, ro.Name + "/" + name);
				}
			}


			/// <summary>
			/// Detect expand arrow mouse over
			/// </summary>
			public bool OnMouseMove(MouseEventArgs e)
			{
				if(e.Y < this._y)
					// Over 
					return false;

				if(e.X < this.m_parent.Left || e.X > this.m_parent.Right)
					return false;

				if(e.Y < this._y + 20)
				{
					// Mouse is on item
                    this.m_parent.MouseOverItem = this;

					if(e.X > this._x + this._itemXOffset && 
						e.X < this._x + (this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset))
					{
						if(this.m_renderableObject is RenderableObjectList)
							DrawArgs.MouseCursor = CursorType.Hand;
						return true;
					}
					return false;
				}

				foreach(LayerMenuItem lmi in this.m_subItems)
				{
					if(lmi.OnMouseMove(e))
					{
						// Mouse is on current item
                        this.m_parent.MouseOverItem = lmi;
						return true;
					}
				}

				return false;
			}

			public bool OnMouseUp(MouseEventArgs e)
			{
				if(e.Y < this._y)
					// Above 
					return false;

				if(e.Y <= this._y + 20)
				{
					if(e.X > this._x + this._itemXOffset &&
						e.X < this._x + (this._itemXOffset + this._width) &&
						e.Button == MouseButtons.Right)
					{
                        this.m_parent.ShowContextMenu( e.X, e.Y, this );
					}

					if(e.X > this._x + this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset &&
						e.X < this._x + (this._itemXOffset + this._width) &&
						e.Button == MouseButtons.Left && this.m_renderableObject != null && this.m_renderableObject.MetaData.Contains("InfoUri"))
					{
						string infoUri = (string) this.m_renderableObject.MetaData["InfoUri"];

						if (World.Settings.UseInternalBrowser || infoUri.StartsWith(@"worldwind://"))
						{
							SplitContainer sc = (SplitContainer)this.ParentControl.Parent.Parent;
							InternalWebBrowserPanel browser = (InternalWebBrowserPanel)sc.Panel1.Controls[0];
							browser.NavigateTo(infoUri);
						}
						else
						{
							ProcessStartInfo psi = new ProcessStartInfo();
							psi.FileName = infoUri;
							psi.Verb = "open";
							psi.UseShellExecute = true;
							psi.CreateNoWindow = true;
							Process.Start(psi);
						}
					}

					if(e.X > this._x + this._itemXOffset && 
						e.X < this._x + (this._itemXOffset + this._expandArrowXSize) && this.m_renderableObject is RenderableObjectList)
					{
						RenderableObjectList rol = (RenderableObjectList) this.m_renderableObject;
						if(!rol.DisableExpansion)
						{
							this.isExpanded = !this.isExpanded;
							return true;
						}
					}
				
					if(e.X > this._x + this._itemXOffset + this._expandArrowXSize && 
						e.X < this._x + (this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset) )
					{
						if(!this.m_renderableObject.IsOn && this.m_renderableObject.ParentList != null && this.m_renderableObject.ParentList.ShowOnlyOneLayer) this.m_renderableObject.ParentList.TurnOffAllChildren();

                        this.m_renderableObject.IsOn = !this.m_renderableObject.IsOn;
						return true;
					}
				}

				if(this.isExpanded)
				{
					foreach(LayerMenuItem lmi in this.m_subItems)
					{
						if(lmi.OnMouseUp(e))
							return true;
					}
				}
				
				return false;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref= "T:WorldWind.LayerMenuItem"/> class.
			/// </summary>
			/// <param name="parent"></param>
			/// <param name="renderableObject"></param>
			public LayerMenuItem(LayerManagerMenu parent, RenderableObject renderableObject)
			{
                this.m_renderableObject = renderableObject;
                this.m_parent = parent;
			}

			private void updateList()
			{
				if(this.isExpanded)
				{
					RenderableObjectList rol = (RenderableObjectList) this.m_renderableObject;
					for(int i = 0; i < rol.ChildObjects.Count; i++)
					{
						RenderableObject childObject = (RenderableObject)rol.ChildObjects[i];
						if(i >= this.m_subItems.Count)
						{
							LayerMenuItem newItem = new LayerMenuItem(this.m_parent, childObject);
                            this.m_subItems.Add(newItem);
						}
						else
						{
							LayerMenuItem curItem = (LayerMenuItem) this.m_subItems[i];

							if(curItem != null && curItem.RenderableObject != null && 
								childObject != null &&
								!curItem.RenderableObject.Name.Equals(childObject.Name))
							{
                                this.m_subItems.Insert(i, new LayerMenuItem(this.m_parent, childObject));
							}
						}
					}

					int extraItems = this.m_subItems.Count - rol.ChildObjects.Count;
					if(extraItems > 0) this.m_subItems.RemoveRange(rol.ChildObjects.Count, extraItems);
				}
			}

			int lastConsumedHeight = 20;
			
			public int Render(DrawArgs drawArgs, int x, int y, int yOffset, int width, int height, 
				SharpDX.Direct3D9.Font drawingFont,
				SharpDX.Direct3D9.Font wingdingsFont,
				SharpDX.Direct3D9.Font worldwinddingsFont, 
				LayerMenuItem mouseOverItem)
			{
				if(this.ParentControl == null) this.ParentControl = drawArgs.parentControl;

				this._x = x;
				this._y = y + yOffset;
				this._width = width;

				int consumedHeight = 20;
				
				Rectangle textRect = drawingFont.MeasureString(null, this.m_renderableObject.Name,
                FontDrawFlags.SingleLine,
					Color.White.ToArgb());

				consumedHeight = textRect.Height;

				if (this.m_renderableObject.Description != null && this.m_renderableObject.Description.Length > 0 && !(this.m_renderableObject is Icon))
				{
					SizeF rectF = DrawArgs.Graphics.MeasureString(this.m_renderableObject.Description,
						drawArgs.defaultSubTitleFont,
						width - (this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset)
						);
				
					consumedHeight += (int)rectF.Height + 15;
				}

                this.lastConsumedHeight = consumedHeight;
				// Layer manager client area height
				int totalHeight = height - y;

                this.updateList();

				if(yOffset >= -consumedHeight)
				{
					// Part of item or whole item visible
					int color = this.m_renderableObject.IsOn ? this.itemOnColor : this.itemOffColor;
					if(mouseOverItem==this)
					{
						if(!this.m_renderableObject.IsOn)
							// mouseover + inactive color (black)
							color = 0xff << 24;
						MenuUtils.DrawBox(this.m_parent.ClientLeft, this._y, this.m_parent.ClientWidth,consumedHeight,0,
							World.Settings.menuOutlineColor, drawArgs.device);
					}

					if(this.m_renderableObject is RenderableObjectList)
					{
						RenderableObjectList rol = (RenderableObjectList) this.m_renderableObject;
						if(!rol.DisableExpansion)
						{
							worldwinddingsFont.DrawText(
								null,
								(this.isExpanded ? "L" : "A"),
								new Rectangle(x + this._itemXOffset, this._y, this._expandArrowXSize, height),
                                FontDrawFlags.SingleLine,
								color );
						}
					}

					string checkSymbol = null;
					if(this.m_renderableObject.ParentList != null && this.m_renderableObject.ParentList.ShowOnlyOneLayer)
						// Radio check
						checkSymbol = this.m_renderableObject.IsOn ? "O" : "P";
					else				
						// Normal check
						checkSymbol = this.m_renderableObject.IsOn ? "N" : "F";

					worldwinddingsFont.DrawText(
							null,
							checkSymbol,
							new Rectangle(
							x + this._itemXOffset + this._expandArrowXSize, this._y,
							this._checkBoxXOffset,
							height),
                            FontDrawFlags.NoClip,
							color );


					drawingFont.DrawText(
						null, this.m_renderableObject.Name,
						new Rectangle(
						x + this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset, this._y,
						width - (this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset),
						height),
                        FontDrawFlags.SingleLine,
						color );

					if(this.m_renderableObject.Description != null && this.m_renderableObject.Description.Length > 0 && !(this.m_renderableObject is Icon))
					{
						drawArgs.defaultSubTitleDrawingFont.DrawText(
							null, this.m_renderableObject.Description,
							new Rectangle(
								x + this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset, this._y + textRect.Height,
								width - (this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset),
								height),
                            FontDrawFlags.WordBreak,
							Color.Gray.ToArgb());
					}

					if(this.m_renderableObject.MetaData.Contains("InfoUri"))
					{
						Vector2[] underlineVerts = new Vector2[2];
						underlineVerts[0].X = x + this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset;
						underlineVerts[0].Y = this._y + textRect.Height;
						underlineVerts[1].X = underlineVerts[0].X + textRect.Width;
						underlineVerts[1].Y = this._y + textRect.Height;

						MenuUtils.DrawLine(underlineVerts, color, drawArgs.device);
					}
				}
				
				if(this.isExpanded)
				{
					for(int i = 0; i < this.m_subItems.Count; i++)
					{
						int yRealOffset = yOffset + consumedHeight;
						if(yRealOffset > totalHeight)
							// No more space for items
							break;
						LayerMenuItem lmi = (LayerMenuItem) this.m_subItems[i];
						consumedHeight += lmi.Render(
							drawArgs,
							x + this._subItemXIndent,
							y,
							yRealOffset,
							width - this._subItemXIndent,
							height,
							drawingFont,
							wingdingsFont,
							worldwinddingsFont,
							mouseOverItem );
					}
				}

				return consumedHeight;
			}
		}

	public class LayerShortcutMenuButton : MenuButton
	{
		#region Private Members
		bool _isPushed;
		RenderableObject _ro;
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.LayerShortcutMenuButton"/> class.
		/// </summary>
		/// <param name="imageFilePath"></param>
		/// <param name="ro"></param>
		public LayerShortcutMenuButton(
			string imageFilePath, RenderableObject ro)
			: base(imageFilePath)
		{
			this.Description = ro.Name;
			this._ro = ro;
			this._isPushed = ro.IsOn;
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override bool IsPushed()
		{
			return this._isPushed;
		}

		public override void SetPushed(bool isPushed)
		{
			this._isPushed = isPushed;
			if(!this._ro.IsOn && this._ro.ParentList != null && this._ro.ParentList.ShowOnlyOneLayer)
				this._ro.ParentList.TurnOffAllChildren();

			this._ro.IsOn = this._isPushed;

			//HACK: Temporary fix
			if(this._ro.Name=="Placenames" )
				World.Settings.showPlacenames = isPushed;
			else if(this._ro.Name=="Boundaries" )
				World.Settings.showBoundaries = isPushed;
		}

		public override void OnKeyDown(KeyEventArgs keyEvent)
		{
		}

		public override void OnKeyUp(KeyEventArgs keyEvent)
		{

		}
		public override void Update(DrawArgs drawArgs)
		{
			if(this._ro.IsOn != this._isPushed)
				this._isPushed = this._ro.IsOn;
		}

		public override void Render(DrawArgs drawArgs)
		{
		}

		public override bool OnMouseDown(MouseEventArgs e)
		{
			return false;
		}

		public override bool OnMouseMove(MouseEventArgs e)
		{
			return false;
		}

		public override bool OnMouseUp(MouseEventArgs e)
		{
			return false;
		}

		public override bool OnMouseWheel(MouseEventArgs e)
		{
			return false;
		}
	}

	public enum MenuAnchor
	{
		Top,
		Bottom,
		Left,
		Right
	}

	public sealed class MenuUtils
	{
		private MenuUtils(){}

		public static void DrawLine(Vector2[] linePoints, int color, Device device)
		{
			CustomVertex.TransformedColored[] lineVerts = new CustomVertex.TransformedColored[linePoints.Length];

			for(int i = 0; i < linePoints.Length; i++)
			{
				lineVerts[i].X = linePoints[i].X;
				lineVerts[i].Y = linePoints[i].Y;
				lineVerts[i].Z = 0.0f;

				lineVerts[i].Color = color;
			}

			device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);
			device.VertexFormat = CustomVertex.TransformedColored.Format;

			device.DrawUserPrimitives(PrimitiveType.LineStrip, lineVerts.Length - 1, lineVerts);
		}

		public static void DrawBox(int ulx, int uly, int width, int height, float z, int color, Device device)
		{
			CustomVertex.TransformedColored[] verts = new CustomVertex.TransformedColored[4];
			verts[0].X = (float)ulx;
			verts[0].Y = (float)uly;
			verts[0].Z = z;
			verts[0].Color = color;

			verts[1].X = (float)ulx;
			verts[1].Y = (float)uly + height;
			verts[1].Z = z;
			verts[1].Color = color;

			verts[2].X = (float)ulx + width;
			verts[2].Y = (float)uly;
			verts[2].Z = z;
			verts[2].Color = color;

			verts[3].X = (float)ulx + width;
			verts[3].Y = (float)uly + height;
			verts[3].Z = z;
			verts[3].Color = color;

			device.VertexFormat = CustomVertex.TransformedColored.Format;
			device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);
			device.DrawUserPrimitives(PrimitiveType.TriangleStrip, verts.Length - 2, verts);
		}

		public static void DrawSector(double startAngle, double endAngle, int centerX, int centerY, int radius, float z, int color, Device device)
		{
			int prec = 7;

			CustomVertex.TransformedColored[] verts = new CustomVertex.TransformedColored[prec + 2];
			verts[0].X = centerX;
			verts[0].Y = centerY;
			verts[0].Z = z;
			verts[0].Color = color;
			double angleInc = (double)(endAngle - startAngle) / prec;

			for(int i = 0; i <= prec; i++)
			{
				verts[i + 1].X = (float)Math.Cos((double)(startAngle + angleInc * i))*radius + centerX;
				verts[i + 1].Y = (float)Math.Sin((double)(startAngle + angleInc * i))*radius*(-1.0f) + centerY;
				verts[i + 1].Z = z;
				verts[i + 1].Color = color;
			}

			device.VertexFormat = CustomVertex.TransformedColored.Format;
			device.SetTextureStageState(0, TextureStage.ColorOperation , TextureOperation.Disable);
			device.DrawUserPrimitives(PrimitiveType.TriangleFan, verts.Length - 2, verts);
		}
	}
}
