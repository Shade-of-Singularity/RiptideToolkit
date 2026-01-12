/// - - -    Copyright (c) 2026     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the MIT License. Permission is hereby granted, free of charge,
/// to any person obtaining a copy of this software and associated documentation
/// files to deal in the Software without restriction. Full license terms are
/// available in the LICENSE.md file located at the following repository path:
///   
///                 "Eclipse/Eclipse.Riptide/LICENSE.md"
/// 
/// Note: Eclipse.Riptide and Eclipse are licensed under different licenses.
/// See "Eclipse/LICENSE.md" for details.
/// 
/// ]]>

namespace Eclipse.Riptide.Messages
{
    /// <summary>
    /// Base class for custom groups.
    /// </summary>
    /// <typeparam name="TGroup">Class that inherited this class.</typeparam>
    /// TODO: Add non-generic listing if needed.
    public abstract class NetworkGroup<TGroup> : NetworkGroup where TGroup : NetworkGroup<TGroup>
    {
        public static readonly byte GroupID = NetworkIndex.NextGroupID();
    }

    /// <summary>
    /// Non-generic type for <see cref="NetworkGroup{TGroup}"/>. Exist to make reflections easier.
    /// </summary>
    public abstract class NetworkGroup { }
}
