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

namespace Riptide.Toolkit.Relaying
{
    public sealed class RegionAdvancedRelayFilter : AdvancedRelayFilter
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Size (in bits) of one flag in a filter array.
        /// </summary>
        private const uint FlagSize = sizeof(uint) * 8;
        /// <summary>
        /// Offset (in bits) from bit #0, covering <see cref="FlagMask"/>.
        /// </summary>
        private const int FlagOffset = 5;
        /// <summary>
        /// Mask covering all bits for one filter flag.
        /// </summary>
        private const uint FlagMask = FlagSize - 1;
        /// <summary>
        /// Size (in flags) of inner region map.
        /// </summary>
        private const uint RegionSize = 8;
        /// <summary>
        /// Mask covering all bits for one region flag.
        /// </summary>
        private const uint RegionMask = RegionSize - 1;
        /// <summary>
        /// Offset (in bits) from bit #0, covering <see cref="FlagMask"/> and <see cref="RegionSize"/>
        /// </summary>
        private const int RegionOffset = FlagOffset + 3;



        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private uint[][] m_Filters = new uint[0][];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public override void EnableRelay(uint messageID)
        {
            // On separate lines, so we can **attempt** to use parallel CPU instruction execution, if C# v8 even supports it.
            uint rootIndex = messageID >> RegionOffset;
            uint regionIndex = (messageID & RegionMask) >> FlagOffset;
            uint pin = 1u << (int)(messageID & FlagMask);

            uint[][] root = m_Filters;
            if (rootIndex >= root.Length)
            {
                Array.Resize(ref root, (int)(rootIndex + 1));
                m_Filters = root;
            }

            uint[] regions = root[rootIndex];
            if (regions is null)
            {
                root[rootIndex] = regions = new uint[RegionSize];
            }

            regions[regionIndex] |= pin;
        }

        public override void DisableRelay(uint messageID)
        {
            // On separate lines, so we can **attempt** to use parallel CPU instruction execution, if C# v8 even supports it.
            uint rootIndex = messageID >> RegionOffset;
            uint regionIndex = (messageID & RegionMask) >> FlagOffset;

            uint[][] root = m_Filters;
            if (rootIndex >= root.Length)
            {
                return;
            }

            uint[] regions = root[rootIndex];
            if (regions is null)
            {
                return;
            }

            regions[regionIndex] &= ~(1u << (int)(messageID & FlagMask));
        }

        public override bool ShouldRelay(uint messageID)
        {
            // On separate lines, so we can **attempt** to use parallel CPU instruction execution, if C# v8 even supports it.
            uint rootIndex = messageID >> RegionOffset;
            uint regionIndex = (messageID & RegionMask) >> FlagOffset;

            uint[][] root = m_Filters;
            if (rootIndex >= root.Length)
            {
                return false;
            }

            uint[] regions = root[rootIndex];
            if (regions is null)
            {
                return false;
            }

            return (regions[regionIndex] & (1u << (int)(messageID & FlagMask))) != 0;
        }

        public override void Clear()
        {
            uint[][] root = m_Filters;
            for (int i = 0; i < root.Length; i++)
            {
                uint[] regions = root[i];
                for (int j = 0; j < root.Length; j++)
                {
                    regions[j] = 0u;
                }
            }
        }
    }
}
