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
using Riptide.Toolkit.Storage;

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Collection of all client-side message handlers for specific handler GroupID.
    /// </summary>
    public readonly struct ServerMessageHandlers
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
        public ServerMessageHandlers(IReadOnlyGroupMessageIndexer indexer) => Group = indexer;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Retrieves data, needed for clients to easily fire server-side message handlers.
        /// </summary>
        /// <param name="groupID">GroupID to use for a client.</param>
        /// <returns>Struct, which has plenty of useful methods and shortcuts.</returns>
        public static ServerMessageHandlers Create(byte groupID) => new ServerMessageHandlers(NetworkIndex.GetGroup(groupID));




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Attempts to fire message handler under given <paramref name="messageID"/>.
        /// </summary>
        /// <remarks>
        /// Doesn't release <paramref name="message"/> automatically.
        /// </remarks>
        /// <param name="server">(WIP) Will be used to send responses for messages which support it.</param>
        /// <param name="messageID">ID of a message handler.</param>
        /// <param name="message">Message to read.</param>
        /// <returns><c>false</c> if there is no handler under given <paramref name="messageID"/> registered. <c>true</c> otherwise.</returns>
        public bool TryFire(AdvancedServer server, uint messageID, ushort clientID, Message message)
        {
            if (Group.HasServer(messageID))
            {
                Fire(server, messageID, clientID, message);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fires handler to a client under given <paramref name="clientID"/> with specified <paramref name="messageID"/> server-side.
        /// </summary>
        /// <remarks>
        /// Throws if handler wasn't found.
        /// Doesn't release <paramref name="message"/> automatically.
        /// </remarks>
        /// <param name="server">(WIP) Will be used to send responses for messages which support it.</param>
        /// <param name="messageID">ID of a message handler.</param>
        /// <param name="clientID">Riptide ID of a client which sent the message.</param>
        /// <param name="message">Message to read.</param>
        public void Fire(AdvancedServer server, uint messageID, ushort clientID, Message message)
        {
            MessageHandlerInfo info = NetworkIndex.RawServerHandlers.Get(messageID);
            if (info.MessageType is null) throw new System.Exception("This");
            object[] args = new object[2];
            args[0] = clientID;

            object result;
            if (info.MessageType == typeof(Message))
            {
                args[1] = message;
                result = info.Method.Invoke(null, args);

                // Message is released by Riptide itself. No need for this code.
                //if (info.Release) message.Release();
            }
            else
            {
                NetworkMessage container = (NetworkMessage)MessagePools.Retrieve(info.MessageType);
                container.Read(message);
                args[1] = container;
                result = info.Method.Invoke(null, args);

                // Automatically releases if requested.
                if (info.Release) container.Release();
            }

            switch (result)
            {
                // If you return regular message - it will immediately send its contents as a response.
                // TODO: Make sure that FlagMessages will also be supported.
                //case Message msg: server.SendResponse(msg, clientID); break;

                // Automatically packs and sends response message.
                // TODO: Make sure that MessageID is read correctly.
                //case NetworkMessage net: server.SendResponse(net.Write(message), clientID); break;
                default: break; // WIP: While responses is not supported - response handlers are commented-out.
            }
        }
    }
}
