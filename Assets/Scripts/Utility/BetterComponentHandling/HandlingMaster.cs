using System;
using System.Collections.Generic;
using BetterComponentHandling.CleanAssignment;
using BetterComponentHandling.PropertyCaching;
using JetBrains.Annotations;
using UnityEngine;
using static BetterComponentHandling.NullChecks.NullChecks;

namespace BetterComponentHandling
{
    public static class HandlingMaster
    {
        [NotNull]
        public static T GetCachedComponent<T>(this MonoBehaviour monoBehaviour,
            [CanBeNull] ref T componentField,
            Func<T> getComponentFunction,
            bool overwriteValue = false) where T : class
        {
            if (!overwriteValue)
            {
                return monoBehaviour.CustomGetCachedProperty(ref componentField, getComponentFunction);
            }

            monoBehaviour.GetComponentAndAssignTo(out componentField, getComponentFunction);
            return componentField;
        }   
        
        [NotNull]
        public static void GetComponentBetter<T>(this MonoBehaviour monoBehaviour,
            [CanBeNull] out T componentField,
            Func<T> getComponentFunction) where T : class
        {
            monoBehaviour.GetComponentAndAssignTo(out componentField, getComponentFunction);
        }  
        
      
        
    }

    
 

    public static class BetterAssignment
    {

       
        
        public interface IGetComponentFunctionStep<T> where T : Component
        {
         T AssignValue(out T componentField);
         T AssignValueIfNull(ref T componentField);
         Func<T> GetComponentFunction { get; }
        }

   

       

        [NotNull]
        public static IGetComponentFunctionStep<T> WithFunction<T>(this MonoBehaviour _,
            [NotNull] Func<T> getComponentFunction) where T : Component
            => new GetComponentFunctionStep<T>(getComponentFunction);


        

        public static void AssignTo<T>(this IGetComponentFunctionStep<T> componentValue,
            out T componentField) where T : Component
            => componentValue.AssignValue(out componentField);

        public static void AssignToIfNull<T>(this IGetComponentFunctionStep<T> componentValue,
            [CanBeNull] ref T componentField) where T : Component
            => componentValue.AssignValueIfNull(ref componentField);

        public static T CacheAndReturnValue<T>(this IGetComponentFunctionStep<T> componentValue,
            [CanBeNull] ref T componentField) where T : Component
            => componentValue.AssignValueIfNull(ref componentField);

        public static T ReturnValue<T>(this IGetComponentFunctionStep<T> componentValue) where T : Component 
            => componentValue.GetComponentFunction();


        private class GetComponentFunctionStep<T> :  IGetComponentFunctionStep<T> where T : Component
        {
            public Func<T> GetComponentFunction { get; }
            
            public GetComponentFunctionStep(Func<T> getComponentFunction)
            {
                GetComponentFunction = getComponentFunction;
            }

            public T AssignValue(out T componentField)
            {
                return AssignAndAssetNotNull(out componentField);
            }

            public T AssignValueIfNull([NotNull] ref T componentField)
            {
                return IsNotNullUnity(componentField) 
                    ? componentField 
                    : AssignAndAssetNotNull(out componentField);
            }

            private T AssignAndAssetNotNull(out T componentField)
            {
                componentField = GetComponentFunction();
                AssertNotNull(componentField);
                return componentField;
            }
        }
        
    }

  
}