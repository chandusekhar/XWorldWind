using System;
using System.Collections;
using System.IO;
using WorldWind;

namespace WorldWind.PluginEngine
{
	/// <summary>
	/// Stores information on a plugin.
	/// </summary>
	public class PluginInfo
	{
		Plugin m_plugin;
		string m_fullPath;
		string m_name;
		string m_description;
		string m_developer;
		string m_webSite;
		string m_references;
        MainApplication m_parentApplication;

        public PluginInfo(MainApplication parentApplication)
        {
            this.m_parentApplication = parentApplication;
        }

		/// <summary>
		/// The plugin instance.
		/// </summary>
		public Plugin Plugin
		{
			get
			{
				return this.m_plugin;
			}
			set
			{
                this.m_plugin = value;
			}
		}

		/// <summary>
		/// Directory and filename of the plugin.
		/// </summary>
		public string FullPath
		{
			get
			{
				return this.m_fullPath;
			}
			set
			{
                this.m_fullPath = value;
			}
		}

		/// <summary>
		/// The plugin ID
		/// </summary>
		public string ID
		{
			get
			{
				if(this.m_fullPath!=null)
					return Path.GetFileNameWithoutExtension(this.m_fullPath);
				return this.m_name;
			}
		}

		/// <summary>
		/// The plugin name (from plugin comment header "NAME" tag)
		/// </summary>
		public string Name
		{
			get
			{
				if(this.m_name==null) this.ReadMetaData();

				return this.m_name;
			}
			set
			{
                this.m_name=value;
			}
		}

		/// <summary>
		/// The plugin description (from plugin comment header "DESCRIPTION" tag)
		/// </summary>
		public string Description
		{
			get
			{
				if(this.m_description==null) this.ReadMetaData();

				return this.m_description;
			}
			set
			{
                this.m_description=value;
			}
		}

		/// <summary>
		/// The plugin developer's name (from plugin comment header "DEVELOPER" tag)
		/// </summary>
		public string Developer
		{
			get
			{
				if(this.m_developer==null) this.ReadMetaData();

				return this.m_developer;
			}
		}

		/// <summary>
		/// The plugin web site url (from plugin comment header "WEBSITE" tag)
		/// </summary>
		public string WebSite
		{
			get
			{
				if(this.m_webSite==null) this.ReadMetaData();

				return this.m_webSite;
			}
		}

		/// <summary>
		/// Comma separated list of additional libraries this plugin requires a reference to.
		/// </summary>
		public string References
		{
			get
			{
				if(this.m_references==null) this.ReadMetaData();

				return this.m_references;
			}
		}

		/// <summary>
		/// Check whether a plugin is currently loaded.
		/// </summary>
		public bool IsCurrentlyLoaded
		{
			get
			{
				if(this.m_plugin==null)
					return false;
				return this.m_plugin.IsLoaded;
			}
		}

		/// <summary>
		/// Set always load on application startup flag for the plugin.
		/// </summary>
		public bool IsLoadedAtStartup
		{
			get
			{
				foreach(string startupId in this.m_parentApplication.Settings.PluginsLoadedOnStartup)
					if(this.ID==startupId)
						return true;
				return false;
			}
			set
			{
                ArrayList startupPlugins = this.m_parentApplication.Settings.PluginsLoadedOnStartup;
				for(int index=0; index < startupPlugins.Count; index++)
				{
					string startupId = (string)startupPlugins[index];
					if(this.ID==startupId)
					{
						startupPlugins.RemoveAt(index);
						break;
					}
				}

				if(value)
					startupPlugins.Add(this.ID);
			}
		}

		/// <summary>
		/// Reads strings from the source file header tags
		/// </summary>
		private void ReadMetaData()
		{
			try
			{
				if(this.m_fullPath==null)
					// Source code comments not available
					return;

				// Initialize variables (prevents more than one call here)
				if(this.m_name==null) this.m_name = "";
				if(this.m_description==null) this.m_description = "";
				if(this.m_developer==null) this.m_developer = "";
				if(this.m_webSite==null) this.m_webSite = "";
				if(this.m_references==null) this.m_references = "";

				using(TextReader tr = File.OpenText(this.m_fullPath))
				{
					while(true)
					{
						string line = tr.ReadLine();
						if(line==null)
							break;

						FindTagInLine(line, "NAME", ref this.m_name);
						FindTagInLine(line, "DESCRIPTION", ref this.m_description);
						FindTagInLine(line, "DEVELOPER", ref this.m_developer);
						FindTagInLine(line, "WEBSITE", ref this.m_webSite);
						FindTagInLine(line, "REFERENCES", ref this.m_references);
					}
				}
			}
			catch(IOException)
			{
				// Ignore
			}
			finally
			{
				if(this.m_name.Length==0)
					// If name is not defined, use the filename
                    this.m_name = Path.GetFileNameWithoutExtension(this.m_fullPath);
			}
		}

		/// <summary>
		/// Extract tag value from input source line.
		/// </summary>
		static void FindTagInLine(string inputLine, string tag, ref string value)
		{
			if(value!=string.Empty)
				// Already found
				return;

			// Pattern: _TAG:_<value>EOL
			tag = " " + tag + ": ";
			int index = inputLine.IndexOf(tag);
			if(index<0)
				return;

			value = inputLine.Substring(index+tag.Length);
		}
	}
}
