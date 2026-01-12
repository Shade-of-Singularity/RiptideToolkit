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

namespace Riptide.Toolkit.Storage
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TProfile">Class that inherited this <see cref="StorageProfile{T}"/>.</typeparam>
    public abstract class StorageProfile<TProfile> : StorageProfile where TProfile : StorageProfile<TProfile>, new()
    {
        public static readonly TProfile Instance = new TProfile();
    }

    /// <summary>
    /// Controls how much storage in internal pools in allocated to each type.
    /// </summary>
    public abstract class StorageProfile
    {
        /// <summary>
        /// Storage amount this load profile demands.
        /// </summary>
        public abstract int Storage { get; }
    }
}
