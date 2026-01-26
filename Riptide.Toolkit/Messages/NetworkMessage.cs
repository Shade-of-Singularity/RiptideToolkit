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

namespace Riptide.Toolkit.Messages
{
    /// <inheritdoc cref="NetworkMessage{TMessage, TProfile}"/>
    /// <remarks>
    /// <para>Implements <see cref="S0"/> as <see cref="StorageProfile{TProfile}"/> by default.</para>
    /// </remarks>
    /// TODO: Add message pooling based on load.
    public abstract class NetworkMessage<TMessage> : NetworkMessage<TMessage, S0>
        where TMessage : NetworkMessage<TMessage, S0>, new()
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// Since this instance has <see cref="StorageProfile"/> set to <see cref="S0"/> - it guarantees that instance will always be collected by GC.
        /// Because of that, this method is empty. You can still override it, but outside of internal callback you will gain nothing from it.
        /// So better to inherit <see cref="NetworkMessage{TMessage, TProfile}"/> and change <see cref="S0"/> to at least <see cref="S1"/> storage.
        /// </remarks>
        protected override void Dispose() { }
    }

    /// <summary>
    /// Base class for custom messages.
    /// </summary>
    /// <typeparam name="TMessage">Class that inherited this <see cref="NetworkMessage{TMessage, TProfile}"/></typeparam>
    /// <typeparam name="TProfile"><see cref="StorageProfile{TProfile}"/> of this network message. Will pool some of the message instances based on it.</typeparam>
    public abstract class NetworkMessage<TMessage, TProfile> : NetworkMessage
        where TMessage : NetworkMessage<TMessage, TProfile>, new()
        where TProfile : StorageProfile<TProfile>, new()
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Message ID of this <see cref="NetworkMessage{TMessage, TGroup, TLoad}"/>.
        /// </summary>
        [MessageID] public static uint MessageID { get; private set; } = NetworkIndex.InvalidMessageID;

        /// <inheritdoc cref="MessageID"/>
        public override uint ID => MessageID;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly TMessage[] m_Pool = new TMessage[StorageProfile<TProfile>.Instance.Storage];
        private static int m_PoolHead = -1; // Negative values indicate that there is no items in the pool. Values always change by 1.




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Retrieves message from pool. If pool is empty - simply creates new instance.
        /// </summary>
        /// <returns>Empty <typeparamref name="TMessage"/> instance to be used.</returns>
        public static TMessage Get()
        {
            // Note: can this part be multithreaded in some way?
            // I doubt, but if there will be a way to do it, no matter the LoadProfile option - it **might** be work implementing.
            lock (m_Pool)
            {
                if (m_PoolHead < 0)
                {
                    return new TMessage();
                }
                else
                {
                    TMessage result = m_Pool[m_PoolHead];
                    m_Pool[m_PoolHead--] = default;
                    return result;
                }
            }
        }

        /// <summary>
        /// Releases given <paramref name="message"/> by running <see cref="Dispose()"/> method on it and storing it in a pool, if available.
        /// </summary>
        /// <param name="message">Message data container to dispose and store.</param>
        public static void Release(NetworkMessage<TMessage, TProfile> message)
        {
            // Always disposes.
            message.Dispose();
            lock (m_Pool)
            {
                // Checks if there is enough space in pool to store more messages.
                int index = m_PoolHead + 1;
                if (index >= m_Pool.Length)
                {
                    // If index item will occupy is outside of the array bounds - it returns.
                    return;
                }

                // If index is within array bounds - it will store message there and move head there. 
                m_Pool[index] = (TMessage)message;
                m_PoolHead = index;
            }
        }

        /// <summary>
        /// Creates new message with ID of this <see cref="NetworkMessage{TMessage, TProfile}"/>.
        /// </summary>
        /// <param name="mode">Message sending mode. Needed for message factory method - <see cref="Message.Create(MessageSendMode, ushort)"/></param>
        /// <returns>Newly created message.</returns>
        public static Message Message(MessageSendMode mode)
        {
            return NetMessage.Create(mode, MessageID);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Releases itself by running <see cref="Dispose()"/> method and storing itself in a pool, if available.
        /// </summary>
        public override void Release() => Release(this);

        /// <summary>
        /// Unpacks given message by overwriting values of this <see cref="NetworkMessage{TMessage, TProfile}"/> instance.
        /// </summary>
        /// <param name="message">Message to unpack.</param>
        /// <returns>Itself, for convenience.</returns>
        public NetworkMessage<TMessage, TProfile> Unpack(Message message)
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
        /// Disposes all arrays and strings under instance control, making it possible for <see cref="System.GC"/> to collect those arrays and strings.
        /// </summary>
        protected abstract void Dispose();
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
