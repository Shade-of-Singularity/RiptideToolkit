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
    /// Settings related to modding support.
    /// </summary>
    /// TODO: Provide a way to block mods from initializing the system/networking during game initialization, and make any attempts throw error.
    public static class Modding
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Setups <see cref="ModIDTotalBits"/> using flags.
        /// Describes how many **bits** [4-16](ushort limit) is used to encode ModID in networking messages.
        /// </summary>
        public const string ModIDHeaderSizeFlag = "-riptide.modding.modidsize";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Describes how many **bits** [4-16](ushort limit) is used to encode ModID in networking messages.
        /// </summary>
        public static byte ModIDTotalBits
        {
            get => m_ModIDHeaderSize;
            set
            {
                if (NetworkIndex.IsEverInitialized) throw new Exception("Cannot modify ModID header size after networking initialized.");
                value = Math.Max((byte)4, Math.Min((byte)16, value));
                if (m_ModIDHeaderSize != value) m_ModIDHeaderSize = value;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static byte m_ModIDHeaderSize = ConsoleArgs.TryGet(ModIDHeaderSizeFlag, out string str)
            && byte.TryParse(str, out byte value) ? Math.Max((byte)4, Math.Min((byte)16, value)) : (byte)12;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>

    }
}
