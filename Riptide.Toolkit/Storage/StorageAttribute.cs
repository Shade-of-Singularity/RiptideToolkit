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
using System;

namespace Riptide.Toolkit.Storage
{
    /// <summary>
    /// Controls how much storage in internal pools in allocated to each type.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public abstract class StorageAttribute : Attribute
    {
        /// <summary>
        /// Targets which are allowed for storage attribute.
        /// At the moment used with <see cref="NetworkMessage"/>s only, but we allow all just in case other people need it.
        /// </summary>
        public const AttributeTargets Targets = AttributeTargets.All;
        /// <summary>
        /// Never allows multiple, as it will throw on <see cref="System.Reflection.MemberInfo.GetCustomAttributes(Type, bool)"/>.
        /// </summary>
        public const bool AllowMultiple = false;
        /// <summary>
        /// <c>true</c>/<c>false</c> doesn't actually matter at the moment - we keep it <c>true</c> just in case.
        /// </summary>
        public const bool Inherited = true;
        /// <summary>
        /// Storage amount this load profile demands.
        /// </summary>
        public abstract int Storage { get; }
    }
}
