using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace onlyconnect
{

[ComVisible(true)] 
public class ChangeMonitor : IHTMLChangeSink
{
    private HtmlEditor mHtmlEditor;

    public ChangeMonitor(HtmlEditor he) : base()
    {
        this.mHtmlEditor = he;
    }

    public void Notify()
    {
        this.mHtmlEditor.InvokeContentChanged();
    }
}
}

