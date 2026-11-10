using LibDSSound.IO;

namespace LibDSSound.Software
{
	// What the ARM9 asks of the driver when a game starts an archive sequence, in its order, before the
	// sequence's first tick: prepare it, give every track the player's allowed channels when the player has a
	// mask, set the sequence's channel priority, send the volume, start it.
	public static class SequenceStart
	{
		public static void Start(SNDWork work, int playerNr, byte[] data, int offset, SBNK bank, byte volume,
			byte channelPriority, ushort playerChannelMask)
		{
			work.PrepareSeq(playerNr, data, offset, bank);
			if (playerChannelMask != 0)
			{
				work.SetTrackAllocatableChannel(playerNr, 0xFFFF, playerChannelMask);
			}
			work.SetPlayerChannelPriority(playerNr, channelPriority);
			work.SetPlayerVolume(playerNr, InitialFader(volume));
			work.StartPreparedSeq(playerNr);
		}

		// The fader the player sends each frame: the decibels of the initial volume, the handle's volume, the
		// player's volume and the volume fader. Only the first is not 127 at the start.
		public static short InitialFader(byte volume)
		{
			int fader = Util.CalcDecibel(volume) + Util.CalcDecibel(127) + Util.CalcDecibel(127) + Util.CalcDecibel(127);
			if (fader < short.MinValue)
			{
				fader = short.MinValue;
			}
			return (short)fader;
		}
	}
}
