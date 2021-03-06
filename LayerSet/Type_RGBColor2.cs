//
// Type_RGBColor2.cs.cs
//
// This file was generated by XMLSpy 2005 Enterprise Edition.
//
// YOU SHOULD NOT MODIFY THIS FILE, BECAUSE IT WILL BE
// OVERWRITTEN WHEN YOU RE-RUN CODE GENERATION.
//
// Refer to the XMLSpy Documentation for further details.
// http://www.altova.com/xmlspy
//


using System;
using System.Collections;
using System.Xml;
using Altova.Types;

namespace LayerSet
{
	public class Type_RGBColor2 : Altova.Xml.Node
	{
		#region Forward constructors
		public Type_RGBColor2() : base() {
            this.SetCollectionParents(); }
		public Type_RGBColor2(XmlDocument doc) : base(doc) {
            this.SetCollectionParents(); }
		public Type_RGBColor2(XmlNode node) : base(node) {
            this.SetCollectionParents(); }
		public Type_RGBColor2(Altova.Xml.Node node) : base(node) {
            this.SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

			for (int i = 0; i < this.DomChildCount(NodeType.Element, "", "Red"); i++)
			{
				XmlNode DOMNode = this.GetDomChildAt(NodeType.Element, "", "Red", i);
				InternalAdjustPrefix(DOMNode, true);
			}

			for (int i = 0; i < this.DomChildCount(NodeType.Element, "", "Green"); i++)
			{
				XmlNode DOMNode = this.GetDomChildAt(NodeType.Element, "", "Green", i);
				InternalAdjustPrefix(DOMNode, true);
			}

			for (int i = 0; i < this.DomChildCount(NodeType.Element, "", "Blue"); i++)
			{
				XmlNode DOMNode = this.GetDomChildAt(NodeType.Element, "", "Blue", i);
				InternalAdjustPrefix(DOMNode, true);
			}
		}


		#region Red accessor methods
		public int GetRedMinCount()
		{
			return 1;
		}

		public int RedMinCount
		{
			get
			{
				return 1;
			}
		}

		public int GetRedMaxCount()
		{
			return 1;
		}

		public int RedMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetRedCount()
		{
			return this.DomChildCount(NodeType.Element, "", "Red");
		}

		public int RedCount
		{
			get
			{
				return this.DomChildCount(NodeType.Element, "", "Red");
			}
		}

		public bool HasRed()
		{
			return this.HasDomChild(NodeType.Element, "", "Red");
		}

		public RedType2 GetRedAt(int index)
		{
			return new RedType2(GetDomNodeValue(this.GetDomChildAt(NodeType.Element, "", "Red", index)));
		}

		public XmlNode GetStartingRedCursor()
		{
			return this.GetDomFirstChild( NodeType.Element, "", "Red" );
		}

		public XmlNode GetAdvancedRedCursor( XmlNode curNode )
		{
			return this.GetDomNextChild( NodeType.Element, "", "Red", curNode );
		}

		public RedType2 GetRedValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new RedType2( curNode.InnerText );
		}


		public RedType2 GetRed()
		{
			return this.GetRedAt(0);
		}

		public RedType2 Red
		{
			get
			{
				return this.GetRedAt(0);
			}
		}

		public void RemoveRedAt(int index)
		{
            this.RemoveDomChildAt(NodeType.Element, "", "Red", index);
		}

		public void RemoveRed()
		{
			while (this.HasRed()) this.RemoveRedAt(0);
		}

		public void AddRed(RedType2 newValue)
		{
            this.AppendDomChild(NodeType.Element, "", "Red", newValue.ToString());
		}

		public void InsertRedAt(RedType2 newValue, int index)
		{
            this.InsertDomChildAt(NodeType.Element, "", "Red", index, newValue.ToString());
		}

		public void ReplaceRedAt(RedType2 newValue, int index)
		{
            this.ReplaceDomChildAt(NodeType.Element, "", "Red", index, newValue.ToString());
		}
		#endregion // Red accessor methods

