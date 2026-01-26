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
/// 

using System;

namespace Riptide.Toolkit.Handlers
{
    public sealed class RegionGroupMessageIndexer : GroupMessageIndexer
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// How many flags one region stores.
        /// <para>
        /// RegionMap layout: <c>[Area][Region][Index]</c>
        /// </para>
        /// </summary>
        private const int RegionSize = 32;
        /// <summary>
        /// Mask which describes bits that one flag covers.
        /// </summary>
        private const uint RegionMask = RegionSize - 1;
        /// <summary>
        /// By how much bits we need to move an input value to the right to rebase its value around bit #0.
        /// </summary>
        private const int RegionOffset = 5;
        /// <summary>
        /// How many regions one area list stores.
        /// <para>
        /// RegionMap layout: <c>[Area][Region][Index]</c>
        /// </para>
        /// </summary>
        private const uint AreaSize = 64;
        /// <summary>
        /// Mask which describes bits that one area covers.
        /// </summary>
        private const uint AreaMask = AreaSize - 1;
        /// <summary>
        /// By how much bits we need to move an input value to the right to rebase its value around bit #0.
        /// </summary>
        private const int AreaOffset = RegionOffset + 6;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private readonly object _lock = new object();
        private uint[][][] m_Flags;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public RegionGroupMessageIndexer(byte groupID) : base(groupID) => m_Flags = new uint[0][][];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public override bool Has(uint messageID)
        {
            NetworkIndex.Initialize();
            // TODO: Benchmark to test if this solution really more performant than dictionaries.
            //  Maybe we can optimize it even further with a right region map sizes.
            uint areaIndex = messageID >> AreaOffset;
            uint regionIndex = (messageID & AreaMask) >> RegionOffset;
            uint index = messageID & RegionMask;
            uint bit = 1u << (int)(messageID & RegionMask);
            lock (_lock)
            {
                var area = m_Flags;
                if (area is null || areaIndex >= area.Length) return false;

                var region = area[areaIndex];
                if (region is null) return false;

                var flags = region[regionIndex];
                if (flags is null) return false;

                return (flags[index] & bit) != 0;
            }
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            lock (_lock)
            {
                var root = m_Flags;
                for (uint i = 0; i < root.Length; i++)
                {
                    var area = root[i];
                    if (area is null) continue;
                    for (uint j = 0; j < area.Length; j++)
                    {
                        var region = area[j];
                        if (region is null) continue;
                        Array.Clear(region, 0, RegionSize);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Register(uint messageID)
        {
            uint areaIndex = messageID >> AreaOffset;
            uint regionIndex = (messageID & AreaMask) >> RegionOffset;
            uint index = messageID & RegionMask;
            uint bit = 1u << (int)(messageID & RegionMask);
            lock (_lock)
            {
                var root = m_Flags;
                if (areaIndex >= root.Length)
                {
                    Array.Resize(ref root, (int)(areaIndex + 1));
                    m_Flags = root;
                }

                var area = root[areaIndex];
                if (area is null)
                {
                    root[areaIndex] = area = new uint[AreaSize][];
                }

                var region = area[regionIndex];
                if (region is null)
                {
                    area[regionIndex] = region = new uint[RegionSize];
                    region[index] = bit;
                }
                else
                {
                    region[index] |= bit;
                }
            }
        }

        /// <inheritdoc/>
        public override void Remove(uint messageID)
        {
            uint areaIndex = messageID >> AreaOffset;
            uint regionIndex = (messageID & AreaMask) >> RegionOffset;
            uint index = messageID & RegionMask;
            uint bit = 1u << (int)(messageID & RegionMask);
            lock (_lock)
            {
                var root = m_Flags;
                if (areaIndex >= root.Length)
                {
                    return;
                }

                var area = root[areaIndex];
                if (area is null)
                {
                    return;
                }

                var region = area[regionIndex];
                if (region is null)
                {
                    return;
                }
                else
                {
                    region[index] = region[index] & ~bit;
                }
            }
        }
    }
}
