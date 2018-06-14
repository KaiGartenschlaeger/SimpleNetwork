namespace ConsoleWrapper
{
    partial class FormMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbxClientInput = new System.Windows.Forms.TextBox();
            this.tbxClient = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbxServerInput = new System.Windows.Forms.TextBox();
            this.tbxServer = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(634, 452);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tbxClientInput);
            this.panel2.Controls.Add(this.tbxClient);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 229);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(628, 220);
            this.panel2.TabIndex = 1;
            // 
            // tbxClientInput
            // 
            this.tbxClientInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxClientInput.AutoCompleteCustomSource.AddRange(new string[] {
            "send ",
            "sendf",
            "spam ",
            "statistics",
            "clear",
            "start",
            "stop",
            "exit"});
            this.tbxClientInput.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.tbxClientInput.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbxClientInput.BackColor = System.Drawing.Color.Black;
            this.tbxClientInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxClientInput.ForeColor = System.Drawing.Color.White;
            this.tbxClientInput.Location = new System.Drawing.Point(0, 200);
            this.tbxClientInput.Name = "tbxClientInput";
            this.tbxClientInput.Size = new System.Drawing.Size(628, 20);
            this.tbxClientInput.TabIndex = 0;
            this.tbxClientInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbxClientInput_KeyUp);
            // 
            // tbxClient
            // 
            this.tbxClient.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxClient.BackColor = System.Drawing.Color.Black;
            this.tbxClient.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxClient.ForeColor = System.Drawing.Color.White;
            this.tbxClient.Location = new System.Drawing.Point(0, 0);
            this.tbxClient.Multiline = true;
            this.tbxClient.Name = "tbxClient";
            this.tbxClient.ReadOnly = true;
            this.tbxClient.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxClient.Size = new System.Drawing.Size(628, 194);
            this.tbxClient.TabIndex = 1;
            this.tbxClient.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tbxServerInput);
            this.panel1.Controls.Add(this.tbxServer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(628, 220);
            this.panel1.TabIndex = 0;
            // 
            // tbxServerInput
            // 
            this.tbxServerInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxServerInput.AutoCompleteCustomSource.AddRange(new string[] {
            "send",
            "sendf",
            "spam ",
            "statistics",
            "clear",
            "start",
            "stop",
            "exit"});
            this.tbxServerInput.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.tbxServerInput.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbxServerInput.BackColor = System.Drawing.Color.Black;
            this.tbxServerInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxServerInput.ForeColor = System.Drawing.Color.White;
            this.tbxServerInput.Location = new System.Drawing.Point(0, 200);
            this.tbxServerInput.Name = "tbxServerInput";
            this.tbxServerInput.Size = new System.Drawing.Size(628, 20);
            this.tbxServerInput.TabIndex = 0;
            this.tbxServerInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbxServerInput_KeyUp);
            // 
            // tbxServer
            // 
            this.tbxServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxServer.BackColor = System.Drawing.Color.Black;
            this.tbxServer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxServer.ForeColor = System.Drawing.Color.White;
            this.tbxServer.Location = new System.Drawing.Point(0, 0);
            this.tbxServer.Multiline = true;
            this.tbxServer.Name = "tbxServer";
            this.tbxServer.ReadOnly = true;
            this.tbxServer.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxServer.Size = new System.Drawing.Size(628, 194);
            this.tbxServer.TabIndex = 1;
            this.tbxServer.TabStop = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(634, 452);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testformular";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox tbxClientInput;
        private System.Windows.Forms.TextBox tbxClient;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbxServerInput;
        private System.Windows.Forms.TextBox tbxServer;
    }
}

