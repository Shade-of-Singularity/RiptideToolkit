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

namespace Riptide.Toolkit
{
    public readonly struct AdvancedMessageToHandle
    {
        /// <summary>MessageID, registered separately.</summary>
        public readonly uint MessageID;
        /// <summary>The message that needs to be handled.</summary>
        public readonly Message Message;
        /// <summary>The message's header type.</summary>
        public readonly MessageHeader Header;
        /// <summary>The connection on which the message was received.</summary>
        public readonly Connection FromConnection;

        /// <summary>Handles initialization.</summary>
        /// <param name="message">The message that needs to be handled.</param>
        /// <param name="header">The message's header type.</param>
        /// <param name="fromConnection">The connection on which the message was received.</param>
        public AdvancedMessageToHandle(uint messageID, Message message, MessageHeader header, Connection fromConnection)
        {
            MessageID = messageID;
            Message = message;
            Header = header;
            FromConnection = fromConnection;
        }
    }
}
