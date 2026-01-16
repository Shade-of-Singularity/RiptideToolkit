
using System;

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
namespace Riptide.Toolkit.Messages
{
    /// <summary>
    /// Base class for custom groups.
    /// </summary>
    /// <typeparam name="TGroup">Class that inherited this class.</typeparam>
    /// TODO: Add non-generic listing if needed.
    public abstract class NetworkGroup<TGroup> : NetworkGroup where TGroup : NetworkGroup<TGroup>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Handle = typeof(TGroup);

        /// <summary>
        /// 
        /// </summary>
        [GroupID] public static readonly byte GroupID = NetworkIndex.NextGroupID();
    }

    /// <summary>
    /// Non-generic type for <see cref="NetworkGroup{TGroup}"/>. Exist to make reflections easier.
    /// </summary>
    public abstract class NetworkGroup { }
}
