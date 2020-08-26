namespace HZJ.StockMarket
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.StockManager = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.ltView = new System.Windows.Forms.ListView();
            this.cCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cDiffPercent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cDiffPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StockManager});
            this.MenuStrip.Name = "contextMenuStrip1";
            this.MenuStrip.Size = new System.Drawing.Size(125, 26);
            // 
            // StockManager
            // 
            this.StockManager.Name = "StockManager";
            this.StockManager.Size = new System.Drawing.Size(124, 22);
            this.StockManager.Text = "代码管理";
            this.StockManager.Click += new System.EventHandler(this.StockManager_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // trackBar1
            // 
            this.trackBar1.AutoSize = false;
            this.trackBar1.ContextMenuStrip = this.MenuStrip;
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.trackBar1.Location = new System.Drawing.Point(312, 0);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(21, 119);
            this.trackBar1.TabIndex = 3;
            this.trackBar1.Value = 50;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            this.trackBar1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ESCKeyDown_AppExit);
            // 
            // ltView
            // 
            this.ltView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ltView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cCode,
            this.cName,
            this.cPrice,
            this.cDiffPercent,
            this.cDiffPrice});
            this.ltView.ContextMenuStrip = this.MenuStrip;
            this.ltView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ltView.FullRowSelect = true;
            this.ltView.GridLines = true;
            this.ltView.HideSelection = false;
            this.ltView.Location = new System.Drawing.Point(0, 0);
            this.ltView.MultiSelect = false;
            this.ltView.Name = "ltView";
            this.ltView.ShowGroups = false;
            this.ltView.Size = new System.Drawing.Size(312, 119);
            this.ltView.TabIndex = 5;
            this.ltView.UseCompatibleStateImageBehavior = false;
            this.ltView.View = System.Windows.Forms.View.Details;
            this.ltView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ESCKeyDown_AppExit);
            // 
            // cCode
            // 
            this.cCode.Text = "代码";
            // 
            // cName
            // 
            this.cName.Text = "名称";
            // 
            // cPrice
            // 
            this.cPrice.Text = "价格";
            // 
            // cDiffPercent
            // 
            this.cDiffPercent.DisplayIndex = 4;
            this.cDiffPercent.Text = "涨跌";
            // 
            // cDiffPrice
            // 
            this.cDiffPrice.DisplayIndex = 3;
            this.cDiffPrice.Text = "涨幅";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 119);
            this.Controls.Add(this.ltView);
            this.Controls.Add(this.trackBar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Opacity = 0.6D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ESCKeyDown_AppExit);
            this.MenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip MenuStrip;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.ListView ltView;
        private System.Windows.Forms.ColumnHeader cCode;
        private System.Windows.Forms.ColumnHeader cName;
        private System.Windows.Forms.ColumnHeader cPrice;
        private System.Windows.Forms.ColumnHeader cDiffPrice;
        private System.Windows.Forms.ColumnHeader cDiffPercent;
        private System.Windows.Forms.ToolStripMenuItem StockManager;
    }
}

