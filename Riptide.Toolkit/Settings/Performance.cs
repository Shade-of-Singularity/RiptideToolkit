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
        public const string PerformanceTypeFlag = "-riptide.performance.handlers";
        public const string PerformanceTypeCPU = "cpu";
        public const string PerformanceTypeRAM = "ram";
        public const string PerformanceTypeIO = "io";
        public const string PerformanceTypeDisk = "disk";




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
                    throw new Exception($"Cannot modify performance settings after {nameof(NetworkIndex)} initialization! Set performance options on app launch.");
                }

                m_MessageHandlerFocus = value;
            }
        }

        /// <summary>
        /// Region size for <see cref="Handlers.RegionHandlerCollection"/>.
        /// </summary>
        public static int RegionSize
        {
            get => m_RegionSize;
            set
            {
                if (m_RegionSize == value) return;
                if (NetworkIndex.IsEverInitialized)
                {
                    throw new Exception($"Cannot modify performance settings after {nameof(NetworkIndex)} initialization! Set performance options on app launch.");
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
        private static int m_RegionSize = 32;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static Performance()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (TryFind(PerformanceTypeFlag, out string type))
            {
                switch (type)
                {
                    case PerformanceTypeCPU: m_MessageHandlerFocus = PerformanceType.OptimizeCPU; break;
                    case PerformanceTypeRAM: m_MessageHandlerFocus = PerformanceType.OptimizeRAM; break;

                    case PerformanceTypeIO:
                    case PerformanceTypeDisk: throw new Exception("Disk optimization is not supported yet.");
                    case "": break;
                    default: throw new Exception($"Unknown target performance type in command line args: {type}");
                }
            }

            // Simplifications:
            bool Has(string key) => Array.IndexOf(args, key) != -1;
            bool TryFind(string key, out string value)
            {
                int index = Array.IndexOf(args, key);
                if (index == -1)
                {
                    value = null;
                    return false;
                }

                return TryGet(index + 1, out value);
            }

            bool TryGet(int index, out string value)
            {
                if (index < args.Length)
                {
                    value = args[index];
                    return true;
                }

                value = null;
                return false;
            }
        }
    }
}
