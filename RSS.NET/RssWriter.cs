/* RssWriter.cs
 * ============
 * 
 * RSS.NET (http://rss-net.sf.net/)
 * Copyright © 2002 - 2005 George Tsiokos. All Rights Reserved.
 * 
 * RSS 2.0 (http://blogs.law.harvard.edu/tech/rss)
 * RSS 2.0 is offered by the Berkman Center for Internet & Society at 
 * Harvard Law School under the terms of the Attribution/Share Alike 
 * Creative Commons license.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining 
 * a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
*/
using System;
using System.Xml;
using System.Text;
using System.IO;

namespace Rss
{
	/// <summary>Writes an RSS XML file.</summary>
	/// <remarks>Represents a writer that provides a fast, non-cached, forward-only way of generating streams or files containing RSS XML data that conforms to the W3C Extensible Markup Language (XML) 1.0 and the Namespaces in XML recommendations.</remarks>
	public abstract class RssWriter : IDisposable
	{
        private static readonly Encoding DefaultEncoding = Encoding.GetEncoding("ISO-8859-1");
        private const RssVersion DefaultRssVersion = RssVersion.RSS20;
		private XmlTextWriter writer;
        private bool disposed;
		
		// functional var
		private bool documentBegun;
		private bool wroteChannel;

		// preferences
		private Formatting xmlFormat = Formatting.Indented;
		private int xmlIndentation = 2;

		// constants
        /// <summary>
        /// The default format string for parsing DateTime strings.
        /// </summary>
		public const string DateTimeFormatString = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";

		// modules
		private RssModuleCollection _rssModules = new RssModuleCollection();

        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <value>The <see cref="XmlTextWriter"/> used for generating output.</value>
        protected XmlTextWriter Writer
        {
            get { return this.writer; }
        }

        private bool DocumentBegun
        {
            get { return this.documentBegun; }
            set { this.documentBegun = value;}
        }

	    private bool ChannelBegun
	    {
	        get { return this.wroteChannel; }
            set { this.wroteChannel = value; }
	    }

        #region factory methods and constructors

        /// <summary>Creates an instance of the RssWriter class using the specified file and Encoding.</summary>
        /// <exception cref="ArgumentException">The encoding is not supported; the filename is empty, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
        /// <exception cref="ArgumentNullException">The filename is a (null c#, Nothing vb) reference.</exception>
        /// <exception cref="DirectoryNotFoundException">The directory to write to is not found.</exception>
        /// <exception cref="IOException">The filename includes an incorrect or invalid syntax for file name, directory name, or volume label syntax.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <param name="fileName">specified file (including path) If the file exists, it will be truncated with the new content.</param>
        /// <param name="version">the RSS version for output.</param>
        /// <param name="encoding">specified Encoding</param>
        public static RssWriter Create(string fileName, RssVersion version, Encoding encoding)
        {
            return Create(new XmlTextWriter(fileName, encoding), version);
        }   
        
        /// <summary>Creates an instance of the RssWriter class using the specified file.</summary>
        /// <remarks>The encoding is ISO-8859-1 and the version is RSS 2.0.</remarks>
        /// <exception cref="ArgumentException">The filename is empty, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
        /// <exception cref="ArgumentNullException">The filename is a (null c#, Nothing vb) reference.</exception>
        /// <exception cref="DirectoryNotFoundException">The directory to write to is not found.</exception>
        /// <exception cref="IOException">The filename includes an incorrect or invalid syntax for file name, directory name, or volume label syntax.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <param name="fileName">specified file (including path) If the file exists, it will be truncated with the new content.</param>
        /// <param name="version">the RSS version to output.</param>
        /// <returns>The new instance.</returns>
        public static RssWriter Create(string fileName, RssVersion version)
        {
            return Create(new XmlTextWriter(fileName, DefaultEncoding), DefaultRssVersion);
        }  
        
        /// <summary>Creates an instance of the RssWriter class using the specified file.</summary>
        /// <remarks>The encoding is ISO-8859-1 and the version is RSS 2.0.</remarks>
        /// <exception cref="ArgumentException">The filename is empty, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
        /// <exception cref="ArgumentNullException">The filename is a (null c#, Nothing vb) reference.</exception>
        /// <exception cref="DirectoryNotFoundException">The directory to write to is not found.</exception>
        /// <exception cref="IOException">The filename includes an incorrect or invalid syntax for file name, directory name, or volume label syntax.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <param name="fileName">specified file (including path) If the file exists, it will be truncated with the new content.</param>
        /// <returns>The new instance.</returns>
        public static RssWriter Create(string fileName)
        {
            return Create(fileName, DefaultRssVersion, DefaultEncoding);
        }

