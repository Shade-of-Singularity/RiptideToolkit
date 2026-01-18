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

using Riptide.Toolkit.Messages;

namespace Riptide.Toolkit
{
    /// <summary>
    /// Group for all default messages.
    /// Can always be used unless you want introduce more than one group.
    /// </summary>
    /// TODO: Make sure it has GroupID of 0, no matter what.
    public sealed class DefaultGroup : NetworkGroup<DefaultGroup> { }
}
