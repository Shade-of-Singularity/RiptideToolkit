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
        /// RegionMap layout: <c>[Area][Region][Flag]</c>
        /// </para>
        /// </summary>
        private const int FlagCapacity = 16;
        /// <summary>
        /// Mask which describes bits that one flag covers.
        /// </summary>
        private const uint FlagMask = FlagCapacity - 1;
        /// <summary>
        /// By how much bits we need to move an input value to the right to rebase its value around bit #0.
        /// </summary>
        private const int FlagOffset = 4;
        /// <summary>
        /// How many regions one area array stores.
        /// <para>
        /// RegionMap layout: <c>[Area][Region][Flag]</c>
        /// </para>
        /// </summary>
        private const uint RegionCapacity = 64;
        /// <summary>
        /// Mask which describes bits that one area covers.
        /// </summary>
        private const uint RegionMask = RegionCapacity - 1;
        /// <summary>
        /// By how much bits we need to move an input value to the right to rebase its value around bit #0.
        /// </summary>
        private const int RegionOffset = FlagOffset + 6;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private readonly object _lock = new object();
        private uint m_MessageIDHeadIndex;
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
        public override void Clear()
        {
            lock (_lock)
            {
                var area = m_Flags;
                for (uint i = 0; i < area.Length; i++)
                {
                    var regions = area[i];
                    if (regions is null) continue;
                    for (uint j = 0; j < regions.Length; j++)
                    {
                        var flags = regions[j];
                        if (flags is null) continue;
                        Array.Clear(flags, 0, FlagCapacity);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override IndexDefinition Get(uint messageID)
        {
            NetworkIndex.Initialize();
            return GetInternal(messageID);
        }

        /// <inheritdoc/>
        public override void Set(uint messageID, IndexDefinition definition) => SetInternal(messageID, definition, clear: IndexDefinition.Both);

        /// <inheritdoc/>
        public override void Add(uint messageID, IndexDefinition definition)
        {
            switch (definition)
            {
                case IndexDefinition.Both:
                case IndexDefinition.Client:
                case IndexDefinition.Server: SetInternal(messageID, definition, clear: definition); return;

                default:
                case IndexDefinition.None: return;
            }
        }

        /// <inheritdoc/>
        public override uint Put(IndexDefinition definition)
        {
            uint head = m_MessageIDHeadIndex;
            while (true)
            {
                // TODO: Optimize.
                if (GetInternal(head) == IndexDefinition.None)
                {
                    if (definition == IndexDefinition.None) return m_MessageIDHeadIndex = head;
                    else
                    {
                        SetInternal(head, definition, clear: IndexDefinition.Both);
                        m_MessageIDHeadIndex = head + 1;
                        return head;
                    }
                }

                checked { head++; }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private IndexDefinition GetInternal(uint messageID)
        {
            // TODO: Benchmark to test if this solution really more performant than dictionaries.
            //  I suspect branches might hit performance to some degree.
            //  Maybe we can optimize it even further with a right region map sizes.
            uint regionsIndex = messageID >> RegionOffset;
            uint flagsIndex = (messageID & RegionMask) >> FlagOffset;
            uint index = messageID & FlagMask;
            int offset = (int)(messageID & FlagMask);
            lock (_lock)
            {
                var area = m_Flags;
                if (area is null || regionsIndex >= area.Length) return IndexDefinition.None;

                var region = area[regionsIndex];
                if (region is null) return IndexDefinition.None;

                var flags = region[flagsIndex];
                if (flags is null) return IndexDefinition.None;

                return (IndexDefinition)(flags[index] >> offset) & IndexDefinition.Both;
            }
        }

        /// <param name="clear">Describes bits in flag, that have to be cleared before updating value.</param>
        private void SetInternal(uint messageID, IndexDefinition definition, IndexDefinition clear)
        {
            uint regionsIndex = messageID >> RegionOffset;
            uint flagsIndex = (messageID & RegionMask) >> FlagOffset;
            uint index = messageID & FlagMask;
            int offset = (int)(messageID & FlagMask);

            if (definition == IndexDefinition.None)
            {
                lock (_lock)
                {
                    var area = m_Flags;
                    if (area is null || regionsIndex >= area.Length) return;

                    var region = area[regionsIndex];
                    if (region is null) return;

                    var flags = region[flagsIndex];
                    if (flags is null) return;

                    flags[index] &= ~((uint)clear << offset);
                }
            }
            else
            {
                lock (_lock)
                {
                    var area = m_Flags;
                    if (regionsIndex >= area.Length)
                    {
                        Array.Resize(ref area, (int)(regionsIndex + 1));
                        m_Flags = area;
                    }

                    var regions = area[regionsIndex];
                    if (regions is null)
                    {
                        area[regionsIndex] = regions = new uint[RegionCapacity][];
                    }

                    var flags = regions[flagsIndex];
                    if (flags is null)
                    {
                        regions[flagsIndex] = flags = new uint[FlagCapacity];
                        flags[index] = (uint)definition << offset;
                    }
                    else
                    {
                        uint frame = flags[index] & ~((uint)clear << offset);
                        uint value = (uint)(definition & clear) << offset;
                        flags[index] = frame | value;
                    }
                }
            }
        }
    }
}
