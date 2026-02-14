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

using Riptide.Transports;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Riptide.Toolkit
{
    /// <summary>
    /// Stores methods for adding and processing custom headers to messages.
    /// </summary>
    /// <remarks>(WIP) You can define custom headers here.</remarks>
    /// Partial to allow people to extend it and store values in one place.
    public static partial class NetHeaders
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Properly combines Riptide's <see cref="MessageHeader"/> and Toolkit's <see cref="SystemMessageID"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Combine(MessageHeader header, SystemMessageID id)
        {
            return (byte)((uint)header | ((uint)id << SystemMessaging.SystemMessageIDOffset));
        }

        /// <inheritdoc cref="GetHeaderSize(MessageSendMode)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHeaderSize(Message message) => GetHeaderSize(message.SendMode);

        /// <summary>
        /// Retrieves bit position in a message data where message content starts.
        /// </summary>
        /// <remarks>
        /// Needed because <see cref="Toolkit"/> uses some of the first bits to encode system message IDs and such.
        /// Thus assumes that you are using messages created with <see cref="NetMessage"/>.
        /// </remarks>
        /// <returns>Size of a header in bits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHeaderSize(MessageSendMode mode)
        {
            switch (mode)
            {
                case MessageSendMode.Notify: return SystemMessaging.NotifyHeaderBits;
                case MessageSendMode.Unreliable: return SystemMessaging.UnreliableHeaderBits;
                case MessageSendMode.Reliable: return SystemMessaging.ReliableHeaderBits;
                default: throw new NotSupportedException($"Cannot retrieve header base for {nameof(MessageSendMode)} of ({mode})!");
            }
        }

        /// <summary>
        /// Retrieves bit position in a message data where message content starts.
        /// </summary>
        /// <remarks>
        /// Needed because <see cref="Toolkit"/> uses some of the first bits to encode system message IDs and such.
        /// Thus assumes that you are using messages created with <see cref="NetMessage"/>.
        /// </remarks>
        /// <returns>Size of a header in bits.</returns>
        public static int GetHeaderSize(MessageHeader header)
        {
            if (header == MessageHeader.Notify)
            {
                return SystemMessaging.NotifyHeaderBits;
            }
            else if (header > MessageHeader.Notify)
            {
                return SystemMessaging.ReliableHeaderBits;
            }
            else
            {
                return SystemMessaging.UnreliableHeaderBits;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Custom Headers
        /// .                              (W.I.P.) - hard to implement conditional headers.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [Obsolete("WIP", error: true)]
        internal static byte GetHeaderID() => 0;

        [Obsolete("WIP", error: true)]
        internal abstract class Header<T>
        {
            public static readonly byte ID = GetHeaderID();
            public static readonly ulong Pin = 1uL << ID;
            public static bool IsDefined(Message message, ulong pin) { message.PeekBits(64, 0, out ulong value); return (pin & value) == pin; }
            public abstract ushort Length { get; }
            public abstract Message Write(Message message);
            public abstract Message Read(Message message);
        }

        [Obsolete("WIP", error: true)]
        internal sealed class SystemIDHeader : Header<SystemIDHeader>
        {
            public override ushort Length => SystemMessaging.HeaderBits;
            public override Message Read(Message message) => throw new NotImplementedException();
            public override Message Write(Message message) => throw new NotImplementedException();
        }
    }
}
