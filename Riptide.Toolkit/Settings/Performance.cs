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

namespace Riptide.Toolkit.Settings
{
    /// <summary>
    /// Performance settings for <see cref="Toolkit"/> systems.
    /// </summary>
    /// <remarks>
    /// Most of the settings here can only be set before starting any client or server.
    /// It is recommended to set those settings somewhere around application initialization.
    /// </remarks>
    public static class Performance
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const string MessageHandlersPerformanceFocusFlag = "-riptide.performance.handlers";
        public const string GroupIndexerPerformanceFocusFlag = "-riptide.performance.indexers";
        public const string PerformanceFocusCPU = "cpu";
        public const string PerformanceFocusRAM = "ram";
        public const string PerformanceFocusIO = "io";
        public const string PerformanceFocusDisk = "disk";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Type of optimization <see cref="Handlers.ClientHandlers"/> and <see cref="Handlers.ServerHandlers"/> will use.
        /// </summary>
        /// <remarks>
        /// <para>With <see cref="PerformanceType.OptimizeCPU"/> - adds ~4MB of RAM overhead. Internal list access takes '1.477 ns'.</para>
        /// <para>With <see cref="PerformanceType.OptimizeRAM"/> - adds ~32KB of overhead. Internal list access takes '7.035 ns'.</para>
        /// </remarks>
        /// TODO: Benchmark data was only tested in a lab environment. We need to test again using handler implementations directly.
        public static PerformanceType MessageHandlerFocus
        {
            get => m_MessageHandlerFocus;
            set
            {
                if (m_MessageHandlerFocus == value) return;
                if (NetworkIndex.IsEverInitialized)
                {
                    throw new Exception(
                        $"Cannot modify performance settings after {nameof(NetworkIndex)} initialization! Set performance options on app launch.");
                }

                m_MessageHandlerFocus = value;
            }
        }

        /// <summary>
        /// Type of optimization <see cref="Handlers.ClientHandlers"/> and <see cref="Handlers.ServerHandlers"/> will use.
        /// </summary>
        /// <remarks>
        /// <para>With <see cref="PerformanceType.OptimizeCPU"/> - adds ~4MB of RAM overhead. Internal list access takes '1.477 ns'.</para>
        /// <para>With <see cref="PerformanceType.OptimizeRAM"/> - adds ~32KB of overhead. Internal list access takes '7.035 ns'.</para>
        /// </remarks>
        /// TODO: Benchmark data was only tested in a lab environment. We need to test again using handler implementations directly.
        public static PerformanceType GroupIndexerFocus
        {
            get => m_GroupIndexerFocus;
            set
            {
                if (m_GroupIndexerFocus == value) return;
                if (NetworkIndex.IsEverInitialized)
                {
                    throw new Exception(
                        $"Cannot modify performance settings after {nameof(NetworkIndex)} initialization! Set performance options on app launch.");
                }

                m_GroupIndexerFocus = value;
            }
        }

        /// <summary>
        /// Region size for <see cref="Handlers.RegionHandlerCollection"/>.
        /// </summary>
        public static uint RegionSize
        {
            get => m_RegionSize;
            set
            {
                if (m_RegionSize == value) return;
                if (NetworkIndex.IsEverInitialized)
                {
                    throw new Exception(
                        $"Cannot modify performance settings after {nameof(NetworkIndex)} initialization! Set performance options on app launch.");
                }

                m_RegionSize = value;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static PerformanceType m_MessageHandlerFocus = PerformanceType.OptimizeCPU;
        private static PerformanceType m_GroupIndexerFocus = PerformanceType.OptimizeCPU;
        private static uint m_RegionSize = 32;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static Performance()
        {
            if (ConsoleArgs.TryGet(GroupIndexerPerformanceFocusFlag, out string type))
            {
                switch (type)
                {
                    case PerformanceFocusCPU: m_MessageHandlerFocus = PerformanceType.OptimizeCPU; break;
                    case PerformanceFocusRAM: m_MessageHandlerFocus = PerformanceType.OptimizeRAM; break;

                    case PerformanceFocusIO:
                    case PerformanceFocusDisk: throw new Exception("Disk optimization is not supported yet.");
                    case "": break;
                    default: throw new Exception($"Unknown target performance type in command line args: {type}");
                }
            }
        }
    }
}
