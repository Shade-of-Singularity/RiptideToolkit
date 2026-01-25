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

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Collection of all client-side message handlers for specific handler GroupID.
    /// </summary>
    public readonly struct ClientHandlers
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public readonly IReadOnlyGroupMessageIndexer Group;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public ClientHandlers(IReadOnlyGroupMessageIndexer indexer) => Group = indexer;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Attempts to fire message handler under given <paramref name="messageID"/>.
        /// </summary>
        /// <param name="messageID">ID of a message handler.</param>
        /// <param name="message">Message to read.</param>
        /// <returns><c>false</c> if there is no handler under given <paramref name="messageID"/> registered. <c>true</c> otherwise.</returns>
        public bool TryFire(AdvancedClient client, uint messageID, Message message)
        {
            if (Group.Has(messageID))
            {
                Fire(client, messageID, message);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fires handler with specified ID client-side.
        /// </summary>
        /// <remarks>
        /// Throws if handler wasn't found.
        /// </remarks>
        /// <param name="messageID">ID of a message handler.</param>
        /// <param name="message">Message to read.</param>
        public void Fire(AdvancedClient client, uint messageID, Message message)
        {
            MessageHandlerInfo info = NetworkIndex.Handlers.Get(messageID);
            object[] args = new object[1];

            if (info.MessageType == typeof(Message))
            {
                args[0] = message;
            }
            else
            {
                NetworkMessage container = (NetworkMessage)Activator.CreateInstance(info.MessageType);
                container.Read(message);
                args[0] = container;
            }

            // Note: untested.
            switch (info.Method.Invoke(null, args))
            {
                // If you return regular message - it will immediately send its contents as a response.
                // TODO: Make sure that FlagMessages will also be supported.
                //case Message msg: client.SendResponse(msg); break;

                // Automatically packs and sends response message.
                // TODO: Make sure that MessageID is read correctly.
                //case NetworkMessage net: client.SendResponse(net.Write(message)); break;
                default: break; // WIP: While responses is not supported - response handlers are commented-out.
            }
        }
    }
}
