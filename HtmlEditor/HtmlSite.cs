using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using WorldWind;

namespace onlyconnect
{
    /// <summary>
    /// Implements the site on which mshtml is hosted
    /// </summary>
    /// 
    public class HtmlSite :
        IDisposable,
        IOleClientSite,
        IOleContainer,
        IDocHostUIHandler,
        IOleInPlaceFrame,
        IOleInPlaceSite,
        IOleInPlaceSiteEx,
        IOleDocumentSite,
        IAdviseSink,
        IHTMLEditDesigner,
        IServiceProvider,
        IDocHostShowUI,
        IOleInPlaceUIWindow,
        HTMLDocumentEvents2,
        IPropertyNotifySink
    {
        HtmlEditor container;
        IOleObject m_document;
        internal IOleDocumentView view;
        IOleInPlaceActiveObject activeObject;
        int iAdviseCookie;
        int iEventsCookie;
        internal int iPropertyNotifyCookie;
        internal IConnectionPoint icp;
        IntPtr m_docHwnd = IntPtr.Zero;
        internal bool mFullyActive;

        ~HtmlSite()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            //doc should already be closed, this
            //is defensive
            this.CloseDocument();
        }


        [DispId(dispids.DISPID_AMBIENT_DLCONTROL)]
        public int setFlags()
        {
            //This determines which features are enabled in the editor

            if (this.container.mEnableActiveContent)
            {
                return (int)constants.DLCTL_DLIMAGES | (int)constants.DLCTL_VIDEOS
                    | (int)constants.DLCTL_BGSOUNDS | 0;

            }
            else
            {
                return (int)constants.DLCTL_NO_SCRIPTS | (int)constants.DLCTL_NO_JAVA | (int)constants.DLCTL_NO_DLACTIVEXCTLS
                    | (int)constants.DLCTL_NO_RUNACTIVEXCTLS | (int)constants.DLCTL_SILENT | (int)constants.DLCTL_DLIMAGES | 0;
            }

        }

        public HtmlSite(HtmlEditor container)
        {
            if ((container == null) || (container.IsHandleCreated == false)) throw
                                                                                 new ArgumentException();
            this.container = container;
            container.Resize += new EventHandler(this.Container_Resize);
        }

        public Object Document
        {
            get { return this.m_document; }
        }

        public IntPtr DocumentHandle
        {
            get { return this.m_docHwnd; }
        }

        public void CreateDocument()
        {
            Debug.Assert(this.m_document == null, "Must call Close before recreating.");

            Boolean created = false;
            try
            {
                this.m_document = (IOleObject)new HTMLDocument();

                int iRetval;

                iRetval = win32.OleRun(this.m_document);

                iRetval = this.m_document.SetClientSite(this);

                Debug.Assert(iRetval == HRESULT.S_OK, "SetClientSite failed");

                // Lock the object in memory
                iRetval = win32.OleLockRunning(this.m_document, true, false);

                this.m_document.SetHostNames("HtmlEditor", "HtmlEditor");
                this.m_document.Advise(this, out this.iAdviseCookie);

                //hook up HTMLDocumentEvents2
                Guid guid = new Guid("3050f613-98b5-11cf-bb82-00aa00bdce0b");
                IConnectionPointContainer icpc = (IConnectionPointContainer) this.m_document;

                icpc.FindConnectionPoint(ref guid, out this.icp);
                this.icp.Advise(this, out this.iEventsCookie);

                created = true;
            }
            finally
            {
                if (created == false) this.m_document = null;
            }
        }

        internal void SetPropertyNotifyEvents()
        {
            //hook up PropertyNotify events
            //get IPropertyNotifySink interface
            Guid g = new Guid("9BFBBC02-EFF1-101A-84ED-00AA00341D07");

            IConnectionPointContainer icpc = (IConnectionPointContainer) this.container.Document;

            icpc.FindConnectionPoint(ref g, out this.icp);

            //pass a pointer to the host to the connection point
            this.icp.Advise(this, out this.iPropertyNotifyCookie);

        }

