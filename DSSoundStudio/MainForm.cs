using System;
using System.Windows.Forms;
using DSSoundStudio.UI;
using DSSoundStudio.Util;
using NAudio.Wave;

namespace DSSoundStudio
{
	// Token: 0x02000013 RID: 19
	public partial class MainForm : Form
	{
		public static WaveOut waveOut;
		public static BufferedWaveProvider bufferedWaveProvider;
		public const int woutSamplesPerIteration = 341;
		public const int woutChannels = 2;
        public const int woutByteSize = woutSamplesPerIteration * woutChannels * 2;
        public const int woutChunks = 16;
        public const int woutSampleRate = 65456;

        // Token: 0x06000055 RID: 85 RVA: 0x000088F3 File Offset: 0x00006AF3
        public MainForm()
		{
			waveOut = new WaveOut {
				DesiredLatency = 150
			};

            WaveFormat wf = new WaveFormat(woutSampleRate, 16, woutChannels);
            bufferedWaveProvider = new BufferedWaveProvider(wf) {
				DiscardOnBufferOverflow = true,
				BufferLength = woutByteSize * woutChunks,
			};
			waveOut.Init(bufferedWaveProvider);
			InitializeComponent();
		}

		// Token: 0x06000056 RID: 86 RVA: 0x0000891C File Offset: 0x00006B1C
		private void MainForm_Load(object sender, EventArgs e) {
            for (int i = 0; i < Controls.Count; i++) {
                if (Controls[i] is MdiClient mdiClient) {
                    Win32Util.SetMDIBorderStyle(mdiClient, BorderStyle.None);
                }
            }
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00008968 File Offset: 0x00006B68
		private void menuOpen(object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Length > 0) {
                if (openFileDialog1.FileName == CurPath && SDATViewerForm != null) {
					MessageBox.Show("This file is already open!");
					SDATViewerForm.BringToFront();
                } else if (SDATViewerForm == null || MessageBox.Show("Are you sure you want to close the currently opened SDAT and its subfiles?", "Close", MessageBoxButtons.YesNo) != DialogResult.No) {
                    foreach (Form form in MdiChildren) {
						form.Close();
					}
					CurPath = openFileDialog1.FileName;
                    SDATViewerForm = new SDATViewer(openFileDialog1.FileName) {
                        MdiParent = this
                    };
                    SDATViewerForm.FormClosed += SDATViewerForm_FormClosed;
					SDATViewerForm.Show();
				}
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00008A90 File Offset: 0x00006C90
		private void SDATViewerForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			foreach (Form form in MdiChildren)
			{
				if (form != SDATViewerForm)
				{
					form.Close();
				}
			}
			SDATViewerForm = null;
			CurPath = null;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00008AE0 File Offset: 0x00006CE0
		private void MainForm_MdiChildActivate(object sender, EventArgs e)
		{
			if (ActiveMdiChild is ISaveable)
			{
				toolStripButtonSave.Enabled = menuItemSave.Enabled = true;
			}
			else
			{
				toolStripButtonSave.Enabled = menuItemSave.Enabled = false;
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00008B40 File Offset: 0x00006D40
		private void toolStripButtonSave_Click(object sender, EventArgs e)
		{
			if (ActiveMdiChild is ISaveable)
			{
				((ISaveable)ActiveMdiChild).Save();
			}
		}

		// Token: 0x0400007E RID: 126
		private SDATViewer SDATViewerForm;

		// Token: 0x0400007F RID: 127
		private string CurPath;
	}
}
