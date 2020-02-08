using System;

namespace onlyconnect
{
    
    public class DocHTML
    {
        HTMLDocument mDoc;
        IHTMLDocument2 mDoc2;
        IHTMLDocument3 mDoc3;
        IHTMLDocument4 mDoc4;
        IHTMLDocument5 mDoc5;

        public DocHTML(HTMLDocument doc)
        {
            this.mDoc = doc;
            this.mDoc2 = (IHTMLDocument2) this.mDoc;
            this.mDoc3 = (IHTMLDocument3) this.mDoc;
            this.mDoc4 = (IHTMLDocument4) this.mDoc;
            this.mDoc5 = (IHTMLDocument5) this.mDoc;
        }

        //IHTMLDocument2

        public bool ExecCommand(String cmdID)
        {
            return this.mDoc2.ExecCommand(cmdID, true, null);
        }

        public IHTMLWindow2 ParentWindow()
        {
            return this.mDoc2.GetParentWindow();
        }

        public String ReadyState
        {
            get
            {
                return this.mDoc2.GetReadyState();
            }
        }

        //IHTMLDocument3
        public IHTMLElement GetElementByID(string idval)
        {
            return this.mDoc3.getElementById(idval);
        }

        public IHTMLElementCollection GetElementsByTagName(String tagname)
        {
            return this.mDoc3.getElementsByTagName(tagname);
        }


     }
}