        public void ActivateDocument()
        {
            if (this.m_document == null) return;
            if (!this.container.Visible) return;
            RECT rect = new RECT();
            win32.GetClientRect(this.container.Handle, rect);
            int iRetVal = this.m_document.DoVerb(OLEIVERB.UIACTIVATE, IntPtr.Zero, this, 0, this.container.Handle, rect);

            if (iRetVal == 0)
            {
                this.container.bNeedsActivation = false;
            }
        }

        public void CloseDocument()
        {
            try
            {
                this.container.releaseWndProc();
                this.container.Resize -= new EventHandler(this.Container_Resize);

                if (this.m_document == null) return;

                try
                {
                    //this may raise an exception, however it does work and must
                    //be called
                    if (this.view != null)
                    {
                        this.view.Show(-1);
                        this.view.UIActivate(-1);
                        this.view.SetInPlaceSite(null);
                        this.view.CloseView(0);
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine("CloseView raised exception: " + e.Message);
                }

                try
                {
                    //this could raise an exception too, but it must be called
                    this.m_document.Close((int)tagOLECLOSE.OLECLOSE_NOSAVE);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Close document raised exception: " + e.Message);
                }

                this.m_document.SetClientSite(null);

                win32.OleLockRunning(this.m_document, false, false);

                if (this.iAdviseCookie != 0)
                {
                    this.m_document.Unadvise(this.iAdviseCookie);
                }

                if (this.iEventsCookie != 0)
                {
                    this.m_document.Unadvise(this.iEventsCookie);
                }

                if (this.iPropertyNotifyCookie != 0)
                {
                    this.m_document.Unadvise(this.iPropertyNotifyCookie);
                }

                if (this.container.changeCookie != 0)
                {
                    ((IMarkupContainer2) this.m_document).UnRegisterForDirtyRange(this.container.changeCookie);
                    this.container.changeCookie = 0;
                }

                //release COM objects
                int RefCount = 0;

                if (this.m_document != null)
                    do
                    {
                        RefCount = Marshal.ReleaseComObject(this.m_document);
                    } while (RefCount > 0);

                if (this.view != null)

                    do
                    {
                        RefCount = Marshal.ReleaseComObject(this.view);
                    } while (RefCount > 0);

                if (this.activeObject != null)

                    do
                    {
                        RefCount = Marshal.ReleaseComObject(this.activeObject);
                    } while (RefCount > 0);

                this.m_document = null;
                this.view = null;
                this.activeObject = null;
                this.container.mHtmlDoc = null;
                this.container.mDocHTML = null;

            }
            catch (Exception e)
            {
                Debug.WriteLine("CloseDocument raised exception: " + e.Message);

            }
        }

        void Container_Resize(Object src, EventArgs e)
        {
            if (this.view == null) return;
            RECT rect = new RECT();
            win32.GetClientRect(this.container.Handle, rect);
            this.view.SetRect(rect);
        }

        public Boolean CallTranslateAccelerator(MSG msg)
        {
            //returns true if Mshtml handled the message
            if (this.activeObject != null)
                if (this.activeObject.TranslateAccelerator(msg) != HRESULT.S_FALSE)
                    return true;

            return false;
        }

        #region IOleClientSite

        // IOleClientSite

        public int SaveObject()
        {

            return HRESULT.S_OK;
        }

        public int GetMoniker(uint dwAssign, uint dwWhichMoniker, out Object ppmk)
        {

            ppmk = null;
            return HRESULT.E_NOTIMPL;
        }

        public int GetContainer(out IOleContainer ppContainer)
        {

            ppContainer = (IOleContainer)this;
            return HRESULT.S_OK;
        }

        public int ShowObject()
        {

            return HRESULT.S_OK;
        }

        public int OnShowWindow(int fShow)
        {

            return HRESULT.S_OK;
        }

        public int RequestNewObjectLayout()
        {

            return HRESULT.S_OK;
        }

        #endregion

        #region IOleContainer

        // IOleContainer
        /*
                 *  used to enumerate objects in a compound document
                 *  or lock a container in the running state. 
                 * 
                 * Container and object applications both implement this interface
                 * */

        public int ParseDisplayName(Object pbc, String pszDisplayName, int[]
            pchEaten, Object[] ppmkOut)
        {

            return HRESULT.E_NOTIMPL;
        }

        public int EnumObjects(uint grfFlags, Object[] ppenum)
        {
            return HRESULT.E_NOTIMPL;
        }

        public int LockContainer(bool fLock)
        {
            return HRESULT.S_OK;
        }

        #endregion

        #region IOleDocumentSite

        // IOleDocumentSite Implementation

        public int ActivateMe(IOleDocumentView pViewToActivate)
        {
            Debug.Assert(pViewToActivate != null,
                "The view to activate was null");

            if (pViewToActivate == null) return HRESULT.E_INVALIDARG;
            RECT rect = new RECT();
            win32.GetClientRect(this.container.Handle, rect);
            this.view = pViewToActivate;
            int iResult = this.view.SetInPlaceSite((IOleInPlaceSite)this);
            iResult = this.view.UIActivate(1);
            iResult = this.view.SetRect(rect);
            int iShow = 1;
            iResult = this.view.Show(iShow);  //1 is a boolean for True

            return HRESULT.S_OK;

        }

        #endregion

        #region IOleWindow
        //IOleWindow implementation

        public int GetWindow(ref IntPtr hwnd)
        {

            hwnd = IntPtr.Zero;

            if (this.container != null)
            {
                hwnd = this.container.Handle;
                return HRESULT.S_OK;
            }
            else
            {
                return HRESULT.E_FAIL;
            }
        }

        #endregion

        #region IOleInPlaceSite

        // IOleInPlaceSite Implementation
        public int ContextSensitiveHelp(bool fEnterMode)
        {
            return HRESULT.E_NOTIMPL;
        }

        public int CanInPlaceActivate()
        {

            return HRESULT.S_OK;
        }

        public int OnInPlaceActivate()
        {

            return HRESULT.S_OK;
        }

        public int OnUIActivate()
        {
            //return HESULT.S_FALSE prevents focus grab
            //but means no caret
            //return HRESULT.S_FALSE;
            return HRESULT.S_OK;
        }

        public int GetWindowContext(out IOleInPlaceFrame ppFrame, out IOleInPlaceUIWindow
            ppDoc, RECT lprcPosRect, RECT lprcClipRect, tagOIFI lpFrameInfo)
        {

            ppDoc = null; //set to null because same as Frame window
            ppFrame = (IOleInPlaceFrame)this;
            if (lprcPosRect != null)
            {
                win32.GetClientRect(this.container.Handle, lprcPosRect);
            }

            if (lprcClipRect != null)
            {
                win32.GetClientRect(this.container.Handle, lprcClipRect);
            }

            //lpFrameInfo.cb = Marshal.SizeOf(typeof(tagOIFI));
            //This value is set by the caller

            lpFrameInfo.fMDIApp = 0;
            lpFrameInfo.hwndFrame = this.container.Handle;
            lpFrameInfo.hAccel = IntPtr.Zero;
            lpFrameInfo.cAccelEntries = 0;
            return HRESULT.S_OK;
        }

        public int Scroll(tagSIZE scrollExtant)
        {
            return HRESULT.E_NOTIMPL;
        }

        public int OnUIDeactivate(int fUndoable)
        {
            return HRESULT.S_OK;
        }

        public int OnInPlaceDeactivate()
        {
            this.activeObject = null;
            return HRESULT.S_OK;
        }

        public int DiscardUndoState()
        {
            return HRESULT.E_NOTIMPL;
        }

        public int DeactivateAndUndo()
        {
            return HRESULT.S_OK;
        }

        public int OnPosRectChange(RECT lprcPosRect)
        {
            return HRESULT.S_OK;
        }

        #endregion

        #region IOLEInPlaceSiteEx
        // IOLEInPlaceSiteEx
        public int OnInPlaceActivateEx(out bool pfNoRedraw, int dwFlags)
        {
            pfNoRedraw = false; //false means object needs to redraw


            return HRESULT.S_OK;
        }

        public int OnInPlaceDeactivateEx(bool fNoRedraw)
        {

            if (!fNoRedraw)
            {
                //redraw container
                this.container.Invalidate();
            }

            return HRESULT.S_OK;
        }

        public int RequestUIActivate()
        {

            //return S_FALSE to prevent activation
            //solves focus problems, but no editing

            if (this.container.mAllowActivation)
            {
                return HRESULT.S_OK;
            }
            else
            {
                return HRESULT.S_FALSE;
            }


        }

        #endregion

        #region IOLEInPlaceUIWindow
        // IOLEInPlaceUIWindow

        public int GetBorder(RECT lprectBorder)
        {
            return HRESULT.E_NOTIMPL;
        }

        public int RequestBorderSpace(RECT pborderwidths)
        {
            return HRESULT.E_NOTIMPL;
        }

        public int SetBorderSpace(RECT pborderwidths)
        {
            return HRESULT.E_NOTIMPL;
        }

        #endregion

        #region iOleinPlaceFrame

        // IOleInPlaceFrame Implementation

        public int SetActiveObject(IOleInPlaceActiveObject pActiveObject, String
            pszObjName)
        {

            try
            {

                if (pActiveObject == null)
                {
                    this.container.releaseWndProc();
                    if (this.activeObject != null)
                    {
                        Marshal.ReleaseComObject(this.activeObject);
                    }
                    this.activeObject = null;
                    this.m_docHwnd = IntPtr.Zero;
                    this.mFullyActive = false;
                }
                else
                {
                    this.activeObject = pActiveObject;
                    this.m_docHwnd = new IntPtr();
                    pActiveObject.GetWindow(ref this.m_docHwnd);
                    this.mFullyActive = true;
                    //we have the handle to the doc so set up WndProc override
                    this.container.setupWndProc();
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: " + e.Message + e.StackTrace);

            }

            return HRESULT.S_OK;

        }

        public int InsertMenus(IntPtr hmenuShared, tagOleMenuGroupWidths
            lpMenuWidths)
        {
            return HRESULT.E_NOTIMPL;
        }

        public int SetMenu(IntPtr hmenuShared, IntPtr holemenu, IntPtr
            hwndActiveObject)
        {
            return HRESULT.E_NOTIMPL;
        }

        public int RemoveMenus(IntPtr hmenuShared)
        {
            return HRESULT.E_NOTIMPL;
        }

        public int SetStatusText(String pszStatusText)
        {
            return HRESULT.E_NOTIMPL;
        }

        public int TranslateAccelerator(MSG lpmsg, short wID)
        {
            return HRESULT.S_FALSE;
        }

        #endregion

        #region IDocHostUIHandler
        // IDocHostUIHandler Implementation

        public int ShowContextMenu(uint dwID, ref win32POINT ppt,
            [MarshalAs(UnmanagedType.IUnknown)] object pcmdtReserved,
            [MarshalAs(UnmanagedType.IDispatch)]object pdispReserved)
        {

            if (this.container.IsContextMenuEnabled)
            {
                if (
                    (this.container.ContextMenu != null)
                    ||
                    (this.container.ContextMenuStrip != null)
                    )
                {
                    //show the assigned ContextMenu
                    Point pt = new Point(ppt.x, ppt.y);
                    pt = this.container.PointToClient(pt);
                    if (this.container.ContextMenuStrip != null)
                    {
                        this.container.ContextMenuStrip.Show(this.container, pt);
                    }
                    else
                    {
                        this.container.ContextMenu.Show(this.container, pt);
                    }
                    return HRESULT.S_OK;
                }
                else
                {
                    //show the default IE ContextMenu
                    Debug.WriteLine("Show context menu");
                    return HRESULT.S_FALSE;
                }

            }
            else
            {
                return HRESULT.S_OK;
            }
        }

        public int GetHostInfo_(DOCHOSTUIINFO info)
        {
            //Debug.WriteLine("GetHostInfo");
            info.cbSize = Marshal.SizeOf(typeof(DOCHOSTUIINFO));
            info.dwDoubleClick = DOCHOSTUIDBLCLICK.DEFAULT;
            info.dwFlags = (int)(DOCHOSTUIFLAG.NO3DBORDER | DOCHOSTUIFLAG.ENABLE_INPLACE_NAVIGATION |
                DOCHOSTUIFLAG.DISABLE_SCRIPT_INACTIVE | DOCHOSTUIFLAG.FLAT_SCROLLBAR);
            info.dwReserved1 = 0;
            info.dwReserved2 = 0;
            return HRESULT.S_OK;
        }

        public int GetHostInfo(DOCHOSTUIINFO info)
        {
            //Debug.WriteLine("GetHostInfo");

            int iFlags = (int)(DOCHOSTUIFLAG.NO3DBORDER | DOCHOSTUIFLAG.ENABLE_INPLACE_NAVIGATION |
                DOCHOSTUIFLAG.DISABLE_SCRIPT_INACTIVE);

            if (this.container.IsAutoCompleteEnabled)
            {
                iFlags = iFlags | (int)DOCHOSTUIFLAG.ENABLE_FORMS_AUTOCOMPLETE;
                iFlags = iFlags | (int)DOCHOSTUIFLAG.DISABLE_EDIT_NS_FIXUP;
            }
            else
            {
                iFlags = iFlags & ~(int)DOCHOSTUIFLAG.ENABLE_FORMS_AUTOCOMPLETE;
            }

            if (this.container.IsDivOnEnter)
            {
                iFlags = iFlags | (int)DOCHOSTUIFLAG.DIV_BLOCKDEFAULT;
            }
            else
            {
                iFlags = iFlags & ~(int)DOCHOSTUIFLAG.DIV_BLOCKDEFAULT;
            }

            if (this.container.IsScrollBarShown)
            {
                iFlags = iFlags & ~(int)DOCHOSTUIFLAG.SCROLL_NO;
                iFlags = iFlags | (int)DOCHOSTUIFLAG.FLAT_SCROLLBAR;
            }
            else
            {
                iFlags = iFlags | (int)DOCHOSTUIFLAG.SCROLL_NO;
                iFlags = iFlags & ~(int)DOCHOSTUIFLAG.FLAT_SCROLLBAR;
            }

            info.cbSize = Marshal.SizeOf(typeof(DOCHOSTUIINFO));
            info.dwDoubleClick = DOCHOSTUIDBLCLICK.DEFAULT;
            info.dwFlags = iFlags;
            info.dwReserved1 = 0;
            info.dwReserved2 = 0;
            return HRESULT.S_OK;
        }

        public int EnableModeless(Boolean fEnable)
        {
            //Debug.WriteLine("EnableModeless");
            return HRESULT.S_OK;
        }

        public int ShowUI(int dwID, IOleInPlaceActiveObject activeObject,
            IOleCommandTarget commandTarget, IOleInPlaceFrame frame, IOleInPlaceUIWindow doc)
        {
            //Debug.WriteLine("ShowUI");
            return HRESULT.S_OK;
        }

        public int HideUI()
        {
            // Debug.WriteLine("HideUI");
            return HRESULT.S_OK;
        }

        public int UpdateUI()
        {

            //return 0;
            //Debug.WriteLine("UpdateUI");
            if (this.mFullyActive && (this.m_document != null) && (this.container.mDesignMode == true))
            {

                try
                {
                    HTMLDocument thisdoc = (HTMLDocument) this.m_document;

                    //we need IDisplayServices to get the caret position
                    IDisplayServices ds = (IDisplayServices)thisdoc;

                    if (ds == null)
                    {
                        return HRESULT.S_OK;
                    }


                    IHTMLCaret caret;
                    int iRetVal = ds.GetCaret(out caret);

                    if (caret == null)
                    {
                        return HRESULT.S_OK;
                    }

                    win32POINT pt = new win32POINT();

                    caret.GetLocation(out pt, true);

                    IHTMLDocument2 htmldoc = (IHTMLDocument2)this.m_document;

                    IHTMLElement el = htmldoc.ElementFromPoint(pt.x, pt.y);

                    if (el == null)
                    {
                        return HRESULT.S_OK;
                    }

                    this.container.mcurrentElement = el;
                    this.container.InvokeUpdateUI(el);

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message + e.StackTrace);

                }

            }

            //should always return S_OK unless error
            return HRESULT.S_OK;
        }

        public int OnDocWindowActivate(Boolean fActivate)
        {
            //Debug.WriteLine("OnDocWindowActivate");
            return HRESULT.E_NOTIMPL;
        }

        public int OnFrameWindowActivate(Boolean fActivate)
        {
            //Debug.WriteLine("OnFrameWindowActivate");
            return HRESULT.E_NOTIMPL;
        }

        public int ResizeBorder(RECT rect, IOleInPlaceUIWindow doc, bool fFrameWindow)
        {
            //Debug.WriteLine("ResizeBorder");
            return HRESULT.E_NOTIMPL;
        }

        public int GetOptionKeyPath(out IntPtr pbstrKey, uint dw)
        {
            Debug.WriteLine("GetOptionKeyPath");
            //use this to set your own app-specific preferences

            //eg pbstrKey = Marshal.StringToBSTR("Software\\myapp\\mysettings\\mshtml");
            if ((this.container.OptionKeyPath == null) | (this.container.OptionKeyPath == String.Empty))
            {
                pbstrKey = IntPtr.Zero;
            }
            else
            {
                pbstrKey = Marshal.StringToBSTR(this.container.OptionKeyPath);
            }

            return HRESULT.S_OK;

        }

        public int GetDropTarget(IOleDropTarget pDropTarget, out IOleDropTarget ppDropTarget)
        {
            //  Debug.WriteLine("GetDropTarget");
            ppDropTarget = null;
            return HRESULT.E_NOTIMPL;
        }

        public int GetExternal(out Object ppDispatch)
        {
            //Debug.WriteLine("GetExternal");
            //Note from Jamie re the container is dead and the container's parent is null.
            //Wasn't handled before and was causing an error.
            if (this.container == null)
            {
                ppDispatch = null;
                return HRESULT.S_FALSE;
            }

            ppDispatch = this.container.Parent;
            return HRESULT.S_OK;

            //Note from Stephen Wood
            //1. GetExternal function.
            //Please change the following line
            //ppDispatch = container;
            //to
            //ppDispatch = container.Parent;
            //This allows the user to do something like window.external.MyFunction;
            //and get the function fired in the parent container.

        }

        public int TranslateAccelerator(MSG msg, ref Guid group, int nCmdID)
        {
            //illustrates how to trap and cancel an accelerator command
            // Debug.WriteLine("Translate Accel");

            if (nCmdID == commandids.IDM_PASTE)
            {
                BeforePasteArgs e = new BeforePasteArgs();
                this.container.OnBeforePaste(e);
                if (e.Cancel)
                {
                    return HRESULT.S_OK; //cancel the paste
                }

            }

            return HRESULT.S_FALSE;
        }

        public int TranslateUrl(int dwTranslate, String strURLIn, out String
            pstrURLOut)
        {
            pstrURLOut = null;

            BeforeNavigateEventArgs e = new BeforeNavigateEventArgs(strURLIn);

            this.container.OnBeforeNavigate(e);

            bool translated = false;

            if (e.Cancel)
            {
                if (e.Target.StartsWith("javascript"))
                {
                    pstrURLOut = null;
                }
                else
                {
                    pstrURLOut = " "; 
                }

                translated = true;
            }
            else if (e.NewTarget != e.Target)
            {
                pstrURLOut = e.NewTarget;
                translated = true;
            }

            //if scripts are disabled we can't navigate to javascript links
            else if ((e.Target.StartsWith("javascript")) & (!this.container.mEnableActiveContent))
            {
                pstrURLOut = null;
                translated = true;
            }
            else if ((this.container.mLinksNewWindow))
            {
                pstrURLOut = " ";

                // Ashish Datta - THIS IS A HACK!
                //MainApplication nt = (MainApplication) container.FindForm().Owner;
                //nt.BrowseTo(e.NewTarget);

                
                //Load the link in an external window.
                Process p = new Process();
                p.StartInfo.FileName = e.NewTarget;
                p.Start();

                translated = true;
            }


            if (e.Target.StartsWith("res://shdoclc"))
            {
                //error condition - redirect to about blank
                pstrURLOut = "about:blank";
                translated = true;
            }

            if (translated)
            {
                return HRESULT.S_OK;
            }
            else
            {
                 return HRESULT.S_FALSE; //False = not translated.
            }

        }

        public int FilterDataObject(IOleDataObject pDO, out IOleDataObject ppDORet)
        {
            //  Debug.WriteLine("FilterDataObject");
            ppDORet = null;
            return HRESULT.E_NOTIMPL;
        }

        #endregion

        #region IDocHostShowUI --show message boxes and Help ======================
        //IDocHostShowUI
        /*
             * A host can supply mechanisms that will 
             * show message boxes and Help 
             * by implementing the IDocHostShowUI interface
             * */
        public int ShowMessage(IntPtr hwnd, String lpStrText,
            String lpstrCaption, uint dwType, String lpHelpFile,
            uint dwHelpContext, IntPtr lpresult)
        {

            return HRESULT.E_NOTIMPL;
        }

        public int ShowHelp(IntPtr hwnd, String lpHelpFile,
            uint uCommand, uint dwData, win32POINT ptMouse,
            Object pDispatchObjectHit)
        {

            return HRESULT.E_NOTIMPL;
        }
        #endregion IDocHostShowUI =============

        #region HTMLDocumentEvents2
        //HTMLDocumentEvents2 implementation
        //return false to prevent mshtml further handling the event
        //return true for normal processing
        public bool onhelp(IHTMLEventObj o)
        {
            return true;
        }

        public bool onclick(IHTMLEventObj o)
        {
            return true;
        }

        public bool ondblclick(IHTMLEventObj o)
        {
            return true;
        }

        public void onkeydown(IHTMLEventObj o)
        {

        }

        public void onkeyup(IHTMLEventObj o)
        {
        }

        public bool onkeypress(IHTMLEventObj o)
        {
            return true;
        }

        public void onmousedown(IHTMLEventObj o)
        {
        }

        public void onmousemove(IHTMLEventObj o)
        {

        }

        public void onmouseup(IHTMLEventObj o)
        {

        }

        public void onmouseout(IHTMLEventObj o)
        {

        }

        public void onmouseover(IHTMLEventObj o)
        {

        }

        public void onreadystatechange(IHTMLEventObj o)
        {
            this.container.ReadyStateChangeActions(o);
        }

        public bool onbeforeupdate(IHTMLEventObj o)
        {
            return true;
        }

        public void onafterupdate(IHTMLEventObj o)
        {
        }

        public bool onrowexit(IHTMLEventObj o)
        {
            return true;
        }

        public void onrowenter(IHTMLEventObj o)
        {

        }

        public bool ondragstart(IHTMLEventObj o)
        {
            return true;
        }

        public bool onselectstart(IHTMLEventObj o)
        {
            return true;
        }

        public bool onerrorupdate(IHTMLEventObj o)
        {
            return true;
        }

        public bool oncontextmenu(IHTMLEventObj o)
        {
            return true;
        }

        public bool onstop(IHTMLEventObj o)
        {
            return true;
        }

        public void onrowsdelete(IHTMLEventObj o)
        {
        }

        public void onrowsinserted(IHTMLEventObj o)
        {

        }

        public void oncellchange(IHTMLEventObj o)
        {

        }

        public void onpropertychange(IHTMLEventObj o)
        {

        }

        public void ondatasetchanged(IHTMLEventObj o)
        {
        }

        public void ondataavailable(IHTMLEventObj o)
        {

        }

        public void ondatasetcomplete(IHTMLEventObj o)
        {
        }

        public void onbeforeeditfocus(IHTMLEventObj o)
        {

        }

        public void onselectionchange(IHTMLEventObj o)
        {

        }

        public bool oncontrolselect(IHTMLEventObj o)
        {
            return true;
        }

        public bool onmousewheel(IHTMLEventObj o)
        {
            return true;
        }

        public void onfocusin(IHTMLEventObj o)
        {

        }

        public void onfocusout(IHTMLEventObj o)
        {
        }

        public void onactivate(IHTMLEventObj o)
        {

        }

        public void ondeactivate(IHTMLEventObj o)
        {

        }

        public bool onbeforeactivate(IHTMLEventObj o)
        {
            return true;
        }

        public bool onbeforedeactivate(IHTMLEventObj o)
        {
            return true;
        }


        //IAdviseSink implementation
        public void OnClose()
        {

        }

        public void OnDataChange(object pStgmed, object pFormatEtc)
        {
        }

        public void OnRename(IMoniker pmk)
        {
        }

        public void OnSave()
        {
        }

        public void OnViewChange(int dwAspect, int lindex)
        {
        }

        #endregion

        #region IServiceProvider -- retrieve a service object ========================
        //implementation of IServiceProvider
        /*
             * Defines a mechanism for retrieving a service object; 
             * that is, an object that provides custom support to other objects
             * */
        public int QueryService(ref Guid guidservice, ref Guid interfacerequested, out IntPtr ppserviceinterface)
        {

            int hr = HRESULT.E_NOINTERFACE;
            Guid iid_htmledithost = new Guid("3050f6a0-98b5-11cf-bb82-00aa00bdce0b");
            Guid sid_shtmledithost = new Guid("3050F6A0-98B5-11CF-BB82-00AA00BDCE0B");


            if ((guidservice == sid_shtmledithost) & (interfacerequested == iid_htmledithost))
            {
                CSnap snapper = new CSnap();
                ppserviceinterface = Marshal.GetComInterfaceForObject(snapper, typeof(IHTMLEditHost));
                if (ppserviceinterface != IntPtr.Zero)
                {
                    hr = HRESULT.S_OK;
                }

            }
            else
            {
                ppserviceinterface = IntPtr.Zero;
            }

            return hr;

        }
        #endregion IServiceProvider ========================

        #region IHTMLEditDesigner change the IE editor's default behavior ==========
        // IHTMLEditDesigner
        /*
             * This custom interface provides methods that enable clients using the editor 
             * to intercept Microsoft® Internet Explorer events 
             * so that they can change the editor's default behavior
             * */
        public int PreHandleEvent(int inEvtDispID, IHTMLEventObj pIEventObj)
        {
            //CGID_MSHTML
            //Guid pguidCmdGroup = new Guid("d4db6850-5385-11d0-89e9-00a0c90a90ac");
            Guid pguidCmdGroup = new Guid("DE4BA900-59CA-11CF-9592-444553540000");
            IOleCommandTarget ct = (IOleCommandTarget)this.m_document;


            switch (inEvtDispID)
            {
                case dispids.DISPID_IHTMLELEMENT_ONCLICK:
                    break;
                case dispids.DISPID_IHTMLELEMENT_ONKEYDOWN:
                    break;
                case dispids.DISPID_IHTMLELEMENT_ONKEYUP:
                    break;
                case dispids.DISPID_IHTMLELEMENT_ONKEYPRESS:
                    break;
                case dispids.DISPID_MOUSEMOVE:
                    break;
                case dispids.DISPID_MOUSEDOWN:
                    break;
                case dispids.DISPID_KEYDOWN:
                    //Need to trap Del here
                    if (pIEventObj.keyCode == 46)
                    {
                        //delete
                        // this.container.DeleteSelection();
                    }

                    break;
                case dispids.DISPID_KEYPRESS:
                    //Gets called on keypress
                    // no longer needed thanks to wndproc implementation
                    // see doShortCut in HtmlEditor.cs
                    this.container.InvokeHtmlKeyPress(ref pIEventObj);
                    break;
                case dispids.DISPID_EVMETH_ONDEACTIVATE:
                    break;
                default:
                    // Debug.WriteLine("Eventobj: " + pIEventObj.EventType);

                    break;
            }

            return HRESULT.S_FALSE;
        }

        public int PostHandleEvent(int inEvtDispID, IHTMLEventObj pIEventObj)
        {

            return HRESULT.S_FALSE;
        }

        public int TranslateAccelerator(int inEvtDispID, IHTMLEventObj pIEventObj)
        {
            return HRESULT.S_FALSE;
        }

        public int PostEditorEventNotify(int inEvtDispID, IHTMLEventObj pIEventObj)
        {

            return HRESULT.S_FALSE;
        }

        #endregion IHTMLEditDesigner ================

        #region IPropertyNotifySink -- notifications of property changes =====================

        //implementation of IPropertyNotifySink -- notifications of property changes
        /*
             * The IPropertyNotifySink interface is implemented 
             * by a sink object 
             * to receive notifications about property changes 
             * from an object that supports IPropertyNotifySink as an "outgoing" interface. 
             * */

        public int OnChanged(int iDispID)
        {
            return HRESULT.S_OK;
        }

        public int OnRequestEdit(int iDispID)
        {
            return HRESULT.S_OK; //indicates change is allowed
        }

        #endregion IPropertyNotifySink ===============

    }
}