		#region Red collection
        public RedCollection	MyReds = new RedCollection( );

        public class RedCollection: IEnumerable
        {
            Type_RGBColor2 parent;
            public Type_RGBColor2 Parent
			{
				set
				{
                    this.parent = value;
				}
			}
			public RedEnumerator GetEnumerator() 
			{
				return new RedEnumerator(this.parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return this.GetEnumerator();
			}
        }

        public class RedEnumerator: IEnumerator 
        {
			int nIndex;
			Type_RGBColor2 parent;
			public RedEnumerator(Type_RGBColor2 par) 
			{
                this.parent = par;
                this.nIndex = -1;
			}
			public void Reset() 
			{
                this.nIndex = -1;
			}
			public bool MoveNext() 
			{
                this.nIndex++;
				return(this.nIndex < this.parent.RedCount );
			}
			public RedType2  Current 
			{
				get 
				{
					return(this.parent.GetRedAt(this.nIndex));
				}
			}
			object IEnumerator.Current 
			{
				get 
				{
					return(this.Current);
				}
			}
    	}

        #endregion // Red collection

		#region Green accessor methods
		public int GetGreenMinCount()
		{
			return 1;
		}

		public int GreenMinCount
		{
			get
			{
				return 1;
			}
		}

		public int GetGreenMaxCount()
		{
			return 1;
		}

		public int GreenMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetGreenCount()
		{
			return this.DomChildCount(NodeType.Element, "", "Green");
		}

		public int GreenCount
		{
			get
			{
				return this.DomChildCount(NodeType.Element, "", "Green");
			}
		}

		public bool HasGreen()
		{
			return this.HasDomChild(NodeType.Element, "", "Green");
		}

		public GreenType2 GetGreenAt(int index)
		{
			return new GreenType2(GetDomNodeValue(this.GetDomChildAt(NodeType.Element, "", "Green", index)));
		}

		public XmlNode GetStartingGreenCursor()
		{
			return this.GetDomFirstChild( NodeType.Element, "", "Green" );
		}

		public XmlNode GetAdvancedGreenCursor( XmlNode curNode )
		{
			return this.GetDomNextChild( NodeType.Element, "", "Green", curNode );
		}

		public GreenType2 GetGreenValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new GreenType2( curNode.InnerText );
		}


		public GreenType2 GetGreen()
		{
			return this.GetGreenAt(0);
		}

		public GreenType2 Green
		{
			get
			{
				return this.GetGreenAt(0);
			}
		}

		public void RemoveGreenAt(int index)
		{
            this.RemoveDomChildAt(NodeType.Element, "", "Green", index);
		}

		public void RemoveGreen()
		{
			while (this.HasGreen()) this.RemoveGreenAt(0);
		}

		public void AddGreen(GreenType2 newValue)
		{
            this.AppendDomChild(NodeType.Element, "", "Green", newValue.ToString());
		}

		public void InsertGreenAt(GreenType2 newValue, int index)
		{
            this.InsertDomChildAt(NodeType.Element, "", "Green", index, newValue.ToString());
		}

		public void ReplaceGreenAt(GreenType2 newValue, int index)
		{
            this.ReplaceDomChildAt(NodeType.Element, "", "Green", index, newValue.ToString());
		}
		#endregion // Green accessor methods

		#region Green collection
        public GreenCollection	MyGreens = new GreenCollection( );

        public class GreenCollection: IEnumerable
        {
            Type_RGBColor2 parent;
            public Type_RGBColor2 Parent
			{
				set
				{
                    this.parent = value;
				}
			}
			public GreenEnumerator GetEnumerator() 
			{
				return new GreenEnumerator(this.parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return this.GetEnumerator();
			}
        }

