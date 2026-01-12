/// - - -    Copyright (c) 2026     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the MIT License. Permission is hereby granted, free of charge,
/// to any person obtaining a copy of this software and associated documentation
/// files to deal in the Software without restriction. Full license terms are
/// available in the LICENSE.md file located at the following repository path:
///   
///                 "Eclipse/Eclipse.Riptide/LICENSE.md"
/// 
/// Note: Eclipse.Riptide and Eclipse are licensed under different licenses.
/// See "Eclipse/LICENSE.md" for details.
/// 
/// ]]>

using System;
using System.Diagnostics.CodeAnalysis;

namespace Eclipse.Riptide.Handlers
{
    /// <summary>
    /// Collection of all network message handlers.
    /// </summary>
    /// <remarks>
    /// DO NOT inherit this class!
    /// Use <see cref="ServerHandlers"/> and <see cref="ClientHandlers"/> directly instead.
    /// This is because custom message handler collections are not supported at the moment.
    /// </remarks>
    public abstract class MessageHandlers<THandler>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private THandler[] m_Handlers = new THandler[(int)SystemMessageID.Amount];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public THandler Get(ushort id)
        {
            NetworkIndex.Initialize();
            // We don't need checks here - it will throw anyway.
            // if (id > m_Handlers.Length) throw new ArgumentOutOfRangeException(nameof(id));
            return m_Handlers[id];
        }
        
        public bool Has(ushort id)
        {
            NetworkIndex.Initialize();
            if (id > m_Handlers.Length) return false;
            return m_Handlers[id] != null;
        }

        public bool TryGet(ushort id, [NotNullWhen(true)] out THandler hander)
        {
            NetworkIndex.Initialize();
            if (id > m_Handlers.Length)
            {
                hander = default!;
                return false;
            }

            hander = m_Handlers[id];
            return hander != null;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Internal
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        internal static class Unsafe
        {
            public static THandler[] GetHandlers(MessageHandlers<THandler> handlers) => handlers.m_Handlers;
            public static void SetHandlers(MessageHandlers<THandler> handlers, THandler[] value) => handlers.m_Handlers = value;
            public static ushort GetCapacity(MessageHandlers<THandler> handlers) => (ushort)handlers.m_Handlers.Length;
            public static void Resize(MessageHandlers<THandler> handlers, ushort size) => Array.Resize(ref handlers.m_Handlers, size);
            public static void Clear(MessageHandlers<THandler> handlers) => Array.Fill(handlers.m_Handlers, default);
            public static void Reset(MessageHandlers<THandler> handlers, ushort size)
            {
                if (handlers.m_Handlers.Length == size)
                {
                    Array.Fill(handlers.m_Handlers, default);
                }
                else
                {
                    handlers.m_Handlers = new THandler[size];
                }
            }

            public static void Put(MessageHandlers<THandler> handlers, ushort messageID, THandler handler)
            {
                if (messageID >= handlers.m_Handlers.Length)
                {
                    // Resizes array to the next power of two (or the same amount if ID is already PoT).
                    Array.Resize(ref handlers.m_Handlers, GetPoTArraySize(messageID + 1));
                }

                handlers.m_Handlers[messageID] = handler;
            }

            public static void Remove(MessageHandlers<THandler> handlers, ushort messageID)
            {
                if (messageID < handlers.m_Handlers.Length)
                {
                    // Resets only if target id is even present.
                    handlers.m_Handlers[messageID] = default!;
                }
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public static int GetPoTArraySize(int x)
            {
                if (x < (ushort)SystemMessageID.Amount)
                {
                    // Clamps to the minimal allowed amount.
                    return (int)SystemMessageID.Amount;
                }

                int v = x - 1;
                v |= v >> 1;
                v |= v >> 2;
                v |= v >> 4;
                v |= v >> 8;
                return v + 1;
            }
        }
    }
}
