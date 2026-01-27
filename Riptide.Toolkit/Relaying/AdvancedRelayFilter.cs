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

using Riptide.Toolkit.Settings;
using System;
using System.Runtime.CompilerServices;

namespace Riptide.Toolkit.Relaying
{
    public abstract class AdvancedRelayFilter
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static AdvancedRelayFilter Create()
        {
            // TODO: Use RegionMap/Dictionary instead of switch, as to allow overwriting.
            switch (Performance.RelayFiltersFocus)
            {
                case PerformanceType.OptimizeCPU: return new RegionAdvancedRelayFilter();
                case PerformanceType.OptimizeRAM: return new DictionaryAdvancedRelayFilter();
                default:
                    throw new NotSupportedException(
                        $"{nameof(AdvancedRelayFilter)} performance focus of ({Performance.GroupIndexerFocus}) is not supported.");
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="EnableRelay(uint)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EnableRelay(Enum messageID) => EnableRelay((uint)(object)messageID);
        /// <inheritdoc cref="MessageRelayFilter.EnableRelay(ushort)"/>
        public abstract void EnableRelay(uint messageID);


        /// <inheritdoc cref="DisableRelay(uint)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void DisableRelay(Enum messageID) => DisableRelay((uint)(object)messageID);
        /// <inheritdoc cref="MessageRelayFilter.DisableRelay(ushort)"/>
        public abstract void DisableRelay(uint messageID);


        /// <inheritdoc cref="ShouldRelay(uint)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool ShouldRelay(Enum messageID) => ShouldRelay((uint)(object)messageID);
        /// <summary>Checks whether or not messages with the given ID should be relayed.</summary>
        /// <param name="messageID">The message ID to check.</param>
        /// <returns>Whether or not messages with the given ID should be relayed.</returns>
        public abstract bool ShouldRelay(uint messageID);

        /// <summary>
        /// Clears all filters.
        /// </summary>
        public abstract void Clear();
    }
}
