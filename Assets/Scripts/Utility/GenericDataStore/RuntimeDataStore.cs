using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Game
{
    public interface IGameManagerRuntimeDataStore
    {
        void Remember<T>([NotNull] string name, [NotNull] T data);
        [NotNull] T RecallOrThrow<T>([NotNull] string name);
        [CanBeNull] RuntimeDataStore.Data<T> TryRecall<T>([NotNull] string name);
        void Forget([NotNull] string name);
    }

    /// <summary>
    /// Helper to temporarily store data
    /// </summary>
    public class RuntimeDataStore : IGameManagerRuntimeDataStore
    {
        private readonly Dictionary<string, IData> storedData = new();

        public void Remember<T>(string name, T data)
        {
            storedData[name] = new Data<T>(data);
        }

        public T RecallOrThrow<T>(string name)
        {
            var data = TryRecall<T>(name);
            return data != null
                ? data.Value
                : throw new CouldNotRecallException<T>(name);
        } 
        
        public Data<T> TryRecall<T>(string name)
        {
            if (!storedData.ContainsKey(name))
                return null;
            
            var data = storedData[name];

            return data as Data<T>;
        }
        
        public void Forget(string name) => storedData.Remove(name);
        
        public class Data<T> : IData
        {
            public Data([NotNull] T data)
            {
                Value = data;
            }
            
            public readonly T Value;
            public Type Type => typeof(T);
        }

        private interface IData
        { }

        private class CouldNotRecallException<T> : Exception
        {
            public CouldNotRecallException(string name) 
                : base($"Data \"{name}\" of type {typeof(T)} could not be recalled.")
            { } 
        }
    }
}