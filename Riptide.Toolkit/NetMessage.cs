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
using System;
using System.Runtime.CompilerServices;

namespace Riptide.Toolkit
{
    /// <summary>
    /// [Mandatory] Work-around for <see cref="Message"/>s to introduce custom message headers.
    /// </summary>
    /// <remarks>
    /// If <see cref="Riptide"/> will ever allow extending headers - this class and <see cref="NetHeaders"/> will be removed.
    /// </remarks>
    /// Partial, to allow expanding it if needed.
    /// TODO: In Docs, be VERY explicit that using <see cref="Message.Create(MessageSendMode)"/> is PROHIBITED.
    public static partial class NetMessage
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                      Constructors without mod support.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Creates regular message with <see cref="MessageSendMode.Reliable"/> mode.
        /// Doesn't encode MessageID (which makes it an invalid message until <see cref="Message.Add(ushort)"/> or similar is used)
        /// </summary>
        /// <seealso cref="Message.Create()"/>
        public static Message Create() => Message.Create();

        /// <summary>
        /// Creates message with default headers.
        /// </summary>
        /// <param name="header">Riptide's header to apply.</param>
        /// <param name="ID">Toolkit's message ID to apply.</param>
        /// <returns>Message with fully encoded header.</returns>
        public static Message Create(Transports.MessageHeader header, SystemMessageID ID = SystemMessageID.Regular)
        {
            return Message.Create((MessageSendMode)header).AddSystemMessageID(ID).SkipHeaders();
        }

        /// <summary>
        /// Creates regular message with custom send <paramref name="mode"/>.
        /// Doesn't encode MessageID (which makes it an invalid message until you add it manually).
        /// </summary>
        /// <seealso cref="Message.Create(MessageSendMode)"/>
        public static Message Create(MessageSendMode mode, SystemMessageID ID = SystemMessageID.Regular)
        {
            return Message.Create(mode).AddSystemMessageID(ID).SkipHeaders();
        }

        /// <inheritdoc cref="Create(MessageSendMode, uint, SystemMessageID)"/>
        /// <seealso cref="Message.Create(MessageSendMode, Enum)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Create(MessageSendMode mode, Enum messageID, SystemMessageID ID = SystemMessageID.Regular)
        {
            return Create(mode, (uint)(object)messageID, ID);
        }

        /// <summary>
        /// Creates regular message with custom send <paramref name="mode"/>.
        /// Encodes <paramref name="messageID"/>.
        /// </summary>
        public static Message Create(MessageSendMode mode, uint messageID, SystemMessageID ID = SystemMessageID.Regular)
        {
            return Message.Create(mode)
                .AddSystemMessageID(ID)
                .ReserveHeaders(ID)
                .AddID(messageID); // TODO: Create the same method, but for uint.
        }

