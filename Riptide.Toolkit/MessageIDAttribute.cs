using System;

namespace Riptide.Toolkit
{
    /// <summary>
    /// Marks ModID in a custom base mod class.
    /// Can be used on both fields and properties.
    /// </summary>
    /// <remarks>
    /// ModID field/property should be static.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MessageIDAttribute : Attribute { }
}
