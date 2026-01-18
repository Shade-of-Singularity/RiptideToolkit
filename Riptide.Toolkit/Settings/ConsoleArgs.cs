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
using System.Collections.Generic;

namespace Riptide.Toolkit.Settings
{
    /// <summary>
    /// Simple class to check for console args.
    /// </summary>
    /// <remarks>
    /// Advanced: Should be called accessed anything else in <see cref="Toolkit"/> if you want it to have an effect on it.
    /// Note: Most of the things in <see cref="Toolkit"/> is initialized on direct reference, or when client/server is created.
    /// </remarks>
    public static class ConsoleArgs
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Delegates:

        // Events:

        // Properties:
        /// <summary>
        /// Whether to remove all nested quotes in arguments like: -p """abc"""
        /// <para>When <c>true</c>, will produce: (-p """abc""") -> (-p abc)  or  (-p ""abc""""") -> (-p abc""")</para>
        /// <para>When <c>false</c>, will produce: (-p """abc""") -> (-p ""abc"")   or  (-p ""abc""""") -> (-p "abc"""")</para>
        /// </summary>
        public static bool FullyUnpackQuotes
        {
            get => m_FullyUnpackQuotes;
            set
            {
                if (m_FullyUnpackQuotes == value) return;
                m_FullyUnpackQuotes = value;
                m_IsInitialized = false;
            }
        }

        /// <summary>
        /// Whether to enable usage of (\"abc\"\") to produce ("abc"") output with <see cref="FullyUnpackQuotes"/> enabled.
        /// Will also support (\\) to produce (\) in paths, etc.
        /// </summary>
        /// At the moment it's in TODO state, since it's not needed.
        // public static bool EnableEscapeNotation { get; set; } = false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:
        private static readonly Dictionary<string, string[]> m_CustomArgsAndOverwrites = new Dictionary<string, string[]>();
        private static readonly Dictionary<string, string[]> m_Args = new Dictionary<string, string[]>();
        private static volatile bool m_IsInitialized = false;

        // Encapsulated Fields:
        private static bool m_FullyUnpackQuotes = true;

        // Local Fields:





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Registers custom flag.
        /// </summary>
        /// <remarks>
        /// Advanced: Should be called before anything else in <see cref="Toolkit"/> if you want it to have an effect on it.
        /// Note: Most of the things in <see cref="Toolkit"/> is initialized on direct reference, or when client/server is created.
        /// </remarks>
        public static void Register(string flag)
        {
            if (!m_IsInitialized) lock (m_Args) Initialize();
            string[] values = Array.Empty<string>();
            m_CustomArgsAndOverwrites[flag] = values;
            m_Args[flag] = values;
        }

        /// <summary>
        /// Registers custom flag.
        /// </summary>
        /// <remarks>
        /// Advanced: Should be called before anything else in <see cref="Toolkit"/> if you want it to have an effect on it.
        /// Note: Most of the things in <see cref="Toolkit"/> is initialized on direct reference, or when client/server is created.
        /// </remarks>
        public static void Register(string flag, string value)
        {
            if (!m_IsInitialized) lock (m_Args) Initialize();
            string[] values = new string[1] { value };
            m_CustomArgsAndOverwrites[flag] = values;
            m_Args[flag] = values;
        }

        /// <summary>
        /// Registers custom flag.
        /// </summary>
        /// <remarks>
        /// Advanced: Should be called before anything else in <see cref="Toolkit"/> if you want it to have an effect on it.
        /// Note: Most of the things in <see cref="Toolkit"/> is initialized on direct reference, or when client/server is created.
        /// </remarks>
        public static void Register(string flag, params string[] values)
        {
            if (!m_IsInitialized) lock (m_Args) Initialize();
            m_CustomArgsAndOverwrites[flag] = values;
            m_Args[flag] = values;
        }

        /// <summary>
        /// Checks if <paramref name="flag"/> was defined, regardless of the values it contains.
        /// </summary>
        public static bool Has(string flag)
        {
            if (!m_IsInitialized) lock (m_Args) Initialize();
            return m_Args.ContainsKey(flag);
        }

        /// <summary>
        /// Checks if <paramref name="flag"/> was defined. <paramref name="value"/> is a first value from a list, if multiple is provided.
        /// </summary>
        /// <remarks>
        /// <paramref name="value"/> can be <see cref="string.Empty"/> if user haven't defined any value for it.
        /// </remarks>
        public static bool TryGet(string flag, out string value)
        {
            if (!m_IsInitialized) lock (m_Args) Initialize();
            if (m_Args.TryGetValue(flag, out string[] values))
            {
                value = values is null ? string.Empty : values[0];
                return true;
            }

            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Checks if <paramref name="flag"/> was defined. Also provides all <paramref name="values"/> defined under a flag.
        /// </summary>
        /// <remarks>
        /// <paramref name="value"/> can be <see cref="string.Empty"/> if user haven't defined any value for it.
        /// </remarks>
        public static bool TryGet(string flag, out string[] values)
        {
            if (!m_IsInitialized) lock (m_Args) Initialize();
            return m_Args.TryGetValue(flag, out values);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static void Initialize()
        {
            m_Args.Clear();
            var args = m_Args;
            string lastFlag = null;
            List<string> values = new List<string>(1);
            foreach (var argument in Environment.GetCommandLineArgs())
            {
                if (string.IsNullOrEmpty(argument))
                {
                    // Adds empty fields, as they are usually added in commands.
                    // Let me know if you need this to be changed.
                    values.Add(argument);
                    continue;
                }

                if (argument.StartsWith("-"))
                {
                    // Registers all found values under the last flag.
                    if (!(lastFlag is null))
                    {
                        args[lastFlag] = values.Count == 0 ? Array.Empty<string>() : values.ToArray();
                    }

                    values.Clear();
                    lastFlag = argument;
                    continue;
                }

                int length = argument.Length;
                if (length == 1)
                {
                    // Skips check if value is too small to be enclosed in quotes.
                    values.Add(argument);
                    continue;
                }

                // Removes quotes.
                if (argument[0] == '"' && argument[length - 1] == '"')
                {
                    if (FullyUnpackQuotes)
                    {
                        // Length is at least 2 here.
                        int start = 1; int end = length - 2;

                        // Avoids array indexing.
                        // We also can't use "while (true)" here since we need "break" keyword in switch statement.
                        LoopBack:
                        switch (end - start)
                        {
                            // Value delta will never be less than '-1'
                            // '-1' for strings with length [2,4,6,8,10,12,...]
                            case -1:
                                values.Add(string.Empty);
                                continue;

                            // '0' for strings with length [1,3,5,7,9,11,...]
                            case 0:
                                values.Add(argument.Substring(start, 1));
                                continue;

                            default:
                                // Removes quotes on the inside, if there is any.
                                if (argument[start] == '"' && argument[end] == '"')
                                {
                                    start++; end--;
                                    goto LoopBack;
                                }

                                // Adds value without outer quotes.
                                values.Add(argument.Substring(start, end - start + 1));
                                continue;
                        }
                    }
                    else
                    {
                        values.Add(argument.Substring(1, length - 2));
                        continue;
                    }
                }
                else
                {
                    values.Add(argument);
                    continue;
                }
            }

            // Registers all found values under the very last flag.
            if (!(lastFlag is null))
            {
                args[lastFlag] = values.Count == 0 ? Array.Empty<string>() : values.ToArray();
            }

            // Checks for custom values and overwrites.
            foreach (var custom in m_CustomArgsAndOverwrites)
            {
                args[custom.Key] = custom.Value;
            }

            m_IsInitialized = true;
        }
    }
}
