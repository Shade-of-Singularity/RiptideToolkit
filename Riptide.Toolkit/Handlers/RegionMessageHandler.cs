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
using System.Drawing;

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
    public sealed class RegionHandlerCollection<THandler> : MessageHandlerCollection<THandler>
        where THandler : IStructValidator
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Array of RegionMaps with all the handlers.
        /// </summary>
        private readonly THandler[][][] m_Regions;

        /// <summary>
        /// Direct reference to RegionMap under core mod.
        /// Saves few instructions.
        /// </summary>
        private readonly THandler[][] m_MainRegions;

        /// <summary>
        /// Size of one handler region.
        /// </summary>
        private readonly int m_RegionSize;

        /// <summary>
        /// Mask which covers all bits, representing values within a region.
        /// For example, with RegionSize set to 16, mask will be: 0b1111.
        /// </summary>
        private readonly int m_RegionMask;

        /// <summary>
        /// By how much bits inputs has to be offset to move bits in input value to a bit #0.
        /// </summary>
        private readonly int m_RegionOffset;

        /// <summary>
        /// Index of the next (probably) free cell in region array.
        /// </summary>
        private int m_HeadIndex = 0;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Simple constructor for <see cref="RegionHandlerCollection{THandler}"/>.
        /// Uses <see cref="Performance.RegionSize"/> as default region size for this collection.
        /// </summary>
        public RegionHandlerCollection() : this(Performance.RegionSize) { }

        /// <summary>
        /// Constructor for <see cref="RegionHandlerCollection{THandler}"/>
        /// </summary>
        /// <remarks>
        /// <paramref name="size"/> MUST be "Power of Two"! (i.e. 2, 4, 8, 16, 32, ...)
        /// </remarks>
        public RegionHandlerCollection(int size)
        {
            size = Math.Max(2, size);
            m_RegionSize = size;
            m_RegionMask = size - 1;
            m_RegionOffset = CountSetBits(m_RegionMask);

            m_Regions = new THandler[1][][]; // Only initializes region map for one mod.
            m_Regions[0] = m_MainRegions = new THandler[(ushort.MaxValue + 1) / size][];
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
        public override THandler Get(ushort messageID)
        {
            NetworkIndex.Initialize();
            return m_MainRegions[messageID >> m_RegionOffset][messageID & m_RegionMask];
        }

        /// <inheritdoc/>
        /// <exception cref="NullReferenceException">
        /// Means that region for handlers under <paramref name="messageID"/> is not defined.
        /// Implies that message under this ID was not registered.
        /// </exception>
        public override THandler Get(ushort modID, ushort messageID)
        {
            NetworkIndex.Initialize();
            return m_Regions[modID][messageID >> m_RegionOffset][messageID & m_RegionMask];
        }

        /// <inheritdoc/>
        public override bool Has(ushort messageID)
        {
            NetworkIndex.Initialize();
            var region = m_MainRegions[messageID >> m_RegionOffset];
            return !(region is null) && !region[messageID & m_RegionMask].IsDefault;
        }

        /// <inheritdoc/>
        public override bool Has(ushort modID, ushort messageID)
        {
            NetworkIndex.Initialize();
            var region = m_Regions[modID][messageID >> m_RegionOffset];
            return !(region is null) && !region[messageID & m_RegionMask].IsDefault;
        }

        /// <inheritdoc/>
        public override bool TryGet(ushort messageID, out THandler hander)
        {
            NetworkIndex.Initialize();
            var region = m_MainRegions[messageID >> m_RegionOffset];
            if (region is null)
            {
                hander = default;
                return false;
            }

            hander = region[messageID & m_RegionMask];
            return !hander.IsDefault;
        }

        /// <inheritdoc/>
        public override bool TryGet(ushort modID, ushort messageID, out THandler hander)
        {
            NetworkIndex.Initialize();
            var region = m_Regions[modID][messageID >> m_RegionOffset];
            if (region is null)
            {
                hander = default;
                return false;
            }

            hander = region[messageID & m_RegionMask];
            return !hander.IsDefault;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        protected override void Clear()
        {
            int size = m_RegionSize;
            var mods = m_Regions;
            for (int i = 0; i < mods.Length; i++)
            {
                var mod = mods[i];
                if (mod is null) continue;
                for (int j = 0; j < mod.Length; j++)
                {
                    var region = mod[i];
                    if (region is null) continue;
                    Array.Clear(region, 0, size);
                }
            }
        }

        /// <inheritdoc/>
        protected override void Reset()
        {
            var mods = m_Regions;
            for (int i = 0; i < mods.Length; i++)
            {
                var mod = mods[i];
                if (mod is null) continue;
                for (int j = 0; j < mod.Length; j++)
                {
                    mod[j] = null;
                }
            }
        }

        /// <inheritdoc/>
        protected override void Set(ushort modID, ushort messageID, THandler handler)
        {
            int regionIndex = messageID >> m_RegionOffset;
            int referenceIndex = messageID & m_RegionMask;

            var mods = m_Regions[modID];
            var region = mods[regionIndex];
            if (region is null)
            {
                mods[regionIndex] = region = new THandler[(ushort.MaxValue + 1) / m_RegionSize];
                region[referenceIndex] = handler;
            }
            else
            {
                region[referenceIndex] = handler;
            }
        }

        /// <inheritdoc/>
        protected override ushort Put(ushort modID, THandler handler)
        {
            // Allocates local array variable to reduce instruction amount.
            var regions = m_Regions[modID];
            for (; m_HeadIndex <= ushort.MaxValue; m_HeadIndex++)
            {
                int regionIndex = m_HeadIndex >> m_RegionOffset;
                int referenceIndex = m_HeadIndex & m_RegionMask;
                THandler[] region = regions[regionIndex];
                if (region is null)
                {
                    regions[regionIndex] = region = new THandler[(ushort.MaxValue + 1) / m_RegionSize];
                    region[referenceIndex] = handler;
                    break;
                }
                else if (region[referenceIndex].IsDefault)
                {
                    region[referenceIndex] = handler;
                }
            }

            // Moves head to (potentially free) next ID. 
            return (ushort)(++m_HeadIndex);
        }

        /// <inheritdoc/>
        protected override void Remove(ushort modID, ushort messageID)
        {
            var region = m_Regions[modID][messageID >> m_RegionOffset];
            if (!(region is null))
            {
                region[messageID & m_RegionMask] = default;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static int CountSetBits(int i)
        {
            i -= (i >> 1) & 0x55555555;
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }
    }
}