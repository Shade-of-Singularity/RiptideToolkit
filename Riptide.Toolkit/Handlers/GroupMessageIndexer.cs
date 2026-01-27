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

using Riptide.Toolkit.Relaying;
using Riptide.Toolkit.Settings;
using System;

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Stores info about which MessageIDs are defined under specific GroupIDs.
    /// </summary>
    /// TODO: Implement both Dictionary-based and Region-based indexers.
    public abstract class GroupMessageIndexer : IGroupMessageIndexer
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Covers all bits in a flag that can correspond to a <see cref="IndexDefinition.Server"/>.
        /// </summary>
        protected const uint ClientInclusiveMask = (uint)IndexDefinition.Client
            | (uint)IndexDefinition.Client << 2 | (uint)IndexDefinition.Client << 4| (uint)IndexDefinition.Client << 6
            | (uint)IndexDefinition.Client << 8 | (uint)IndexDefinition.Client << 10 | (uint)IndexDefinition.Client << 12
            | (uint)IndexDefinition.Client << 14 | (uint)IndexDefinition.Client << 16 | (uint)IndexDefinition.Client << 18
            | (uint)IndexDefinition.Client << 20 | (uint)IndexDefinition.Client << 22 | (uint)IndexDefinition.Client << 24
            | (uint)IndexDefinition.Client << 26 | (uint)IndexDefinition.Client << 28 | (uint)IndexDefinition.Client << 30;

        /// <summary>
        /// Covers all bits in a flag that can correspond to a <see cref="IndexDefinition.Server"/>.
        /// </summary>
        protected const uint ServerInclusiveMask = (uint)IndexDefinition.Server
            | (uint)IndexDefinition.Server << 2 | (uint)IndexDefinition.Server << 4 | (uint)IndexDefinition.Server << 6
            | (uint)IndexDefinition.Server << 8 | (uint)IndexDefinition.Server << 10 | (uint)IndexDefinition.Server << 12
            | (uint)IndexDefinition.Server << 14 | (uint)IndexDefinition.Server << 16 | (uint)IndexDefinition.Server << 18
            | (uint)IndexDefinition.Server << 20 | (uint)IndexDefinition.Server << 22 | (uint)IndexDefinition.Server << 24
            | (uint)IndexDefinition.Server << 26 | (uint)IndexDefinition.Server << 28 | (uint)IndexDefinition.Server << 30;

        protected const uint AllInclusiveMask = uint.MaxValue;
        protected const uint AllExclusiveMask = uint.MinValue;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Creates new instance of <see cref="MessageHandlerCollection{THandler}"/> optimized for <see cref="Performance.MessageHandlerFocus"/> mode.
        /// </summary>
        /// <param name="groupID">GroupID to associate new <see cref="GroupMessageIndexer"/> with.</param>
        public static GroupMessageIndexer Create(byte groupID)
        {
            // TODO: Use RegionMap/Dictionary instead of switch, as to allow overwriting.
            switch (Performance.GroupIndexerFocus)
            {
                case PerformanceType.OptimizeCPU: return new RegionGroupMessageIndexer(groupID);
                case PerformanceType.OptimizeRAM: return new DictionaryGroupMessageIndexer(groupID);
                default:
                    throw new NotSupportedException(
                        $"{nameof(GroupMessageIndexer)} performance focus of ({Performance.GroupIndexerFocus}) is not supported.");
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public byte GroupID { get; } // Note: should we make it modifiable?




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public GroupMessageIndexer(byte groupID) => GroupID = groupID;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public abstract void Clear();

        /// <inheritdoc/>
        public abstract IndexDefinition Get(uint messageID);

        /// <inheritdoc/>
        public abstract void Set(uint messageID, IndexDefinition definition);

        /// <inheritdoc/>
        public abstract void Add(uint messageID, IndexDefinition definition);
    }
}
