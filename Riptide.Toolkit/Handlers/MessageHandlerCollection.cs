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
    /// Base class for collections, which store client-side/server-side message handlers.
    /// </summary>
    /// <remarks>
    /// No reason to implement this one - Toolkit won't use any custom implementation (as of right now).
    /// Implemented in <see cref="RegionHandlerCollection{THandler}"/> and {TODO: insert DictionaryHandlerCollection}.
    /// </remarks>
    public abstract class MessageHandlerCollection<THandler> : IMessageHandlerCollection<THandler> where THandler : IStructValidator
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Creates new instance of <see cref="MessageHandlerCollection{THandler}"/> optimized for <see cref="Performance.MessageHandlerFocus"/> mode.
        /// </summary>
        public static MessageHandlerCollection<THandler> Create()
        {
            // TODO: Use RegionMap/Dictionary instead of switch, as to allow overwriting.
            switch (Performance.GroupIndexerFocus)
            {
                case PerformanceType.OptimizeCPU: return new RegionHandlerCollection<THandler>(Performance.RegionSize);
                case PerformanceType.OptimizeRAM: return new DictionaryHandlerCollection<THandler>();
                default:
                    throw new NotSupportedException(
                        $"{nameof(GroupMessageIndexer)} performance focus of ({Performance.GroupIndexerFocus}) is not supported.");
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public abstract THandler Get(uint messageID);

        /// <inheritdoc/>
        public abstract bool Has(uint messageID);

        /// <inheritdoc/>
        public abstract bool TryGet(uint messageID, out THandler hander);

        /// <inheritdoc/>
        public abstract void Clear();

        /// <inheritdoc/>
        public abstract void Set(uint messageID, THandler handler);

        /// <inheritdoc/>
        public abstract uint Put(THandler handler);

        /// <inheritdoc/>
        public abstract void Remove(uint messageID);
    }
}
