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

using Riptide.Toolkit.Extensions;

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Base class for collections, which store client-side/server-side message handlers.
    /// </summary>
    /// <remarks>
    /// No reason to implement this one - Toolkit won't use any custom implementation (as of right now).
    /// Implemented in <see cref="RegionHandlerCollection{THandler}"/> and {TODO: insert DictionaryHandlerCollection}.
    /// </remarks>
    public abstract class MessageHandlerCollection<THandler> : IMessageHandlerCollection<THandler> where THandler : IStructValidator
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public abstract THandler Get(ushort messageID);

        /// <inheritdoc/>
        public abstract THandler Get(ushort modID, ushort messageID);

        /// <inheritdoc/>
        public abstract bool Has(ushort messageID);

        /// <inheritdoc/>
        public abstract bool Has(ushort modID, ushort messageID);

        /// <inheritdoc/>
        public abstract bool TryGet(ushort messageID, out THandler hander);

        /// <inheritdoc/>
        public abstract bool TryGet(ushort modID, ushort messageID, out THandler hander);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Clears internal handler array.
        /// </summary>
        /// <remarks>
        /// Clears even system messages.
        /// </remarks>
        protected abstract void Clear();

        /// <summary>
        /// Clears internal handler array and resizes buffers to default size.
        /// GC will be able to collect released resources, if there is any.
        /// </summary>
        /// <remarks>
        /// Clears even system messages.
        /// </remarks>
        protected abstract void Reset();

        /// <summary>
        /// Registers message handler on target <paramref name="messageID"/>.
        /// </summary>
        /// <param name="messageID">Handler MessageID to associate with <paramref name="handler"/>.</param>
        /// <param name="handler">Message handler to register.</param>
        protected abstract void Set(ushort modID, ushort messageID, THandler handler);

        /// <summary>
        /// Puts <paramref name="handler"/> on the next free MessageID.
        /// </summary>
        /// <param name="handler">Message handler to store.</param>
        /// <returns>MessageID under which handler was registered.</returns>
        protected abstract ushort Put(ushort modID, THandler handler);

        /// <summary>
        /// Removes message handler under given <paramref name="messageID"/>.
        /// </summary>
        /// <param name="messageID">Handler MessageID to check and remove.</param>
        protected abstract void Remove(ushort modID, ushort messageID);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Internal
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        internal static class Unsafe
        {
            /// <inheritdoc cref="MessageHandlerCollection{THandler}.Clear()"/>
            public static void Clear(MessageHandlerCollection<THandler> handlers)
            {
                handlers.Clear();
            }

            /// <inheritdoc cref="MessageHandlerCollection{THandler}.Reset()"/>
            public static void Reset(MessageHandlerCollection<THandler> handlers)
            {
                handlers.Reset();
            }

            /// <inheritdoc cref="MessageHandlerCollection{THandler}.Set(ushort, ushort, THandler)"/>
            public static void Set(MessageHandlerCollection<THandler> handlers, ushort messageID, THandler handler)
            {
                handlers.Set(0, messageID, handler);
            }

            /// <inheritdoc cref="MessageHandlerCollection{THandler}.Set(ushort, ushort, THandler)"/>
            public static void Set(MessageHandlerCollection<THandler> handlers, ushort modID, ushort messageID, THandler handler)
            {
                handlers.Set(modID, messageID, handler);
            }

            /// <inheritdoc cref="MessageHandlerCollection{THandler}.Put(ushort, THandler)"/>
            public static ushort Put(MessageHandlerCollection<THandler> handlers, THandler handler)
            {
                return handlers.Put(0, handler);
            }

            /// <inheritdoc cref="MessageHandlerCollection{THandler}.Put(ushort, THandler)"/>
            public static ushort Put(MessageHandlerCollection<THandler> handlers, ushort modID, THandler handler)
            {
                return handlers.Put(modID, handler);
            }

            /// <inheritdoc cref="MessageHandlerCollection{THandler}.Remove(ushort, ushort)"/>
            public static void Remove(MessageHandlerCollection<THandler> handlers, ushort messageID)
            {
                handlers.Remove(0, messageID);
            }

            /// <inheritdoc cref="MessageHandlerCollection{THandler}.Remove(ushort, ushort)"/>
            public static void Remove(MessageHandlerCollection<THandler> handlers, ushort modID, ushort messageID)
            {
                handlers.Remove(modID, messageID);
            }
        }
    }
}
