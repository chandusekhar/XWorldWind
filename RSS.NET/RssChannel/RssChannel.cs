/* RssChannel.cs
 * =============
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

namespace Rss
{
	/// <summary>Grouping of related content items on a site</summary>
	[Serializable()]
	public class RssChannel : RssElement
	{
		private string title = RssDefault.String;
		private Uri link = RssDefault.Uri;
		private string description = RssDefault.String;
		private string language = RssDefault.String;
		private string copyright = RssDefault.String;
		private string managingEditor = RssDefault.String;
		private string webMaster = RssDefault.String;
		private DateTime pubDate = RssDefault.DateTime;
		private DateTime lastBuildDate = RssDefault.DateTime;
		private RssCategoryCollection categories = new RssCategoryCollection();
		private string generator = RssDefault.String;
		private string docs = RssDefault.String;
		private RssCloud cloud;
		private int timeToLive = RssDefault.Int;
		private RssImage image;
		private RssTextInput textInput;
		private Hour skipHours = new Hour();
		private Day skipDays = new Day();
		private string rating = RssDefault.String;
		private RssItemCollection items = new RssItemCollection();

		internal RssChannel() {}

        /// <summary>
        /// Creates a new <see cref="RssChannel"/> instance.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="description">Description.</param>
        /// <param name="link">Link.</param>
		public RssChannel(string title, string description, Uri link)
		{
		    if (title == null) throw new ArgumentNullException("title");
		    if (description == null) throw new ArgumentNullException("description");
		    if (link == null) throw new ArgumentNullException("link");
            if (title.Length == 0) throw new ArgumentException("A non zero-length string is required.", title);
            if (description.Length == 0) throw new ArgumentException("A non zero-length string is required.", description);

            this.title = title;
            this.description = description;
            this.link = link;
		}
		/// <summary>Returns a string representation of the current Object.</summary>
		/// <returns>The channel's title, description, or "RssChannel" if the title and description are blank.</returns>
		public override string ToString()
		{
		    return this.Title;
		}
		/// <summary>The name of the channel</summary>
		/// <remarks>Maximum length is 100 characters (For RSS 0.91)</remarks>
		public string Title
		{
			get { return this.title; }
			set { this.title = RssDefault.Check(value); }
		}
		/// <summary>URL of the website named in the title</summary>
		/// <remarks>Maximum length is 500 characters (For RSS 0.91)</remarks>
		public Uri Link
		{
			get { return this.link; }
			set { this.link = RssDefault.Check(value); }
		}
		/// <summary>Description of the channel</summary>
		/// <remarks>Maximum length is 500 characters (For RSS 0.91)</remarks>
		public string Description
		{
			get { return this.description; }
			set { this.description = RssDefault.Check(value); }
		}
		/// <summary>Language the channel is written in</summary>
		public string Language
		{
			get { return this.language; }
			set { this.language = RssDefault.Check(value); }
		}
		/// <summary>A link and description for a graphic icon that represent a channel</summary>
		public RssImage Image
		{
			get { return this.image; }
			set { this.image = value; }
		}
		/// <summary>Copyright notice for content in the channel</summary>
		/// <remarks>Maximum length is 100 (For RSS 0.91)</remarks>
		public string Copyright
		{
			get { return this.copyright; }
			set { this.copyright = RssDefault.Check(value); }
		}
		/// <summary>The email address of the managing editor of the channel, the person to contact for editorial inquiries</summary>
		/// <remarks>
		/// <para>Maximum length is 100 (For RSS 0.91)</para>
		/// <para>The suggested format for email addresses in RSS elements is</para>
		/// <para>bull@mancuso.com (Bull Mancuso)</para>
		/// </remarks>
		public string ManagingEditor
		{
			get { return this.managingEditor; }
			set { this.managingEditor = RssDefault.Check(value); }
		}
		/// <summary>The email address of the webmaster for the channel</summary>
		/// <remarks>
		/// <para>Person to contact if there are technical problems</para>
		/// <para>Maximum length is 100 (For RSS 0.91)</para>
		/// <para>The suggested format for email addresses in RSS elements is</para>
		/// <para>bull@mancuso.com (Bull Mancuso)</para>
		/// </remarks>
		public string WebMaster
		{
			get { return this.webMaster; }
			set { this.webMaster = RssDefault.Check(value); }
		}
		/// <summary>The PICS rating for the channel</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91)</remarks>
		public string Rating
		{
			get { return this.rating; }
			set { this.rating = RssDefault.Check(value); }
		}
		/// <summary>The publication date for the content in the channel, expressed as the coordinated universal time (UTC)</summary>
		public DateTime PubDate
		{
			get { return this.pubDate; }
			set { this.pubDate = value; }
		}
		/// <summary>The date-time the last time the content of the channel changed, expressed as the coordinated universal time (UTC)</summary>
		public DateTime LastBuildDate
		{
			get { return this.lastBuildDate; }
			set { this.lastBuildDate = value; }
		}
		/// <summary>One or more categories the channel belongs to.</summary>
		public RssCategoryCollection Categories
		{
			get { return this.categories; }
		}
		/// <summary>A string indicating the program used to generate the channel</summary>
		public string Generator
		{
			get { return this.generator; }
			set { this.generator = RssDefault.Check(value); }
		}
		/// <summary>A URL, points to the documentation for the format used in the RSS file</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91).</remarks>
		public string Docs
		{
			get { return this.docs; }
			set { this.docs = RssDefault.Check(value); }
		}
		/// <summary>Provides information about an HTTP GET feature, typically for a search or subscription</summary>
		public RssTextInput TextInput
		{
			get { return this.textInput; }
			set { this.textInput = value; }
		}
		/// <summary>Readers should not read the channel during days listed. (UTC)</summary>
		public Day SkipDays
		{
			get { return this.skipDays; }
			set { this.skipDays = value; }
		}
		/// <summary>Readers should not read the channel during hours listed (UTC)</summary>
		/// <remarks>Represents a time in UTC - 1.</remarks>
		public Hour SkipHours
		{
			get { return this.skipHours; }
			set { this.skipHours = value; }
		}
		/// <summary>Allow processes to register with a cloud to be notified of updates to the channel</summary>
		public RssCloud Cloud
		{
			get { return this.cloud; }
			set { this.cloud = value; }
		}
		/// <summary>The number of minutes that a channel can be cached.</summary>
		public int TimeToLive
		{
			get { return this.timeToLive; }
			set { this.timeToLive = RssDefault.Check(value); }
		}
		/// <summary>All items within the channel</summary>
		public RssItemCollection Items
		{
			get { return this.items; }
		}
	}
}
