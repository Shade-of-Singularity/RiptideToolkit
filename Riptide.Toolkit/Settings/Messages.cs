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

namespace Riptide.Toolkit.Settings
{
    /// <summary>
    /// Stores <see cref="Toolkit"/> settings related to messages.
    /// </summary>
    public static class Messages
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Timeout in milliseconds for request <see cref="Message"/>s.
        /// (TODO) On such timeouts, <see cref="Server"/> or <see cref="Client"/> callback will fire.
        /// By default, any timed-out reliable messages will close connection with server (<seealso cref="CloseConnectionWithRequestTimeout"/>).
        /// </summary>
        /// <remarks>
        /// (TODO) Timeouts are updated using <see cref="Server.Update"/> or <see cref="Client.Update"/>.
        /// </remarks>
        public static uint RequestTimeoutMs { get; set; } = 300_000;

        /// <summary>
        /// Whether connection with remote host should be closed when any <see cref="MessageSendMode.Reliable"/> request gets timed out.
        /// </summary>
        /// TODO: What to do with long-breeding requests though? What about multiple requests sent to one awaiting target?
        public static bool CloseConnectionWithRequestTimeout { get; set; } = true;
    }
}
