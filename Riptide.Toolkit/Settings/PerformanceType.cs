namespace Riptide.Toolkit.Settings
{
    /// <summary>
    /// Type of performance <see cref="Toolkit"/> should focus on.
    /// </summary>
    /// <remarks>
    /// You should not be worried about any of those options unless you experience CPU or RAM problems.
    /// </remarks>
    /// Performance types are usually mutually exclusive.
    /// This is why we are not using flags.
    /// If system can provide support to multiple modes - just implement the best amongst them.
    public enum PerformanceType : byte
    {
        /// <summary>
        /// <para> Optimizes CPU usage. </para>
        /// <para> Increases RAM usage. </para>
        /// <para> Disk usage usually stays intact. </para>
        /// </summary>
        OptimizeCPU = 0,

        /// <summary>
        /// <para> Optimizes RAM usage. </para>
        /// <para> Increases CPU usage. </para>
        /// <para> Disk usage usually stays intact. </para>
        /// </summary>
        OptimizeRAM = 1,

        /// <summary>
        /// <para> Optimizes disk usage and reduces storage amount. </para>
        /// <para> Increases CPU and RAM usage. </para>
        /// </summary>
        /// <remarks>
        /// This mode can be used if you are worried about disk wear or have slow drive, like HDD.
        /// </remarks>
        // [Obsolete("Type not in use as toolkit doesn't interact with disk yet.", error: true)]
        // OptimizeIO = 2,
    }
}
