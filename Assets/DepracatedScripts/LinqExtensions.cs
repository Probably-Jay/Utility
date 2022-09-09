using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace LinqExtensions
{
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