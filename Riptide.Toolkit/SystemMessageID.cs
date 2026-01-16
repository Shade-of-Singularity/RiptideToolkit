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
    /// All message IDs used with <see cref="Riptide"/> networking to determine if modified game versions are compatible or not.
    /// </summary>
    internal enum SystemMessageID : byte
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
        /// Message ID used in on-demand responses.
        /// </summary>
        Response = 1,

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
