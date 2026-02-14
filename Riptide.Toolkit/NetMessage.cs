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
        /// <param name="id">Toolkit's message ID to apply.</param>
        /// <returns>Message with fully encoded header.</returns>
        public static Message Create(Transports.MessageHeader header, SystemMessageID id = SystemMessageID.Regular)
        {
            return Message.Create((MessageSendMode)header).AddBits((byte)id, SystemMessaging.SystemMessageIDBits);
        }

        /// <summary>
        /// Creates regular message with custom send <paramref name="mode"/>.
        /// Doesn't encode MessageID (which makes it an invalid message until <see cref="Message.Add(ushort)"/> or similar is used)
        /// </summary>
        /// <seealso cref="Message.Create(MessageSendMode)"/>
        public static Message Create(MessageSendMode mode, SystemMessageID id = SystemMessageID.Regular)
        {
            return Message.Create(mode, id);
        }

        /// <inheritdoc cref="Create(MessageSendMode, uint, SystemMessageID)"/>
        /// <seealso cref="Message.Create(MessageSendMode, Enum)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Create(MessageSendMode mode, Enum messageID, SystemMessageID id = SystemMessageID.Regular)
        {
            return Create(mode, (uint)(object)messageID, id);
        }

        /// <summary>
        /// Creates regular message with custom send <paramref name="mode"/>.
        /// Encodes <paramref name="messageID"/>.
        /// </summary>
        public static Message Create(MessageSendMode mode, uint messageID, SystemMessageID id = SystemMessageID.Regular)
        {
            return Message.Create(mode)
                .AddBits((byte)id, SystemMessaging.SystemMessageIDBits)
                .AddVarULong(messageID); // TODO: Create the same method, but for uint.;
        }

        /// <summary>
        /// Creates regular message with custom send <paramref name="mode"/>.
        /// Encodes <paramref name="messageID"/>.
        /// </summary>
        public static Message Create<TMessage>(MessageSendMode mode, SystemMessageID id = SystemMessageID.Regular)
            where TMessage : NetworkMessage<TMessage>, new()
        {
            return Message.Create(mode)
                .AddBits((byte)id, SystemMessaging.SystemMessageIDBits)
                .AddVarULong(NetworkMessage<TMessage>.MessageID); // TODO: Create the same method, but for uint.;
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Reliable(Enum messageID, SystemMessageID id = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Reliable, (uint)(object)messageID, id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Reliable(uint messageID, SystemMessageID id = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Reliable, messageID, id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Reliable<TMessage>(SystemMessageID id = SystemMessageID.Regular) where TMessage : NetworkMessage<TMessage>, new()
        {
            return Create<TMessage>(MessageSendMode.Reliable, id);
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Unreliable(Enum messageID, SystemMessageID id = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Unreliable, (uint)(object)messageID, id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Unreliable(uint messageID, SystemMessageID id = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Unreliable, messageID, id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Unreliable<TMessage>(SystemMessageID id = SystemMessageID.Regular) where TMessage : NetworkMessage<TMessage>, new()
        {
            return Create<TMessage>(MessageSendMode.Unreliable, id);
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Notify(Enum messageID, SystemMessageID id = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Unreliable, (uint)(object)messageID, id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Notify(uint messageID, SystemMessageID id = SystemMessageID.Regular)
        {
            return Create(MessageSendMode.Unreliable, messageID, id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Notify<TMessage, TProfile>(SystemMessageID id = SystemMessageID.Regular) where TMessage : NetworkMessage<TMessage>, new()
        {
            return Create<TMessage>(MessageSendMode.Notify, id);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Helpers
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Moves reader head over all custom message headers. Doesn't skip MessageID.
        /// </summary>
        /// <remarks>
        /// Using twice will make you skip valid data!
        /// </remarks>
        public static Message SkipCustomHeaders(this Message message) => message.SkipBits(SystemMessaging.SystemMessageIDBits);

        /// <summary>
        /// Adds <see cref="NetMessage"/> default headers.
        /// Should be used on empty <see cref="Message"/>s only, as it doesn't overwrite the header. (TODO: Add SetHeaders methods with insert)
        /// </summary>
        /// <param name="systemMessageID"><see cref="SystemMessageID"/> to add.</param>
        public static Message AddCustomHeaders(this Message message, SystemMessageID systemMessageID)
        {
            return message.AddBits((byte)systemMessageID, SystemMessaging.SystemMessageIDBits);
        }

        /// <summary>
        /// Reads (and consumes) data about <paramref name="isModded"/> flag, <paramref name="systemMessageID"/> and <paramref name="modID"/>.
        /// Should only be ran once. Used internally by <see cref="AdvancedClient"/> and <see cref="AdvancedServer"/>.
        /// You **should not** run it yourself, unless you make custom <see cref="Client"/> and <see cref="Server"/> implementation.
        /// </summary>
        /// <remarks>
        /// Headers ALWAYS go before regular MessageID, because they are used for 3rdParty server relaying.
        /// </remarks>
        /// <param name="message">Message to read.</param>
        /// <param name="systemMessageID">Internal message type of the message.</param>
        /// <returns></returns>
        public static Message ReadHeaders(this Message message, out SystemMessageID systemMessageID)
        {
            message.GetBits(SystemMessaging.SystemMessageIDBits, out byte result);
            systemMessageID = (SystemMessageID)result;
            return message;
        }
    }
}
