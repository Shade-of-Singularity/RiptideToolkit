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
    /// Collection of all network message handlers.
    /// </summary>
    /// <remarks>
    /// DO NOT inherit this class!
    /// Use <see cref="ServerHandlers"/> and <see cref="ClientHandlers"/> directly instead.
    /// This is because custom message handler collections are not supported at the moment.
    /// </remarks>
    /// Note: Supports only structs as <typeparam name="THandler".
    public abstract class MessageHandlers<THandler> where THandler : IStructValidation
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private THandler[] m_Handlers = new THandler[(int)SystemMessageID.Amount];
        private int m_HeadIndex = (int)SystemMessageID.Amount;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public THandler Get(ushort messageID)
        {
            NetworkIndex.Initialize();
            // We don't need checks here - it will throw anyway.
            // if (id > m_Handlers.Length) throw new ArgumentOutOfRangeException(nameof(id));
            return m_Handlers[messageID];
        }

        /// <summary>
        /// Checks if handler is defined.
        /// </summary>
        /// <param name="messageID">Handler MessageID to check.</param>
        /// <returns><c>true</c> if defined. <c>false</c> otherwise.</returns>
        public bool Has(ushort messageID)
        {
            NetworkIndex.Initialize();
            if (messageID > m_Handlers.Length) return false;
            return !m_Handlers[messageID].IsDefault;
        }

        public bool TryGet(ushort messageID, out THandler hander)
        {
            NetworkIndex.Initialize();
            if (messageID > m_Handlers.Length)
            {
                hander = default;
                return false;
            }

            hander = m_Handlers[messageID];
            return !hander.IsDefault;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Internal
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        internal static class Unsafe
        {
            /// <summary>
            /// Clears internal handler array.
            /// </summary>
            /// <remarks>
            /// Clears even system messages.
            /// </remarks>
            public static void Clear(MessageHandlers<THandler> handlers)
            {
                handlers.m_Handlers.Clear(ref handlers.m_HeadIndex);
            }

            /// <summary>
            /// Clears internal handler array and resizes it to a target size.
            /// </summary>
            /// <remarks>
            /// Clears even system messages.
            /// </remarks>
            /// <param name="size">New size of the array.</param>
            public static void Reset(MessageHandlers<THandler> handlers, ushort size)
            {
                DynamicArrays.Reset(ref handlers.m_Handlers, ref handlers.m_HeadIndex, size);
            }

            /// <summary>
            /// Registers message handler on target <paramref name="messageID"/>.
            /// </summary>
            /// <param name="messageID">Handler MessageID to associate with <paramref name="handler"/>.</param>
            /// <param name="handler">Message handler to register.</param>
            public static void Set(MessageHandlers<THandler> handlers, ushort messageID, THandler handler)
            {
                DynamicArrays.Set(ref handlers.m_Handlers, messageID, handler, limit: ushort.MaxValue);
            }

            /// <summary>
            /// Puts <paramref name="handler"/> on the next free MessageID.
            /// </summary>
            /// <param name="handler">Message handler to store.</param>
            /// <returns>MessageID under which handler was registered.</returns>
            public static ushort Put(MessageHandlers<THandler> handlers, THandler handler)
            {
                return (ushort)DynamicArrays.Put(ref handlers.m_Handlers, ref handlers.m_HeadIndex, handler, limit: ushort.MaxValue);
            }

            /// <summary>
            /// Removes message handler under given <paramref name="messageID"/>.
            /// </summary>
            /// <param name="messageID">Handler MessageID to check and remove.</param>
            public static void Remove(MessageHandlers<THandler> handlers, ushort messageID)
            {
                DynamicArrays.Remove(ref handlers.m_Handlers, messageID);
            }
        }
    }
}
