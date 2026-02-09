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
using System.Runtime.CompilerServices;

namespace Riptide.Toolkit.Storage
{
    /// <summary>
    /// Struct-based pool for custom values.
    /// </summary>
    public struct StructPool<TValue> : IPool<TValue> where TValue : new()
    {
        private TValue[] pool;
        private uint stored;
        public StructPool(TValue[] pool, uint stored)
        {
            this.pool = pool;
            this.stored = stored;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] object IPool.Retrieve() => Retrieve();
        public TValue Retrieve()
        {
            if (stored == 0 || pool is null)
            {
                return new TValue();
            }

            return pool[--stored];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] void IPool.Store(object value) => Store((TValue)value);
        public void Store(TValue value)
        {
            if (pool is null || stored >= pool.Length)
            {
                return;
            }

            pool[stored++] = value;
        }

        public int GetCapacity() => pool is null ? 0 : pool.Length;
        public void SetCapacity(int size) => Array.Resize(ref pool, size);
        public void EnsureCapacity(int size)
        {
            if (pool is null) pool = new TValue[size];
            if (pool.Length < size) SetCapacity(size);
        }
    }
}