        /// <summary>
        /// Creates regular message with custom send <paramref name="mode"/>.
        /// Encodes <paramref name="messageID"/>.
        /// </summary>
        public static Message Create<TMessage>(MessageSendMode mode, SystemMessageID ID = SystemMessageID.Regular)
            where TMessage : NetworkMessage<TMessage>, new()
        {
            return Message.Create(mode)
                .AddSystemMessageID(ID)
                .ReserveHeaders(ID)
                .AddID(NetworkMessage<TMessage>.MessageID); // TODO: Create the same method, but for uint.
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Reliable(Enum messageID, SystemMessageID ID = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Reliable, (uint)(object)messageID, ID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Reliable(uint messageID, SystemMessageID ID = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Reliable, messageID, ID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Reliable<TMessage>(SystemMessageID ID = SystemMessageID.Regular) where TMessage : NetworkMessage<TMessage>, new()
        {
            return Create<TMessage>(MessageSendMode.Reliable, ID);
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Unreliable(Enum messageID, SystemMessageID ID = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Unreliable, (uint)(object)messageID, ID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Unreliable(uint messageID, SystemMessageID ID = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Unreliable, messageID, ID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Unreliable<TMessage>(SystemMessageID ID = SystemMessageID.Regular) where TMessage : NetworkMessage<TMessage>, new()
        {
            return Create<TMessage>(MessageSendMode.Unreliable, ID);
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Notify(Enum messageID, SystemMessageID ID = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Notify, (uint)(object)messageID, ID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Notify(uint messageID, SystemMessageID ID = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Notify, messageID, ID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Notify<TMessage, TProfile>(SystemMessageID ID = SystemMessageID.Regular) where TMessage : NetworkMessage<TMessage>, new()
        {
            return Create<TMessage>(MessageSendMode.Notify, ID);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Helpers
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Adds <see cref="NetworkMessage{TMessage}.MessageID"/> to a given <paramref name="message"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message AddID<T>(this Message message) where T : NetworkMessage<T>, new()
        {
            return message.AddVarULong(NetworkMessage<T>.MessageID);
        }

        /// <summary>
        /// Adds MessageID to a given <paramref name="message"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message AddID(this Message message, uint messageID)
        {
            return message.AddVarULong(messageID);
        }

        /// <summary>
        /// Destructively (using <see cref="Message.GetBits(int, out byte)"/>) reads <see cref="SystemMessageID"/>.
        /// </summary>
        /// <param name="message"><see cref="Message"/> to read.</param>
        /// <param name="ID"><see cref="SystemMessageID"/> retrieved from <paramref name="message"/>.</param>
        public static Message GetSystemMessageID(this Message message, out SystemMessageID ID)
        {
            message.GetBits(SystemMessaging.SystemMessageIDBits, out byte bits);
            ID = (SystemMessageID)bits;
            return message;
        }

        /// <summary>
        /// Non-destructively (using <see cref="Message.PeekBits(int, int, out byte)"/>) peeks <see cref="SystemMessageID"/>.
        /// </summary>
        /// <param name="message"><see cref="Message"/> to read.</param>
        /// <param name="ID"><see cref="SystemMessageID"/> peaked in <paramref name="message"/>.</param>
        public static Message PeekSystemMessageID(this Message message, out SystemMessageID ID)
        {
            message.PeekBits(SystemMessaging.SystemMessageIDBits, 0, out byte bits);
            ID = (SystemMessageID)bits;
            return message;
        }

        /// <summary>
        /// Adds given <see cref="SystemMessageID"/> (using <see cref="Message.AddBits(byte, int)"/>) to a <paramref name="message"/>
        /// </summary>
        /// <param name="message"><see cref="Message"/> to be modified.</param>
        /// <param name="ID"><see cref="SystemMessageID"/> to set in a <paramref name="message"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message AddSystemMessageID(this Message message, SystemMessageID ID = SystemMessageID.Regular)
        {
            return message.AddBits((byte)ID, SystemMessaging.SystemMessageIDBits);
        }

        /// <summary>
        /// Sets <see cref="SystemMessageID"/> (using <see cref="Message.SetBits(ulong, int, int)"/>) in a <paramref name="message"/>.
        /// </summary>
        /// <param name="message"><see cref="Message"/> to be modified.</param>
        /// <param name="ID"><see cref="SystemMessageID"/> to set in a <paramref name="message"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message SetSystemMessageID(this Message message, SystemMessageID ID = SystemMessageID.Regular)
        {
            return message.SetBits((byte)ID, SystemMessaging.SystemMessageIDBits, 0);
        }

        /// <summary>
        /// Moves head of a message reader to a position/index, after BOTH internal and custom headers.
        /// (<see cref="SystemMessageID"/> and all <see cref="CustomHeader{T}"/> definitions will be skipped)
        /// </summary>
        /// <seealso cref="CustomHeader{T}"/>
        public static Message SkipHeaders(this Message message, SystemMessageID ID = SystemMessageID.Regular)
        {
            int skip = NetHeaders.GetCustomHeaderLength(ID) + SystemMessaging.SystemMessageIDBits;
            int read = message.ReadBits;
            return message.SkipBits(skip - read);
        }

        /// <summary>
        /// Moves head of a message writer to a position/index, after BOTH internal and custom headers.
        /// (reserves space for <see cref="SystemMessageID"/> and all <see cref="CustomHeader{T}"/> definitions)
        /// </summary>
        public static Message ReserveHeaders(this Message message, SystemMessageID ID = SystemMessageID.Regular)
        {
            int skip = NetHeaders.GetCustomHeaderLength(ID) + SystemMessaging.SystemMessageIDBits;
            int write = message.WrittenBits;
            return message.ReserveBits(skip - write);
        }
    }
}
