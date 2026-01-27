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

        /// <inheritdoc/>
        public abstract uint Put(IndexDefinition definition);
    }
}
