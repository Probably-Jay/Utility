using System;
using System.Collections.Generic;
using System.Linq;
using BetterComponentHandling.Internal;
using BetterComponentHandling.NullChecks;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;
using static BetterComponentHandling.NullChecks.NullChecks;

namespace BetterComponentHandling
{
    
    /*
     * Classes for custom monobehaviour extensions
     * These extensions deal with relocating boilerplate code for handling monobehaviour components
    */

    namespace PropertyCaching
    {
        /// <summary>
        /// Extension methods for caching properties.
        /// Cleanly allows an expensive 'find' method to be called once and the value stored for the next call. 
        /// </summary>
        public static class PropertyCaching
        {
            /// <summary>
            /// Returns the value of <paramref name="componentField"/>, if property is null then it is first assigned a value using <see cref="MonoBehaviour.GetComponent{T}()"/> 
            /// </summary>
            /// <inheritdoc cref="FindCachedProperty{T}"/>
            [NotNull]
            public static T GetCachedProperty<T>(this MonoBehaviour monoBehaviour, [CanBeNull] ref T componentField)
                where T : class
                => (IsNotNullUnity(componentField)
                    ? componentField
                    : componentField = monoBehaviour.GetComponentAndAssert<T>())!;

            /// <summary>
            /// Returns the value of <paramref name="componentField"/>, if property is null then it is first assigned a value using <see cref="Object.FindObjectOfType{T}()"/> 
            /// </summary>
            /// <param name="monoBehaviour">This reference</param>
            /// <param name="componentField">The field to hold the component</param>
            /// <typeparam name="T">The type of the component to be found</typeparam>
            /// <returns>The value of the assigned field</returns>
            [NotNull]
            public static T FindCachedProperty<T>(this MonoBehaviour monoBehaviour, [CanBeNull] ref T componentField)
                where T : class
                => (IsNotNullUnity(componentField)
                    ? componentField
                    : componentField = monoBehaviour.FindComponentAndAssert<T>())!;

            /// <summary>
            /// Returns the value of <paramref name="componentField"/>, if property is null then it is first assigned a value using <see cref="MonoBehaviour.GetComponentInChildren{T}()"/> 
            /// </summary>
            /// <inheritdoc cref="FindCachedProperty{T}"/>
            [NotNull]
            public static T GetInChildrenCachedProperty<T>(this MonoBehaviour monoBehaviour,
                [CanBeNull] ref T componentField) where T : class
                => (IsNotNullUnity(componentField)
                    ? componentField
                    : componentField = monoBehaviour.GetComponentInChildrenAndAssert<T>())!;

            /// <summary>
            /// Returns the value of <paramref name="componentField"/>, if property is null then it is first creates a new instance of <typeparamref name="T"/>
            /// </summary>
            /// <param name="monoBehaviour">This reference</param>
            /// <param name="componentField">The field to hold the component</param>
            /// <typeparam name="T">The type of the component to be found</typeparam>
            /// <returns>The value of the assigned field</returns>
            /// <remarks><typeparamref name="T"/> must have a public parameterless constructor.</remarks>
            [NotNull]
            public static T CreateCachedProperty<T>(this MonoBehaviour monoBehaviour,
                [CanBeNull] ref T componentField) where T : class, new()
                => (IsNotNullUnity(componentField)
                    ? componentField
                    : componentField = new T())!;
            
            
            /// <inheritdoc cref="FindCachedProperty{T}"/>
            /// <summary>
            /// Returns the value of <paramref name="componentField"/>, if property is null then it is first assigned a value using <paramref name="GetComponentFunction"/>
            /// </summary>
            /// <param name="monoBehaviour">This reference</param>
            /// <param name="componentField">The field to hold the object</param>
            /// <param name="GetComponentFunction">A function delegate to get the cached object value</param>
            [NotNull]
            public static T CustomGetCachedProperty<T>(this MonoBehaviour monoBehaviour,
                [CanBeNull] ref T componentField,
                Func<T> GetComponentFunction) where T : class
                => (IsNotNullUnity(componentField)
                    ? componentField
                    : componentField = monoBehaviour.CustomGetComponentAndAssert(GetComponentFunction))!;
            
        }
    }


