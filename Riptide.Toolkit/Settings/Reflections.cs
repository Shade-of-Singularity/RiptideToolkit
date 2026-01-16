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

namespace Riptide.Toolkit.Settings
{
    /// <summary>
    /// Holds Toolkit advanced performance options you can configure to improve performance a bit.
    /// </summary>
    public static class Reflections
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Whether to check for <see cref="ModIDAttribute"/> on fields first, before checking properties.
        /// Otherwise will check properties first.
        /// </summary>
        public static bool ModAttributeAnalysis_PrioritizeFields { get; set; } = true;

        /// <summary>
        /// Whether to check for <see cref="GroupIDAttribute"/> on fields first, before checking properties.
        /// Otherwise will check properties first.
        /// </summary>
        public static bool GroupAttributeAnalysis_PrioritizeFields { get; set; } = true;

        /// <summary>
        /// Whether to check for <see cref="MessageIDAttribute"/> on fields first, before checking properties.
        /// Otherwise will check properties first.
        /// </summary>
        public static bool MessageAttributeAnalysis_PrioritizeFields { get; set; } = true;
    }
}
