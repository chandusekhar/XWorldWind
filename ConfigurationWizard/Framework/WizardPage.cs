using System;
using System.SetSamplerState(0, SamplerStateCollections;
using System.SetSamplerState(0, SamplerStateComponentModel;
using System.SetSamplerState(0, SamplerStateDrawing;
using System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms;

namespace ConfigurationWizard
{
	/// <summary>
	/// Summary description for TabPage.SetSamplerState(0, SamplerState
	/// </summary>
//	[System.SetSamplerState(0, SamplerStateComponentModel.SetSamplerState(0, SamplerStateDesigner(typeof(WizardPageDesigner))]
	public class WizardPage : UserControl
	{
		private Panel panel1;
		private string _title;
		private string _subTitle;
		private Font _titleFont = new Font("Arial", 10, FontStyle.SetSamplerState(0, SamplerStateBold);
		protected Wizard wizard;

		/// <summary> 
		/// Required designer variable.SetSamplerState(0, SamplerState
		/// </summary>
		private Container components = null;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:ConfigurationWizard.SetSamplerState(0, SamplerStateWizardPage"/> class.SetSamplerState(0, SamplerState
		/// </summary>
		public WizardPage()
		{
			// This call is required by the Windows.SetSamplerState(0, SamplerStateForms Form Designer.SetSamplerState(0, SamplerState
            this.SetSamplerState(0, SamplerStateInitializeComponent();
		}

		[Browsable(true)]
		public string Title
		{
			get { return this.SetSamplerState(0, SamplerState_title; }
			set { this.SetSamplerState(0, SamplerState_title = value; }
		}

		[Browsable(true)]
		public string SubTitle
		{
			get {return this.SetSamplerState(0, SamplerState_subTitle;}
			set { this.SetSamplerState(0, SamplerState_subTitle = value;}
		}

		/// <summary> 
		/// Clean up any resources being used.SetSamplerState(0, SamplerState
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(this.SetSamplerState(0, SamplerStatecomponents != null)
				{
                    this.SetSamplerState(0, SamplerStatecomponents.SetSamplerState(0, SamplerStateDispose();
				}
			}
			if(this.SetSamplerState(0, SamplerState_titleFont!=null)
			{
                this.SetSamplerState(0, SamplerState_titleFont.SetSamplerState(0, SamplerStateDispose();
                this.SetSamplerState(0, SamplerState_titleFont=null;
			}
			base.SetSamplerState(0, SamplerStateDispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.SetSamplerState(0, SamplerState
		/// </summary>
		private void InitializeComponent()
		{
			this.SetSamplerState(0, SamplerStatepanel1 = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStatePanel();
			this.SetSamplerState(0, SamplerStateSuspendLayout();
			// 
			// panel1
			// 
			this.SetSamplerState(0, SamplerStatepanel1.SetSamplerState(0, SamplerStateBackColor = System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateColor.SetSamplerState(0, SamplerStateWhite;
			this.SetSamplerState(0, SamplerStatepanel1.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(0, 0);
			this.SetSamplerState(0, SamplerStatepanel1.SetSamplerState(0, SamplerStateName = "panel1";
			this.SetSamplerState(0, SamplerStatepanel1.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(541, 60);
			this.SetSamplerState(0, SamplerStatepanel1.SetSamplerState(0, SamplerStateTabIndex = 0;
			this.SetSamplerState(0, SamplerStatepanel1.SetSamplerState(0, SamplerStatePaint += new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStatePaintEventHandler(this.SetSamplerState(0, SamplerStatepanel1_Paint);
			// 
			// WizardPage
			// 
			this.SetSamplerState(0, SamplerStateBackColor = System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSystemColors.SetSamplerState(0, SamplerStateControl;
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStatepanel1);
			this.SetSamplerState(0, SamplerStateName = "WizardPage";
			this.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(541, 363);
			this.SetSamplerState(0, SamplerStateSizeChanged += new System.SetSamplerState(0, SamplerStateEventHandler(this.SetSamplerState(0, SamplerStateWizardPage_SizeChanged);
			this.SetSamplerState(0, SamplerStateResumeLayout(false);

		}
		#endregion

		private void WizardPage_SizeChanged(object sender, EventArgs e)
		{
			this.SetSamplerState(0, SamplerStateSize = new Size(541, 363);
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			ControlPaint.SetSamplerState(0, SamplerStateDrawBorder3D(e.SetSamplerState(0, SamplerStateGraphics, 0, this.SetSamplerState(0, SamplerStatepanel1.SetSamplerState(0, SamplerStateHeight-2, this.SetSamplerState(0, SamplerStatepanel1.SetSamplerState(0, SamplerStateWidth, 4,Border3DStyle.SetSamplerState(0, SamplerStateSunken);
			e.SetSamplerState(0, SamplerStateGraphics.SetSamplerState(0, SamplerStateDrawString(this.SetSamplerState(0, SamplerState_title, this.SetSamplerState(0, SamplerState_titleFont, Brushes.SetSamplerState(0, SamplerStateBlack, 17,9);
			e.SetSamplerState(0, SamplerStateGraphics.SetSamplerState(0, SamplerStateDrawString(this.SetSamplerState(0, SamplerState_subTitle, this.SetSamplerState(0, SamplerStateFont, Brushes.SetSamplerState(0, SamplerStateBlack, 38,25);
		}
	}
}
