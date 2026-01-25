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

namespace Riptide.Toolkit
{
    /// <summary>
    /// Marks MessageID in a custom base class.
    /// By default, set this property to <see cref="NetworkIndex.InvalidMessageID"/> to avoid confusion, and probably bugs.
    /// </summary>
    /// <remarks>
    /// Property MUST define both <c>getter</c> and <c>setter</c>.
    /// <c>setter</c> can be private, e.g.: <c>{ get; private set; }</c>.
    /// MessageID will be provided by <see cref="NetworkIndex"/> via <see cref="System.Reflection"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MessageIDAttribute : Attribute { }
}
