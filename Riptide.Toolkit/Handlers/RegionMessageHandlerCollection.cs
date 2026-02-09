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
using Riptide.Toolkit.Settings;
using System;

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Collection for network <see cref="Message"/> handlers.
    /// </summary>
    /// <remarks>
    /// Based on custom Region map implementation.
    /// Has higher memory usage, but 
    /// </remarks>
    /// Note: If in the future someone will create dictionary with ushort as a base - we will use it instead.
    /// TODO: Benchmark! Compare this one with <see cref="DictionaryMessageHandlerCollection{THandler}"/> CPU-wise and RAM-wise.
    /// TODO: Bound-check once entire system is setup.
    public sealed class RegionMessageHandlerCollection<THandler> : MessageHandlerCollection<THandler>
        where THandler : IStructValidator
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Array of RegionMaps with all the handlers.
        /// With region size set to 32 in a constructor,
        /// you will have arrays covering: [N][32][32] cells, totaling to [N][1024] per section.
        /// </summary>
        private THandler[][][] m_Regions;
        /// <summary>
        /// Mask which covers all the bits, not managed by main regions.
        /// </summary>
        private readonly uint m_RootMask;
        /// <summary>
        /// By how much bits inputs has to be offset to move bits in input value to a bit #0.
        /// </summary>
        private readonly int m_RootOffset;
        /// <summary>
        /// Mask which covers all bits, representing values within a larger region.
        /// </summary>
        private readonly uint m_LargeRegionMask;
        /// <summary>
        /// By how much bits inputs has to be offset to move bits in input value to a bit #0.
        /// </summary>
        private readonly int m_LargeRegionOffset;
        /// <summary>
        /// Mask which covers all bits, representing values within a region.
        /// For example, with RegionSize set to 16, mask will be: 0b1111.
        /// </summary>
        private readonly uint m_SmallRegionMask;
        /// <summary>
        /// Size of one small handler region.
        /// </summary>
        private readonly uint m_RegionSize;
        /// <summary>
        /// Index of the next (probably) free cell in region array.
        /// </summary>
        private uint m_HeadIndex;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Simple constructor for <see cref="RegionMessageHandlerCollection{THandler}"/>.
        /// Uses <see cref="Performance.RegionSize"/> as default region size for this collection.
        /// </summary>
        public RegionMessageHandlerCollection() : this(Performance.RegionSize) { }

        /// <summary>
        /// Constructor for <see cref="RegionMessageHandlerCollection{THandler}"/>
        /// </summary>
        /// <remarks>
        /// <paramref name="regionSize"/> MUST be "Power of Two"! (i.e. 2, 4, 8, 16, 32, ...)
        /// </remarks>
        public RegionMessageHandlerCollection(uint regionSize)
        {
            regionSize = Math.Max(2, Math.Min(256, regionSize));
            m_RegionSize = regionSize;

            m_SmallRegionMask = regionSize - 1;
            m_LargeRegionOffset = (int)CountSetBits(m_SmallRegionMask);
            m_LargeRegionMask = (regionSize - 1) << m_LargeRegionOffset;
            m_RootMask = uint.MaxValue ^ m_SmallRegionMask ^ m_LargeRegionMask;
            m_RootOffset = (int)CountSetBits(m_RootMask) + m_LargeRegionOffset;

            m_Regions = new THandler[1][][];
            m_Regions[0] = new THandler[regionSize][];
            m_Regions[0][0] = new THandler[regionSize];
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        /// <exception cref="NullReferenceException">
        /// Means that region for handlers under <paramref name="messageID"/> is not defined.
        /// Implies that message under this ID was not registered.
        /// </exception>
        public override THandler Get(uint messageID)
        {
            if (messageID >= NetworkIndex.InvalidMessageID)
            {
                throw new InvalidOperationException(
                    $"Cannot retrieve message handler for an invalid MessageID (see also: {nameof(NetworkIndex)}.{NetworkIndex.InvalidMessageID})");
            }

            NetworkIndex.Initialize();
            // Makes all bitmask and bit offset operations here to use CPU parallel instruction execution.
            uint region = (messageID & m_LargeRegionMask) >> m_LargeRegionOffset;
            uint rest = messageID >> m_RootOffset;
            uint index = messageID & m_SmallRegionMask;
            return m_Regions[rest][region][index];
        }

        /// <inheritdoc/>
        public override bool Has(uint messageID)
        {
            if (messageID >= NetworkIndex.InvalidMessageID)
            {
                return false;
            }

            NetworkIndex.Initialize();
            // Makes all bitmask and bit offset operations here to use CPU parallel instruction execution.
            uint region = (messageID & m_LargeRegionMask) >> m_LargeRegionOffset;
            uint rest = messageID >> m_RootOffset;
            uint index = messageID & m_SmallRegionMask;
            return m_Regions?[rest]?[region]?[index]?.IsDefault == false;
        }

        /// <inheritdoc/>
        public override bool TryGet(uint messageID, out THandler hander)
        {
            if (messageID >= NetworkIndex.InvalidMessageID)
            {
                hander = default;
                return false;
            }

            NetworkIndex.Initialize();
            // Makes all bitmask and bit offset operations here to use CPU parallel instruction execution.
            uint region = (messageID & m_LargeRegionMask) >> m_LargeRegionOffset;
            uint rest = messageID >> m_RootOffset;
            uint index = messageID & m_SmallRegionMask;
            THandler[] array = m_Regions?[rest]?[region];
            if (array is null)
            {
                hander = default;
                return false;
            }

            hander = array[index];
            return !hander.IsDefault;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public override void Clear()
        {
            int size = (int)m_RegionSize;
            var all = m_Regions;
            for (uint i = 0; i < all.Length; i++)
            {
                var area = all[i];
                if (area is null) continue;
                for (uint j = 0; j < area.Length; j++)
                {
                    var region = area[j];
                    if (region is null) continue;
                    Array.Clear(region, 0, size);
                }
            }

            m_HeadIndex = 0;
        }

        /// <inheritdoc/>
        public override void Set(uint messageID, THandler handler)
        {
            if (messageID >= NetworkIndex.InvalidMessageID)
            {
                throw new InvalidOperationException(
                    $"Cannot set message handler for an invalid MessageID (see also: {nameof(NetworkIndex)}.{NetworkIndex.InvalidMessageID})");
            }

            if (handler == null) throw new ArgumentNullException(nameof(handler));

            // Makes all bitmask and bit offset operations here to use CPU parallel instruction execution.
            uint areaIndex = (messageID & m_LargeRegionMask) >> m_LargeRegionOffset;
            uint restIndex = messageID >> m_RootOffset;
            uint index = messageID & m_SmallRegionMask;
            var rest = m_Regions;
            if (restIndex >= rest.Length)
            {
                Array.Resize(ref rest, (int)(restIndex + 1));
                m_Regions = rest;
            }

            var area = rest[restIndex];
            if (area is null)
            {
                rest[restIndex] = area = new THandler[m_RegionSize][];
            }

            var region = area[areaIndex];
            if (region is null)
            {
                area[areaIndex] = region = new THandler[m_RegionSize];
            }

            region[index] = handler;
        }

        /// <inheritdoc/>
        public override uint Put(THandler handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            // TODO: Optimize by only updating references to larger regions after leaving range of a given region.
            //  And do it without much branching.
            // Allocates local array variable to reduce instruction amount.
            var rest = m_Regions;

            // Allocates masks and offsets on stack to avoid often memory lookups, hopefully.
            uint largeMask = m_LargeRegionMask;
            int largeOffset = m_LargeRegionOffset;
            uint smallMask = m_SmallRegionMask;
            int restOffset = m_RootOffset;

            uint head = m_HeadIndex;
            while (head < NetworkIndex.InvalidMessageID)
            {
                uint areaIndex = (head & largeMask) >> largeOffset;
                uint restIndex = head >> restOffset;
                uint index = head & smallMask;
                if (restIndex >= rest.Length)
                {
                    Array.Resize(ref rest, (int)(restIndex + 1));
                    m_Regions = rest;
                }

                var area = rest[restIndex];
                if (area is null)
                {
                    rest[restIndex] = area = new THandler[m_RegionSize][];
                }

                var region = area[areaIndex];
                if (region is null)
                {
                    area[areaIndex] = region = new THandler[m_RegionSize];
                }

                if (region[index].IsDefault)
                {
                    region[index] = handler;

                    // Moves head to (potentially free) next ID. 
                    m_HeadIndex = head + 1;
                    return head;
                }

                head++;
            }

            throw new InvalidOperationException($"Exhausted all free MessageIDs (see also: {nameof(NetworkIndex)}.{NetworkIndex.InvalidMessageID})");
        }

        /// <inheritdoc/>
        public override void Remove(uint messageID)
        {
            // Makes all bitmask and bit offset operations here to use CPU parallel instruction execution.
            uint areaIndex = (messageID & m_LargeRegionMask) >> m_LargeRegionOffset;
            uint restIndex = messageID >> m_RootOffset;
            uint index = messageID & m_SmallRegionMask;
            var rest = m_Regions;
            if (rest.Length < restIndex)
            {
                return;
            }

            var area = rest[restIndex];
            if (area is null)
            {
                return;
            }

            var region = area[areaIndex];
            if (region is null)
            {
                return;
            }

            region[index] = default;
            if (m_HeadIndex > messageID)
            {
                m_HeadIndex = messageID;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static uint CountSetBits(uint i)
        {
            i -= (i >> 1) & 0x55555555;
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }
    }
}