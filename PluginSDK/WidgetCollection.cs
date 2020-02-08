namespace WorldWind
{
	/// <summary>
	/// Summary description for WidgetCollection.
	/// </summary>
	public class WidgetCollection : IWidgetCollection
	{
		System.Collections.ArrayList m_ChildWidgets = new System.Collections.ArrayList();
		
		public WidgetCollection()
		{

		}

		#region Methods
		public void BringToFront(int index)
		{
			IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;
			if(currentWidget != null)
			{
                this.m_ChildWidgets.RemoveAt(index);
                this.m_ChildWidgets.Insert(0, currentWidget);
			}
		}

		public void BringToFront(IWidget widget)
		{
			int foundIndex = -1;

			for(int index = 0; index < this.m_ChildWidgets.Count; index++)
			{
				IWidget currentWidget = this.m_ChildWidgets[index] as IWidget;
				if(currentWidget != null)
				{		
					if(currentWidget == widget)
					{
						foundIndex = index;
						break;
					}
				}	
			}

			if(foundIndex > 0)
			{
                this.BringToFront(foundIndex);
			}	
		}

		public void Add(IWidget widget)
		{
            this.m_ChildWidgets.Add(widget);
		}

		public void Clear()
		{
            this.m_ChildWidgets.Clear();
		}

		public void Insert(IWidget widget, int index)
		{
			if(index <= this.m_ChildWidgets.Count)
			{
                this.m_ChildWidgets.Insert(index, widget);
			}
			//probably want to throw an indexoutofrange type of exception
		}

		public IWidget RemoveAt(int index)
		{
			if(index < this.m_ChildWidgets.Count)
			{
				IWidget oldWidget = this.m_ChildWidgets[index] as IWidget;
                this.m_ChildWidgets.RemoveAt(index);
				return oldWidget;
			}
			else
			{
				return null;
			}
		}
		#endregion

		#region Properties
		public int Count
		{
			get
			{
				return this.m_ChildWidgets.Count;
			}
		}
		#endregion

		#region Indexers
		public IWidget this[int index]
		{
			get
			{
				return this.m_ChildWidgets[index] as IWidget;
			}
			set
			{
                this.m_ChildWidgets[index] = value;
			}
		}
		#endregion

	}
}
