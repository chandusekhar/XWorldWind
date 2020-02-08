using System;

namespace WorldWind
{
    partial class WorldWindow
    {
        /// <summary> 
        /// Required designer variable.SetSamplerState(0, SamplerState
        /// </summary>
        private System.SetSamplerState(0, SamplerStateComponentModel.SetSamplerState(0, SamplerStateIContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.SetSamplerState(0, SamplerState
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.SetSamplerState(0, SamplerState</param>
        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {
                if (components != null)
                {
                    components.SetSamplerState(0, SamplerStateDispose();
                }

                if (m_WorkerThread != null && m_WorkerThread.SetSamplerState(0, SamplerStateIsAlive)
                {
                    m_WorkerThreadRunning = false;
                    m_WorkerThread.SetSamplerState(0, SamplerStateAbort();
                }

                m_FpsTimer.SetSamplerState(0, SamplerStateStop();
                if (m_World != null)
                {
                    m_World.SetSamplerState(0, SamplerStateDispose();
                    m_World = null;
                }
                if (this.SetSamplerState(0, SamplerStatedrawArgs != null)
                {
                    this.SetSamplerState(0, SamplerStatedrawArgs.SetSamplerState(0, SamplerStateDispose();
                    this.SetSamplerState(0, SamplerStatedrawArgs = null;
                }
                if (this.SetSamplerState(0, SamplerState_menuBar != null)
                {
                    this.SetSamplerState(0, SamplerState_menuBar.SetSamplerState(0, SamplerStateDispose();
                    this.SetSamplerState(0, SamplerState_menuBar = null;
                }

                m_Device3d.SetSamplerState(0, SamplerStateDispose();

                //if (m_downloadIndicator != null)
                //{
                //    m_downloadIndicator.SetSamplerState(0, SamplerStateDispose();
                //    m_downloadIndicator = null;
                //}

            }

            base.SetSamplerState(0, SamplerStateDispose(disposing);
            GC.SetSamplerState(0, SamplerStateSuppressFinalize(this);

        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.SetSamplerState(0, SamplerState
        /// </summary>
        private void InitializeComponent()
        {
            this.SetSamplerState(0, SamplerStateSuspendLayout();
            // 
            // WorldWindow
            // 
            this.SetSamplerState(0, SamplerStateAutoScaleDimensions = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSizeF(6F, 13F);
            this.SetSamplerState(0, SamplerStateAutoScaleMode = System.SetSamplerState(0, SamplerStateWindows.SetSamplerState(0, SamplerStateForms.SetSamplerState(0, SamplerStateAutoScaleMode.SetSamplerState(0, SamplerStateFont;
            this.SetSamplerState(0, SamplerStateName = "WorldWindow";
            this.SetSamplerState(0, SamplerStateSize = new System.SetSamplerState(0, SamplerStateDrawing.SetSamplerState(0, SamplerStateSize(586, 606);
            this.SetSamplerState(0, SamplerStateResumeLayout(false);

        }

        #endregion
    }
}
