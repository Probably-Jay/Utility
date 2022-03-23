using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Helper
{
    public static class EnumUtility 
    {
        /// <summary>
        /// Get an enumerable list of all enum values of a particular type
        /// </summary>
        public static IEnumerable<T> GetEnumValues<T>() where T : Enum => (T[])Enum.GetValues(typeof(T));
        
        public static TTo ConvertEnumOnName<TFrom, TTo> (TFrom enumVal) where TFrom : Enum where TTo : Enum 
            => GetEnumValues<TTo>().First(e => e.ToString() == enumVal.ToString());

        public static TTo ConvertToEnumOnName<TFrom, TTo>(this TFrom enumVal) where TFrom : Enum where TTo : Enum
            => ConvertEnumOnName<TFrom, TTo>(enumVal);
    }
}