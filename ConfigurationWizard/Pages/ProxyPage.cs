using System;
using System.SetSamplerState(0, SamplerStateComponentModel;
using System.SetSamplerState(0, SamplerStateDrawing;
using System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms;
using WorldWind;

namespace ConfigurationWizard
{
	/// <summary>
	/// Summary description for ProxyPage.SetSamplerState(0, SamplerState
	/// </summary>
	public class ProxyPage : WizardPage
	{
		private IContainer components;
		private ToolTip toolTipProxyPage;

		private RadioButton radioButtonUseWindowsDefaultProxy;
		private RadioButton radioButtonNoProxy;
		private RadioButton radioButtonUserDefinedProxy;
      
		private GroupBox groupBoxUserDefinedSettings;
		private TextBox textBoxProxyUrl;
		private CheckBox checkBoxUseProxyScript;
      
		private GroupBox groupBoxCredentials;
		private TextBox textBoxUsername;
		private TextBox textBoxPassword;

		private Label labelProxyPageInfo;
		private Label labelProxyUrl;
		private Label labelUsername;
		private Label labelPassword;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:ConfigurationWizard.SetSamplerState(0, SamplerStateProxyPage"/> class.SetSamplerState(0, SamplerState
		/// </summary>
		public ProxyPage()
		{
			// This call is required by the Windows.SetSamplerState(0, SamplerStateForms Form Designer.SetSamplerState(0, SamplerState
            this.SetSamplerState(0, SamplerStateInitializeComponent();
		}

		protected override void OnValidating(CancelEventArgs e)
		{
			try
			{
				// TODO: additional plausibility checks here
				Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateProxyUrl = this.SetSamplerState(0, SamplerStatetextBoxProxyUrl.SetSamplerState(0, SamplerStateText;
				Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateProxyUsername = this.SetSamplerState(0, SamplerStatetextBoxUsername.SetSamplerState(0, SamplerStateText;
				Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateProxyPassword = this.SetSamplerState(0, SamplerStatetextBoxPassword.SetSamplerState(0, SamplerStateText;

				Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateUseWindowsDefaultProxy = this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy.SetSamplerState(0, SamplerStateChecked;

				if (this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy.SetSamplerState(0, SamplerStateChecked) 
				{
					Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateUseDynamicProxy = this.SetSamplerState(0, SamplerStatecheckBoxUseProxyScript.SetSamplerState(0, SamplerStateChecked;
				}
				else 
				{
					// must set proxyurl to empty string to indicate "no proxy"
					Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateProxyUrl ="";

					// no user-defined proxy means no dynamic one as well
					Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateUseDynamicProxy = false;
				}
			}
			catch(Exception caught)
			{
				MessageBox.SetSamplerState(0, SamplerStateShow(caught.SetSamplerState(0, SamplerStateMessage, "Error", MessageBoxButtons.SetSamplerState(0, SamplerStateOK, MessageBoxIcon.SetSamplerState(0, SamplerStateError );
				e.SetSamplerState(0, SamplerStateCancel = true;
			}
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
			base.SetSamplerState(0, SamplerStateDispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.SetSamplerState(0, SamplerState
		/// </summary>
		private void InitializeComponent()
		{
			this.SetSamplerState(0, SamplerStatecomponents = new System.SetSamplerState(0, SamplerStateComponentModel.SetSamplerState(0, SamplerStateContainer();
			this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateRadioButton();
			this.SetSamplerState(0, SamplerStateradioButtonNoProxy = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateRadioButton();
			this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateRadioButton();
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateGroupBox();
			this.SetSamplerState(0, SamplerStatecheckBoxUseProxyScript = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateCheckBox();
			this.SetSamplerState(0, SamplerStatetextBoxProxyUrl = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateTextBox();
			this.SetSamplerState(0, SamplerStatelabelProxyUrl = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateLabel();
			this.SetSamplerState(0, SamplerStatelabelProxyPageInfo = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateLabel();
			this.SetSamplerState(0, SamplerStategroupBoxCredentials = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateGroupBox();
			this.SetSamplerState(0, SamplerStatetextBoxPassword = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateTextBox();
			this.SetSamplerState(0, SamplerStatelabelPassword = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateLabel();
			this.SetSamplerState(0, SamplerStatetextBoxUsername = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateTextBox();
			this.SetSamplerState(0, SamplerStatelabelUsername = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateLabel();
			this.SetSamplerState(0, SamplerStatetoolTipProxyPage = new System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateToolTip(this.SetSamplerState(0, SamplerStatecomponents);
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateSuspendLayout();
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateSuspendLayout();
			this.SetSamplerState(0, SamplerStateSuspendLayout();
			// 
			// radioButtonUserDefinedProxy
			// 
			this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(16, 176);
			this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy.SetSamplerState(0, SamplerStateName = "radioButtonUserDefinedProxy";
			this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(128, 24);
			this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy.SetSamplerState(0, SamplerStateTabIndex = 2;
			this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy.SetSamplerState(0, SamplerStateText = "User defined (HTTP)";
			this.SetSamplerState(0, SamplerStatetoolTipProxyPage.SetSamplerState(0, SamplerStateSetToolTip(this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy, "This will use the proxy settings defined below");
			this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy.SetSamplerState(0, SamplerStateCheckedChanged += new System.SetSamplerState(0, SamplerStateEventHandler(this.SetSamplerState(0, SamplerStateRelevantControl_Changed);
			// 
			// radioButtonNoProxy
			// 
			this.SetSamplerState(0, SamplerStateradioButtonNoProxy.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(16, 152);
			this.SetSamplerState(0, SamplerStateradioButtonNoProxy.SetSamplerState(0, SamplerStateName = "radioButtonNoProxy";
			this.SetSamplerState(0, SamplerStateradioButtonNoProxy.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(160, 16);
			this.SetSamplerState(0, SamplerStateradioButtonNoProxy.SetSamplerState(0, SamplerStateTabIndex = 1;
			this.SetSamplerState(0, SamplerStateradioButtonNoProxy.SetSamplerState(0, SamplerStateText = "Don\'t use a proxy";
			this.SetSamplerState(0, SamplerStatetoolTipProxyPage.SetSamplerState(0, SamplerStateSetToolTip(this.SetSamplerState(0, SamplerStateradioButtonNoProxy, "This will not use a proxy, disregarding both user-defined settings, credentials, " +
				"and browser settings.SetSamplerState(0, SamplerState");
			this.SetSamplerState(0, SamplerStateradioButtonNoProxy.SetSamplerState(0, SamplerStateCheckedChanged += new System.SetSamplerState(0, SamplerStateEventHandler(this.SetSamplerState(0, SamplerStateRelevantControl_Changed);
			// 
			// radioButtonUseWindowsDefaultProxy
			// 
			this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy.SetSamplerState(0, SamplerStateChecked = true;
			this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(16, 120);
			this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy.SetSamplerState(0, SamplerStateName = "radioButtonUseWindowsDefaultProxy";
			this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(176, 24);
			this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy.SetSamplerState(0, SamplerStateTabIndex = 0;
			this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy.SetSamplerState(0, SamplerStateTabStop = true;
			this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy.SetSamplerState(0, SamplerStateText = "Use Internet Settings (default)";
			this.SetSamplerState(0, SamplerStatetoolTipProxyPage.SetSamplerState(0, SamplerStateSetToolTip(this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy, "This will use the proxy settings configured in the browser.SetSamplerState(0, SamplerState Credentials can still" +
				" be provided in the box below");
			this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy.SetSamplerState(0, SamplerStateCheckedChanged += new System.SetSamplerState(0, SamplerStateEventHandler(this.SetSamplerState(0, SamplerStateRelevantControl_Changed);
			// 
			// groupBoxUserDefinedSettings
			// 
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStatecheckBoxUseProxyScript);
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStatetextBoxProxyUrl);
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStatelabelProxyUrl);
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(8, 208);
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateName = "groupBoxUserDefinedSettings";
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(288, 144);
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateTabIndex = 3;
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateTabStop = false;
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateText = "User-defined settings";
			// 
			// checkBoxUseProxyScript
			// 
			this.SetSamplerState(0, SamplerStatecheckBoxUseProxyScript.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(32, 88);
			this.SetSamplerState(0, SamplerStatecheckBoxUseProxyScript.SetSamplerState(0, SamplerStateName = "checkBoxUseProxyScript";
			this.SetSamplerState(0, SamplerStatecheckBoxUseProxyScript.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(200, 16);
			this.SetSamplerState(0, SamplerStatecheckBoxUseProxyScript.SetSamplerState(0, SamplerStateTabIndex = 2;
			this.SetSamplerState(0, SamplerStatecheckBoxUseProxyScript.SetSamplerState(0, SamplerStateText = "Use a script to determine proxy";
			this.SetSamplerState(0, SamplerStatetoolTipProxyPage.SetSamplerState(0, SamplerStateSetToolTip(this.SetSamplerState(0, SamplerStatecheckBoxUseProxyScript, "Check this box if your proxy is determined by the use of a script - if unsure, as" +
				"k your network administrator.SetSamplerState(0, SamplerState");
			this.SetSamplerState(0, SamplerStatecheckBoxUseProxyScript.SetSamplerState(0, SamplerStateCheckedChanged += new System.SetSamplerState(0, SamplerStateEventHandler(this.SetSamplerState(0, SamplerStateRelevantControl_Changed);
			// 
			// textBoxProxyUrl
			// 
			this.SetSamplerState(0, SamplerStatetextBoxProxyUrl.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(13, 56);
			this.SetSamplerState(0, SamplerStatetextBoxProxyUrl.SetSamplerState(0, SamplerStateName = "textBoxProxyUrl";
			this.SetSamplerState(0, SamplerStatetextBoxProxyUrl.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(267, 20);
			this.SetSamplerState(0, SamplerStatetextBoxProxyUrl.SetSamplerState(0, SamplerStateTabIndex = 1;
			this.SetSamplerState(0, SamplerStatetextBoxProxyUrl.SetSamplerState(0, SamplerStateText = "";
			this.SetSamplerState(0, SamplerStatetoolTipProxyPage.SetSamplerState(0, SamplerStateSetToolTip(this.SetSamplerState(0, SamplerStatetextBoxProxyUrl, "Enter the address of the proxy to be used here");
			// 
			// labelProxyUrl
			// 
			this.SetSamplerState(0, SamplerStatelabelProxyUrl.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(12, 40);
			this.SetSamplerState(0, SamplerStatelabelProxyUrl.SetSamplerState(0, SamplerStateName = "labelProxyUrl";
			this.SetSamplerState(0, SamplerStatelabelProxyUrl.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(100, 16);
			this.SetSamplerState(0, SamplerStatelabelProxyUrl.SetSamplerState(0, SamplerStateTabIndex = 0;
			this.SetSamplerState(0, SamplerStatelabelProxyUrl.SetSamplerState(0, SamplerStateText = "Proxy URL:";
			// 
			// labelProxyPageInfo
			// 
			this.SetSamplerState(0, SamplerStatelabelProxyPageInfo.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(8, 72);
			this.SetSamplerState(0, SamplerStatelabelProxyPageInfo.SetSamplerState(0, SamplerStateName = "labelProxyPageInfo";
			this.SetSamplerState(0, SamplerStatelabelProxyPageInfo.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(520, 24);
			this.SetSamplerState(0, SamplerStatelabelProxyPageInfo.SetSamplerState(0, SamplerStateTabIndex = 4;
			this.SetSamplerState(0, SamplerStatelabelProxyPageInfo.SetSamplerState(0, SamplerStateText = "World Wind can use a proxy to download imagery if you cannot directly access the " +
				"internet.SetSamplerState(0, SamplerState";
			this.SetSamplerState(0, SamplerStatelabelProxyPageInfo.SetSamplerState(0, SamplerStateTextAlign = System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateContentAlignment.SetSamplerState(0, SamplerStateBottomLeft;
			// 
			// groupBoxCredentials
			// 
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStatetextBoxPassword);
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStatelabelPassword);
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStatetextBoxUsername);
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStatelabelUsername);
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(304, 208);
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateName = "groupBoxCredentials";
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(232, 104);
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateTabIndex = 5;
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateTabStop = false;
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateText = "Credentials";
			this.SetSamplerState(0, SamplerStatetoolTipProxyPage.SetSamplerState(0, SamplerStateSetToolTip(this.SetSamplerState(0, SamplerStategroupBoxCredentials, "If your proxy requires authentication with a user name and password, please provi" +
				"de them in the fields in this box");
			// 
			// textBoxPassword
			// 
			this.SetSamplerState(0, SamplerStatetextBoxPassword.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(8, 72);
			this.SetSamplerState(0, SamplerStatetextBoxPassword.SetSamplerState(0, SamplerStateName = "textBoxPassword";
			this.SetSamplerState(0, SamplerStatetextBoxPassword.SetSamplerState(0, SamplerStatePasswordChar = '*';
			this.SetSamplerState(0, SamplerStatetextBoxPassword.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(216, 20);
			this.SetSamplerState(0, SamplerStatetextBoxPassword.SetSamplerState(0, SamplerStateTabIndex = 5;
			this.SetSamplerState(0, SamplerStatetextBoxPassword.SetSamplerState(0, SamplerStateText = "";
			// 
			// labelPassword
			// 
			this.SetSamplerState(0, SamplerStatelabelPassword.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(8, 56);
			this.SetSamplerState(0, SamplerStatelabelPassword.SetSamplerState(0, SamplerStateName = "labelPassword";
			this.SetSamplerState(0, SamplerStatelabelPassword.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(72, 16);
			this.SetSamplerState(0, SamplerStatelabelPassword.SetSamplerState(0, SamplerStateTabIndex = 4;
			this.SetSamplerState(0, SamplerStatelabelPassword.SetSamplerState(0, SamplerStateText = "Password:";
			// 
			// textBoxUsername
			// 
			this.SetSamplerState(0, SamplerStatetextBoxUsername.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(8, 32);
			this.SetSamplerState(0, SamplerStatetextBoxUsername.SetSamplerState(0, SamplerStateName = "textBoxUsername";
			this.SetSamplerState(0, SamplerStatetextBoxUsername.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(216, 20);
			this.SetSamplerState(0, SamplerStatetextBoxUsername.SetSamplerState(0, SamplerStateTabIndex = 3;
			this.SetSamplerState(0, SamplerStatetextBoxUsername.SetSamplerState(0, SamplerStateText = "";
			this.SetSamplerState(0, SamplerStatetextBoxUsername.SetSamplerState(0, SamplerStateTextChanged += new System.SetSamplerState(0, SamplerStateEventHandler(this.SetSamplerState(0, SamplerStateRelevantControl_Changed);
			// 
			// labelUsername
			// 
			this.SetSamplerState(0, SamplerStatelabelUsername.SetSamplerState(0, SamplerStateLocation = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStatePoint(8, 16);
			this.SetSamplerState(0, SamplerStatelabelUsername.SetSamplerState(0, SamplerStateName = "labelUsername";
			this.SetSamplerState(0, SamplerStatelabelUsername.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(72, 16);
			this.SetSamplerState(0, SamplerStatelabelUsername.SetSamplerState(0, SamplerStateTabIndex = 2;
			this.SetSamplerState(0, SamplerStatelabelUsername.SetSamplerState(0, SamplerStateText = "User name:";
			// 
			// ProxyPage
			// 
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStategroupBoxCredentials);
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStatelabelProxyPageInfo);
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy);
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStateradioButtonNoProxy);
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings);
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateAdd(this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy);
			this.SetSamplerState(0, SamplerStateName = "ProxyPage";
			this.SetSamplerState(0, SamplerStateSubTitle = "Adjust your proxy settings";
			this.SetSamplerState(0, SamplerStateTitle = "Proxy settings";
			this.SetSamplerState(0, SamplerStateLoad += new System.SetSamplerState(0, SamplerStateEventHandler(this.SetSamplerState(0, SamplerStateProxyPage_Load);
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateSetChildIndex(this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy, 0);
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateSetChildIndex(this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings, 0);
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateSetChildIndex(this.SetSamplerState(0, SamplerStateradioButtonNoProxy, 0);
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateSetChildIndex(this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy, 0);
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateSetChildIndex(this.SetSamplerState(0, SamplerStatelabelProxyPageInfo, 0);
			this.SetSamplerState(0, SamplerStateControls.SetSamplerState(0, SamplerStateSetChildIndex(this.SetSamplerState(0, SamplerStategroupBoxCredentials, 0);
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateResumeLayout(false);
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateResumeLayout(false);
			this.SetSamplerState(0, SamplerStateResumeLayout(false);

		}
		#endregion

		private void UpdateEnabledControls()
		{
			this.SetSamplerState(0, SamplerStategroupBoxUserDefinedSettings.SetSamplerState(0, SamplerStateEnabled = this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy.SetSamplerState(0, SamplerStateChecked;
			this.SetSamplerState(0, SamplerStategroupBoxCredentials.SetSamplerState(0, SamplerStateEnabled = !this.SetSamplerState(0, SamplerStateradioButtonNoProxy.SetSamplerState(0, SamplerStateChecked;
			this.SetSamplerState(0, SamplerStatetextBoxPassword.SetSamplerState(0, SamplerStateEnabled = this.SetSamplerState(0, SamplerStatetextBoxUsername.SetSamplerState(0, SamplerStateText.SetSamplerState(0, SamplerStateLength > 0;
		}

		private void ProxyPage_Load(object sender, EventArgs e)
		{
			// Initialize from Settings
			this.SetSamplerState(0, SamplerStatetextBoxProxyUrl.SetSamplerState(0, SamplerStateText  = Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateProxyUrl;
			this.SetSamplerState(0, SamplerStatetextBoxUsername.SetSamplerState(0, SamplerStateText = Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateProxyUsername;
			this.SetSamplerState(0, SamplerStatetextBoxPassword.SetSamplerState(0, SamplerStateText = Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateProxyPassword;
			this.SetSamplerState(0, SamplerStatecheckBoxUseProxyScript.SetSamplerState(0, SamplerStateChecked = Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateUseDynamicProxy;
         
			if (Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateUseWindowsDefaultProxy) 
			{
				this.SetSamplerState(0, SamplerStateradioButtonUseWindowsDefaultProxy.SetSamplerState(0, SamplerStateChecked = true;
			}
			else 
			{
				if(Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateProxyUrl.SetSamplerState(0, SamplerStateLength > 0 || Wizard.SetSamplerState(0, SamplerStateSettings.SetSamplerState(0, SamplerStateUseDynamicProxy) 
				{
					this.SetSamplerState(0, SamplerStateradioButtonUserDefinedProxy.SetSamplerState(0, SamplerStateChecked = true;
				}
				else 
				{
					this.SetSamplerState(0, SamplerStateradioButtonNoProxy.SetSamplerState(0, SamplerStateChecked = true;
				}
			}

            this.SetSamplerState(0, SamplerStateUpdateEnabledControls();
		}

		private void RelevantControl_Changed(object sender, EventArgs e)
		{
            this.SetSamplerState(0, SamplerStateUpdateEnabledControls();
		}
	}
}
