using Cindy.Logic.ReferenceValues;
using CindyRun.Util;
using System.Collections.Generic;
using UnityEngine;

namespace CindyRun.Game
{
    /// <summary>
    /// 路径生成器
    /// </summary>
    public class PathGenerator : MonoBehaviour
    {
        public Transform target;

        public ReferenceFloat distance;

        public PathNode[] templates;

        public PathNode[] endOfRoads;

        protected IObjectPool<PathNode> pool;

        public IList<PathNode> Nodes { get { return pool.GetActivedInstances(); } }
        
        Vector3 o;

        Way way = Way.Forward;

        protected PathNode RandomRoadNode
        {
            get
            {
                float weightSum = 0;
                float[] map = new float[templates.Length];
                int i = 0;
                foreach (PathNode node in templates)
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
        }

        protected PathNode RandomEndOfRoad
        {
            get
            {
                if(endOfRoads.Length > 0)
                {
                    return endOfRoads[Random.Range(0, endOfRoads.Length)];
                }
                else
                {
                    return null;
                }
            }
        }

        private void Awake()
        {
            pool = new SimpleObjectPool<PathNode>();
        }
        
        public void Generate(int count)
        {
            for (int i = 0; i < count; i++)
                Generate();
        }

        public void Generate()
        {
            if(Nodes.Count > 0)
            {
                for(int i = Nodes.Count - 1;i >= 0; i--)
                {
                    if (Nodes[i] is ForkNode f)
                    {
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
            PathNode node = pool.Instantiate(RandomRoadNode, transform);
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
            for(int i = 0;i < Nodes.Count; i++)
            {
                if ((Nodes[i].transform.position - target.transform.position).magnitude > distance.value)
                    garbage.Add(Nodes[i]);
                else
                    break;
            }
            foreach (PathNode g in garbage)
                pool.Recycle(g);
            while (Nodes.Count == 0 || (target.transform.position - Nodes[Nodes.Count - 1].transform.position).magnitude <= distance.Value)
                Generate();
        }

        protected enum Way
        {
            Forward,
            Right,
            Left
        }
    }

}
