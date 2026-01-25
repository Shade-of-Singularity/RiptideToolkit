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

using System;
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
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const int NotifyHeaderBits = 44;
        public const int ReliableHeaderBits = 20;
        public const int UnreliableHeaderBits = 4;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="GetMessageBase(MessageSendMode)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMessageBase(Message message) => GetMessageBase(message.SendMode);

        /// <summary>
        /// Retrieves position in a message data where message content starts.
        /// </summary>
        /// <remarks>
        /// Needed because <see cref="Toolkit"/> uses some of the first bits to encode system message IDs and such.
        /// </remarks>
        public static int GetMessageBase(MessageSendMode mode)
        {
            switch (mode)
            {
                case MessageSendMode.Notify: return NotifyHeaderBits + SystemMessaging.TotalBits;
                case MessageSendMode.Unreliable: return UnreliableHeaderBits + SystemMessaging.TotalBits;
                case MessageSendMode.Reliable: return ReliableHeaderBits + SystemMessaging.TotalBits;
                default: throw new NotSupportedException($"Cannot retrieve header base for {nameof(MessageSendMode)} of ({mode})!");
            }
        }

        /// <inheritdoc cref="GetHeaderBase(MessageSendMode)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHeaderBase(Message message) => GetHeaderBase(message.SendMode);
        public static int GetHeaderBase(MessageSendMode mode)
        {
            switch (mode)
            {
                case MessageSendMode.Notify: return NotifyHeaderBits;
                case MessageSendMode.Unreliable: return UnreliableHeaderBits;
                case MessageSendMode.Reliable: return ReliableHeaderBits;
                default: throw new NotSupportedException($"Cannot retrieve header base for {nameof(MessageSendMode)} of ({mode})!");
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
            public override ushort Length => SystemMessaging.TotalBits;
            public override Message Read(Message message) => throw new NotImplementedException();
            public override Message Write(Message message) => throw new NotImplementedException();
        }
    }
}
