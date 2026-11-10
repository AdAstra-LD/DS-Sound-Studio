using System;
using System.Collections.Generic;
using System.IO;
using LibDSSound.IO;

namespace LibDSSound.Tests
{
	// Console checks for LibDSSound. Each command returns 0 on success.
	//   baseline SDAT HASHFILE [--check] [--pcm DIR] [--seconds N] [--seqs A,B,...]
	//       Renders ten sequences (spread over the SDAT, or the listed ones) and writes their PCM hashes, or with
	//       --check compares them with HASHFILE and lists the ones that differ.
	//   survey SDAT [--seconds N]
	//       Every sequence: its INFO parameters, how long it sounds and the most channels it holds at once.
	internal static class Program
	{
		private static int Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.Error.WriteLine("usage: LibDSSound.Tests <command> ...; see Program.cs");
				return 2;
			}
			try
			{
				switch (args[0])
				{
				case "baseline":
					return Baseline(args);
				case "survey":
					return Survey(args);
				default:
					Console.Error.WriteLine("unknown command " + args[0]);
					return 2;
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
				return 1;
			}
		}

		private static string Option(string[] args, string name, string fallback)
		{
			int i = Array.IndexOf(args, name);
			return i >= 0 && i + 1 < args.Length ? args[i + 1] : fallback;
		}

		// Ten sequences spread evenly over the archive's non-empty entries, or the ones --seqs lists.
		private static List<int> PickSequences(SDAT sdat, int count, string list)
		{
			if (list != null)
			{
				return new List<int>(Array.ConvertAll(list.Split(','), int.Parse));
			}
			List<int> valid = new List<int>();
			for (int i = 0; i < sdat.InfoBlock.SequenceInfos.Entries.Count; i++)
			{
				if (sdat.InfoBlock.SequenceInfos[i] != null)
				{
					valid.Add(i);
				}
			}
			List<int> picked = new List<int>();
			for (int i = 0; i < count && i < valid.Count; i++)
			{
				picked.Add(valid[(int)((long)i * valid.Count / count)]);
			}
			return picked;
		}

		private static int Baseline(string[] args)
		{
			SDAT sdat = new SDAT(File.ReadAllBytes(args[1]));
			string hashFile = args[2];
			bool check = Array.IndexOf(args, "--check") >= 0;
			string pcmDir = Option(args, "--pcm", null);
			int frames = Render.FramesFor(double.Parse(Option(args, "--seconds", "60")));
			if (pcmDir != null)
			{
				Directory.CreateDirectory(pcmDir);
			}

			Dictionary<int, string> expected = new Dictionary<int, string>();
			if (check)
			{
				foreach (string line in File.ReadAllLines(hashFile))
				{
					string[] f = line.Split('\t');
					expected[int.Parse(f[0])] = f[2];
				}
			}

			List<string> lines = new List<string>();
			int differ = 0;
			foreach (int seq in PickSequences(sdat, 10, Option(args, "--seqs", null)))
			{
				string name = sdat.SymbolBlock?.SequenceSymbols?.Entries[seq] ?? ("SEQ_" + seq);
				Render.ResetRandom();
				string hash = Render.RunFrames(Render.StartSdatSeq(sdat, seq), frames,
					pcmDir != null ? Path.Combine(pcmDir, seq + ".pcm") : null);
				string line = seq + "\t" + name + "\t" + hash;
				lines.Add(line);
				if (check)
				{
					bool same = expected.TryGetValue(seq, out string old) && old == hash;
					if (!same)
					{
						differ++;
					}
					Console.WriteLine((same ? "same    " : "DIFFERS ") + line);
				}
				else
				{
					Console.WriteLine(line);
				}
			}
			if (!check)
			{
				File.WriteAllLines(hashFile, lines);
				return 0;
			}
			Console.WriteLine(differ == 0 ? "baseline: all equal" : ("baseline: " + differ + " differ"));
			return differ == 0 ? 0 : 1;
		}

		// Every sequence for a few seconds: how long it sounds and how many channels it holds at once, to choose
		// baseline sequences that exercise the allocator.
		private static int Survey(string[] args)
		{
			SDAT sdat = new SDAT(File.ReadAllBytes(args[1]));
			int frames = Render.FramesFor(double.Parse(Option(args, "--seconds", "20")));
			byte[] buffer = new byte[Render.SamplesPerFrame * 4];
			for (int seq = 0; seq < sdat.InfoBlock.SequenceInfos.Entries.Count; seq++)
			{
				SDAT.INFO.SequenceInfo info = sdat.InfoBlock.SequenceInfos[seq];
				if (info == null)
				{
					continue;
				}
				Render.ResetRandom();
				SNDWork work = Render.StartSdatSeq(sdat, seq);
				int lastSounding = -1;
				int maxChannels = 0;
				for (int f = 0; f < frames; f++)
				{
					Render.Frame(work, buffer);
					int active = 0;
					foreach (LibDSSound.Hardware.Channel ch in work.Hardware.Channels)
					{
						if (ch.Enabled)
						{
							active++;
						}
					}
					if (active > 0)
					{
						lastSounding = f;
					}
					maxChannels = Math.Max(maxChannels, active);
				}
				string name = sdat.SymbolBlock?.SequenceSymbols?.Entries[seq] ?? ("SEQ_" + seq);
				Console.WriteLine("{0}\t{1}\tplayer={2}\tcprio={3}\tpprio={4}\tvol={5}\tsounding={6:F1}s\tmaxch={7}",
					seq, name, info.PlayerNr, info.ChannelPriority, info.PlayerPriority, info.Volume,
					(lastSounding + 1) * (double)Render.SamplesPerFrame / Render.SampleRate, maxChannels);
			}
			return 0;
		}
	}
}
