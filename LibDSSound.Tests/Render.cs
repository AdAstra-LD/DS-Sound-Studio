using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using LibDSSound.IO;
using LibDSSound.Software;

namespace LibDSSound.Tests
{
	// Offline renders, frame for frame as the app's export does: UpdateExChannel, SeqMain, ExChannelMain,
	// CalcRandom, then 341 samples at 65456 Hz.
	internal static class Render
	{
		public const int SamplesPerFrame = 341;
		public const int SampleRate = 65456;

		// The engine's random generator is one static for the process; every render starts it from its seed.
		public static void ResetRandom()
		{
			typeof(Util).GetField("rand", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, 305419896U);
		}

		public static int FramesFor(double seconds)
		{
			return (int)Math.Ceiling(seconds * SampleRate / SamplesPerFrame);
		}

		public static SBNK LoadSdatBank(SDAT sdat, int bankNo)
		{
			SDAT.INFO.BankInfo bnkInfo = sdat.InfoBlock.BankInfos[bankNo];
			SBNK bank = new SBNK(sdat.GetFileData(bnkInfo.FileId));
			for (int i = 0; i < 4; i++)
			{
				if (bnkInfo.WaveArchives[i] != 65535)
				{
					bank.AssignWaveArc(i, new SWAR(sdat.GetFileData(sdat.InfoBlock.WaveArchiveInfos[bnkInfo.WaveArchives[i]].FileId)));
				}
			}
			return bank;
		}

		// Which of the game's start-up settings a render applies; All is what the sequence viewer does. The others
		// isolate one setting, to explain a baseline hash that changed.
		[Flags]
		public enum StartParts
		{
			None = 0,
			ChannelPriority = 1,
			ChannelMask = 2,
			// the volume as the player's fader; without it, the old viewer's write of the main volume register
			Volume = 4,
			All = ChannelPriority | ChannelMask | Volume
		}

		public static StartParts Parts = StartParts.All;

		// A sequence of an SDAT started as the sequence viewer starts it.
		public static SNDWork StartSdatSeq(SDAT sdat, int seqIdx)
		{
			SDAT.INFO.SequenceInfo info = sdat.InfoBlock.SequenceInfos[seqIdx];
			SSEQ seq = new SSEQ(sdat.GetFileData(info.FileId));
			SBNK bank = LoadSdatBank(sdat, info.Bank);
			SNDWork work = new SNDWork();
			work.ExChannelInit();
			work.SeqInit();
			bool volume = (Parts & StartParts.Volume) != 0;
			SequenceStart.Start(work, 0, seq.Data, 0, bank, volume ? info.Volume : (byte)127,
				(Parts & StartParts.ChannelPriority) != 0 ? info.ChannelPriority : (byte)64,
				(Parts & StartParts.ChannelMask) != 0 ? PlayerChannelMask(sdat, info.PlayerNr) : (ushort)0);
			if (!volume)
			{
				work.Players[0].Volume = info.Volume;
			}
			return work;
		}

		public static ushort PlayerChannelMask(SDAT sdat, int playerNr)
		{
			SDAT.INFO.PlayerInfo playerInfo = playerNr < sdat.InfoBlock.PlayerInfos.Entries.Count
				? sdat.InfoBlock.PlayerInfos[playerNr] : null;
			return playerInfo != null ? playerInfo.ChannelAllocationMask : (ushort)0;
		}

		// Runs a started engine for a number of frames, hashing the 16-bit stereo PCM and optionally writing it.
		public static string RunFrames(SNDWork work, int frames, string pcmPath)
		{
			byte[] buffer = new byte[SamplesPerFrame * 4];
			using (SHA256 sha = SHA256.Create())
			using (FileStream pcm = pcmPath != null ? File.Create(pcmPath) : null)
			{
				for (int f = 0; f < frames; f++)
				{
					Frame(work, buffer);
					sha.TransformBlock(buffer, 0, buffer.Length, null, 0);
					pcm?.Write(buffer, 0, buffer.Length);
				}
				sha.TransformFinalBlock(buffer, 0, 0);
				return BitConverter.ToString(sha.Hash).Replace("-", "").ToLowerInvariant();
			}
		}

		public static void Frame(SNDWork work, byte[] buffer)
		{
			work.UpdateExChannel();
			work.SeqMain(play: true);
			work.ExChannelMain(doUpdate: true);
			Util.CalcRandom();
			for (int i = 0; i < SamplesPerFrame; i++)
			{
				work.Hardware.Evaluate(256, out short left, out short right);
				buffer[i * 4] = (byte)left;
				buffer[i * 4 + 1] = (byte)(left >> 8);
				buffer[i * 4 + 2] = (byte)right;
				buffer[i * 4 + 3] = (byte)(right >> 8);
			}
		}
	}
}