        /// <summary>Creates an instance of the RssWriter class using the specified Stream.</summary>
        /// <remarks>The encoding is ISO-8859-1, and the RSS version is 2.0.</remarks>
        /// <exception cref="ArgumentException">The Stream cannot be written to.</exception>
        /// <param name="stream">specified Stream</param>
        /// <returns>The new instance.</returns>
        public static RssWriter Create(Stream stream)
        {
            return Create(stream, DefaultRssVersion, DefaultEncoding);
        }

        /// <summary>Creates an instance of the RssWriter class using the specified Stream, and encoded to ISO-8859-1.</summary>
        /// <param name="stream">Stream to output to</param>
        /// <param name="version">The RSS version for output.</param>
        /// <returns>The new instance.</returns>
        public static RssWriter Create(Stream stream, RssVersion version)
        {
            return Create(stream, version, DefaultEncoding);
        }
        
        /// <summary>Creates an instance of the RssWriter class using the specified Stream and Encoding.</summary>
        /// <exception cref="ArgumentException">The encoding is not supported or the stream cannot be written to.</exception>
        /// <param name="stream">Stream to output to</param>
        /// <param name="version">The RSS version for output.</param>
        /// <param name="encoding">The encoding to use. If encoding is (null c#, Nothing vb) it writes out the stream as UTF-8.</param>
        /// <returns>The new instance.</returns>
        public static RssWriter Create(Stream stream, RssVersion version, Encoding encoding)
        {
            return Create(new XmlTextWriter(stream, encoding), version);
        } 

        /// <summary>Creates an instance of the RssWriter class using the specified TextWriter.</summary>
        /// <param name="textWriter">specified TextWriter</param>
        /// <param name="version">The RSS version for output.</param>
        public static RssWriter Create(TextWriter textWriter, RssVersion version)
        {
            return Create(new XmlTextWriter(textWriter), version);
        }

        private static RssWriter Create(XmlTextWriter writer, RssVersion version)
        {
            RssWriter rssWriter = null;
            switch(version)
            {
                case RssVersion.RSS20:
                    rssWriter = new Rss20Writer(writer);
                    break;
                default:
                    throw new NotSupportedException("The specified RSS version is not currently supported.");
            }
            return rssWriter;
        }

        /// <summary>
        /// Creates a new <see cref="RssWriter"/> instance.
        /// </summary>
        /// <param name="writer">Writer.</param>
        protected RssWriter(XmlTextWriter writer)
        {
            this.writer = writer;
        }

		#endregion

		/// <summary>Writes the begining data to the RSS file</summary>
		/// <remarks>This routine is called from the WriteChannel and WriteItem subs</remarks>
		/// <exception cref="NotSupportedException">RDF Site Summary (RSS) 1.0 is not currently supported.</exception>
		private void BeginDocument()
		{
			if (!this.DocumentBegun)
			{
                this.Writer.Formatting = this.xmlFormat;
                this.Writer.Indentation = this.xmlIndentation;
                this.Writer.WriteStartDocument();
                this.Writer.WriteComment("Generated by RSS.NET: http://rss-net.sf.net");
                this.OpenRootElement();
                this.DocumentBegun = true;
			}
		}

        /// <summary>
        /// Opens the root RSS element.
        /// </summary>
	    protected abstract void OpenRootElement();

