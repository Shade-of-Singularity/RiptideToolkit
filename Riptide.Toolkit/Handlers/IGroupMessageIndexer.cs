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

        /// <summary>
        /// Resets internal arrays to default size, allowing GC to collect everything eventually.
        /// </summary>
        void Reset();

        /// <summary>
        /// Registers <paramref name="messageID"/> under this <see cref="IReadOnlyGroupMessageIndexer.GroupID"/>.
        /// </summary>
        /// <param name="messageID">MessageID to register.</param>
        void Register(uint messageID);

        /// <summary>
        /// Removes <paramref name="messageID"/> from this <see cref="IReadOnlyGroupMessageIndexer.GroupID"/>.
        /// </summary>
        /// <param name="messageID">MessageID to remove.</param>
        void Remove(uint messageID);
    }
}
