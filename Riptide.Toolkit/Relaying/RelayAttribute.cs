using System;

namespace Riptide.Toolkit.Relaying
{
    /// <summary>
    /// Messages with this attribute will be automatically relayed server-side.
    /// </summary>
    /// <remarks>
    /// If you are working with enums - apply it to both enum and target values to auto-relay them.
    /// </remarks>
    [Obsolete("Not obsolete, but WIP and does nothing at the moment.")]
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class RelayAttribute : Attribute { }

    [Relay, Obsolete("Not obsolete, but WIP and does nothing at the moment.")]
    internal enum TestEnum : uint
    {
        [Relay] RelayedMessage,
        NonRelayedMessage,
    }
}
