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
    /// Doesn't request any storage.
    /// </summary>
    public sealed class S0 : StorageProfile<S0>
    {
        /// <inheritdoc/>
        public override int Storage => 0;
    }

    /// <summary>
    /// Requests storage amount of 1 item.
    /// </summary>
    public sealed class S1 : StorageProfile<S1>
    {
        /// <inheritdoc/>
        public override int Storage => 1;
    }

    /// <summary>
    /// Requests storage amount of 2 items.
    /// </summary>
    public sealed class S2 : StorageProfile<S2>
    {
        /// <inheritdoc/>
        public override int Storage => 2;
    }

    /// <summary>
    /// Requests storage amount of 3 items.
    /// </summary>
    public sealed class S3 : StorageProfile<S3>
    {
        /// <inheritdoc/>
        public override int Storage => 3;
    }

    /// <summary>
    /// Requests storage amount of 4 items.
    /// </summary>
    public sealed class S4 : StorageProfile<S4>
    {
        /// <inheritdoc/>
        public override int Storage => 4;
    }

    /// <summary>
    /// Requests storage amount of 6 items.
    /// </summary>
    public sealed class S6 : StorageProfile<S6>
    {
        /// <inheritdoc/>
        public override int Storage => 6;
    }

    /// <summary>
    /// Requests storage amount of 8 items.
    /// </summary>
    public sealed class S8 : StorageProfile<S8>
    {
        /// <inheritdoc/>
        public override int Storage => 8;
    }

    /// <summary>
    /// Requests storage amount of 12 items.
    /// </summary>
    public sealed class S12 : StorageProfile<S12>
    {
        /// <inheritdoc/>
        public override int Storage => 12;
    }

    /// <summary>
    /// Requests storage amount of 16 items.
    /// </summary>
    public sealed class S16 : StorageProfile<S16>
    {
        /// <inheritdoc/>
        public override int Storage => 16;
    }

    /// <summary>
    /// Requests storage amount of 24 items.
    /// </summary>
    public sealed class S24 : StorageProfile<S24>
    {
        /// <inheritdoc/>
        public override int Storage => 24;
    }

    /// <summary>
    /// Requests storage amount of 32 items.
    /// </summary>
    public sealed class S32 : StorageProfile<S32>
    {
        /// <inheritdoc/>
        public override int Storage => 32;
    }

    /// <summary>
    /// Requests storage amount of 48 items.
    /// </summary>
    public sealed class S48 : StorageProfile<S48>
    {
        /// <inheritdoc/>
        public override int Storage => 48;
    }

    /// <summary>
    /// Requests storage amount of 64 items.
    /// </summary>
    public sealed class S64 : StorageProfile<S64>
    {
        /// <inheritdoc/>
        public override int Storage => 64;
    }

    /// <summary>
    /// Requests storage amount of 96 items.
    /// </summary>
    public sealed class S96 : StorageProfile<S96>
    {
        /// <inheritdoc/>
        public override int Storage => 96;
    }

    /// <summary>
    /// Requests storage amount of 128 items.
    /// </summary>
    public sealed class S128 : StorageProfile<S128>
    {
        /// <inheritdoc/>
        public override int Storage => 128;
    }
}
