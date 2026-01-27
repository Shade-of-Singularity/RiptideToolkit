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
    // Note: I couldn't use generics for better storage solution. Would be nice to come up with one after all.
    //public static class Pools<T> where T : new()
    //{
    //    private static T[] m_Storage;
    //}

    /// <summary>
    /// Pool for custom objects. Used for <see cref="Messages.NetworkMessage"/>s.
    /// </summary>
    /// TODO: Make overwritable.
    public static class Pools
    {
        private readonly struct Entry
        {
            public readonly object[] pool;
            public readonly uint stored;
            public Entry(object[] pool, uint stored)
            {
                this.pool = pool;
                this.stored = stored;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<Type, Entry> m_Pools = new Dictionary<Type, Entry>();



        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static T Retrieve<T>() where T : new()
        {
            lock (m_Pools)
            {
                if (m_Pools.TryGetValue(typeof(T), out var entry) && entry.stored > 0)
                {
                    m_Pools[typeof(T)] = new Entry(entry.pool, entry.stored - 1);
                    return (T)entry.pool[entry.stored];
                }
            }

            return new T();
        }

        public static object Retrieve(Type type)
        {
            lock (m_Pools)
            {
                if (m_Pools.TryGetValue(type, out var entry) && entry.stored > 0)
                {
                    m_Pools[type] = new Entry(entry.pool, entry.stored - 1);
                    return entry.pool[entry.stored];
                }
            }

            return Activator.CreateInstance(type);
        }

        public static void Store<T>(T item)
        {
            lock (m_Pools)
            {
                // Pool must exist to allow it to store items.
                if (m_Pools.TryGetValue(typeof(T), out var entry))
                {
                    if (entry.stored >= entry.pool.Length)
                    {
                        // Pool space exhausted.
                        return;
                    }

                    entry.pool[entry.stored] = item;
                    m_Pools[typeof(T)] = new Entry(entry.pool, entry.stored + 1);
                }
            }
        }

        public static void Store(object item)
        {
            Type type = item.GetType();
            lock (m_Pools)
            {
                // Pool must exist to allow it to store items.
                if (m_Pools.TryGetValue(type, out var entry))
                {
                    if (entry.stored >= entry.pool.Length)
                    {
                        // Pool space exhausted.
                        return;
                    }

                    entry.pool[entry.stored] = item;
                    m_Pools[type] = new Entry(entry.pool, entry.stored + 1);
                }
            }
        }

        public static int GetCapacity<T>()
        {
            lock (m_Pools) return m_Pools.TryGetValue(typeof(T), out var entry) ? entry.pool.Length : -1;
        }

        public static void EnsureCapacity<T>(int size)
        {
            if (size <= 0) return;
            if (GetCapacity<T>() < size)
            {
                // Copy of SetCapacity, but with one less branch.
                lock (m_Pools)
                {
                    if (m_Pools.TryGetValue(typeof(T), out var entry))
                    {
                        var array = entry.pool;
                        Array.Resize(ref array, size);
                        m_Pools[typeof(T)] = new Entry(array, entry.stored);
                    }
                    else
                    {
                        m_Pools[typeof(T)] = new Entry(new object[size], 0);
                    }
                }
            }
        }

        public static void SetCapacity<T>(int size)
        {
            if (size <= 0)
            {
                lock (m_Pools) m_Pools.Remove(typeof(T));
                return;
            }


            lock (m_Pools)
            {
                if (m_Pools.TryGetValue(typeof(T), out var entry))
                {
                    var array = entry.pool;
                    Array.Resize(ref array, size);
                    m_Pools[typeof(T)] = new Entry(array, entry.stored);
                }
                else
                {
                    m_Pools[typeof(T)] = new Entry(new object[size], 0);
                }
            }
        }

        public static int GetCapacity(Type type) => m_Pools.TryGetValue(type, out var entry) ? entry.pool.Length : -1;
        public static void EnsureCapacity(Type type, int size)
        {
            if (size <= 0) return;
            if (GetCapacity(type) < size)
            {
                lock (m_Pools)
                {
                    if (m_Pools.TryGetValue(type, out var entry))
                    {
                        var array = entry.pool;
                        Array.Resize(ref array, size);
                        m_Pools[type] = new Entry(array, entry.stored);
                    }
                    else
                    {
                        m_Pools[type] = new Entry(new object[size], 0);
                    }
                }
            }
        }

        public static void SetCapacity(Type type, int size)
        {
            if (size <= 0)
            {
                lock (m_Pools) m_Pools.Remove(type);
                return;
            }

            lock (m_Pools)
            {
                if (m_Pools.TryGetValue(type, out var entry))
                {
                    var array = entry.pool;
                    Array.Resize(ref array, size);
                    m_Pools[type] = new Entry(array, entry.stored);
                }
                else
                {
                    m_Pools[type] = new Entry(new object[size], 0);
                }
            }
        }
    }
}
