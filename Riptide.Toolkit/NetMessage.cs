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
        /// .                                                 Overwrites
        /// .                                 Allows encoding different data in messages.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="SetMessageID(Message, ushort)"/>
        public static Message SetMessageID(this Message message, Enum messageID) => message.SetMessageID((ushort)(object)messageID);

        /// <summary>
        /// **Overwrites** MessageID to <paramref name="messageID"/> on a message with already defined MessageID.
        /// </summary>
        /// <remarks>
        /// <para>Supports only messages defined using <see cref="Create()"/> - regular messages will get corrupted.</para>
        /// <para>USage on clean messages will corrupt them, or throw and exception!</para>
        /// </remarks>
        /// <param name="message">Message to modify directly.</param>
        /// <param name="messageID">MessageID to set instead of an original value.</param>
        public static Message SetMessageID(this Message message, ushort messageID)
        {
            return message.SetBits(messageID, 16, NetHeaders.GetMessageBase(message));
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                      Constructors without mod support.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Creates regular message with <see cref="MessageSendMode.Reliable"/> mode.
        /// Doesn't encode MessageID (which makes it an invalid message until <see cref="Message.Add(ushort)"/> or similar is used)
        /// Doesn't encode ModID in the message (In which case defaults to ModID '0')
        /// </summary>
        /// <seealso cref="Message.Create()"/>
        public static Message Create() => Message.Create();

        /// <summary>
        /// Creates regular message with custom send <paramref name="mode"/>.
        /// Doesn't encode MessageID (which makes it an invalid message until <see cref="Message.Add(ushort)"/> or similar is used)
        /// Doesn't encode ModID in the message (In which case defaults to ModID '0')
        /// </summary>
        /// <seealso cref="Message.Create(MessageSendMode)"/>
        public static Message Create(MessageSendMode mode) => Message.Create(mode);

        /// <inheritdoc cref="Create(MessageSendMode, ushort)"/>
        /// <seealso cref="Message.Create(MessageSendMode, Enum)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Create(MessageSendMode mode, Enum messageID) => Create(mode, (ushort)(object)messageID);

        /// <summary>
        /// Creates regular message with custom send <paramref name="mode"/>.
        /// Encodes <paramref name="messageID"/> in a spot where ModID would have been (hence messages created without ModID cannot have one).
        /// Doesn't encode ModID in the message (In which case defaults to ModID '0')
        /// </summary>
        public static Message Create(MessageSendMode mode, ushort messageID)
        {
            return Message.Create(mode)
                .AddVarULong(messageID)
                .AddBool(false) // Indicates that message does not contain ModID.
                .AddBits((byte)SystemMessageID.Regular, SystemMessaging.TotalBits);
        }


        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                     Constructors with supporting mods.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="Create(MessageSendMode, ushort)"/>
        /// <seealso cref="Message.Create(MessageSendMode, Enum)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Message Create(ushort modID, MessageSendMode mode, Enum messageID) => Create(modID, mode, (ushort)(object)messageID);

        /// <summary>
        /// Creates regular message with custom send <paramref name="mode"/>.
        /// Encodes <paramref name="messageID"/> after all headers.
        /// Doesn't encode ModID in the message (In which case defaults to ModID '0')
        /// </summary>
        public static Message Create(ushort modID, MessageSendMode mode, ushort messageID)
        {
            return Message.Create(mode)
                .AddVarULong(messageID)
                .AddBool(true) // Indicates that message contains ModID.
                .AddBits((byte)SystemMessageID.Regular, SystemMessaging.TotalBits)
                .AddBits(modID, Modding.ModIDTotalBits);
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
        /// Using twice will make you skip valid data as well!
        /// </remarks>
        public static Message SkipHeaders(this Message message)
        {
            // Skips ModID only if defined.
            return message.PeekBits(NetHeaders.IsModdedTotalBits, NetHeaders.GetHeaderBase(message) + NetHeaders.IsModdedLocation, out byte result)
                .SkipBits(NetHeaders.IsModdedTotalBits + SystemMessaging.TotalBits + (result == 0 ? 0 : Modding.ModIDTotalBits));
        }

        /// <summary>
        /// Adds <see cref="NetMessage"/> headers with **doesn't** support ModIDs.
        /// Should be used on empty <see cref="Message"/>s only, as it doesn't overwrite the header. (TODO: Add SetHeaders methods with insert)
        /// </summary>
        /// <remarks>
        /// Use <see cref="AddHeaders(Message, SystemMessageID, ushort)"/> to define mod ModID.
        /// </remarks>
        /// <param name="systemMessageID"><see cref="SystemMessageID"/> to add.</param>
        public static Message AddHeaders(this Message message, SystemMessageID systemMessageID)
        {
            return message
                .AddBool(false) // Indicates that message doesn't contain ModID.
                .AddBits((byte)systemMessageID, SystemMessaging.TotalBits);
        }

        /// <summary>
        /// Adds <see cref="NetMessage"/> headers with support ModIDs.
        /// Should be used on empty <see cref="Message"/>s only, as it doesn't overwrite the header. (TODO: Add SetHeaders methods with insert)
        /// </summary>
        /// <param name="systemMessageID"><see cref="SystemMessageID"/> to add.</param>
        public static Message AddHeaders(this Message message, SystemMessageID systemMessageID, ushort modID)
        {
            return message
                .AddBool(true) // Indicates that message contains ModID.
                .AddBits((byte)systemMessageID, SystemMessaging.TotalBits)
                .AddBits(modID, Modding.ModIDTotalBits);
        }

        /// <summary>
        /// Adds <see cref="NetMessage"/> headers with support or doesn't support ModIDs based on <paramref name="isModded"/> flag.
        /// Should be used on empty <see cref="Message"/>s only, as it doesn't overwrite the header. (TODO: Add SetHeaders methods with insert)
        /// </summary>
        /// <param name="systemMessageID"><see cref="SystemMessageID"/> to add.</param>
        public static Message AddHeaders(this Message message, bool isModded, SystemMessageID systemMessageID, ushort modID)
        {
            return isModded
                ? message.AddHeaders(systemMessageID, modID)
                : message.AddHeaders(systemMessageID);
        }

        /// <summary>
        /// Reads (and consumes) data about <paramref name="isModded"/> flag, <paramref name="systemMessageID"/> and <paramref name="modID"/>.
        /// Should only be ran once. Used internally by <see cref="AdvancedClient"/> and <see cref="AdvancedServer"/>.
        /// You **should not** run it yourself, unless you make custom <see cref="Client"/> and <see cref="Server"/> implementation.
        /// </summary>
        /// <param name="message">Message to read.</param>
        /// <param name="isModded">Whether <paramref name="modID"/> is defined. Otherwise <paramref name="modID"/> defaults to (0).</param>
        /// <param name="systemMessageID"></param>
        /// <param name="modID"></param>
        /// <returns></returns>
        public static Message ReadHeaders(this Message message, out bool isModded, out SystemMessageID systemMessageID, out ushort modID)
        {
            isModded = message.GetBool();
            message.GetBits(SystemMessaging.TotalBits, out byte result);
            systemMessageID = (SystemMessageID)result;
            if (isModded) message.GetBits(Modding.ModIDTotalBits, out modID);
            else modID = 0;
            return message;
        }
    }
}
