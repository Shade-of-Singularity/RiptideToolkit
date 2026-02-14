using System;

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
/// 
namespace Riptide.Toolkit
{
    /// <summary>
    /// Stores some constant values for internal messaging logic.
    /// </summary>
    /// <seealso cref="NetHeaders"/>
    public static class SystemMessaging
    {
        /// <summary>
        /// Bits occupied by all headers (<see cref="Transports.MessageHeader"/> and <see cref="SystemMessageID"/>).
        /// </summary>
        public const int HeaderBits = MessageHeaderBits + SystemMessageIDBits;
        /// <summary>
        /// Bit-mask covering all the bits, used to encode <see cref="Transports.MessageHeader"/> and <see cref="SystemMessageID"/>.
        /// </summary>
        public const int HeaderMask = (1 << HeaderBits) - 1;

        /// <summary>
        /// Size of a message header with <see cref="MessageSendMode.Unreliable"/> mode.
        /// </summary>
        /// <remarks>
        /// IMPORTANT! Regular Riptide messages are 4 bits smaller.
        /// Toolkit adds 4 bits to both: Encode <see cref="SystemMessageID"/>, and align data to a size of a byte,
        /// to allow raw package reading with network relays.
        /// </remarks>
        public const int UnreliableHeaderBits = HeaderBits;
        /// <summary>
        /// Size of a message header with <see cref="MessageSendMode.Reliable"/> mode.
        /// </summary>
        /// <remarks>
        /// IMPORTANT! Regular Riptide messages are 4 bits smaller.
        /// Toolkit adds 4 bits to both: Encode <see cref="SystemMessageID"/>, and align data to a size of a byte,
        /// to allow raw package reading with network relays.
        /// </remarks>
        public const int ReliableHeaderBits = HeaderBits + 16;
        /// <summary>
        /// Size of a message header with <see cref="MessageSendMode.Notify"/> mode.
        /// </summary>
        /// <remarks>
        /// IMPORTANT! Regular Riptide messages are 4 bits smaller.
        /// Toolkit adds 4 bits to both: Encode <see cref="SystemMessageID"/>, and align data to a size of a byte,
        /// to allow raw package reading with network relays.
        /// </remarks>
        public const int NotifyHeaderBits = HeaderBits + 40;

        /// <summary>
        /// Bits occupied by Riptide's <see cref="Transports.MessageHeader"/>.
        /// </summary>
        public const int MessageHeaderBits = 4;
        /// <summary>
        /// Bit-mask covering all the bits, used to encode <see cref="Transports.MessageHeader"/>,
        /// and by extension - <see cref="MessageSendMode"/>, in <see cref="Message"/>s (and <see cref="NetMessage"/>s).
        /// </summary>
        public const int MessageHeaderMask = (1 << MessageHeaderBits) - 1;

        /// <summary>
        /// Bits occupied by Toolkit's <see cref="SystemMessageID"/>.
        /// </summary>
        public const int SystemMessageIDBits = 4;
        /// <summary>
        /// Bit-mask covering all the bits, used to encode <see cref="SystemMessageID"/>.
        /// </summary>
        /// <remarks>
        /// "Root" mask assumes that <see cref="SystemMessageID"/> is located on 0th bit, which is usually not the case.
        /// Usually it is located after <see cref="MessageHeaderBits"/>: [SystemMessageID][MessageHeader].
        /// Use <see cref="SystemMessageIDRelativeMask"/> to decode the bits you need,
        /// and then offset them by <see cref="SystemMessageIDOffset"/> to the right to get <see cref="SystemMessageID"/>.
        /// </remarks>
        public const int SystemMessageIDRootMask = (1 << SystemMessageIDBits) - 1;
        /// <summary>
        /// Offset of <see cref="SystemMessageIDRelativeMask"/> from 0th bit, in a fully encoded header.
        /// </summary>
        public const int SystemMessageIDOffset = MessageHeaderBits;
        /// <summary>
        /// Bit-mask covering specific bits in a full message header, dedicated to a <see cref="SystemMessageID"/> specifically.
        /// </summary>
        /// <remarks>
        /// After applying mask to the header, move it by <see cref="SystemMessageIDOffset"/>
        /// to the right to obtain <see cref="SystemMessageID"/>.
        /// </remarks>
        public const int SystemMessageIDRelativeMask = SystemMessageIDRootMask << SystemMessageIDOffset;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  ID Masks
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Mask which covers <see cref="SystemMessageID.Regular"/> and <see cref="SystemMessageID.Private"/> modes.
        /// </summary>
        public const byte RouteModeMask = 0b1;
        /// <summary>
        /// Mask which covers <see cref="SystemMessageID.Request"/> and <see cref="SystemMessageID.Response"/> identifiers.
        /// </summary>
        [Obsolete("Request-Response system is WIP.", error: true)]
        public const byte RequestResponseMask = 0b10;
    }
}
