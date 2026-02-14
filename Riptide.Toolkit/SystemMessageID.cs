
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
namespace Riptide.Toolkit
{
    /// <summary>
    /// Similar to <see cref="Transports.MessageHeader"/>. Indicates how message should be treated.
    /// In a bit-mask, added after <see cref="Transports.MessageHeader"/>: [SystemMessageID][MessageHeader].
    /// Use <see cref="SystemMessaging.SystemMessageIDOffset"/> to realign, <see cref="SystemMessageID"/> again.
    /// </summary>
    /// <seealso cref="NetHeaders.Combine(Transports.MessageHeader, SystemMessageID)"/>.
    public enum SystemMessageID : byte
    {
        /// <summary>
        /// Indicates that this is a normal Riptide message with default behaviour.
        /// </summary>
        /// <remarks>
        /// Network Relays should treat this message as "Route to All" when sent by server.
        /// And as "Route to Server" when sent by client.
        /// </remarks>
        Regular = 0b0,

        /// <summary>
        /// Indicates that this is a message, meant to be sent to a specific client.
        /// Does nothing in P2P.
        /// </summary>
        /// <remarks>
        /// Network Relays should treat this message as "Route to Client (...)" and attempt to retrieve ClientID.
        /// Note: way in which ClientID is encoded and retrieves is specific to each relay system.
        /// If such message is sent from client - send it to the server. Unless private messaging is allowed,
        /// in which case attempt to retrieve ClientID.
        /// </remarks>
        Private = 0b1,

        /// <summary>
        /// Message ID used in on-demand requests.
        /// </summary>
        /// <remarks>Request-Response system is WIP.</remarks>
        /// <seealso cref="Response"/>
        [Obsolete("Request-Response system is WIP.", error: true)]
        Request = 0b00,

        /// <summary>
        /// Message ID used in on-demand responses to requests.
        /// </summary>
        /// <remarks>Request-Response system is WIP.</remarks>
        /// <seealso cref="Request"/>
        [Obsolete("Request-Response system is WIP.", error: true)]
        Response = 0b10,
    }
}
