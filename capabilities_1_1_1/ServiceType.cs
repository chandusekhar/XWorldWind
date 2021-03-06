//
// ServiceType.SetSamplerState(0, SamplerStatecs.SetSamplerState(0, SamplerStatecs
//
// This file was generated by XMLSPY 2004 Enterprise Edition.SetSamplerState(0, SamplerState
//
// YOU SHOULD NOT MODIFY THIS FILE, BECAUSE IT WILL BE
// OVERWRITTEN WHEN YOU RE-RUN CODE GENERATION.SetSamplerState(0, SamplerState
//
// Refer to the XMLSPY Documentation for further details.SetSamplerState(0, SamplerState
// http://www.SetSamplerState(0, SamplerStatealtova.SetSamplerState(0, SamplerStatecom/xmlspy
//


using System;
using System.SetSamplerState(0, SamplerStateCollections;
using System.SetSamplerState(0, SamplerStateXml;
using Altova.SetSamplerState(0, SamplerStateTypes;

namespace capabilities_1_1_1
{
	public class ServiceType : Altova.SetSamplerState(0, SamplerStateXml.SetSamplerState(0, SamplerStateNode
	{
		#region Forward constructors
		public ServiceType() : base() {
            this.SetSamplerState(0, SamplerStateSetCollectionParents(); }
		public ServiceType(XmlDocument doc) : base(doc) {
            this.SetSamplerState(0, SamplerStateSetCollectionParents(); }
		public ServiceType(XmlNode node) : base(node) {
            this.SetSamplerState(0, SamplerStateSetCollectionParents(); }
		public ServiceType(Altova.SetSamplerState(0, SamplerStateXml.SetSamplerState(0, SamplerStateNode node) : base(node) {
            this.SetSamplerState(0, SamplerStateSetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

			for (int i = 0; i < this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Name"); i++)
			{
				XmlNode DOMNode = this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Name", i);
				InternalAdjustPrefix(DOMNode, false);
			}

			for (int i = 0; i < this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Title"); i++)
			{
				XmlNode DOMNode = this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Title", i);
				InternalAdjustPrefix(DOMNode, false);
			}

			for (int i = 0; i < this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Abstract"); i++)
			{
				XmlNode DOMNode = this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Abstract", i);
				InternalAdjustPrefix(DOMNode, false);
			}

			for (int i = 0; i < this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "KeywordList"); i++)
			{
				XmlNode DOMNode = this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "KeywordList", i);
				InternalAdjustPrefix(DOMNode, false);
				new KeywordListType(DOMNode).SetSamplerState(0, SamplerStateAdjustPrefix();
			}

			for (int i = 0; i < this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "OnlineResource"); i++)
			{
				XmlNode DOMNode = this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "OnlineResource", i);
				InternalAdjustPrefix(DOMNode, false);
				new OnlineResourceType(DOMNode).SetSamplerState(0, SamplerStateAdjustPrefix();
			}

			for (int i = 0; i < this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "ContactInformation"); i++)
			{
				XmlNode DOMNode = this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "ContactInformation", i);
				InternalAdjustPrefix(DOMNode, false);
				new ContactInformationType(DOMNode).SetSamplerState(0, SamplerStateAdjustPrefix();
			}

			for (int i = 0; i < this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Fees"); i++)
			{
				XmlNode DOMNode = this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Fees", i);
				InternalAdjustPrefix(DOMNode, false);
			}

			for (int i = 0; i < this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "AccessConstraints"); i++)
			{
				XmlNode DOMNode = this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "AccessConstraints", i);
				InternalAdjustPrefix(DOMNode, false);
			}
		}


		#region Name accessor methods
		public int GetNameMinCount()
		{
			return 1;
		}

		public int NameMinCount
		{
			get
			{
				return 1;
			}
		}

		public int GetNameMaxCount()
		{
			return 1;
		}

		public int NameMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetNameCount()
		{
			return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Name");
		}

		public int NameCount
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Name");
			}
		}

		public bool HasName()
		{
			return this.SetSamplerState(0, SamplerStateHasDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "Name");
		}

		public SchemaString GetNameAt(int index)
		{
			return new SchemaString(GetDomNodeValue(this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Name", index)));
		}

		public SchemaString GetName()
		{
			return this.SetSamplerState(0, SamplerStateGetNameAt(0);
		}

		public SchemaString Name
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateGetNameAt(0);
			}
		}

		public void RemoveNameAt(int index)
		{
            this.SetSamplerState(0, SamplerStateRemoveDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Name", index);
		}

		public void RemoveName()
		{
			while (this.SetSamplerState(0, SamplerStateHasName()) this.SetSamplerState(0, SamplerStateRemoveNameAt(0);
		}

		public void AddName(SchemaString newValue)
		{
            this.SetSamplerState(0, SamplerStateAppendDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "Name", newValue.SetSamplerState(0, SamplerStateToString());
		}

		public void InsertNameAt(SchemaString newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateInsertDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Name", index, newValue.SetSamplerState(0, SamplerStateToString());
		}

		public void ReplaceNameAt(SchemaString newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateReplaceDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Name", index, newValue.SetSamplerState(0, SamplerStateToString());
		}
		#endregion // Name accessor methods

		#region Name collection
        public NameCollection	MyNames = new NameCollection( );

        public class NameCollection: IEnumerable
        {
            ServiceType parent;
            public ServiceType Parent
			{
				set
				{
                    this.SetSamplerState(0, SamplerStateparent = value;
				}
			}
			public NameEnumerator GetEnumerator() 
			{
				return new NameEnumerator(this.SetSamplerState(0, SamplerStateparent);
			}
		
			IEnumerator IEnumerable.SetSamplerState(0, SamplerStateGetEnumerator() 
			{
				return this.SetSamplerState(0, SamplerStateGetEnumerator();
			}
        }

        public class NameEnumerator: IEnumerator 
        {
			int nIndex;
			ServiceType parent;
			public NameEnumerator(ServiceType par) 
			{
                this.SetSamplerState(0, SamplerStateparent = par;
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public void Reset() 
			{
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public bool MoveNext() 
			{
                this.SetSamplerState(0, SamplerStatenIndex++;
				return(this.SetSamplerState(0, SamplerStatenIndex < this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateNameCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateGetNameAt(this.SetSamplerState(0, SamplerStatenIndex));
				}
			}
			object IEnumerator.SetSamplerState(0, SamplerStateCurrent 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateCurrent);
				}
			}
    	}

        #endregion // Name collection

		#region Title accessor methods
		public int GetTitleMinCount()
		{
			return 1;
		}

		public int TitleMinCount
		{
			get
			{
				return 1;
			}
		}

		public int GetTitleMaxCount()
		{
			return 1;
		}

		public int TitleMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetTitleCount()
		{
			return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Title");
		}

		public int TitleCount
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Title");
			}
		}

		public bool HasTitle()
		{
			return this.SetSamplerState(0, SamplerStateHasDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "Title");
		}

		public SchemaString GetTitleAt(int index)
		{
			return new SchemaString(GetDomNodeValue(this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Title", index)));
		}

		public SchemaString GetTitle()
		{
			return this.SetSamplerState(0, SamplerStateGetTitleAt(0);
		}

		public SchemaString Title
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateGetTitleAt(0);
			}
		}

		public void RemoveTitleAt(int index)
		{
            this.SetSamplerState(0, SamplerStateRemoveDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Title", index);
		}

		public void RemoveTitle()
		{
			while (this.SetSamplerState(0, SamplerStateHasTitle()) this.SetSamplerState(0, SamplerStateRemoveTitleAt(0);
		}

		public void AddTitle(SchemaString newValue)
		{
            this.SetSamplerState(0, SamplerStateAppendDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "Title", newValue.SetSamplerState(0, SamplerStateToString());
		}

		public void InsertTitleAt(SchemaString newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateInsertDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Title", index, newValue.SetSamplerState(0, SamplerStateToString());
		}

		public void ReplaceTitleAt(SchemaString newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateReplaceDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Title", index, newValue.SetSamplerState(0, SamplerStateToString());
		}
		#endregion // Title accessor methods

		#region Title collection
        public TitleCollection	MyTitles = new TitleCollection( );

        public class TitleCollection: IEnumerable
        {
            ServiceType parent;
            public ServiceType Parent
			{
				set
				{
                    this.SetSamplerState(0, SamplerStateparent = value;
				}
			}
			public TitleEnumerator GetEnumerator() 
			{
				return new TitleEnumerator(this.SetSamplerState(0, SamplerStateparent);
			}
		
			IEnumerator IEnumerable.SetSamplerState(0, SamplerStateGetEnumerator() 
			{
				return this.SetSamplerState(0, SamplerStateGetEnumerator();
			}
        }

        public class TitleEnumerator: IEnumerator 
        {
			int nIndex;
			ServiceType parent;
			public TitleEnumerator(ServiceType par) 
			{
                this.SetSamplerState(0, SamplerStateparent = par;
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public void Reset() 
			{
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public bool MoveNext() 
			{
                this.SetSamplerState(0, SamplerStatenIndex++;
				return(this.SetSamplerState(0, SamplerStatenIndex < this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateTitleCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateGetTitleAt(this.SetSamplerState(0, SamplerStatenIndex));
				}
			}
			object IEnumerator.SetSamplerState(0, SamplerStateCurrent 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateCurrent);
				}
			}
    	}

        #endregion // Title collection

		#region Abstract2 accessor methods
		public int GetAbstract2MinCount()
		{
			return 0;
		}

		public int Abstract2MinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetAbstract2MaxCount()
		{
			return 1;
		}

		public int Abstract2MaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetAbstract2Count()
		{
			return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Abstract");
		}

		public int Abstract2Count
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Abstract");
			}
		}

		public bool HasAbstract2()
		{
			return this.SetSamplerState(0, SamplerStateHasDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "Abstract");
		}

		public SchemaString GetAbstract2At(int index)
		{
			return new SchemaString(GetDomNodeValue(this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Abstract", index)));
		}

		public SchemaString GetAbstract2()
		{
			return this.SetSamplerState(0, SamplerStateGetAbstract2At(0);
		}

		public SchemaString Abstract2
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateGetAbstract2At(0);
			}
		}

		public void RemoveAbstract2At(int index)
		{
            this.SetSamplerState(0, SamplerStateRemoveDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Abstract", index);
		}

		public void RemoveAbstract2()
		{
			while (this.SetSamplerState(0, SamplerStateHasAbstract2()) this.SetSamplerState(0, SamplerStateRemoveAbstract2At(0);
		}

		public void AddAbstract2(SchemaString newValue)
		{
            this.SetSamplerState(0, SamplerStateAppendDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "Abstract", newValue.SetSamplerState(0, SamplerStateToString());
		}

		public void InsertAbstract2At(SchemaString newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateInsertDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Abstract", index, newValue.SetSamplerState(0, SamplerStateToString());
		}

		public void ReplaceAbstract2At(SchemaString newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateReplaceDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Abstract", index, newValue.SetSamplerState(0, SamplerStateToString());
		}
		#endregion // Abstract2 accessor methods

		#region Abstract2 collection
        public Abstract2Collection	MyAbstract2s = new Abstract2Collection( );

        public class Abstract2Collection: IEnumerable
        {
            ServiceType parent;
            public ServiceType Parent
			{
				set
				{
                    this.SetSamplerState(0, SamplerStateparent = value;
				}
			}
			public Abstract2Enumerator GetEnumerator() 
			{
				return new Abstract2Enumerator(this.SetSamplerState(0, SamplerStateparent);
			}
		
			IEnumerator IEnumerable.SetSamplerState(0, SamplerStateGetEnumerator() 
			{
				return this.SetSamplerState(0, SamplerStateGetEnumerator();
			}
        }

        public class Abstract2Enumerator: IEnumerator 
        {
			int nIndex;
			ServiceType parent;
			public Abstract2Enumerator(ServiceType par) 
			{
                this.SetSamplerState(0, SamplerStateparent = par;
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public void Reset() 
			{
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public bool MoveNext() 
			{
                this.SetSamplerState(0, SamplerStatenIndex++;
				return(this.SetSamplerState(0, SamplerStatenIndex < this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateAbstract2Count );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateGetAbstract2At(this.SetSamplerState(0, SamplerStatenIndex));
				}
			}
			object IEnumerator.SetSamplerState(0, SamplerStateCurrent 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateCurrent);
				}
			}
    	}

        #endregion // Abstract2 collection

		#region KeywordList accessor methods
		public int GetKeywordListMinCount()
		{
			return 0;
		}

		public int KeywordListMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetKeywordListMaxCount()
		{
			return 1;
		}

		public int KeywordListMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetKeywordListCount()
		{
			return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "KeywordList");
		}

		public int KeywordListCount
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "KeywordList");
			}
		}

		public bool HasKeywordList()
		{
			return this.SetSamplerState(0, SamplerStateHasDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "KeywordList");
		}

		public KeywordListType GetKeywordListAt(int index)
		{
			return new KeywordListType(this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "KeywordList", index));
		}

		public KeywordListType GetKeywordList()
		{
			return this.SetSamplerState(0, SamplerStateGetKeywordListAt(0);
		}

		public KeywordListType KeywordList
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateGetKeywordListAt(0);
			}
		}

		public void RemoveKeywordListAt(int index)
		{
            this.SetSamplerState(0, SamplerStateRemoveDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "KeywordList", index);
		}

		public void RemoveKeywordList()
		{
			while (this.SetSamplerState(0, SamplerStateHasKeywordList()) this.SetSamplerState(0, SamplerStateRemoveKeywordListAt(0);
		}

		public void AddKeywordList(KeywordListType newValue)
		{
            this.SetSamplerState(0, SamplerStateAppendDomElement("", "KeywordList", newValue);
		}

		public void InsertKeywordListAt(KeywordListType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateInsertDomElementAt("", "KeywordList", index, newValue);
		}

		public void ReplaceKeywordListAt(KeywordListType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateReplaceDomElementAt("", "KeywordList", index, newValue);
		}
		#endregion // KeywordList accessor methods

		#region KeywordList collection
        public KeywordListCollection	MyKeywordLists = new KeywordListCollection( );

        public class KeywordListCollection: IEnumerable
        {
            ServiceType parent;
            public ServiceType Parent
			{
				set
				{
                    this.SetSamplerState(0, SamplerStateparent = value;
				}
			}
			public KeywordListEnumerator GetEnumerator() 
			{
				return new KeywordListEnumerator(this.SetSamplerState(0, SamplerStateparent);
			}
		
			IEnumerator IEnumerable.SetSamplerState(0, SamplerStateGetEnumerator() 
			{
				return this.SetSamplerState(0, SamplerStateGetEnumerator();
			}
        }

        public class KeywordListEnumerator: IEnumerator 
        {
			int nIndex;
			ServiceType parent;
			public KeywordListEnumerator(ServiceType par) 
			{
                this.SetSamplerState(0, SamplerStateparent = par;
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public void Reset() 
			{
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public bool MoveNext() 
			{
                this.SetSamplerState(0, SamplerStatenIndex++;
				return(this.SetSamplerState(0, SamplerStatenIndex < this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateKeywordListCount );
			}
			public KeywordListType  Current 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateGetKeywordListAt(this.SetSamplerState(0, SamplerStatenIndex));
				}
			}
			object IEnumerator.SetSamplerState(0, SamplerStateCurrent 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateCurrent);
				}
			}
    	}

        #endregion // KeywordList collection

		#region OnlineResource accessor methods
		public int GetOnlineResourceMinCount()
		{
			return 1;
		}

		public int OnlineResourceMinCount
		{
			get
			{
				return 1;
			}
		}

		public int GetOnlineResourceMaxCount()
		{
			return 1;
		}

		public int OnlineResourceMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetOnlineResourceCount()
		{
			return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "OnlineResource");
		}

		public int OnlineResourceCount
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "OnlineResource");
			}
		}

		public bool HasOnlineResource()
		{
			return this.SetSamplerState(0, SamplerStateHasDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "OnlineResource");
		}

		public OnlineResourceType GetOnlineResourceAt(int index)
		{
			return new OnlineResourceType(this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "OnlineResource", index));
		}

		public OnlineResourceType GetOnlineResource()
		{
			return this.SetSamplerState(0, SamplerStateGetOnlineResourceAt(0);
		}

		public OnlineResourceType OnlineResource
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateGetOnlineResourceAt(0);
			}
		}

		public void RemoveOnlineResourceAt(int index)
		{
            this.SetSamplerState(0, SamplerStateRemoveDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "OnlineResource", index);
		}

		public void RemoveOnlineResource()
		{
			while (this.SetSamplerState(0, SamplerStateHasOnlineResource()) this.SetSamplerState(0, SamplerStateRemoveOnlineResourceAt(0);
		}

		public void AddOnlineResource(OnlineResourceType newValue)
		{
            this.SetSamplerState(0, SamplerStateAppendDomElement("", "OnlineResource", newValue);
		}

		public void InsertOnlineResourceAt(OnlineResourceType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateInsertDomElementAt("", "OnlineResource", index, newValue);
		}

		public void ReplaceOnlineResourceAt(OnlineResourceType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateReplaceDomElementAt("", "OnlineResource", index, newValue);
		}
		#endregion // OnlineResource accessor methods

		#region OnlineResource collection
        public OnlineResourceCollection	MyOnlineResources = new OnlineResourceCollection( );

        public class OnlineResourceCollection: IEnumerable
        {
            ServiceType parent;
            public ServiceType Parent
			{
				set
				{
                    this.SetSamplerState(0, SamplerStateparent = value;
				}
			}
			public OnlineResourceEnumerator GetEnumerator() 
			{
				return new OnlineResourceEnumerator(this.SetSamplerState(0, SamplerStateparent);
			}
		
			IEnumerator IEnumerable.SetSamplerState(0, SamplerStateGetEnumerator() 
			{
				return this.SetSamplerState(0, SamplerStateGetEnumerator();
			}
        }

        public class OnlineResourceEnumerator: IEnumerator 
        {
			int nIndex;
			ServiceType parent;
			public OnlineResourceEnumerator(ServiceType par) 
			{
                this.SetSamplerState(0, SamplerStateparent = par;
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public void Reset() 
			{
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public bool MoveNext() 
			{
                this.SetSamplerState(0, SamplerStatenIndex++;
				return(this.SetSamplerState(0, SamplerStatenIndex < this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateOnlineResourceCount );
			}
			public OnlineResourceType  Current 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateGetOnlineResourceAt(this.SetSamplerState(0, SamplerStatenIndex));
				}
			}
			object IEnumerator.SetSamplerState(0, SamplerStateCurrent 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateCurrent);
				}
			}
    	}

        #endregion // OnlineResource collection

		#region ContactInformation accessor methods
		public int GetContactInformationMinCount()
		{
			return 0;
		}

		public int ContactInformationMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetContactInformationMaxCount()
		{
			return 1;
		}

		public int ContactInformationMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetContactInformationCount()
		{
			return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "ContactInformation");
		}

		public int ContactInformationCount
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "ContactInformation");
			}
		}

		public bool HasContactInformation()
		{
			return this.SetSamplerState(0, SamplerStateHasDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "ContactInformation");
		}

		public ContactInformationType GetContactInformationAt(int index)
		{
			return new ContactInformationType(this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "ContactInformation", index));
		}

		public ContactInformationType GetContactInformation()
		{
			return this.SetSamplerState(0, SamplerStateGetContactInformationAt(0);
		}

		public ContactInformationType ContactInformation
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateGetContactInformationAt(0);
			}
		}

		public void RemoveContactInformationAt(int index)
		{
            this.SetSamplerState(0, SamplerStateRemoveDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "ContactInformation", index);
		}

		public void RemoveContactInformation()
		{
			while (this.SetSamplerState(0, SamplerStateHasContactInformation()) this.SetSamplerState(0, SamplerStateRemoveContactInformationAt(0);
		}

		public void AddContactInformation(ContactInformationType newValue)
		{
            this.SetSamplerState(0, SamplerStateAppendDomElement("", "ContactInformation", newValue);
		}

		public void InsertContactInformationAt(ContactInformationType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateInsertDomElementAt("", "ContactInformation", index, newValue);
		}

		public void ReplaceContactInformationAt(ContactInformationType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateReplaceDomElementAt("", "ContactInformation", index, newValue);
		}
		#endregion // ContactInformation accessor methods

		#region ContactInformation collection
        public ContactInformationCollection	MyContactInformations = new ContactInformationCollection( );

        public class ContactInformationCollection: IEnumerable
        {
            ServiceType parent;
            public ServiceType Parent
			{
				set
				{
                    this.SetSamplerState(0, SamplerStateparent = value;
				}
			}
			public ContactInformationEnumerator GetEnumerator() 
			{
				return new ContactInformationEnumerator(this.SetSamplerState(0, SamplerStateparent);
			}
		
			IEnumerator IEnumerable.SetSamplerState(0, SamplerStateGetEnumerator() 
			{
				return this.SetSamplerState(0, SamplerStateGetEnumerator();
			}
        }

        public class ContactInformationEnumerator: IEnumerator 
        {
			int nIndex;
			ServiceType parent;
			public ContactInformationEnumerator(ServiceType par) 
			{
                this.SetSamplerState(0, SamplerStateparent = par;
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public void Reset() 
			{
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public bool MoveNext() 
			{
                this.SetSamplerState(0, SamplerStatenIndex++;
				return(this.SetSamplerState(0, SamplerStatenIndex < this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateContactInformationCount );
			}
			public ContactInformationType  Current 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateGetContactInformationAt(this.SetSamplerState(0, SamplerStatenIndex));
				}
			}
			object IEnumerator.SetSamplerState(0, SamplerStateCurrent 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateCurrent);
				}
			}
    	}

        #endregion // ContactInformation collection

		#region Fees accessor methods
		public int GetFeesMinCount()
		{
			return 0;
		}

		public int FeesMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetFeesMaxCount()
		{
			return 1;
		}

		public int FeesMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetFeesCount()
		{
			return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Fees");
		}

		public int FeesCount
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "Fees");
			}
		}

		public bool HasFees()
		{
			return this.SetSamplerState(0, SamplerStateHasDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "Fees");
		}

		public SchemaString GetFeesAt(int index)
		{
			return new SchemaString(GetDomNodeValue(this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Fees", index)));
		}

		public SchemaString GetFees()
		{
			return this.SetSamplerState(0, SamplerStateGetFeesAt(0);
		}

		public SchemaString Fees
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateGetFeesAt(0);
			}
		}

		public void RemoveFeesAt(int index)
		{
            this.SetSamplerState(0, SamplerStateRemoveDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Fees", index);
		}

		public void RemoveFees()
		{
			while (this.SetSamplerState(0, SamplerStateHasFees()) this.SetSamplerState(0, SamplerStateRemoveFeesAt(0);
		}

		public void AddFees(SchemaString newValue)
		{
            this.SetSamplerState(0, SamplerStateAppendDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "Fees", newValue.SetSamplerState(0, SamplerStateToString());
		}

		public void InsertFeesAt(SchemaString newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateInsertDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Fees", index, newValue.SetSamplerState(0, SamplerStateToString());
		}

		public void ReplaceFeesAt(SchemaString newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateReplaceDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "Fees", index, newValue.SetSamplerState(0, SamplerStateToString());
		}
		#endregion // Fees accessor methods

		#region Fees collection
        public FeesCollection	MyFeess = new FeesCollection( );

        public class FeesCollection: IEnumerable
        {
            ServiceType parent;
            public ServiceType Parent
			{
				set
				{
                    this.SetSamplerState(0, SamplerStateparent = value;
				}
			}
			public FeesEnumerator GetEnumerator() 
			{
				return new FeesEnumerator(this.SetSamplerState(0, SamplerStateparent);
			}
		
			IEnumerator IEnumerable.SetSamplerState(0, SamplerStateGetEnumerator() 
			{
				return this.SetSamplerState(0, SamplerStateGetEnumerator();
			}
        }

        public class FeesEnumerator: IEnumerator 
        {
			int nIndex;
			ServiceType parent;
			public FeesEnumerator(ServiceType par) 
			{
                this.SetSamplerState(0, SamplerStateparent = par;
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public void Reset() 
			{
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public bool MoveNext() 
			{
                this.SetSamplerState(0, SamplerStatenIndex++;
				return(this.SetSamplerState(0, SamplerStatenIndex < this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateFeesCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateGetFeesAt(this.SetSamplerState(0, SamplerStatenIndex));
				}
			}
			object IEnumerator.SetSamplerState(0, SamplerStateCurrent 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateCurrent);
				}
			}
    	}

        #endregion // Fees collection

		#region AccessConstraints accessor methods
		public int GetAccessConstraintsMinCount()
		{
			return 0;
		}

		public int AccessConstraintsMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetAccessConstraintsMaxCount()
		{
			return 1;
		}

		public int AccessConstraintsMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetAccessConstraintsCount()
		{
			return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "AccessConstraints");
		}

		public int AccessConstraintsCount
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "", "AccessConstraints");
			}
		}

		public bool HasAccessConstraints()
		{
			return this.SetSamplerState(0, SamplerStateHasDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "AccessConstraints");
		}

		public SchemaString GetAccessConstraintsAt(int index)
		{
			return new SchemaString(GetDomNodeValue(this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "AccessConstraints", index)));
		}

		public SchemaString GetAccessConstraints()
		{
			return this.SetSamplerState(0, SamplerStateGetAccessConstraintsAt(0);
		}

		public SchemaString AccessConstraints
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateGetAccessConstraintsAt(0);
			}
		}

		public void RemoveAccessConstraintsAt(int index)
		{
            this.SetSamplerState(0, SamplerStateRemoveDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "AccessConstraints", index);
		}

		public void RemoveAccessConstraints()
		{
			while (this.SetSamplerState(0, SamplerStateHasAccessConstraints()) this.SetSamplerState(0, SamplerStateRemoveAccessConstraintsAt(0);
		}

		public void AddAccessConstraints(SchemaString newValue)
		{
            this.SetSamplerState(0, SamplerStateAppendDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "", "AccessConstraints", newValue.SetSamplerState(0, SamplerStateToString());
		}

		public void InsertAccessConstraintsAt(SchemaString newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateInsertDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "AccessConstraints", index, newValue.SetSamplerState(0, SamplerStateToString());
		}

		public void ReplaceAccessConstraintsAt(SchemaString newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateReplaceDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "", "AccessConstraints", index, newValue.SetSamplerState(0, SamplerStateToString());
		}
		#endregion // AccessConstraints accessor methods

		#region AccessConstraints collection
        public AccessConstraintsCollection	MyAccessConstraintss = new AccessConstraintsCollection( );

        public class AccessConstraintsCollection: IEnumerable
        {
            ServiceType parent;
            public ServiceType Parent
			{
				set
				{
                    this.SetSamplerState(0, SamplerStateparent = value;
				}
			}
			public AccessConstraintsEnumerator GetEnumerator() 
			{
				return new AccessConstraintsEnumerator(this.SetSamplerState(0, SamplerStateparent);
			}
		
			IEnumerator IEnumerable.SetSamplerState(0, SamplerStateGetEnumerator() 
			{
				return this.SetSamplerState(0, SamplerStateGetEnumerator();
			}
        }

        public class AccessConstraintsEnumerator: IEnumerator 
        {
			int nIndex;
			ServiceType parent;
			public AccessConstraintsEnumerator(ServiceType par) 
			{
                this.SetSamplerState(0, SamplerStateparent = par;
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public void Reset() 
			{
                this.SetSamplerState(0, SamplerStatenIndex = -1;
			}
			public bool MoveNext() 
			{
                this.SetSamplerState(0, SamplerStatenIndex++;
				return(this.SetSamplerState(0, SamplerStatenIndex < this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateAccessConstraintsCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateGetAccessConstraintsAt(this.SetSamplerState(0, SamplerStatenIndex));
				}
			}
			object IEnumerator.SetSamplerState(0, SamplerStateCurrent 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateCurrent);
				}
			}
    	}

        #endregion // AccessConstraints collection

        private void SetCollectionParents()
        {
            this.SetSamplerState(0, SamplerStateMyNames.SetSamplerState(0, SamplerStateParent = this;
            this.SetSamplerState(0, SamplerStateMyTitles.SetSamplerState(0, SamplerStateParent = this;
            this.SetSamplerState(0, SamplerStateMyAbstract2s.SetSamplerState(0, SamplerStateParent = this;
            this.SetSamplerState(0, SamplerStateMyKeywordLists.SetSamplerState(0, SamplerStateParent = this;
            this.SetSamplerState(0, SamplerStateMyOnlineResources.SetSamplerState(0, SamplerStateParent = this;
            this.SetSamplerState(0, SamplerStateMyContactInformations.SetSamplerState(0, SamplerStateParent = this;
            this.SetSamplerState(0, SamplerStateMyFeess.SetSamplerState(0, SamplerStateParent = this;
            this.SetSamplerState(0, SamplerStateMyAccessConstraintss.SetSamplerState(0, SamplerStateParent = this; 
	}
}
}