	    private void writeChannel(RssChannel channel)
		{
			if (this.Writer == null)
				throw new InvalidOperationException("RssWriter has been closed, and cannot be written to.");
			if (channel == null)
				throw new ArgumentNullException("Channel must be instanciated with data to be written.");
			
			if (this.ChannelBegun)
                this.Writer.WriteEndElement();
			else
                this.ChannelBegun = true;

            this.BeginDocument();

            this.Writer.WriteStartElement("channel");
            this.WriteElement("title", channel.Title, true);
            this.WriteElement("description", channel.Description, true);
            this.WriteElement("link", channel.Link, true);

			if (channel.Image != null)
			{
                this.Writer.WriteStartElement("image");
                this.WriteElement("title", channel.Image.Title, true);
                this.WriteElement("url", channel.Image.Url, true);
                this.WriteElement("link", channel.Image.Link, true);
                this.WriteElement("description", channel.Image.Description, false);
                this.WriteElement("width", channel.Image.Width, false);
                this.WriteElement("height", channel.Image.Height, false);
                this.Writer.WriteEndElement();
			}

            this.WriteElement("language", channel.Language, this.Version == RssVersion.RSS091);
            this.WriteElement("copyright", channel.Copyright, false);
            this.WriteElement("managingEditor", channel.ManagingEditor, false);
            this.WriteElement("webMaster", channel.WebMaster, false);
            this.WriteElement("pubDate", channel.PubDate, false);
            this.WriteElement("lastBuildDate", channel.LastBuildDate, false);
			if (channel.Docs != RssDefault.String)
                this.WriteElement("docs", channel.Docs, false);
			else
                this.WriteElement("docs", "http://backend.userland.com/rss", false);

            this.WriteElement("rating", channel.Rating, false);
            this.WriteSkipDays(channel);
            this.WriteSkipHours(channel);

	        if (channel.Categories != null)
                foreach(RssCategory category in channel.Categories)
                {
                    if (category.Name != RssDefault.String)
                    {
                        this.Writer.WriteStartElement("category");
                        this.WriteAttribute("domain", category.Domain, false);
                        this.Writer.WriteString(category.Name);
                        this.Writer.WriteEndElement();
                    }
                }

			if (channel.Cloud != null)
			{
                this.Writer.WriteStartElement("cloud");
                this.WriteElement("domain", channel.Cloud.Domain, false);
                this.WriteElement("port", channel.Cloud.Port, false);
                this.WriteElement("path", channel.Cloud.Path, false);
                this.WriteElement("registerProcedure", channel.Cloud.RegisterProcedure, false);
				if (channel.Cloud.Protocol != RssCloudProtocol.Empty) this.WriteElement("Protocol", channel.Cloud.Protocol, false);
                this.Writer.WriteEndElement();
			}

			if (channel.Generator != RssDefault.String)
                this.WriteElement("generator", channel.Generator, false);
			else
                this.WriteElement("generator", "RSS.NET: http://rss-net.sf.net/", false);
            this.WriteElement("ttl", channel.TimeToLive, false);

			// RSS Modules
			foreach(RssModule rssModule in this._rssModules)
			{
				if(rssModule.IsBoundTo(channel.GetHashCode()))
				{
					foreach(RssModuleItem rssModuleItem in rssModule.ChannelExtensions)
					{
						if(rssModuleItem.SubElements.Count == 0)
                            this.WriteElement(rssModule.NamespacePrefix + ":" + rssModuleItem.Name, rssModuleItem.Text, rssModuleItem.IsRequired);
						else
                            this.writeSubElements(rssModuleItem.SubElements, rssModule.NamespacePrefix);
					}
				}
			}

			if (channel.TextInput != null)
			{
                this.Writer.WriteStartElement("textinput");
                this.WriteElement("title", channel.TextInput.Title, true);
                this.WriteElement("description", channel.TextInput.Description, true);
                this.WriteElement("name", channel.TextInput.Name, true);
                this.WriteElement("link", channel.TextInput.Link, true);
                this.Writer.WriteEndElement();
			}
			foreach (RssItem item in channel.Items)
			{
                this.writeItem(item, channel.GetHashCode());
			}

            this.Writer.Flush();
		}

	    private void WriteSkipHours(RssChannel channel)
	    {
            bool elementOpened = false;

            foreach(Hour h in channel.SkipHours.Hours)
            {
                if (!elementOpened)
                {
                    this.Writer.WriteStartElement("skipHours");
                    elementOpened = true;
                }

                this.WriteElement("hour", h.ToString(), false);
            }

            if (elementOpened) this.Writer.WriteEndElement();
	    }

	    private void WriteSkipDays(RssChannel channel)
	    {
            bool elementOpened = false;

            foreach(Day d in channel.SkipDays.Days)
            {
                if (!elementOpened)
                {
                    this.Writer.WriteStartElement("skipDays");
                    elementOpened = true;
                }

                this.WriteElement("day", d.ToString(), false);
            }

            if (elementOpened) this.Writer.WriteEndElement();
	    }

