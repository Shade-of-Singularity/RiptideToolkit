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

using Riptide.Toolkit.Settings;

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
        public static GroupMessageIndexer Create()
        {
            if (Performance.MessageHandlerFocus == PerformanceType.OptimizeCPU)
            {
                return new RegionGroupMessageIndexer();
            }
            else
            {
                return new DictionaryGroupMessageIndexer();
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
        public abstract bool Has(uint messageID);

        /// <inheritdoc/>
        public abstract void Clear();

        /// <inheritdoc/>
        public abstract void Reset();

        /// <inheritdoc/>
        public abstract void Register(uint messageID);

        /// <inheritdoc/>
        public abstract void Remove(uint messageID);
    }
}
