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

using Riptide.Toolkit.Settings;
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

        /// <summary>Comes right after <see cref="SystemMessageIDLocation"/> bit.</summary>
        /// <remarks>Occupies exactly one bit.</remarks>
        public const int IsModdedLocation = 0;
        public const int IsModdedTotalBits = 1;

        /// <summary>Always '0' - written first. (Written right after a header)</summary>
        /// <remarks>Length is defined in <see cref="SystemMessaging.TotalBits"/>.</remarks>
        public const int SystemMessageIDLocation = IsModdedLocation + IsModdedTotalBits;

        /// <summary>Where ModID is stored (occupies 16 bits (TODO: make dynamic))</summary>
        /// <remarks>Length is defined in <see cref="Modding.ModIDTotalBits"/></remarks>
        /// TODO: Define ModID header size dynamically.
        public const int ModIDLocation = SystemMessageIDLocation + SystemMessaging.TotalBits;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
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

        public static int GetMessageBase(Message message)
        {
            message.PeekBits(IsModdedTotalBits, IsModdedLocation, out byte result);
            return GetHeaderBase(message.SendMode) + result == 0
                ? (IsModdedTotalBits + SystemMessaging.TotalBits + Modding.ModIDTotalBits)
                : (IsModdedTotalBits + SystemMessaging.TotalBits);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Custom Headers
        /// .                              (W.I.P.) - hard to implement conditional headers.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        internal static byte GetHeaderID() => 0;
        internal abstract class Header<T>
        {
            public static readonly byte ID = GetHeaderID();
            public static readonly ulong Pin = 1uL << ID;
            public static bool IsDefined(Message message, ulong pin) { message.PeekBits(64, 0, out ulong value); return (pin & value) == pin; }
            public abstract ushort Length { get; }
            public abstract Message Write(Message message);
            public abstract Message Read(Message message);
        }

        internal sealed class IsModdedHeader : Header<IsModdedHeader>
        {
            public override ushort Length => 1;
            public override Message Read(Message message) => throw new System.NotImplementedException();
            public override Message Write(Message message) => throw new System.NotImplementedException();
        }

        internal sealed class SystemIDHeader : Header<IsModdedHeader>
        {
            public override ushort Length => SystemMessaging.TotalBits;
            public override Message Read(Message message) => throw new System.NotImplementedException();
            public override Message Write(Message message) => throw new System.NotImplementedException();
        }

        internal sealed class ModIDHeader : Header<IsModdedHeader>
        {
            public override ushort Length => Modding.ModIDTotalBits;
            public override Message Read(Message message) => throw new System.NotImplementedException();
            public override Message Write(Message message) => throw new System.NotImplementedException();
        }
    }
}
