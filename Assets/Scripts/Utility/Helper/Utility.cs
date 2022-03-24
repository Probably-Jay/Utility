using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

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

    public static class LinqExtensions
    {
        public static string ToListString<T>(this IEnumerable<T> collection, Func<T, string> toStringFunction)
        {
            var sb = new StringBuilder();
            
            foreach (var item in collection) 
                sb.Append($"{toStringFunction(item)}, ");

            return sb.ToString();
        }
    }
    
}