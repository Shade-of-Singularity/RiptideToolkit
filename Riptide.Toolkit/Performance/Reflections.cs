namespace Riptide.Toolkit.Performance
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