    namespace CleanAssignment
    {
        /// <summary>
        /// Extension methods for cleanly assigning fields within a monobehaviour
        /// </summary>
        public static class CleanAssignment
        {
            /// <summary>
            /// Calls <see cref="UnityEngine.GameObject.GetComponent{T}()"/> and assigns the result to <paramref name="componentField"/>. Throws if component cannot be found.
            /// </summary>
            /// <param name="monoBehaviour">This reference</param>
            /// <param name="componentField">The field to hold the component</param>
            /// <typeparam name="T">The type of the component to be found</typeparam>
            /// <exception cref="NullReferenceException">Throws if component cannot be found.</exception>
            public static void GetComponentAndAssignTo<T>(this MonoBehaviour monoBehaviour, out T componentField)
                where T : class
                => componentField = monoBehaviour.GetComponentAndAssert<T>();

            
            /// <summary>
            /// Calls <see cref="UnityEngine.GameObject.GetComponents{T}()"/> and assigns the result to <paramref name="componentField"/>. Throws if components cannot be found.
            /// </summary>
            /// <inheritdoc cref="GetComponentAndAssignTo{T}(UnityEngine.MonoBehaviour,out T, Func{T})"/>
            public static void GetComponentsAndAssignTo<T>(this MonoBehaviour monoBehaviour, out T[] componentField)
                where T : class
                => componentField = monoBehaviour.GetComponentsAndAssert<T>();


            /// <summary>
            /// Calls <see cref="UnityEngine.GameObject.GetComponentInChildren{T}()"/> and assigns the result to <paramref name="componentField"/>. Throws if components cannot be found.
            /// </summary>
            /// <inheritdoc cref="GetComponentsAndAssignTo{T}(UnityEngine.MonoBehaviour,out T[])"/>
            public static void GetComponentInChildrenAndAssignTo<T>(this MonoBehaviour monoBehaviour,
                out T componentField)
                where T : class
                => componentField = monoBehaviour.GetComponentInChildrenAndAssert<T>();
            
            
            /// <summary>
            /// Calls <see cref="UnityEngine.GameObject.GetComponentsInChildren{T}()"/> and assigns the result to <paramref name="componentField"/>. Throws if components cannot be found.
            /// </summary>
            /// <inheritdoc cref="GetComponentsAndAssignTo{T}(UnityEngine.MonoBehaviour,out T[])"/>
            public static void GetComponentsInChildrenAndAssignTo<T>(this MonoBehaviour monoBehaviour,
                out T[] componentField) where T : class
                => componentField = monoBehaviour.GetComponentsInChildrenAndAssert<T>();
            
            
            /// <summary>
            /// Calls <paramref name="GetComponentFunction"/> and assigns the result to <paramref name="componentField"/>. Throws if component cannot be found.
            /// </summary>
            /// <inheritdoc cref="GetComponentAndAssignTo{T}(UnityEngine.MonoBehaviour,out T)"/>
            public static void GetComponentAndAssignTo<T>(this MonoBehaviour monoBehaviour, out T componentField,
                Func<T> GetComponentFunction) where T : class
                => componentField = monoBehaviour.CustomGetComponentAndAssert(GetComponentFunction);
        }
        
    }

    namespace NullChecks
    {
        /// <summary>
        /// Extension methods for cleanly and safely checking for null in Unity <see cref="UnityEngine.Object"/> classes
        /// </summary>
        public static class NullChecks
        {
            /// <summary>
            /// Checks if <paramref name="componentVariable"/> is actually null.
            /// Throws <see cref="NullReferenceException"/> and debug logs an error if it is.
            /// Does nothing if <paramref name="componentVariable"/> is not null
            /// </summary>
            /// <param name="monoBehaviour">This reference</param>
            /// <param name="componentVariable">The component being checked for null</param>
            /// <typeparam name="T">The type of <paramref name="componentVariable"/></typeparam>
            /// <exception cref="NullReferenceException">Thrown if <paramref name="componentVariable"/> is null</exception>
            public static void AssertNotNull<T>(this Object monoBehaviour, [CanBeNull] T componentVariable)
                where T : class
            {
                if (IsNotNullUnity(componentVariable))
                    return;

                var type = typeof(T);
                var message = $"{type.BaseType} {type.Name} is null in {monoBehaviour.name}";
                Debug.LogError(message, monoBehaviour);
                throw new NullReferenceException(message);
            } 
            
            /// <summary>
            /// Checks if <paramref name="componentVariable"/> is actually null.
            /// Throws <see cref="NullReferenceException"/> and debug logs an error if it is.
            /// Does nothing if <paramref name="componentVariable"/> is not null
            /// </summary>
            /// <param name="monoBehaviour">This reference</param>
            /// <param name="componentVariable">The component being checked for null</param>
            /// <typeparam name="T">The type of <paramref name="componentVariable"/></typeparam>
            /// <exception cref="NullReferenceException">Thrown if <paramref name="componentVariable"/> is null</exception>
            public static void AssertNotNull<T>([CanBeNull] T componentVariable)
                where T : class
            {
                if (IsNotNullUnity(componentVariable))
                    return;

                var type = typeof(T);
                var message = $"{type.BaseType} {type.Name} is null";
                throw new NullReferenceException(message);
            }

