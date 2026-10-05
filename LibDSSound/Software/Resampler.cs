using System;

namespace LibDSSound.Software
{
	// Windowed-sinc sample rate converter for offline export.
	// polyphase, 4-term Blackman-Harris window, linear interpolation
	// between neighbouring phases
	//
	// Output frame n is the input interpolated at exactly n * srcRate / dstRate, so the
	// output lines up with the input and there is no delay to trim.
	public class Resampler
	{
		public const int Phases = 128;

		public Resampler(double srcRate, double dstRate, int taps = 0)
		{
			double ratio = srcRate / dstRate;
			stepWhole = (int)ratio;
			stepFrac = ratio - stepWhole;

			// The VST uses 64 taps at every host rate. Export allows rates down to a few
			// hundred Hz, so when going down the filter grows with the ratio instead.
			if (taps <= 0) {
				taps = Math.Max(64, (int)Math.Ceiling(64.0 * ratio / 8.0) * 8);
			}

			this.taps = taps;
			half = taps / 2;

			// Cut just below the lower of the two Nyquist limits (with a small guard band so the transition region is out of the audible range)
			double cutoff = 0.5;
			if (dstRate < srcRate)
			{
				cutoff = 0.5 * (dstRate / srcRate);
			}
			cutoff *= 0.92;

			// Phases + 1 rows: the last one is the first moved on by a whole input sample.
			// phase and phase + 1 can always be read together for interpolation.
			kernel = new float[(Phases + 1) * taps];
			double[] row = new double[taps];
			for (int phase = 0; phase <= Phases; phase++)
			{
				double frac = (double)phase / Phases;
				double sum = 0.0;
				for (int k = 0; k < taps; k++)
				{
					// Offset of this tap from the fractional read position.
					double t = k - (half - 1) - frac;

					// 4-term Blackman-Harris. -92 dB side lobes.
					double wpos = (t + half) / taps;
					double w = 0.0;
					if (wpos >= 0.0 && wpos <= 1.0)
					{
						double a = 2.0 * Math.PI * wpos;
						w = 0.35875 - 0.48829 * Math.Cos(a) + 0.14128 * Math.Cos(2.0 * a) - 0.01168 * Math.Cos(3.0 * a);
					}

					row[k] = 2.0 * cutoff * Sinc(2.0 * cutoff * t) * w;
					sum += row[k];
				}

				double norm = sum != 0.0 ? 1.0 / sum : 1.0;
				for (int k = 0; k < taps; k++)
				{
					kernel[phase * taps + k] = (float)(row[k] * norm);
				}
			}

			// write each sample twice, ringSize apart, so the taps always read one
			// contiguous run, no matter the wrap point
			ringSize = 1;
			while (ringSize < taps * 2)
			{
				ringSize <<= 1;
			}
			ringMask = ringSize - 1;
			ringL = new float[ringSize * 2];
			ringR = new float[ringSize * 2];
		}

		// True when the next output frame cannot be produced yet
		public bool NeedsInput => writeCount < basePos + half + 1;

		public void Push(float left, float right)
		{
			int slot = (int)(writeCount & ringMask);
			ringL[slot] = left;
			ringL[slot + ringSize] = left;
			ringR[slot] = right;
			ringR[slot + ringSize] = right;
			writeCount++;
		}

		public void Pop(out float left, out float right)
		{
			// Blend the two stored phases either side of the read position. Picking the
			// nearest one alone quantizes the read time to one 128th of an input sample
			double scaled = posFrac * Phases;
			int phase = (int)scaled;
			if (phase < 0) {
				phase = 0;
			} else if (phase >= Phases) {
				phase = Phases - 1;
			}
			float t = (float)(scaled - phase);

			int h0 = phase * taps;
			int h1 = h0 + taps;
			int start = (int)((basePos - (half - 1)) & ringMask);

			float accL0 = 0f, accR0 = 0f, accL1 = 0f, accR1 = 0f;
			for (int k = 0; k < taps; k++)
			{
				float inL = ringL[start + k];
				float inR = ringR[start + k];
				accL0 += inL * kernel[h0 + k];
				accR0 += inR * kernel[h0 + k];
				accL1 += inL * kernel[h1 + k];
				accR1 += inR * kernel[h1 + k];
			}

			left = accL0 + (accL1 - accL0) * t;
			right = accR0 + (accR1 - accR0) * t;

			basePos += stepWhole;
			posFrac += stepFrac;
			if (posFrac >= 1.0) {
				posFrac -= 1.0;
				basePos++;
			}
		}

		private static double Sinc(double x) {
			if (x > -1e-9 && x < 1e-9) {
				return 1.0;
			}
			double px = Math.PI * x;
			return Math.Sin(px) / px;
		}

		private readonly int stepWhole;
		private readonly double stepFrac;
		private readonly int taps;
		private readonly int half;
		private readonly float[] kernel;

		private readonly int ringSize;
		private readonly int ringMask;
		private readonly float[] ringL;
		private readonly float[] ringR;

		private long writeCount;
		private long basePos;
		private double posFrac;
	}
}
