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

namespace Riptide.Toolkit.Storage
{
    /// <summary>
    /// Doesn't request any storage.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S0Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 0;
    }

    /// <summary>
    /// Requests storage amount of 1 item.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S1Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 1;
    }

    /// <summary>
    /// Requests storage amount of 2 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S2Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 2;
    }

    /// <summary>
    /// Requests storage amount of 3 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S3Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 3;
    }

    /// <summary>
    /// Requests storage amount of 4 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S4Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 4;
    }

    /// <summary>
    /// Requests storage amount of 6 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S6Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 6;
    }

    /// <summary>
    /// Requests storage amount of 8 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S8Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 8;
    }

    /// <summary>
    /// Requests storage amount of 12 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S12Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 12;
    }

    /// <summary>
    /// Requests storage amount of 16 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S16Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 16;
    }

    /// <summary>
    /// Requests storage amount of 24 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S24Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 24;
    }

    /// <summary>
    /// Requests storage amount of 32 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S32Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 32;
    }

    /// <summary>
    /// Requests storage amount of 48 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S48Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 48;
    }

    /// <summary>
    /// Requests storage amount of 64 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S64Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 64;
    }

    /// <summary>
    /// Requests storage amount of 96 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S96Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 96;
    }

    /// <summary>
    /// Requests storage amount of 128 items.
    /// </summary>
    [AttributeUsage(Targets, AllowMultiple = AllowMultiple, Inherited = Inherited)]
    public sealed class S128Attribute : StorageAttribute
    {
        /// <inheritdoc/>
        public override int Storage => 128;
    }
}
