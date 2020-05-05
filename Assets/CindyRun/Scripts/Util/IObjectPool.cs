using System.Collections.Generic;
using UnityEngine;

namespace CindyRun.Util
{
    public interface IObjectPool<T> where T : Object
    {
        T Instantiate(T template, Transform transform);

        void Recycle(T obj);

        IList<T> GetActivedInstances();
    }
}