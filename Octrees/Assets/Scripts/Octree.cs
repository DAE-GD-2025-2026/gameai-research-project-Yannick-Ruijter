using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Octrees {
    public class Octree {
        public OctreeNode Root;
        public Bounds Bounds;
        private List<OctreeNode> _emptyLeafNodes = new();
        public Graph graph;
        private float _minNodeSize;
        public Octree(List<GameObject> objects, float minNodeSize, Graph graph) {
            _minNodeSize = minNodeSize;
            this.graph = graph;
            CalculateBounds(objects);
            CreateTree(objects, minNodeSize);
            GetEmptyLeaves(Root);
            GetEdges();
            Debug.Log(graph.edges.Count);
        }

        private void GetEdges()
        {
            graph.edges.Clear();
            for (int i = 0; i < _emptyLeafNodes.Count; i++)
            {
                for (int j = i + 1; j < _emptyLeafNodes.Count; j++)
                {
                    if (_emptyLeafNodes[i].bounds.Intersects(_emptyLeafNodes[j].bounds))
                        graph.AddEdge(_emptyLeafNodes[i], _emptyLeafNodes[j]);
                }
            }
        }

        private void GetEmptyLeaves(OctreeNode node) {
            if (node.IsLeaf && node.Objects.Count == 0)
            {
                _emptyLeafNodes.Add(node);
                graph.AddNode(node);
                return;
            }

            if (node.children == null) return;

            foreach (var child in node.children)
            {
                GetEmptyLeaves(child);
            }
        }

        private void CreateTree(List<GameObject> objects, float minNodeSize) {
            Root = new OctreeNode(Bounds, minNodeSize);
            foreach (var obj in objects) 
            {
                Root.Divide(obj);
            }
        }

        void CalculateBounds(List<GameObject> objects) {
            foreach (var obj in objects) {
                Bounds.Encapsulate(obj.GetComponent<Collider>().bounds);
            }

            Vector3 size = Vector3.one * Mathf.Max(Bounds.size.x, Bounds.size.y, Bounds.size.z) * 0.6f;
            Bounds.SetMinMax(Bounds.center - size, Bounds.center + size);
        }
        
        public void RemoveObject(OctreeObject obj)
        {
            if(!Root.ObjectsInChildren.Contains(obj))
            {
                Debug.LogError("This octreeobject is part of the current octree!");
                return;
            }
            Root.RemoveObject(obj);
            Root = Root.TryCollapsing();
            graph.nodes.Clear();
            GetEmptyLeaves(Root);
            GetEdges();
        }

        public void AddObject(OctreeObject obj)
        {
            var bounds = obj.bounds;
            //check if we need to expand the octree
            if (Root.bounds.Contains(bounds.min) && Root.bounds.Contains(bounds.max))
            {
                Root.Divide(obj);
            }
            else
            {
                //if at least something is inside the root, we divide
                if (Root.bounds.Contains(bounds.min) || Root.bounds.Contains(bounds.max))
                    Root.Divide(obj);

                while(!(Root.bounds.Contains(bounds.min) && Root.bounds.Contains(bounds.max)))
                //we see what part is outside of the current octree
                {
                    var outOfBoundsPos = Root.bounds.Contains(bounds.min) ? bounds.max : bounds.min;
                    OctreeNode[] newChildren = new OctreeNode[8];
                    newChildren[0] = Root;

                    Vector3 offsets = new();
                    offsets.x = outOfBoundsPos.x < Root.bounds.center.x ? -Root.bounds.size.x : Root.bounds.size.x;
                    offsets.y = outOfBoundsPos.y < Root.bounds.center.y ? -Root.bounds.size.y : Root.bounds.size.y;
                    offsets.z = outOfBoundsPos.z < Root.bounds.center.z ? -Root.bounds.size.z : Root.bounds.size.z;
                    for (int i = 1; i < 8; i++)
                    {
                        Vector3 currentCenter = new();
                        currentCenter.x = Root.bounds.center.x + ((i & 1) == 0 ? 0 : offsets.x);
                        currentCenter.y = Root.bounds.center.y + ((i & 2) == 0 ? 0 : offsets.y);
                        currentCenter.z = Root.bounds.center.z + ((i & 4) == 0 ? 0 : offsets.z);

                        Bounds currentBounds = new(currentCenter, Root.bounds.size);
                        newChildren[i] = new OctreeNode(currentBounds, _minNodeSize);
                        //we divide it if needed
                        if (newChildren[i].bounds.Intersects(bounds))
                            newChildren[i].Divide(obj); 
                    }

                    //don't need to call divide on this since this already hasd  the 8 children in which divide would split it 
                    //and i'm already only dividing the children if needed
                    Root = new OctreeNode(newChildren, _minNodeSize);
                }
            }

            graph.nodes.Clear();
            GetEmptyLeaves(Root);
            GetEdges();
        }
    }
}
