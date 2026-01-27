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
