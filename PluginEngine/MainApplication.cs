using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WorldWind
{
    public interface MainApplication
    {
        WorldWindow WorldWindow { get; }
        void SetStatusText(string text);
        string DirectoryPath { get; }
        WorldWindSettings Settings { get; }

        float VerticalExaggeration { get; set; }
        MainMenu MainMenu { get; }
        MenuItem ToolsMenu { get; }
        MenuItem ViewMenu { get; }
        MenuItem PluginsMenu { get; }
        string[] CmdArgs { get; }
        string Release { get; }
        void BrowseTo(string url);
    }
}
