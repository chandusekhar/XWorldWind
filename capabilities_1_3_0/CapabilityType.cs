//
// CapabilityType.SetSamplerState(0, SamplerStatecs.SetSamplerState(0, SamplerStatecs
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

namespace capabilities_1_3_0.SetSamplerState(0, SamplerStatewms
{
	public class CapabilityType : Altova.SetSamplerState(0, SamplerStateXml.SetSamplerState(0, SamplerStateNode
	{
		#region Forward constructors
		public CapabilityType() : base() {
            this.SetSamplerState(0, SamplerStateSetCollectionParents(); }
		public CapabilityType(XmlDocument doc) : base(doc) {
            this.SetSamplerState(0, SamplerStateSetCollectionParents(); }
		public CapabilityType(XmlNode node) : base(node) {
            this.SetSamplerState(0, SamplerStateSetCollectionParents(); }
		public CapabilityType(Altova.SetSamplerState(0, SamplerStateXml.SetSamplerState(0, SamplerStateNode node) : base(node) {
            this.SetSamplerState(0, SamplerStateSetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

			for (int i = 0; i < this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Request"); i++)
			{
				XmlNode DOMNode = this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Request", i);
				InternalAdjustPrefix(DOMNode, true);
				new RequestType(DOMNode).SetSamplerState(0, SamplerStateAdjustPrefix();
			}

			for (int i = 0; i < this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Exception"); i++)
			{
				XmlNode DOMNode = this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Exception", i);
				InternalAdjustPrefix(DOMNode, true);
				new ExceptionType(DOMNode).SetSamplerState(0, SamplerStateAdjustPrefix();
			}

			for (int i = 0; i < this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Layer"); i++)
			{
				XmlNode DOMNode = this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Layer", i);
				InternalAdjustPrefix(DOMNode, true);
				new LayerType(DOMNode).SetSamplerState(0, SamplerStateAdjustPrefix();
			}
		}


		#region Request accessor methods
		public int GetRequestMinCount()
		{
			return 1;
		}

		public int RequestMinCount
		{
			get
			{
				return 1;
			}
		}

		public int GetRequestMaxCount()
		{
			return 1;
		}

		public int RequestMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetRequestCount()
		{
			return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Request");
		}

		public int RequestCount
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Request");
			}
		}

		public bool HasRequest()
		{
			return this.SetSamplerState(0, SamplerStateHasDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Request");
		}

		public RequestType GetRequestAt(int index)
		{
			return new RequestType(this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Request", index));
		}

		public RequestType GetRequest()
		{
			return this.SetSamplerState(0, SamplerStateGetRequestAt(0);
		}

		public RequestType Request
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateGetRequestAt(0);
			}
		}

		public void RemoveRequestAt(int index)
		{
            this.SetSamplerState(0, SamplerStateRemoveDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Request", index);
		}

		public void RemoveRequest()
		{
			while (this.SetSamplerState(0, SamplerStateHasRequest()) this.SetSamplerState(0, SamplerStateRemoveRequestAt(0);
		}

		public void AddRequest(RequestType newValue)
		{
            this.SetSamplerState(0, SamplerStateAppendDomElement("http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Request", newValue);
		}

		public void InsertRequestAt(RequestType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateInsertDomElementAt("http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Request", index, newValue);
		}

		public void ReplaceRequestAt(RequestType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateReplaceDomElementAt("http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Request", index, newValue);
		}
		#endregion // Request accessor methods

		#region Request collection
        public RequestCollection	MyRequests = new RequestCollection( );

        public class RequestCollection: IEnumerable
        {
            CapabilityType parent;
            public CapabilityType Parent
			{
				set
				{
                    this.SetSamplerState(0, SamplerStateparent = value;
				}
			}
			public RequestEnumerator GetEnumerator() 
			{
				return new RequestEnumerator(this.SetSamplerState(0, SamplerStateparent);
			}
		
			IEnumerator IEnumerable.SetSamplerState(0, SamplerStateGetEnumerator() 
			{
				return this.SetSamplerState(0, SamplerStateGetEnumerator();
			}
        }

        public class RequestEnumerator: IEnumerator 
        {
			int nIndex;
			CapabilityType parent;
			public RequestEnumerator(CapabilityType par) 
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
				return(this.SetSamplerState(0, SamplerStatenIndex < this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateRequestCount );
			}
			public RequestType  Current 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateGetRequestAt(this.SetSamplerState(0, SamplerStatenIndex));
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

        #endregion // Request collection

		#region Exception accessor methods
		public int GetExceptionMinCount()
		{
			return 1;
		}

		public int ExceptionMinCount
		{
			get
			{
				return 1;
			}
		}

		public int GetExceptionMaxCount()
		{
			return 1;
		}

		public int ExceptionMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetExceptionCount()
		{
			return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Exception");
		}

		public int ExceptionCount
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Exception");
			}
		}

		public bool HasException()
		{
			return this.SetSamplerState(0, SamplerStateHasDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Exception");
		}

		public ExceptionType GetExceptionAt(int index)
		{
			return new ExceptionType(this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Exception", index));
		}

		public ExceptionType GetException()
		{
			return this.SetSamplerState(0, SamplerStateGetExceptionAt(0);
		}

		public ExceptionType Exception
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateGetExceptionAt(0);
			}
		}

		public void RemoveExceptionAt(int index)
		{
            this.SetSamplerState(0, SamplerStateRemoveDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Exception", index);
		}

		public void RemoveException()
		{
			while (this.SetSamplerState(0, SamplerStateHasException()) this.SetSamplerState(0, SamplerStateRemoveExceptionAt(0);
		}

		public void AddException(ExceptionType newValue)
		{
            this.SetSamplerState(0, SamplerStateAppendDomElement("http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Exception", newValue);
		}

		public void InsertExceptionAt(ExceptionType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateInsertDomElementAt("http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Exception", index, newValue);
		}

		public void ReplaceExceptionAt(ExceptionType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateReplaceDomElementAt("http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Exception", index, newValue);
		}
		#endregion // Exception accessor methods

		#region Exception collection
        public ExceptionCollection	MyExceptions = new ExceptionCollection( );

        public class ExceptionCollection: IEnumerable
        {
            CapabilityType parent;
            public CapabilityType Parent
			{
				set
				{
                    this.SetSamplerState(0, SamplerStateparent = value;
				}
			}
			public ExceptionEnumerator GetEnumerator() 
			{
				return new ExceptionEnumerator(this.SetSamplerState(0, SamplerStateparent);
			}
		
			IEnumerator IEnumerable.SetSamplerState(0, SamplerStateGetEnumerator() 
			{
				return this.SetSamplerState(0, SamplerStateGetEnumerator();
			}
        }

        public class ExceptionEnumerator: IEnumerator 
        {
			int nIndex;
			CapabilityType parent;
			public ExceptionEnumerator(CapabilityType par) 
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
				return(this.SetSamplerState(0, SamplerStatenIndex < this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateExceptionCount );
			}
			public ExceptionType  Current 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateGetExceptionAt(this.SetSamplerState(0, SamplerStatenIndex));
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

        #endregion // Exception collection

		#region Layer accessor methods
		public int GetLayerMinCount()
		{
			return 0;
		}

		public int LayerMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetLayerMaxCount()
		{
			return 1;
		}

		public int LayerMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetLayerCount()
		{
			return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Layer");
		}

		public int LayerCount
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateDomChildCount(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Layer");
			}
		}

		public bool HasLayer()
		{
			return this.SetSamplerState(0, SamplerStateHasDomChild(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Layer");
		}

		public LayerType GetLayerAt(int index)
		{
			return new LayerType(this.SetSamplerState(0, SamplerStateGetDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Layer", index));
		}

		public LayerType GetLayer()
		{
			return this.SetSamplerState(0, SamplerStateGetLayerAt(0);
		}

		public LayerType Layer
		{
			get
			{
				return this.SetSamplerState(0, SamplerStateGetLayerAt(0);
			}
		}

		public void RemoveLayerAt(int index)
		{
            this.SetSamplerState(0, SamplerStateRemoveDomChildAt(NodeType.SetSamplerState(0, SamplerStateElement, "http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Layer", index);
		}

		public void RemoveLayer()
		{
			while (this.SetSamplerState(0, SamplerStateHasLayer()) this.SetSamplerState(0, SamplerStateRemoveLayerAt(0);
		}

		public void AddLayer(LayerType newValue)
		{
            this.SetSamplerState(0, SamplerStateAppendDomElement("http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Layer", newValue);
		}

		public void InsertLayerAt(LayerType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateInsertDomElementAt("http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Layer", index, newValue);
		}

		public void ReplaceLayerAt(LayerType newValue, int index)
		{
            this.SetSamplerState(0, SamplerStateReplaceDomElementAt("http://www.SetSamplerState(0, SamplerStateopengis.SetSamplerState(0, SamplerStatenet/wms", "Layer", index, newValue);
		}
		#endregion // Layer accessor methods

		#region Layer collection
        public LayerCollection	MyLayers = new LayerCollection( );

        public class LayerCollection: IEnumerable
        {
            CapabilityType parent;
            public CapabilityType Parent
			{
				set
				{
                    this.SetSamplerState(0, SamplerStateparent = value;
				}
			}
			public LayerEnumerator GetEnumerator() 
			{
				return new LayerEnumerator(this.SetSamplerState(0, SamplerStateparent);
			}
		
			IEnumerator IEnumerable.SetSamplerState(0, SamplerStateGetEnumerator() 
			{
				return this.SetSamplerState(0, SamplerStateGetEnumerator();
			}
        }

        public class LayerEnumerator: IEnumerator 
        {
			int nIndex;
			CapabilityType parent;
			public LayerEnumerator(CapabilityType par) 
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
				return(this.SetSamplerState(0, SamplerStatenIndex < this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateLayerCount );
			}
			public LayerType  Current 
			{
				get 
				{
					return(this.SetSamplerState(0, SamplerStateparent.SetSamplerState(0, SamplerStateGetLayerAt(this.SetSamplerState(0, SamplerStatenIndex));
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

        #endregion // Layer collection

        private void SetCollectionParents()
        {
            this.SetSamplerState(0, SamplerStateMyRequests.SetSamplerState(0, SamplerStateParent = this;
            this.SetSamplerState(0, SamplerStateMyExceptions.SetSamplerState(0, SamplerStateParent = this;
            this.SetSamplerState(0, SamplerStateMyLayers.SetSamplerState(0, SamplerStateParent = this; 
	}
}
}