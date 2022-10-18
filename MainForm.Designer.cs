namespace sshWT
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.connection = new System.Windows.Forms.TextBox();
            this.addBtn = new System.Windows.Forms.Button();
            this.connectionsLbl = new System.Windows.Forms.Label();
            this.start = new System.Windows.Forms.Button();
            this.startSplitPaneHorizontal = new System.Windows.Forms.Button();
            this.placeHolder = new System.Windows.Forms.Label();
            this.deleteBtn = new System.Windows.Forms.Button();
            this.container = new System.Windows.Forms.Panel();
            this.reloadBtn = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "SSH connection parameters:";
            // 
            // connection
            // 
            this.connection.Location = new System.Drawing.Point(12, 27);
            this.connection.Name = "connection";
            this.connection.Size = new System.Drawing.Size(400, 23);
            this.connection.TabIndex = 1;
            this.connection.KeyUp += new System.Windows.Forms.KeyEventHandler(this.connection_KeyUp);
            // 
            // addBtn
            // 
            this.addBtn.Location = new System.Drawing.Point(418, 27);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(52, 23);
            this.addBtn.TabIndex = 2;
            this.addBtn.Text = "Add";
            this.addBtn.UseVisualStyleBackColor = true;
            this.addBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // connectionsLbl
            // 
            this.connectionsLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.connectionsLbl.AutoSize = true;
            this.connectionsLbl.Location = new System.Drawing.Point(12, 69);
            this.connectionsLbl.Name = "connectionsLbl";
            this.connectionsLbl.Size = new System.Drawing.Size(74, 15);
            this.connectionsLbl.TabIndex = 3;
            this.connectionsLbl.Text = "Connections";
            this.connectionsLbl.Click += new System.EventHandler(this.ConnectionsLbl_Click);
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(418, 65);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(52, 23);
            this.start.TabIndex = 4;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Visible = false;
            // 
            // startSplitPaneHorizontal
            // 
            this.startSplitPaneHorizontal.Location = new System.Drawing.Point(476, 65);
            this.startSplitPaneHorizontal.Name = "startSplitPaneHorizontal";
            this.startSplitPaneHorizontal.Size = new System.Drawing.Size(56, 23);
            this.startSplitPaneHorizontal.TabIndex = 5;
            this.startSplitPaneHorizontal.Text = "|—";
            this.startSplitPaneHorizontal.UseVisualStyleBackColor = true;
            this.startSplitPaneHorizontal.Visible = false;
            // 
            // placeHolder
            // 
            this.placeHolder.Location = new System.Drawing.Point(12, 97);
            this.placeHolder.Name = "placeHolder";
            this.placeHolder.Size = new System.Drawing.Size(377, 15);
            this.placeHolder.TabIndex = 7;
            this.placeHolder.Text = "label3";
            this.placeHolder.Visible = false;
            // 
            // deleteBtn
            // 
            this.deleteBtn.Location = new System.Drawing.Point(476, 27);
            this.deleteBtn.Name = "deleteBtn";
            this.deleteBtn.Size = new System.Drawing.Size(56, 23);
            this.deleteBtn.TabIndex = 8;
            this.deleteBtn.Text = "Delete";
            this.deleteBtn.UseVisualStyleBackColor = true;
            this.deleteBtn.Click += new System.EventHandler(this.DeleteBtn_Click);
            // 
            // container
            // 
            this.container.AutoScroll = true;
            this.container.Location = new System.Drawing.Point(0, 115);
            this.container.Name = "container";
            this.container.Size = new System.Drawing.Size(532, 707);
            this.container.TabIndex = 9;
            // 
            // reloadBtn
            // 
            this.reloadBtn.AutoSize = true;
            this.reloadBtn.Location = new System.Drawing.Point(489, 69);
            this.reloadBtn.Name = "reloadBtn";
            this.reloadBtn.Size = new System.Drawing.Size(43, 15);
            this.reloadBtn.TabIndex = 10;
            this.reloadBtn.Text = "Reload";
            this.reloadBtn.Click += new System.EventHandler(this.ReloadBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 834);
            this.Controls.Add(this.reloadBtn);
            this.Controls.Add(this.container);
            this.Controls.Add(this.deleteBtn);
            this.Controls.Add(this.placeHolder);
            this.Controls.Add(this.startSplitPaneHorizontal);
            this.Controls.Add(this.start);
            this.Controls.Add(this.connectionsLbl);
            this.Controls.Add(this.addBtn);
            this.Controls.Add(this.connection);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(560, 873);
            this.MinimumSize = new System.Drawing.Size(560, 873);
            this.Name = "MainForm";
            this.Text = "sshWT";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox connection;
        private Button addBtn;
        private Label connectionsLbl;
        private Button start;
        private Button startSplitPaneHorizontal;
        private Label placeHolder;
        private Button deleteBtn;
        private Panel container;
        private ToolTip toolTip1;
        private Label reloadBtn;
    }
}