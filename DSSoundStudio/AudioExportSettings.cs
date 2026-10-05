using System;
using LibDSSound.Software;
using NAudio.Wave;

namespace DSSoundStudio {
    public class AudioExportSettings {

        public WaveFileWriter waveWriter;
        public int sampleRate;
        public int sampleCount;
        public ushort enabledTracks = ushort.MaxValue;
        public int bytesCount { get; private set; }

        public AudioExportSettings(int sampleRate, string path) {
            this.sampleRate = sampleRate;
            this.waveWriter = new WaveFileWriter(path, new WaveFormat(sampleRate, 16, MainForm.woutChannels));
        }
        public void setOutWaveLength(int minutes, int seconds) {
            this.sampleCount = (seconds + (minutes * 60)) * sampleRate;
            this.bytesCount = this.sampleCount * MainForm.woutChannels * 2;
        }

        // Render as fast as the CPU allows, one sound frame at a time, until the requested length is written or cancelled() returns true
        // The native rate is written as the engine produces it. any other rate goes through the resampler and is dithered back to 16 bits.
        public void Render(SNDWork sndwork, Func<bool> cancelled) {
            try {
                byte[] buffer = new byte[MainForm.woutByteSize];
                Resampler resampler = sampleRate == MainForm.woutSampleRate ? null : new Resampler(MainForm.woutSampleRate, sampleRate);
                int filled = 0;
                int written = 0;
                uint ditherState = 0x9E3779B9u;

                while (!cancelled()) {
                    sndwork.UpdateExChannel();
                    sndwork.SeqMain(play: true);
                    sndwork.ExChannelMain(doUpdate: true);
                    LibDSSound.Software.Util.CalcRandom();

                    if (resampler == null) {
                        if (waveWriter.Length > bytesCount) {
                            break;
                        }
                        for (int i = 0; i < MainForm.woutSamplesPerIteration; i++) {
                            sndwork.Hardware.Evaluate(256, out var Left, out var Right);
                            buffer[i * 4] = (byte)((uint)Left & 0xFFu);
                            buffer[i * 4 + 1] = (byte)((uint)(Left >> 8) & 0xFFu);
                            buffer[i * 4 + 2] = (byte)((uint)Right & 0xFFu);
                            buffer[i * 4 + 3] = (byte)((uint)(Right >> 8) & 0xFFu);
                        }
                        waveWriter.Write(buffer, 0, buffer.Length);
                        continue;
                    }

                    for (int i = 0; i < MainForm.woutSamplesPerIteration && written < sampleCount; i++) {
                        sndwork.Hardware.Evaluate(256, out var Left, out var Right);
                        resampler.Push(Left / 32768f, Right / 32768f);
                        while (!resampler.NeedsInput && written < sampleCount) {
                            resampler.Pop(out float l, out float r);
                            short outL = Quantize(l, ref ditherState);
                            short outR = Quantize(r, ref ditherState);
                            buffer[filled] = (byte)outL;
                            buffer[filled + 1] = (byte)(outL >> 8);
                            buffer[filled + 2] = (byte)outR;
                            buffer[filled + 3] = (byte)(outR >> 8);
                            filled += 4;
                            written++;
                            if (filled == buffer.Length) {
                                waveWriter.Write(buffer, 0, filled);
                                filled = 0;
                            }
                        }
                    }
                    if (written >= sampleCount) {
                        break;
                    }
                }
                waveWriter.Write(buffer, 0, filled);
            } finally {
                waveWriter.Dispose();
            }
        }

        // TPDF dither: 2 uniform values in [0, 1) give a triangular distribution over
        // (-1, 1) LSB. 0s are left alone - true silence
        private static short Quantize(float sample, ref uint state) {
            if (sample == 0f) {
                return 0;
            }
            double v = Math.Floor(sample * 32768.0 + NextUniform(ref state) + NextUniform(ref state) - 1.0 + 0.5);
            if (v > 32767.0) {
                v = 32767.0;
            } else if (v < -32768.0) {
                v = -32768.0;
            }
            return (short)v;
        }

        // xorshift32
        private static double NextUniform(ref uint state) {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return state / 4294967296.0;
        }
    }
}
