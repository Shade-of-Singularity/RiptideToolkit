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
        /// Creates regular message with custom send <paramref name="mode"/>.
        /// Doesn't encode MessageID (which makes it an invalid message until <see cref="Message.Add(ushort)"/> or similar is used)
        /// </summary>
        /// <seealso cref="Message.Create(MessageSendMode)"/>
        public static Message Create(MessageSendMode mode) => Message.Create(mode);

        /// <inheritdoc cref="Create(MessageSendMode, uint)"/>
        /// <seealso cref="Message.Create(MessageSendMode, Enum)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Create(MessageSendMode mode, Enum messageID) => Create(mode, (uint)(object)messageID);

        /// <summary>
        /// Creates regular message with custom send <paramref name="mode"/>.
        /// Encodes <paramref name="messageID"/>.
        /// </summary>
        public static Message Create(MessageSendMode mode, uint messageID)
        {
            return Message.Create(mode)
                .AddVarULong(messageID) // TODO: Create the same method, but for uint.
                .AddBits((byte)SystemMessageID.Regular, SystemMessaging.TotalBits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Reliable(Enum messageID) => Create(MessageSendMode.Reliable, (uint)(object)messageID);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Reliable(uint messageID) => Create(MessageSendMode.Reliable, messageID);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Unreliable(Enum messageID) => Create(MessageSendMode.Unreliable, (uint)(object)messageID);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Unreliable(uint messageID) => Create(MessageSendMode.Unreliable, messageID);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Notify(Enum messageID) => Create(MessageSendMode.Unreliable, (uint)(object)messageID);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Notify(uint messageID) => Create(MessageSendMode.Unreliable, messageID);




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
        public static Message SkipHeaders(this Message message) => message.SkipBits(SystemMessaging.TotalBits);

        /// <summary>
        /// Adds <see cref="NetMessage"/> default headers.
        /// Should be used on empty <see cref="Message"/>s only, as it doesn't overwrite the header. (TODO: Add SetHeaders methods with insert)
        /// </summary>
        /// <param name="systemMessageID"><see cref="SystemMessageID"/> to add.</param>
        public static Message AddHeaders(this Message message, SystemMessageID systemMessageID)
        {
            return message.AddBits((byte)systemMessageID, SystemMessaging.TotalBits);
        }

        /// <summary>
        /// Reads (and consumes) data about <paramref name="isModded"/> flag, <paramref name="systemMessageID"/> and <paramref name="modID"/>.
        /// Should only be ran once. Used internally by <see cref="AdvancedClient"/> and <see cref="AdvancedServer"/>.
        /// You **should not** run it yourself, unless you make custom <see cref="Client"/> and <see cref="Server"/> implementation.
        /// </summary>
        /// <param name="message">Message to read.</param>
        /// <param name="systemMessageID">Internal message type of the message.</param>
        /// <returns></returns>
        public static Message ReadHeaders(this Message message, out SystemMessageID systemMessageID)
        {
            message.GetBits(SystemMessaging.TotalBits, out byte result);
            systemMessageID = (SystemMessageID)result;
            return message;
        }
    }
}
