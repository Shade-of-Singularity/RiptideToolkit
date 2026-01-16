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
    public enum SystemMessageID : ushort
    {
        /// <summary>
        /// Range of all values system messages will take.
        /// This range will be reserved in handler lists and never employed by regular networking messages.
        /// </summary>
        Range = 0b111, // First 8 IDs are reserved.

        /// <summary>
        /// Amount of Message IDs reserved for system messages.
        /// </summary>
        Amount = Range + 1,

        /// <summary>
        /// Message ID used in on-demand responses.
        /// </summary>
        Response = 0,

        /// <summary>
        /// Message handler used to compare whether all message handlers have the same IDs or not.
        /// Used by <see cref="NetworkIndex"/>.
        /// </summary>
        /// <remarks>
        /// Amount of handlers usually different only if two clients, or client and server has different networking mods installed.
        /// Client-side mods never modify handler collection (only by a developer's mistake), so they can be used safely.
        /// </remarks>
        NetworkingValidationCheck = 1,
    }
}