            /// <summary>
            /// Checks if <paramref name="componentVariables"/> is null or empty.
            /// Throws <see cref="NullReferenceException"/> and debug logs an error if it is.
            /// Does nothing if <paramref name="componentVariables"/> is not null and not empty
            /// </summary>
            /// <param name="monoBehaviour">This reference</param>
            /// <param name="componentVariables">The array of components being checked</param>
            /// <typeparam name="T">The type of <paramref name="componentVariables"/></typeparam>
            /// <exception cref="NullReferenceException">Thrown if <paramref name="componentVariables"/> is null or empty</exception>
            public static void AssertNotEmpty<T>(this Object monoBehaviour,
                [CanBeNull] IReadOnlyCollection<T> componentVariables) where T : class
            {
                if (componentVariables is {Count: > 0})
                    return;

                var type = typeof(T);
                var message = $"{type.BaseType} {type.Name} is null in {monoBehaviour.name}";
                Debug.LogError(message, monoBehaviour);
                throw new NullReferenceException(message);
            }

           
            /// <param name="_">This reference (not used)</param>
            /// <inheritdoc cref="IsNotNullUnity{T}(T)"/>
            // ReSharper disable once InvalidXmlDocComment
            public static bool IsNotNullUnity<T>(this Object _, [CanBeNull] T possibleUnityObject)
                where T : class
                => IsNotNullUnity(possibleUnityObject);
            
            
            /// <summary>
            /// Returns if <paramref name="possibleUnityObject"/> is actually null
            /// </summary>
            /// <param name="possibleUnityObject">Object being checked for null</param>
            /// <typeparam name="T">The type of <paramref name="possibleUnityObject"/></typeparam>
            /// <returns><c>true</c> if <paramref name="possibleUnityObject"/> is not null.
            /// <c>false</c> otherwise.</returns>
            public static bool IsNotNullUnity<T>([CanBeNull] T possibleUnityObject) where T : class
            {
                if (possibleUnityObject is UnityEngine.Object unityObject)
                {
                    return unityObject != null && !unityObject.Equals(null);
                }
                
                return possibleUnityObject != null && !possibleUnityObject.Equals(null);
            }
        }
    }

    namespace Internal
    {
        internal static class BetterComponentHandlingInternal
        {
            [NotNull]
            internal static T GetComponentAndAssert<T>(this MonoBehaviour monoBehaviour) where T : class
                => monoBehaviour.SafeGetComponentAndAssert<T>(monoBehaviour.GetComponents<MonoBehaviour>);   
            
            [NotNull]
            internal static T[] GetComponentsAndAssert<T>(this MonoBehaviour monoBehaviour) where T : class
                => monoBehaviour.SafeGetComponentsAndAssert<T>(monoBehaviour.GetComponents<MonoBehaviour>);

            [NotNull]
            internal static T GetComponentInChildrenAndAssert<T>(this MonoBehaviour monoBehaviour) where T : class
                => monoBehaviour.SafeGetComponentAndAssert<T>(monoBehaviour.GetComponentsInChildren<MonoBehaviour>); 
            
            [NotNull]
            internal static T[] GetComponentsInChildrenAndAssert<T>(this MonoBehaviour monoBehaviour) where T : class
                => monoBehaviour.SafeGetComponentsAndAssert<T>(monoBehaviour.GetComponentsInChildren<MonoBehaviour>);
            
            [NotNull]
            internal static T FindComponentAndAssert<T>(this MonoBehaviour monoBehaviour) where T : class
                =>  monoBehaviour.SafeGetComponentAndAssert<T>(Object.FindObjectsOfType<MonoBehaviour>);
            
            // This isn't great but really isn't that bad, only way to make it work finding an interface (see more: https://stackoverflow.com/q/49329764/7711148)
            [NotNull]
            private static T SafeGetComponentAndAssert<T>(this MonoBehaviour monoBehaviour,
                [NotNull] Func<MonoBehaviour[]> GetComponentFunction) where T : class
                => monoBehaviour.CustomGetComponentAndAssert(
                    GetComponentFunction: GetComponentFunction()
                        .OfType<T>()
                        .FirstOrDefault); 
            
            [NotNull]
            private static T[] SafeGetComponentsAndAssert<T>(this MonoBehaviour monoBehaviour,
                [NotNull] Func<MonoBehaviour[]> GetComponentFunction) where T : class
                => monoBehaviour.CustomGetComponentsAndAssert(
                    GetComponentFunction: GetComponentFunction()
                        .OfType<T>()
                        .ToArray);

            [NotNull]
            public static T CustomGetComponentAndAssert<T>(this MonoBehaviour monoBehaviour,
                [NotNull] Func<T> GetComponentFunction)
                where T : class
            {
                var componentVariable = GetComponentFunction();

                monoBehaviour.AssertNotNull(componentVariable);

                return componentVariable;
            }

            [NotNull]
            public static T[] CustomGetComponentsAndAssert<T>(this MonoBehaviour monoBehaviour,
                [NotNull] Func<T[]> GetComponentFunction) where T : class
            {
                var componentVariable = GetComponentFunction();

                monoBehaviour.AssertNotEmpty(componentVariable);

                return componentVariable;
            }
        }
    }
}
