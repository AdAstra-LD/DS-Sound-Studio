using LibDSSound.IO;

namespace LibDSSound.Software
{
	// What the ARM9 asks of the driver when a game starts an archive sequence, in its order, before the
	// sequence's first tick: prepare it, give every track the player's allowed channels when the player has a
	// mask, set the sequence's channel priority, start it.
	public static class SequenceStart
	{
		public static void Start(SNDWork work, int playerNr, byte[] data, int offset, SBNK bank, byte channelPriority,
			ushort playerChannelMask)
		{
			work.PrepareSeq(playerNr, data, offset, bank);
			if (playerChannelMask != 0)
			{
				work.SetTrackAllocatableChannel(playerNr, 0xFFFF, playerChannelMask);
			}
			work.SetPlayerChannelPriority(playerNr, channelPriority);
			work.StartPreparedSeq(playerNr);
		}
	}
}