        public class GreenEnumerator: IEnumerator 
        {
			int nIndex;
			Type_RGBColor2 parent;
			public GreenEnumerator(Type_RGBColor2 par) 
			{
                this.parent = par;
                this.nIndex = -1;
			}
			public void Reset() 
			{
                this.nIndex = -1;
			}
			public bool MoveNext() 
			{
                this.nIndex++;
				return(this.nIndex < this.parent.GreenCount );
			}
			public GreenType2  Current 
			{
				get 
				{
					return(this.parent.GetGreenAt(this.nIndex));
				}
			}
			object IEnumerator.Current 
			{
				get 
				{
					return(this.Current);
				}
			}
    	}

        #endregion // Green collection

		#region Blue accessor methods
		public int GetBlueMinCount()
		{
			return 1;
		}

		public int BlueMinCount
		{
			get
			{
				return 1;
			}
		}

		public int GetBlueMaxCount()
		{
			return 1;
		}

		public int BlueMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetBlueCount()
		{
			return this.DomChildCount(NodeType.Element, "", "Blue");
		}

		public int BlueCount
		{
			get
			{
				return this.DomChildCount(NodeType.Element, "", "Blue");
			}
		}

		public bool HasBlue()
		{
			return this.HasDomChild(NodeType.Element, "", "Blue");
		}

		public BlueType2 GetBlueAt(int index)
		{
			return new BlueType2(GetDomNodeValue(this.GetDomChildAt(NodeType.Element, "", "Blue", index)));
		}

		public XmlNode GetStartingBlueCursor()
		{
			return this.GetDomFirstChild( NodeType.Element, "", "Blue" );
		}

		public XmlNode GetAdvancedBlueCursor( XmlNode curNode )
		{
			return this.GetDomNextChild( NodeType.Element, "", "Blue", curNode );
		}

		public BlueType2 GetBlueValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new BlueType2( curNode.InnerText );
		}


		public BlueType2 GetBlue()
		{
			return this.GetBlueAt(0);
		}

		public BlueType2 Blue
		{
			get
			{
				return this.GetBlueAt(0);
			}
		}

		public void RemoveBlueAt(int index)
		{
            this.RemoveDomChildAt(NodeType.Element, "", "Blue", index);
		}

		public void RemoveBlue()
		{
			while (this.HasBlue()) this.RemoveBlueAt(0);
		}

		public void AddBlue(BlueType2 newValue)
		{
            this.AppendDomChild(NodeType.Element, "", "Blue", newValue.ToString());
		}

		public void InsertBlueAt(BlueType2 newValue, int index)
		{
            this.InsertDomChildAt(NodeType.Element, "", "Blue", index, newValue.ToString());
		}

		public void ReplaceBlueAt(BlueType2 newValue, int index)
		{
            this.ReplaceDomChildAt(NodeType.Element, "", "Blue", index, newValue.ToString());
		}
		#endregion // Blue accessor methods

		#region Blue collection
        public BlueCollection	MyBlues = new BlueCollection( );

        public class BlueCollection: IEnumerable
        {
            Type_RGBColor2 parent;
            public Type_RGBColor2 Parent
			{
				set
				{
                    this.parent = value;
				}
			}
			public BlueEnumerator GetEnumerator() 
			{
				return new BlueEnumerator(this.parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return this.GetEnumerator();
			}
        }

        public class BlueEnumerator: IEnumerator 
        {
			int nIndex;
			Type_RGBColor2 parent;
			public BlueEnumerator(Type_RGBColor2 par) 
			{
                this.parent = par;
                this.nIndex = -1;
			}
			public void Reset() 
			{
                this.nIndex = -1;
			}
			public bool MoveNext() 
			{
                this.nIndex++;
				return(this.nIndex < this.parent.BlueCount );
			}
			public BlueType2  Current 
			{
				get 
				{
					return(this.parent.GetBlueAt(this.nIndex));
				}
			}
			object IEnumerator.Current 
			{
				get 
				{
					return(this.Current);
				}
			}
    	}

        #endregion // Blue collection

        private void SetCollectionParents()
        {
            this.MyReds.Parent = this;
            this.MyGreens.Parent = this;
            this.MyBlues.Parent = this; 
	}
}
}
