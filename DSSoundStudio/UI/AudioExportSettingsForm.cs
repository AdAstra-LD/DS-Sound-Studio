using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DSSoundStudio.UI {
    public partial class AudioExportSettingsForm : Form {
        public int minutes { get; private set; }
        public int seconds { get; private set; }
        public int samplingRate;
        public string path;
        public ushort tracks = 0;
        public string filename { get; set; }
        public AudioExportSettingsForm() {
            InitializeComponent();

            this.minutes = (int)minutesNumericUpDown.Value;
            this.seconds = (int)secondsNumericUpDown.Value;
        }

        private void browseButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog {
                Filter = "WAV File (*.wav)|*.wav",
                FileName = $"{this.filename}.wav"
            };

            if (sf.ShowDialog() == DialogResult.OK) {
                this.browsePathTextBox.Text = sf.FileName;
            }
        }

        private void samplingRateNumericUpDown_ValueChanged(object sender, EventArgs e) {
            this.samplingRate = (int)samplingRateNumericUpDown.Value;
        }

        private void secondsNumericUpDown_ValueChanged(object sender, EventArgs e) {
            this.seconds = (int)secondsNumericUpDown.Value;
        }

        private void minutesNumericUpDown_ValueChanged(object sender, EventArgs e) {
            this.minutes = (int)minutesNumericUpDown.Value;
        }
        private void TrackCheckBox_CheckedChanged(object sender, EventArgs e) {
            ushort t = 0;
            System.Collections.IList list = tracksFlowLayoutPanel.Controls;
            for (int i = 0; i < list.Count; i++) {
                CheckBox c = list[i] as CheckBox;
                if (c.Checked) {
                    t |= (ushort)(1 << i);
                }   
            }
            this.tracks = t;
        }
        private void AudioExportSettingsForm_Load(object sender, EventArgs e) {
            samplingRateNumericUpDown.Value = samplingRate;
            browsePathTextBox.Text = path;

            System.Collections.IList list = tracksFlowLayoutPanel.Controls;
            for (int i = 0; i < list.Count; i++) {
                CheckBox c = (CheckBox)list[i];
                c.Text = i.ToString("D2");
                c.CheckedChanged += TrackCheckBox_CheckedChanged;

                if (c.Checked) {
                    this.tracks |= (ushort)(1 << i);
                }
            }
        }

        private void browsePathTextBox_TextChanged(object sender, EventArgs e) {
            this.path = browsePathTextBox.Text;
        }
    }
}
