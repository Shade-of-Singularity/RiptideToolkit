/// - - Shade of Singularity Community - - - Tom Weiland & Riptide Community, 2026 - - <![CDATA[
/// 
/// Licensed under the MIT License. Permission is hereby granted, free of charge,
/// to any person obtaining a copy of this software and associated documentation
/// files to deal in the Software without restriction. Full license terms are
/// available in the LICENSE.md file located at the following repository path:
///   
///                        "RiptideToolkit/LICENSE.md"
/// 
/// ]]>

using Riptide.Toolkit.Messages;

namespace Riptide.Toolkit.Examples
{
    // Set enum values:
    // Note: Values [0-7] are occupied by system messages.
    // Note #2: Should probably imbed system messages in better way... Maybe write down a byte value? Gives way more room to us.
    public enum ToClientMessages : ushort
    {
        UpdateUsername,
        UpdatePlayerPosition,

        ReceiveChunkData,
    }

    public enum ToServerMessages : ushort
    { 
        RegisterUsername,
        ReceivePlayerPosition,
    }

    // Automatic network messages will take any unoccupied ID.
    public sealed class VFXSignal : FlagMessage<VFXSignal, ExampleGroup> { }
    public sealed class CustomNetworkMessage : NetworkMessage<CustomNetworkMessage, ExampleGroup>
    {
        public const int ChunkSize = 8;
        public const int ChunkHeight = 64;
        public const int ChunkArea = ChunkSize * ChunkSize;
        public const int ChunkVolume = ChunkArea * ChunkHeight;

        public int x, y;
        public uint[] blocks;

        public override Message Read(Message message)
        {
            x = message.GetInt();
            y = message.GetInt();
            blocks = message.GetUInts(ChunkVolume);
            return message;
        }

        public override Message Write(Message message)
        {
            message.AddInt(x);
            message.AddInt(y);
            message.AddUInts(blocks);
            return message;
        }
    }

    public sealed class ValidateChunk : NetworkMessage<ValidateChunk, ExampleGroup>
    {
        public int x, y;
        public ulong hash;

        public override Message Read(Message message)
        {
            x = message.GetInt();
            y = message.GetInt();
            hash = message.GetULong();
            return message;
        }

        public override Message Write(Message message)
        {
            message.AddInt(x);
            message.AddInt(y);
            message.AddULong(hash);
            return message;
        }
    }

    public sealed class ReceiveInventory : NetworkMessage<ReceiveInventory, ExampleGroup>
    {
        public uint[] ids;
        public uint[] amounts;

        public override Message Read(Message message)
        {
            ids = message.GetUInts();
            amounts = message.GetUInts();
            return message;
        }

        public override Message Write(Message message)
        {
            message.AddUInts(ids);
            message.AddUInts(amounts);
            return message;
        }
    }
}
