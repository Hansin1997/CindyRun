using Cindy.Logic.ReferenceValues;
using Cindy.Util.Serializables;
using CindyRun.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CindyRun.Road
{
    /// <summary>
    /// 路径生成器
    /// </summary>
    public class PathGenerator : MonoBehaviour
    {
        public Transform target;

        public ReferenceVector3 v;

        public ReferenceFloat distance;

        public PathNode[] templates;
        public bool allowMirrors = true;
        public PathNode[] Objects;
        public AnimationCurve function = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        public EndOfRoad[] endOfRoads;

        protected IObjectPool<PathNode> pool;

        public IList<PathNode> Nodes { get { return pool.GetActivedInstances(); } }

        
        Vector3 o;

        Way way = Way.Forward;

        SerializedTransform targetOrigin;

        public ReferenceFloat D;

        protected PathNode RandomRoadNode { get { return RandomNode(templates); } }

        protected EndOfRoad RandomEndOfRoad { get { return RandomNode(endOfRoads); } }

        protected PathNode RandomObjects { get { return RandomNode(Objects); } }

        public UnityEvent resetEvent;

        protected T RandomNode<T>(T[] templates) where T : PathNode
        {
            float weightSum = 0;
            float[] map = new float[templates.Length];
            int i = 0;
            foreach (T node in templates)
            {
                map[i++] = weightSum;
                weightSum += node.weight;
            }
            float r = Random.Range(0, weightSum);
            for (i = 0; i < map.Length - 1; i++)
            {
                if (r >= map[i] && r <= map[i + 1])
                    return templates[i];
            }
            return templates.Length > 0 ? templates[templates.Length - 1] : null;
        }

        private void Awake()
        {
            pool = new SimpleObjectPool<PathNode>();
            targetOrigin = new SerializedTransform(target);
        }

        public void DoReset()
        {
            List<PathNode> tmp = new List<PathNode>();
            foreach(PathNode pathNode in Nodes)
            {
                tmp.Add(pathNode);
            }
            foreach (PathNode node in tmp)
                pool.Recycle(node);
            o = Vector3.zero;
            way = Way.Forward;
            targetOrigin.SetTransform(target);
            D.Value = 0;
            resetEvent.Invoke();
        }

        public void Generate(int count)
        {
            for (int i = 0; i < count; i++)
                Generate();
        }

        public void Generate()
        {
            bool fork = Nodes.Count == 0;
            if (Nodes.Count > 0)
            {
                for (int i = Nodes.Count - 1; i >= 0; i--)
                {
                    if (Nodes[i] is ForkNode f)
                    {
                        fork = true;
                        switch (way)
                        {
                            default:
                            case Way.Forward:
                                bool r = Random.Range(0f, 1f) >= 0.5;
                                way = r ? Way.Right : Way.Left;
                                float offsetX = Nodes[Nodes.Count - 1].width / 2 * (r ? 1 : -1);
                                float offsetY = -Nodes[Nodes.Count - 1].height / 2;
                                o += new Vector3(offsetX, 0, offsetY);
                                FillEndOfRoad(f, true, r, !r);
                                break;
                            case Way.Left:
                                way = Way.Forward;
                                o += new Vector3(Nodes[Nodes.Count - 1].height / 2, 0, Nodes[Nodes.Count - 1].width / 2);
                                FillEndOfRoad(f, true, true, false);
                                break;
                            case Way.Right:
                                way = Way.Forward;
                                o += new Vector3(-Nodes[Nodes.Count - 1].height / 2, 0, Nodes[Nodes.Count - 1].width / 2);
                                FillEndOfRoad(f, true, false, true);
                                break;
                        }
                    }
                    if (!(Nodes[i] is EndOfRoad))
                        break;
                }
            }
            int count = 0;
            PathNode t = RandomRoadNode;
            while (fork && t is ForkNode)
            {
                t = RandomRoadNode;
                count++;
                if (count > templates.Length && count > 2000)
                {
                    Debug.LogError("Can't find a road!");
                    break;
                }
            }

            PathNode node = pool.Instantiate(t, transform);
            if (allowMirrors && Random.Range(0f, 1f) >= 0.5f && !(node is ForkNode))
                node.transform.localScale = new Vector3(-node.transform.localScale.x, node.transform.localScale.y, node.transform.localScale.z);
            D.Value += node.height;
            switch (way)
            {
                default:
                case Way.Forward:
                    o += new Vector3(0, 0, node.height / 2);
                    node.transform.position = transform.position + o;
                    o += new Vector3(0, 0, node.height / 2);
                    break;
                case Way.Left:
                    node.transform.Rotate(Vector3.up * -90);

                    o -= new Vector3(node.height / 2, 0, 0);
                    node.transform.position = transform.position + o;
                    o -= new Vector3(node.height / 2, 0, 0);
                    break;
                case Way.Right:
                    node.transform.Rotate(Vector3.up * 90);

                    o += new Vector3(node.height / 2, 0, 0);
                    node.transform.position = transform.position + o;
                    o += new Vector3(node.height / 2, 0, 0);
                    break;
            }
            if(!(node is ForkNode || node is EndOfRoad))
            {
                int len = (int)function.Evaluate(D.Value);
                float unit = node.height / len;
                for (int i = 0; i < len; i++)
                {
                    PathNode ins = pool.Instantiate(RandomObjects, node.transform);
                    ins.transform.position += node.transform.forward * (-node.height / 2 + i * unit) +
                        node.transform.right * (-node.width / 2 + Random.Range(0, node.width));
                    ins.transform.SetParent(transform);
                }
            }
        }

        protected void FillEndOfRoad(ForkNode parent, bool forward, bool left, bool right)
        {
            if (forward)
            {
                PathNode e = RandomEndOfRoad;
                if (e == null || parent.forward == null)
                    return;
                PathNode instance = pool.Instantiate(e, parent.forward);
                instance.transform.position += instance.transform.forward * instance.height / 2;
                instance.transform.SetParent(transform,true);
            }
            if (left)
            {
                PathNode e = RandomEndOfRoad;
                if (e == null || parent.left == null)
                    return;
                PathNode instance = pool.Instantiate(e, parent.left);
                instance.transform.position += instance.transform.forward * instance.height / 2;
                instance.transform.SetParent(transform, true);
            }
            if (right)
            {
                PathNode e = RandomEndOfRoad;
                if (e == null || parent.right == null)
                    return;
                PathNode instance = pool.Instantiate(e, parent.right);
                instance.transform.position += instance.transform.forward * instance.height / 2;
                instance.transform.SetParent(transform, true);
            }
        }

        public void Delete()
        {
            if (Nodes.Count > 0)
                pool.Recycle(Nodes[0]);
        }

        public void Delete(int count)
        {
            for(int i = 0;i < count && Nodes.Count > 0; i++)
            {
                pool.Recycle(Nodes[0]);
            }
        }

        private void Update()
        {
            List<PathNode> garbage = new List<PathNode>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if ((Nodes[i].transform.position - target.transform.position).magnitude > distance.value)
                    garbage.Add(Nodes[i]);
                else
                    break;
            }
            foreach (PathNode g in garbage)
                pool.Recycle(g);
            int c = 0;
            while (Nodes.Count == 0 || (target.transform.position - Nodes[Nodes.Count - 1].transform.position).magnitude <= distance.Value)
            {
                c++;
                Generate();
                if(c > 2000)
                {
                    Debug.LogError("Timeout!");
                }
            }
        }

        protected enum Way
        {
            Forward,
            Right,
            Left
        }
        
    }

}
