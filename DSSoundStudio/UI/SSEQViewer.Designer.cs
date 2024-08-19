using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DSSoundStudio.Properties;
using FastColoredTextBoxNS;

namespace DSSoundStudio.UI
{
	// Token: 0x02000004 RID: 4
	public partial class SSEQViewer : Form
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00004F8C File Offset: 0x0000318C
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00004FDC File Offset: 0x000031DC
		private void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SSEQViewer));
			this.mainMenu1 = new MainMenu(this.components);
			this.vistaMenu1 = new VistaMenu(this.components);
			this.toolStrip1 = new ToolStrip();
			this.toolStripButtonPlayPause = new ToolStripButton();
			this.toolStripButtonStop = new ToolStripButton();
			this.toolStripButtonExport = new ToolStripButton();
            this.splitContainer1 = new SplitContainer();
			this.fastColoredTextBox1 = new FastColoredTextBox();
			this.splitContainer2 = new SplitContainer();
			this.tabControl1 = new TabControl();
			this.tabPage1 = new TabPage();
			this.tabPage2 = new TabPage();
			((ISupportInitialize)this.vistaMenu1).BeginInit();
			this.toolStrip1.SuspendLayout();
			((ISupportInitialize)this.splitContainer1).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((ISupportInitialize)this.fastColoredTextBox1).BeginInit();
			((ISupportInitialize)this.splitContainer2).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.tabControl1.SuspendLayout();
			base.SuspendLayout();
			this.vistaMenu1.ContainerControl = this;
			this.toolStrip1.Items.AddRange(new ToolStripItem[]
			{
				this.toolStripButtonPlayPause,
				this.toolStripButtonStop,
				this.toolStripButtonExport
            });
			this.toolStrip1.Location = new Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new Size(575, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			this.toolStripButtonPlayPause.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonPlayPause.Image = Resources.control;
			this.toolStripButtonPlayPause.ImageTransparentColor = Color.Magenta;
			this.toolStripButtonPlayPause.Name = "toolStripButtonPlayPause";
			this.toolStripButtonPlayPause.Size = new Size(23, 22);
			this.toolStripButtonPlayPause.Click += new EventHandler(this.toolStripButtonPlayPause_Click);
			this.toolStripButtonStop.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonStop.Image = Resources.control_stop_square;
			this.toolStripButtonStop.ImageTransparentColor = Color.Magenta;
			this.toolStripButtonStop.Name = "toolStripButtonStop";
			this.toolStripButtonStop.Size = new Size(23, 22);
			this.toolStripButtonStop.Click += new EventHandler(this.toolStripButtonStop_Click);

            this.toolStripButtonExport.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonExport.Image = (Image)componentResourceManager.GetObject("menuItemSaveAll.Image");
            this.toolStripButtonExport.ImageTransparentColor = Color.Magenta;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Size = new Size(23, 22);
			this.toolStripButtonExport.Click += new EventHandler(this.toolStripButtonExport_Click);


            this.splitContainer1.Dock = DockStyle.Fill;
			this.splitContainer1.FixedPanel = FixedPanel.Panel2;
			this.splitContainer1.Location = new Point(0, 25);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = Orientation.Horizontal;
			this.splitContainer1.Panel1.Controls.Add(this.fastColoredTextBox1);
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new Size(575, 335);
			this.splitContainer1.SplitterDistance = 202;
			this.splitContainer1.TabIndex = 1;
			this.fastColoredTextBox1.AllowMacroRecording = false;
			this.fastColoredTextBox1.AutoCompleteBracketsList = new char[]
			{
				'(',
				')',
				'{',
				'}',
				'[',
				']',
				'"',
				'"',
				'\'',
				'\''
			};
			this.fastColoredTextBox1.AutoScrollMinSize = new Size(195, 14);
			this.fastColoredTextBox1.BackBrush = null;
			this.fastColoredTextBox1.CharHeight = 14;
			this.fastColoredTextBox1.CharWidth = 8;
			this.fastColoredTextBox1.Cursor = Cursors.IBeam;
			this.fastColoredTextBox1.DisabledColor = Color.FromArgb(100, 180, 180, 180);
			this.fastColoredTextBox1.Dock = DockStyle.Fill;
			this.fastColoredTextBox1.IsReplaceMode = false;
			this.fastColoredTextBox1.LeftPadding = 16;
			this.fastColoredTextBox1.Location = new Point(0, 0);
			this.fastColoredTextBox1.Name = "fastColoredTextBox1";
			this.fastColoredTextBox1.Paddings = new Padding(0);
			this.fastColoredTextBox1.ReadOnly = true;
			this.fastColoredTextBox1.SelectionColor = Color.FromArgb(60, 0, 0, 255);
			this.fastColoredTextBox1.ServiceColors = (ServiceColors)componentResourceManager.GetObject("fastColoredTextBox1.ServiceColors");
			this.fastColoredTextBox1.Size = new Size(575, 202);
			this.fastColoredTextBox1.TabIndex = 0;
			this.fastColoredTextBox1.Text = "fastColoredTextBox1";
			this.fastColoredTextBox1.Zoom = 100;
			this.fastColoredTextBox1.TextChanged += new EventHandler<TextChangedEventArgs>(this.fastColoredTextBox1_TextChanged);
			this.fastColoredTextBox1.AutoIndentNeeded += new EventHandler<AutoIndentEventArgs>(this.fastColoredTextBox1_AutoIndentNeeded);
			this.splitContainer2.Dock = DockStyle.Fill;
			this.splitContainer2.Location = new Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Panel1.Controls.Add(this.tabControl1);
			this.splitContainer2.Size = new Size(575, 129);
			this.splitContainer2.SplitterDistance = 272;
			this.splitContainer2.TabIndex = 0;
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = DockStyle.Fill;
			this.tabControl1.Location = new Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new Size(272, 129);
			this.tabControl1.TabIndex = 0;
			this.tabPage1.Location = new Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new Padding(3);
			this.tabPage1.Size = new Size(264, 103);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
			this.tabPage2.Location = new Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new Padding(3);
			this.tabPage2.Size = new Size(264, 103);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(575, 360);
			base.Controls.Add(this.splitContainer1);
			base.Controls.Add(this.toolStrip1);
			base.Menu = this.mainMenu1;
			base.Name = "SSEQViewer";
			this.Text = "SSEQViewer";
			base.FormClosing += new FormClosingEventHandler(this.SSEQViewer_FormClosing);
			base.Load += new EventHandler(this.SSEQViewer_Load);
			((ISupportInitialize)this.vistaMenu1).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((ISupportInitialize)this.splitContainer1).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((ISupportInitialize)this.fastColoredTextBox1).EndInit();
			this.splitContainer2.Panel1.ResumeLayout(false);
			((ISupportInitialize)this.splitContainer2).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

        // Token: 0x04000021 RID: 33
        private IContainer components = null;

		// Token: 0x04000022 RID: 34
		private MainMenu mainMenu1;

		// Token: 0x04000023 RID: 35
		private VistaMenu vistaMenu1;

		// Token: 0x04000024 RID: 36
		private SplitContainer splitContainer1;

		// Token: 0x04000025 RID: 37
		private SplitContainer splitContainer2;

		// Token: 0x04000026 RID: 38
		private TabControl tabControl1;

		// Token: 0x04000027 RID: 39
		private TabPage tabPage1;

		// Token: 0x04000028 RID: 40
		private TabPage tabPage2;

		// Token: 0x04000029 RID: 41
		private ToolStrip toolStrip1;

		// Token: 0x0400002A RID: 42
		private ToolStripButton toolStripButtonPlayPause;

		// Token: 0x0400002B RID: 43
		private ToolStripButton toolStripButtonStop;

        private ToolStripButton toolStripButtonExport;


        // Token: 0x0400002C RID: 44
        private FastColoredTextBox fastColoredTextBox1;
	}
}