	    private void writeItem(RssItem item, int channelHashCode)
		{
			if (this.Writer == null)
				throw new InvalidOperationException("RssWriter has been closed, and can not be written to.");
			if (item == null)
				throw new ArgumentNullException("Item must be instanciated with data to be written.");
			if (!this.ChannelBegun)
				throw new InvalidOperationException("Channel must be written first, before writing an item.");

            this.BeginDocument();

            this.Writer.WriteStartElement("item");
			if ((item.Title == RssDefault.String) && (item.Description == RssDefault.String))
				throw new ArgumentException("item title and description cannot be null");
            this.WriteElement("title", item.Title, false);
            this.WriteElement("description", item.Description, false);
            this.WriteElement("link", item.Link, false);
			if (item.Source != null)
			{
                this.Writer.WriteStartElement("source");
                this.WriteAttribute("url", item.Source.Url, true);
                this.Writer.WriteString(item.Source.Name);
                this.Writer.WriteEndElement();
			}

			if (item.Enclosure != null)
			{
                this.Writer.WriteStartElement("enclosure");
                this.WriteAttribute("url", item.Enclosure.Url, true);
                this.WriteAttribute("length", item.Enclosure.Length, true);
                this.WriteAttribute("type", item.Enclosure.Type, true);
                this.Writer.WriteEndElement();
			}

			foreach(RssCategory category in item.Categories)
				if (category.Name != RssDefault.String)
				{
                    this.Writer.WriteStartElement("category");
                    this.WriteAttribute("domain", category.Domain, false);
                    this.Writer.WriteString(category.Name);
                    this.Writer.WriteEndElement();
				}

            this.WriteElement("author", item.Author, false);
            this.WriteElement("comments", item.Comments, false);
			if ((item.Guid != null) && (item.Guid.Name != RssDefault.String))
			{
                this.Writer.WriteStartElement("guid");
				if (!item.Guid.PermaLink.IsNull) this.WriteAttribute("isPermaLink", (bool)item.Guid.PermaLink, false);
                this.Writer.WriteString(item.Guid.Name);
                this.Writer.WriteEndElement();
			}

            this.WriteElement("pubDate", item.PubDate, false);

			foreach(RssModule rssModule in this._rssModules)
			{
				if(rssModule.IsBoundTo(channelHashCode))
				{
					foreach(RssModuleItemCollection rssModuleItemCollection in rssModule.ItemExtensions)
					{
						if(rssModuleItemCollection.IsBoundTo(item.GetHashCode())) this.writeSubElements(rssModuleItemCollection, rssModule.NamespacePrefix);
					}
				}
			}

            this.Writer.WriteEndElement();
            this.Writer.Flush();
		}
		/// <summary>Closes instance of RssWriter.</summary>
		/// <remarks>Writes end elements, and releases connections</remarks>
		/// <exception cref="InvalidOperationException">Occurs if the RssWriter is already closed or the caller is attempting to close before writing a channel.</exception>
		public void Close()
		{
			if (this.Writer == null)
				throw new InvalidOperationException("RssWriter has been closed, and can not be closed again.");
			
			if (!this.ChannelBegun)
				throw new InvalidOperationException("Can't close RssWriter without first writing a channel.");
			else
                this.Writer.WriteEndElement(); // </channel>

            this.Writer.WriteEndElement(); // </rss> or </rdf>
            this.Dispose();
		}

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The <see cref="RssVersion"/> of the output.</value>
		public abstract RssVersion Version {get;}

		/// <summary>Gets or sets the <see cref="System.Xml.Formatting"/> of the XML output.</summary>
		/// <exception cref="InvalidOperationException">Can't change XML formatting after data has been written.</exception>
		public Formatting XmlFormat
		{
			get { return this.xmlFormat; }
			set
			{
				if(this.DocumentBegun)
					throw new InvalidOperationException("Can't change XML formatting after data has been written.");
				else
                    this.xmlFormat = value;
			}
		}
		/// <summary>Gets or sets how indentation to write for each level in the hierarchy when XmlFormat is set to <see cref="Formatting.Indented"/></summary>
		/// <exception cref="InvalidOperationException">Can't change XML formatting after data has been written.</exception>
		/// <exception cref="ArgumentException">Setting this property to a negative value.</exception>
		public int XmlIndentation
		{
			get { return this.xmlIndentation; }
			set
			{
				if(this.DocumentBegun)
					throw new InvalidOperationException("Can't change XML indentation after data has been written.");
				else
					if (value < 0)
						throw new ArgumentOutOfRangeException("Value cannot be negative.");
					else
                        this.xmlIndentation = value;
			}
		}
		/// <summary>Writes an RSS channel</summary>
		/// <exception cref="InvalidOperationException">RssWriter has been closed, and can not be written to.</exception>
		/// <exception cref="ArgumentNullException">Channel must be instanciated with data, before calling Write.</exception>
		/// <param name="channel">RSS channel to write</param>
		public void Write(RssChannel channel)
		{
            this.writeChannel(channel);
		}
		/// <summary>Writes an RSS item</summary>
		/// <exception cref="InvalidOperationException">Either the RssWriter has already been closed, or the caller is attempting to write an RSS item before an RSS channel.</exception>
		/// <exception cref="ArgumentNullException">Item must be instanciated with data, before calling Write.</exception>
		/// <param name="item">RSS item to write</param>
		public void Write(RssItem item)
		{
			// NOTE: Standalone items cannot adhere to modules, hence -1 is passed. This may not be the case, however, no examples have been seen where this is legal.
            this.writeItem(item, -1);
		}

