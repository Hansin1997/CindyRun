using UnityEngine;
using System.Collections.Generic;

namespace CindyRun.Util
{
    public sealed class SimpleObjectPool<T> : IObjectPool<T> where T : Component
    {
        private List<T> objects;

        private Dictionary<T, List<T>> garbage;
        private Dictionary<T, T> templateMap;

        public SimpleObjectPool()
        {
            objects = new List<T>();
            garbage = new Dictionary<T, List<T>>();
            templateMap = new Dictionary<T, T>();
        }

        public IList<T> GetActivedInstances()
        {
            return objects;
        }

        public T Instantiate(T template,Transform transform)
        {
            if (!garbage.ContainsKey(template))
                garbage[template] = new List<T>();
            List<T> g = garbage[template];
            if (g.Count == 0)
            {
                T r = Object.Instantiate(template, transform);
                templateMap[r] = template;
                objects.Add(r);
                return r;
            }
            else
            {
                T r = g[0];
                r.gameObject.transform.position = template.transform.position;
                r.gameObject.transform.eulerAngles = template.transform.eulerAngles;
                r.gameObject.transform.localScale = template.transform.localScale;
                r.gameObject.SetActive(true);
                g.RemoveAt(0);
                objects.Add(r);
                return r;
            }
        }

        public void Recycle(T obj)
        {
            if (objects.Contains(obj))
            {
                objects.Remove(obj);
                obj.gameObject.SetActive(false);
                garbage[templateMap[obj]].Add(obj);
            }
        }
    }
}
