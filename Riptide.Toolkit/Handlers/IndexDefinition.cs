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

namespace Riptide.Toolkit.Handlers
{
    /// <summary>
    /// Flag definition for <see cref="GroupMessageIndexer"/>s.
    /// Defines for which consumer (client/server) an MessageID under an <see cref="IReadOnlyGroupMessageIndexer.GroupID"/> is defined.
    /// </summary>
    [Flags] public enum IndexDefinition : byte
    {
        None = 0,
        Client = 0b01,
        Server = 0b10,
        Both = 0b11,
    }
}
