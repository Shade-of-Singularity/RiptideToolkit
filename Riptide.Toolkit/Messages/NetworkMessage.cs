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

using Riptide.Toolkit.Storage;
using System;
using System.Reflection;

namespace Riptide.Toolkit.Messages
{
    /// <summary>
    /// Base class for custom messages.
    /// </summary>
    /// <typeparam name="TMessage">Class that inherited this <see cref="NetworkMessage{TMessage, TProfile}"/></typeparam>
    [S0] // Doesn't request any pool capacity (TODO: Optimize it), because regular network messages need disposal but they implement none.
    public abstract class NetworkMessage<TMessage> : NetworkMessage
        where TMessage : NetworkMessage<TMessage>, new()
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Message ID of this <see cref="NetworkMessage"/>.
        /// </summary>
        [MessageID] public static uint MessageID { get; set; } = NetworkIndex.InvalidMessageID;
        /// <summary>
        /// Message ID of this <see cref="NetworkMessage"/>.
        /// </summary>
        public override uint ID => MessageID;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static NetworkMessage()
        {
            NetworkIndex.Initialize(); // Initializes MessageID.
            Type type = typeof(TMessage);
            do
            {
                // Searches for storage definition.
                // Note: if code will cause stuttering during first minutes of networking (unlikely)
                // - run static .ctor on all network messages in all assemblies relying on RiptideToolkit.
                // It will remove stuttering and allocate all pools prematurely, but will increase game loading time.
                // (Note: on modern systems, it won't take even 0.001s to execute this code).
                if (type.IsDefined(typeof(StorageAttribute), inherit: false))
                {
                    StorageAttribute attribute = type.GetCustomAttribute<StorageAttribute>(inherit: false);
                    MessagePool<TMessage>.Instance.EnsureCapacity(attribute.Storage);
                    return;
                }

                type = type.BaseType;
            }
            while (!(type is null));
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Retrieves message from pool. If pool is empty - simply creates new instance.
        /// </summary>
        /// <returns>Empty <typeparamref name="TMessage"/> instance to be used.</returns>
        public static TMessage Get() => MessagePool<TMessage>.Instance.Retrieve();

        /// <summary>
        /// Creates new message with ID of this <see cref="NetworkMessage{TMessage, TProfile}"/>.
        /// </summary>
        /// <param name="mode">Message sending mode. Needed for message factory method - <see cref="Message.Create(MessageSendMode, ushort)"/></param>
        /// <returns>Newly created message.</returns>
        public static Message Message(MessageSendMode mode) => NetMessage.Create(mode, MessageID);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Releases itself by running <see cref="Dispose()"/> method and storing itself in a pool,
        /// if allowed by storage <typeparamref name="TProfile"/>.
        /// </summary>
        public override void Release()
        {
            Dispose(); // Always disposes.
            MessagePool<TMessage>.Instance.Store((TMessage)this);
        }

        /// <summary>
        /// Unpacks given message by overwriting values of this <see cref="NetworkMessage{TMessage, TProfile}"/> instance.
        /// </summary>
        /// <param name="message">Message to unpack.</param>
        /// <returns>Itself, for convenience.</returns>
        public NetworkMessage<TMessage> Unpack(Message message)
        {
            Read(message);
            return this;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Disposes all arrays and strings under instance control, making it possible for <see cref="GC"/> to collect those arrays and strings.
        /// </summary>
        protected virtual void Dispose() { }
    }

    /// <summary>
    /// Non-generic base type for <see cref="NetworkMessage{TMessage, TProfile}"/>. Exist to make reflections easier.
    /// </summary>
    public abstract class NetworkMessage
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// ID of this message, retrieved during <see cref="NetworkIndex"/> initialization.
        /// </summary>
        public abstract uint ID { get; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Reads data from provided <paramref name="message"/> and stores it inside itself to be reused later.
        /// </summary>
        public abstract Message Read(Message message);

        /// <summary>
        /// Writes data about this message to provided <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public abstract Message Write(Message message);

        /// <summary>
        /// Returns message to the pool.
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// Packs <see cref="NetworkMessage{TMessage, TProfile}"/> into a message, including its <see cref="MessageID"/> in the data.
        /// </summary>
        /// <param name="mode"><see cref="Riptide"/> Send mode of the <see cref="Message"/>.</param>
        /// <returns>Fully prepared <see cref="Message"/>, ready to be sent to another party.</returns>
        public Message Pack(MessageSendMode mode) => Write(NetMessage.Create(mode, ID));

        /// <summary>
        /// Packs <see cref="NetworkMessage{TMessage, TProfile}"/> into a message, including its <see cref="MessageID"/> in the data,
        /// and immediately <see cref="Release()"/>s it afterwards.
        /// </summary>
        /// <param name="mode"><see cref="Riptide"/> Send mode of the <see cref="Message"/>.</param>
        /// <returns>Fully prepared <see cref="Message"/>, ready to be sent to another party.</returns>
        public Message PackRelease(MessageSendMode mode)
        {
            Message result = Pack(mode);
            Release();
            return result;
        }
    }
}
