using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.IO;
using System.Security.Cryptography;

namespace DSSoundStudio {
    public class AudioExportSettings {

        public WaveFileWriter waveWriter;
        public string outWaveWriterPath;
        public int sampleRate;
        public int sampleCount;
        public ushort enabledTracks;
        public int bytesCount { get; private set; }

        public AudioExportSettings(int sampleRate, string path, WaveFormat wf) {
            this.sampleRate = sampleRate;

            if (sampleRate == MainForm.woutSampleRate) {
                this.outWaveWriterPath = path;
            } else {
                this.outWaveWriterPath = MainForm.tempWavPath;
            }
            this.waveWriter = new WaveFileWriter(this.outWaveWriterPath, wf);
        }
        public static void Resample(string inFile, string outFile, int outRate) {
            using (var reader = new AudioFileReader(inFile)) {
                var resampler = new WdlResamplingSampleProvider(reader, outRate);
                WaveFileWriter.CreateWaveFile16(outFile, resampler);
            }
        }
        public void TryResample() {
            if (this.sampleRate != MainForm.woutSampleRate) {
                Resample(this.waveWriter.Filename, this.outWaveWriterPath, this.sampleRate);

                if (File.Exists(MainForm.tempWavPath)) {
                    File.Delete(MainForm.tempWavPath);
                }
            }
        }
        public void setOutWaveLength(int minutes, int seconds) {
            this.sampleCount = (seconds + (minutes * 60)) * sampleRate;
            this.bytesCount = this.sampleCount * MainForm.woutChannels * 2;
        }
    }
}