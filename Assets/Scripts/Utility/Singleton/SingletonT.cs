using System;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SingletonT
{
    /// <summary>
    /// Singleton base class. <see cref="T"/> must be the type of the child inheriting from this class.
    /// </summary>
    /// <typeparam name="T">The class *inheriting* from this class that will be a singleton</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;
    
        /// <summary>
        /// The <see cref="Singleton{T}"/> instance of this class.
        /// </summary>
        protected static T Instance
        {
            get
            {
                if (!InstanceExists)
                {
                    TryInitialiseSingleton();
                }
                return instance;
            }
        }

        /// <summary>
        /// Returns <see cref="Instance"/> if one exists, returns null otherwise
        /// </summary>
        [CanBeNull] protected static T TryInstance => InstanceExists ? Instance : null;

        /// <summary>
        /// If an instance of this singleton currently exists
        /// </summary>
        public static bool InstanceExists => instance != null;
    
        /// <summary>
        /// Finds all singletons to ensure theres only one, assigns us to it, dontDestroysUs
        /// </summary>
        private static void TryInitialiseSingleton()
        {
            var singleton = FindSingleton();
            SetSingleton(singleton);
            DontDestroyOnLoad(singleton.transform.root.gameObject);
        }

        /// <summary>
        /// Finds the singletons in the scene, if only one exists then that's us so return it, otherwise throw
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Singleton{T}.SingletonDoesNotExistException"></exception>
        /// <exception cref="Singleton{T}.MultipleSingletonInSceneException"></exception>
        private static T FindSingleton()
        {
            var singletons = FindObjectsOfType<T>();

            return singletons.Length switch
            {
                0 => throw new SingletonDoesNotExistException(),
                1 => singletons.First(),
                _ => throw new MultipleSingletonInSceneException(singletons)
            };
        }

        /// <summary>
        /// Set the found singleton as the instance
        /// </summary>
        /// <param name="thisSingleton">Reference to the found object</param>
        private static void SetSingleton([NotNull]T thisSingleton)
        {
            if (thisSingleton == null) 
                throw new ArgumentNullException(nameof(thisSingleton), "Cannot set singleton as singleton may not exist");
        
            if (thisSingleton.GetType() != typeof(T)) // this should never happen
                throw new Exception(
                    $"Singletons can only reference their own types, {typeof(Singleton<T>)} cannot be used to template typeof {thisSingleton.GetType()}");

            if (instance != null && instance != thisSingleton)
                throw new MultipleSingletonInSceneException(thisSingleton, instance);

            instance = thisSingleton;
        }

    
        [Serializable]
        protected class SingletonDoesNotExistException : Exception
        {
            public static string DoesNotExistMessage => $"{typeof(Singleton<T>)} is required by a script, but does not exist in scene \"{SceneManager.GetActiveScene().name}\".";
            public SingletonDoesNotExistException():base(DoesNotExistMessage) { }
        } 
    
        [Serializable]
        protected class MultipleSingletonInSceneException : Exception
        {
            public static string MultipleInSceneMessage(IEnumerable<T> singletons)
            {
                var message =
                    $"{typeof(Singleton<T>)} is a singleton, but multiple copies exist in the scene {SceneManager.GetActiveScene().name}: ";
                var singletonNames = string.Join(",", singletons.Select(s => s.name));
                return message +  singletonNames;
            }

            public MultipleSingletonInSceneException(params T[] singletons):base(MultipleInSceneMessage(singletons)) { }
        }
    }

    // /// <summary>
    // /// <see cref="Singleton{T}"/> who's <see cref="Instance"/> is public by default
    // /// </summary>
    // public abstract class PublicSingleton<T> : Singleton<T> where T : PublicSingleton<T>
    // {
    //     public new static T Instance => Singleton<T>.Instance;
    //     [CanBeNull] public new static T TryInstance => Singleton<T>.TryInstance;
    // }
}

