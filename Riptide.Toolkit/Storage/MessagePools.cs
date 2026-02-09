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

using System;
using System.Collections.Generic;

namespace Riptide.Toolkit.Storage
{
    /// <summary>
    /// Stores references to all <see cref="MessagePool{TMessage}"/> ever created.
    /// </summary>
    /// <remarks>
    /// Won't contain any pools for messages that were not used yet.
    /// </remarks>
    public static class MessagePools
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<Type, IPool> m_Pools = new Dictionary<Type, IPool>();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <remarks>
        /// Use <see cref="MessagePool{TMessage}.Instance"/> instead, if you have a strong-defined type.
        /// </remarks>
        public static void Define(Type type, IPool pool)
        {
            lock (m_Pools)
            {
                m_Pools[type] = pool;
            }
        }

        /// <remarks>
        /// Use <see cref="MessagePool{TMessage}.Instance"/> instead, if you have a strong-defined type.
        /// </remarks>
        public static object Retrieve(Type type)
        {
            lock (m_Pools)
            {
                if (m_Pools.TryGetValue(type, out IPool pool))
                {
                    return pool.Retrieve();
                }
            }

            return Activator.CreateInstance(type);
        }

        /// <remarks>
        /// Use <see cref="MessagePool{TMessage}.Instance"/> instead, if you have a strong-defined type.
        /// </remarks>
        public static void Store(object message)
        {
            if (message is null) return;

            Type type = message.GetType();
            lock (m_Pools)
            {
                // Pool must exist to allow it to store items.
                if (m_Pools.TryGetValue(type, out IPool pool))
                {
                    pool.Store(message);
                }
            }
        }
    }
}
