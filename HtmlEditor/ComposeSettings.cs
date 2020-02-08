using System;
using System.Drawing;
using System.Text;
using System.ComponentModel;

namespace onlyconnect
{
	/// <summary>
	/// Summary description for ComposeSettings.
	/// </summary>
	public class ComposeSettings
	{
		
		private Font mFont = new Font("Arial", 10);
		private Color mForeColor = Color.Black;
		private Color mBackColor = Color.White;
		private bool mEnabled;
		private HtmlEditor mHtmlEditor;

		public ComposeSettings()
		{
			throw new Exception("You must not use this constructor");
		}
		
		public ComposeSettings(HtmlEditor editor)
		{
			//
			// TODO: Add constructor logic here
			//
			this.mHtmlEditor = editor;
		}

		[Description("Enables the use of the default composition font.")]
		public bool Enabled
		{
			get
			{
				return this.mEnabled;
			}
		
			set
			{
                this.mEnabled = value;
                this.mHtmlEditor.setDefaultFont();
			}
		}

        [Browsable(false)]
			public String CommandString
		{
			get
			{
				if (!this.mEnabled)
				{
					//clear the compose settings
					return ",,,,,,";
				}

				StringBuilder sb = new StringBuilder();

				if (this.mFont.Bold) sb.Append("1,");
				else sb.Append("0,");
				
				if (this.mFont.Italic) sb.Append("1,");
				else sb.Append("0,");

				if (this.mFont.Underline) sb.Append("1,");
				else sb.Append("0,");
			
				if (this.mFont.SizeInPoints <= 8) sb.Append ("1,");
				else if (this.mFont.SizeInPoints <= 10) sb.Append ("2,");
				else if (this.mFont.SizeInPoints <= 12) sb.Append ("3,");
				else if (this.mFont.SizeInPoints <= 18) sb.Append ("4,");
				else if (this.mFont.SizeInPoints <= 24) sb.Append ("5,");
				else if (this.mFont.SizeInPoints <= 36) sb.Append ("6,");
				else sb.Append ("7,");

				sb.Append(this.mForeColor.R);
				sb.Append(".");
				sb.Append(this.mForeColor.G);
				sb.Append(".");
				sb.Append(this.mForeColor.B);
				sb.Append(",");

				sb.Append(this.mBackColor.R);
				sb.Append(".");
				sb.Append(this.mBackColor.G);
				sb.Append(".");
				sb.Append(this.mBackColor.B);
				sb.Append(",");

				sb.Append(this.mFont.Name);
				return sb.ToString();
			}
		}

		[Description("Get/Sets the default BackColor that will be used for the editor.")]
		public Color BackColor 
		{
			get {return this.mBackColor;}
			set 
			{
				if (this.mBackColor != value) 
				{
                    this.mBackColor = value;
                    this.mHtmlEditor.setDefaultFont();
				}
			}
		}

		[Description("Get/Sets the default ForeColor that will be used for the editor.")]
		public Color ForeColor 
		{
			get {return this.mForeColor;}
			set 
			{
				if (this.mForeColor != value) 
				{
                    this.mForeColor = value;
                    this.mHtmlEditor.setDefaultFont();
				}
			}
		}

		/// <summary>
		/// Gets/Sets the default font that the editor will use.
		/// </summary>
		[Description("Gets/Sets the default font that the editor will use.")]
		public Font DefaultFont 
		{
			get {return this.mFont;}
			set 
			{
				if (this.mFont != value) 
				{
                    this.mFont = value;
                    this.mHtmlEditor.setDefaultFont();
				}
			}
		}
	}
}
