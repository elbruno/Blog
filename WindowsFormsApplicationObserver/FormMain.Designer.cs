namespace WindowsFormsApplicationObserver
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnOpenMessageDispatcher = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(15, 79);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(468, 347);
            this.txtLog.TabIndex = 0;
            // 
            // btnOpenMessageDispatcher
            // 
            this.btnOpenMessageDispatcher.Location = new System.Drawing.Point(15, 20);
            this.btnOpenMessageDispatcher.Name = "btnOpenMessageDispatcher";
            this.btnOpenMessageDispatcher.Size = new System.Drawing.Size(468, 41);
            this.btnOpenMessageDispatcher.TabIndex = 2;
            this.btnOpenMessageDispatcher.Text = "Open message Dispatcher";
            this.btnOpenMessageDispatcher.UseVisualStyleBackColor = true;
            this.btnOpenMessageDispatcher.Click += new System.EventHandler(this.btnOpenMessageDispatcher_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 438);
            this.Controls.Add(this.btnOpenMessageDispatcher);
            this.Controls.Add(this.txtLog);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnOpenMessageDispatcher;
    }
}

