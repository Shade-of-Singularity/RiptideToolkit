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

namespace Riptide.Toolkit
{
    /// <summary>
    /// Stores some constant values for internal messaging logic.
    /// </summary>
    public static class SystemMessaging
    {
        /// <summary>
        /// (PoT) Amount of IDs allocated for system messaging.
        /// Might be increased in future updates.
        /// </summary>
        /// <remarks>Max ever value - 256 (<see cref="byte.MaxValue"/> + 1).</remarks>
        public const ushort TotalIDs = 8;
        /// <summary>
        /// Mask which covers all values described by <see cref="TotalIDs"/>
        /// </summary>
        /// <remarks>Max ever value - 0b1111_1111.</remarks>
        public const ushort IDMask = TotalIDs - 1;
        /// <summary>
        /// How many bits is used to encode all <see cref="TotalIDs"/>.
        /// </summary>
        /// <remarks>Max ever value - 8.</remarks>
        public const ushort TotalBits = 3;
    }

    /// <summary>
    /// All message IDs used with <see cref="Riptide"/> networking to determine if modified game versions are compatible or not.
    /// </summary>
    public enum SystemMessageID : byte
    {
        /// <summary>
        /// Regular message. Messages with this data will be passed down as regular messages.
        /// </summary>
        /// <remarks>
        /// Anything other than regular message will be read as system message.
        /// <see cref="Client.MessageReceived"/> or <see cref="Server.MessageReceived"/> won't fire for those messages.
        /// </remarks>
        Regular = 0,

        /// <summary>
        /// Message ID used in on-demand requests.
        /// </summary>
        /// <seealso cref="Response"/>
        Request = 1,

        /// <summary>
        /// Message ID used in on-demand responses to requests.
        /// </summary>
        /// <seealso cref="Request"/>
        Response = 2,

        /// <summary>
        /// Message handler used to compare whether all message handlers have the same IDs or not.
        /// Used by <see cref="NetworkIndex"/>.
        /// </summary>
        /// <remarks>
        /// Amount of handlers usually different only if two clients, or client and server has different networking mods installed.
        /// Client-side mods never modify handler collection (only by a developer's mistake), so they can be used safely.
        /// </remarks>
        NetworkingValidationCheck,
    }
}
