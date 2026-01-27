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
using System.Runtime.CompilerServices;

namespace Riptide.Toolkit.Extensions
{
    internal static class DynamicArrays
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const int MinimalArraySize = 4;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static void Clear<T>(this T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = default;
            }
        }

        public static void Clear<T>(this T[] array, ref int head)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = default;
            }

            head = 0;
        }

        public static void Fill<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Reset<T>(this T[] array, int size)
        {
            Reset(ref array, size);
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Reset<T>(this T[] array, ref int head, int size)
        {
            Reset(ref array, ref head, size);
            return array;
        }

        public static void Reset<T>(ref T[] array, int size)
        {
            if (array.Length == size)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = default;
                }
            }
            else
            {
                array = new T[size];
            }
        }

        public static void Reset<T>(ref T[] array, ref int head, int size)
        {
            if (array.Length == size)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = default;
                }
            }
            else
            {
                array = new T[size];
            }

            head = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Set<T>(this T[] array, int at, T value)
        {
            Set(ref array, at, value);
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Set<T>(this T[] array, int at, T value, int limit)
        {
            Set(ref array, at, value, limit);
            return array;
        }

        public static void Set<T>(ref T[] array, int at, T value)
        {
            if (at >= array.Length)
            {
                Array.Resize(ref array, NextArraySize(at));
            }

            array[at] = value;
        }

        /// <param name="increment">By how much array size is allowed to grow. With 4, you will get: [4,8,12,...]. With one: [1,2,3,...]</param>
        public static void SetLinear<T>(ref T[] array, int at, T value, int increment = 1)
        {
            if (at >= array.Length)
            {
                increment = Math.Max(1, increment);
                Array.Resize(ref array, at / increment + increment);
            }

            array[at] = value;
        }

        /// <param name="limit">Array size limit.</param>
        public static void Set<T>(ref T[] array, int at, T value, int limit)
        {
            if (at >= array.Length)
            {
                // This point can only be reached after iterating over whole array.
                int size = NextArraySize(at);
                if (size > limit) throw new Exception($"{nameof(DynamicArrays)} Reached array size limit of ({limit}).");
                Array.Resize(ref array, size);
            }

            array[at] = value;
        }

        public static int Put<T>(ref T[] array, ref int head, T value)
        {
            // Allocated local array variable to reduce instruction amount.
            int length = array.Length;

            // We use 'for' loop here instead of 'while', in case compiler is more familiar with this format.
            for (; head < length; head++)
            {
                if (EqualityComparer<T>.Default.Equals(array[head], default)) goto Finish;
            }

            // This point can only be reached after iterating over whole array.
            Array.Resize(ref array, NextArraySize(head));

            Finish:
            array[head] = value;
            head++; // Picks (potentially free) next ID as head. 
            return head;
        }

        /// <param name="limit">Array size limit.</param>
        public static int Put<T>(ref T[] array, ref int head, T value, int limit)
        {
            // Allocated local array variable to reduce instruction amount.
            int length = array.Length;

            // We use 'for' loop here instead of 'while', in case compiler is more familiar with this format.
            for (; head < length; head++)
            {
                if (EqualityComparer<T>.Default.Equals(array[head], default)) goto Finish;
            }

            // This point can only be reached after iterating over whole array.
            int size = NextArraySize(head);
            if (size > limit) throw new Exception($"{nameof(DynamicArrays)} Reached array size limit of ({limit}).");
            Array.Resize(ref array, size);

            Finish:
            array[head] = value;
            head++; // Picks (potentially free) next ID as head. 
            return head;
        }

        public static int PutStruct<T>(ref T[] array, ref int head, T value) where T : IStructValidator
        {
            // Allocated local array variable to reduce instruction amount.
            int length = array.Length;

            // We use 'for' loop here instead of 'while', in case compiler is more familiar with this format.
            for (; head < length; head++)
            {
                if (array[head].IsDefault) goto Finish;
            }

            // This point can only be reached after iterating over whole array.
            Array.Resize(ref array, NextArraySize(head));

            Finish:
            array[head] = value;
            head++; // Picks (potentially free) next ID as head. 
            return head;
        }

        public static int PutStruct<T>(ref T[] array, ref int head, T value, int limit) where T : IStructValidator
        {
            // Allocated local array variable to reduce instruction amount.
            int length = array.Length;

            // We use 'for' loop here instead of 'while', in case compiler is more familiar with this format.
            for (; head < length; head++)
            {
                if (array[head].IsDefault) goto Finish;
            }

            // This point can only be reached after iterating over whole array.
            int size = NextArraySize(head);
            if (size > limit) throw new Exception($"{nameof(DynamicArrays)} Reached array size limit of ({limit}).");
            Array.Resize(ref array, size);

            Finish:
            array[head] = value;
            head++; // Picks (potentially free) next ID as head. 
            return head;
        }

        public static void Remove<T>(ref T[] array, int at)
        {
            if (array.Length > at)
            {
                array[at] = default;
            }
        }

        public static void Remove<T>(ref T[] array, ref int head, int at)
        {
            if (array.Length > at)
            {
                array[at] = default;
                head = Math.Min(head, at);
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Helpers
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static int NextArraySize(int v)
        {
            // Clamps to the minimal allowed size.
            if (v < MinimalArraySize)
            {
                return MinimalArraySize;
            }

            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            return v + 1;
        }
    }
}