		/// <summary>RSS modules</summary>
		public RssModuleCollection Modules
		{
			get { return this._rssModules; }
			set
			{
			    if (value == null) throw new ArgumentNullException();
                
                this._rssModules = value;
			}
		}
		#region WriteElement
		/// <summary>Writes an element with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteElement(string localName, DateTime input, bool required)
		{
			if (input != RssDefault.DateTime)
                this.Writer.WriteElementString(localName, XmlConvert.ToString(input,DateTimeFormatString));
			else if (required)
					throw new ArgumentException(localName + " cannot be null.");
		}
		/// <summary>Writes an element with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteElement(string localName, int input, bool required)
		{
			if (input != RssDefault.Int)
                this.Writer.WriteElementString(localName, XmlConvert.ToString(input));
			else if (required)
				throw new ArgumentException(localName + " cannot be null.");
		}
		/// <summary>Writes an element with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteElement(string localName, string input, bool required)
		{
			if (input != RssDefault.String)
                this.Writer.WriteElementString(localName, input);
			else if (required)
				throw new ArgumentException(localName + " cannot be null.");
		}
		/// <summary>Writes an element with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteElement(string localName, Uri input, bool required)
		{
			if (input != RssDefault.Uri)
                this.Writer.WriteElementString(localName, input.ToString());
			else if (required)
				throw new ArgumentException(localName + " cannot be null.");
		}
		/// <summary>Writes an element with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteElement(string localName, object input, bool required)
		{
			if (input != null)
                this.Writer.WriteElementString(localName, input.ToString());
			else if (required)
				throw new ArgumentException(localName + " cannot be null.");
		}
		#endregion
		#region WriteAttribute
		/// <summary>Writes an attribute with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		protected void WriteAttribute(string localName, int input, bool required)
		{
			if (input != RssDefault.Int)
                this.Writer.WriteAttributeString(localName, XmlConvert.ToString(input));
			else if (required)
				throw new ArgumentException(localName + " cannot be null.");
		}
		/// <summary>Writes an attribute with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		protected void WriteAttribute(string localName, string input, bool required)
		{
			if (input != RssDefault.String)
                this.Writer.WriteAttributeString(localName, input);
			else if (required)
				throw new ArgumentException(localName + " cannot be null.");
		}
		/// <summary>Writes an attribute with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		protected void WriteAttribute(string localName, Uri input, bool required)
		{
			if (input != RssDefault.Uri)
                this.Writer.WriteAttributeString(localName, input.ToString());
			else if (required)
				throw new ArgumentException(localName + " cannot be null.");
		}
		/// <summary>Writes an attribute with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		protected void WriteAttribute(string localName, object input, bool required)
		{
			if (input != null)
                this.Writer.WriteAttributeString(localName, input.ToString());
			else if (required)
				throw new ArgumentException(localName + " cannot be null.");
		}
		#endregion
		#region WriteSubElements
		private void writeSubElements(RssModuleItemCollection items, string NamespacePrefix)
		{
			foreach(RssModuleItem rssModuleItem in items)
			{
				if(rssModuleItem.SubElements.Count == 0)
                    this.WriteElement(NamespacePrefix + ":" + rssModuleItem.Name, rssModuleItem.Text, rssModuleItem.IsRequired);
				else
				{
                    this.Writer.WriteStartElement(NamespacePrefix + ":" + rssModuleItem.Name);
                    this.writeSubElements(rssModuleItem.SubElements, NamespacePrefix);
                    this.Writer.WriteEndElement();
				}
			}
		}
		#endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose() 
        {
            if (this.disposed) return;

            this.disposed = true;
            this.Dispose(true);
            GC.SuppressFinalize(this); 
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
        protected virtual void Dispose(bool disposing) 
        {
            if (disposing) 
            {
                if (this.writer != null)
                {
                    this.writer.Close();
                    this.writer = null;
                }
            }
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~RssWriter()
        {
            this.Dispose (false);
        }


        #endregion
    }
}
