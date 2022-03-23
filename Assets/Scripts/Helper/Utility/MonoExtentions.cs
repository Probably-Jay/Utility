using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public static class MonoExtentions
{
    /// <summary>
    /// Calls <see cref="GameObject.GetComponent{T}"/> and assigns the result to <paramref name="componentVariable"/>. Debug logs if component cannot be found.
    /// </summary>
    /// <param name="componentVariable">The variable to hold the component</param>
    /// <typeparam name="T">The type of the component to be found</typeparam>
    /// <exception cref="NullReferenceException">Throws if component cannot be found.</exception>
    public static void GetComponentAndAssignTo<T>(this MonoBehaviour gameObject, out T componentVariable) where T : Component
    {     
        componentVariable = gameObject.GetComponent<T>();
        
        if(componentVariable == null || componentVariable.Equals(null))
        {
            var type = typeof(T);
            Debug.LogError($"{type.BaseType} {type.Name} is null in {gameObject.name}", gameObject);
            throw new NullReferenceException();
        }
    }        
    
    /// <summary>
    /// Calls <see cref="GameObject.GetComponentInChildren{T}()"/> and assigns the result to <paramref name="componentVariable"/>. Debug logs if component cannot be found.
    /// </summary>
    /// <param name="componentVariable">The variable to hold the component</param>
    /// <typeparam name="T">The type of the component to be found</typeparam>
    /// <exception cref="NullReferenceException">Throws if component cannot be found.</exception>
    public static void GetComponentInChildrenAndAssignTo<T>(this MonoBehaviour gameObject, out T componentVariable) where T : Component
    {     
        componentVariable = gameObject.GetComponentInChildren<T>();
        
        if(componentVariable == null || componentVariable.Equals(null))
        {
            var type = typeof(T);
            Debug.LogError($"{type.BaseType} {type.Name} is null in {gameObject.name}", gameObject);
            throw new NullReferenceException();
        }
    }
}
