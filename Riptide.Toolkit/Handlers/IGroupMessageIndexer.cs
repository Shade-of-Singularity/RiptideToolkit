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

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Allows to check if MessageID belongs to this <see cref="IReadOnlyGroupMessageIndexer.GroupID"/>.
    /// Specifies API for <see cref="GroupMessageIndexer"/>.
    /// </summary>
    public interface IGroupMessageIndexer : IReadOnlyGroupMessageIndexer
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Resets internal flags.
        /// </summary>
        void Clear();

        // void Trim();

        /// <summary>
        /// Defines whether specific <paramref name="messageID"/> is defined client-side and/or server-side.
        /// </summary>
        /// <remarks>
        /// Overwrites existing definition.
        /// </remarks>
        /// <param name="messageID">MessageID to register.</param>
        /// <param name="definition">Definition under <paramref name="messageID"/>.</param>
        void Set(uint messageID, IndexDefinition definition);

        /// <summary>
        /// Defines whether specific <paramref name="messageID"/> is defined client-side and/or server-side.
        /// </summary>
        /// <remarks>
        /// Adds to an existing definition. <see cref="IndexDefinition.None"/> is ignored.
        /// </remarks>
        /// <param name="messageID">MessageID to register.</param>
        /// <param name="definition">Definition under <paramref name="messageID"/>.</param>
        void Add(uint messageID, IndexDefinition definition);

        /// <summary>
        /// Puts <paramref name="definition"/> under the next free index.
        /// </summary>
        /// <remarks>
        /// Using <see cref="IndexDefinition.None"/> will not store anything in the collection, and will return free MessageID spot.
        /// </remarks>
        /// <param name="definition">Index definition to put in the next free spot.</param>
        /// <returns>MessageID under which index was defined.</returns>
        uint Put(IndexDefinition definition);
    }
}
