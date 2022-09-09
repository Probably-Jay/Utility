using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoroutineExtensions
{
    /// <summary>
    /// Wraps a coroutine so can be queried if is still active
    /// </summary>
    public class WrappedCoroutine
    {
        private readonly MonoBehaviour monoBehaviour;
        private readonly Func<IEnumerator> enumeratorFunctionDelegate;
        private Coroutine outerCoroutine;
        private Coroutine innerCoroutine;
        
        /// <summary>
        /// If the coroutine has finished execution
        /// </summary>
        public bool Complete { get; private set; }

        /// <summary>
        /// Creates a new <see cref="WrappedCoroutine"/>.
        /// </summary>
        /// <param name="monoBehaviour">The monobehaviour that will run the coroutine</param>
        /// <param name="enumeratorFunctionDelegate">A delegate which return the function call to be ran as a coroutine</param>
        /// <returns>The created <see cref="WrappedCoroutine"/></returns>
        /// <seealso cref="CoroutineWrapperExtension.StartWrappedCoroutine"/>
        public static WrappedCoroutine CreateAndStart(MonoBehaviour monoBehaviour, Func<IEnumerator> enumeratorFunctionDelegate) 
            => new WrappedCoroutine(monoBehaviour, enumeratorFunctionDelegate);

        private WrappedCoroutine(MonoBehaviour monoBehaviour, Func<IEnumerator> enumeratorFunctionDelegate)
        {
            this.monoBehaviour = monoBehaviour;
            this.enumeratorFunctionDelegate = enumeratorFunctionDelegate;
            
            Complete = false;
            StartCoroutine();
        }

        private void StartCoroutine()
        {
            outerCoroutine = monoBehaviour.StartCoroutine(WrappedCoroutineCall());
        }

        private IEnumerator WrappedCoroutineCall()
        {
            var enumeratorFunction = enumeratorFunctionDelegate();
            innerCoroutine = monoBehaviour.StartCoroutine(enumeratorFunction);
            
            yield return innerCoroutine;
            Complete = true;
        }

        public void StopCoroutine()
        {
            if(outerCoroutine != null)
                monoBehaviour.StopCoroutine(outerCoroutine);
            if (innerCoroutine != null)
                monoBehaviour.StopCoroutine(innerCoroutine);
            Complete = true;
        }
        
    }
    
    public static class CoroutineWrapperExtension
    {
        /// <summary>
        /// Starts a <see cref="WrappedCoroutine"/>, which can be queried if it has finished or not
        /// </summary>
        /// <example>
        /// The call
        ///     <code>monoBehaviour.StartCoroutine(CoroutineFunction());</code>
        /// Would become
        ///     <code>monoBehaviour.StartWrappedCoroutine(() => CoroutineFunction())</code>
        /// </example>
        public static WrappedCoroutine StartWrappedCoroutine(this MonoBehaviour monoBehaviour, Func<IEnumerator> enumeratorFunctionDelegate) 
            => WrappedCoroutine.CreateAndStart(monoBehaviour, enumeratorFunctionDelegate);

        public static bool AllCoroutinesAreComplete(this IEnumerable<WrappedCoroutine> coroutineWrapperContainer) 
            => coroutineWrapperContainer.All(c => c.Complete); 
        
        public static IEnumerator WaitUntilAllCoroutinesAreComplete(this IEnumerable<WrappedCoroutine> coroutineWrapperContainer)
        {
            yield return new WaitUntil(coroutineWrapperContainer.AllCoroutinesAreComplete);
        }
    }
}