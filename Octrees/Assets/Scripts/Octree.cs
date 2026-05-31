using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Octrees {
    public class Octree {
        public OctreeNode Root;
        public Bounds Bounds;
        private HashSet<OctreeNode> _emptyLeafNodes = new();
        public Graph graph;
        private HashSet<OctreeNode> _lastLeafNodesFound = new();
        private float _minNodeSize;
        public Octree(List<GameObject> objects, float minNodeSize, Graph graph) {
            _minNodeSize = minNodeSize;
            this.graph = graph;
            CalculateBounds(objects);
            CreateTree(objects, minNodeSize);
            _lastLeafNodesFound.Clear();
            GetEmptyLeaves(Root);
            graph.edges.Clear();
            GetEdges(_emptyLeafNodes);
            Debug.Log(graph.edges.Count);
        }

        //could be more efficient i know
        private void GetEdges(HashSet<OctreeNode> nodes)
        {
            foreach (var node in nodes)
            {
                foreach (var otherNode in _emptyLeafNodes)
                {
                    if (node == otherNode) continue;

                    Bounds expanded = node.bounds;
                    expanded.Expand(0.01f);

                    if (expanded.Intersects(otherNode.bounds))
                        graph.AddEdge(node, otherNode);
                }
            }
        }

        private void GetEmptyLeaves(OctreeNode node) {
            if (node.IsLeaf && node.Objects.Count == 0)
            {
                _emptyLeafNodes.Add(node);
                graph.AddNode(node);
                _lastLeafNodesFound.Add(node);
                return;
            }

            if (node.children == null) return;

            foreach (var child in node.children)
            {
                //probably never happens but once again for my sanity
                if (child == null) continue;
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
            if(obj.ParentNodes.Count == 0)
            {
                Debug.LogError("This octreeobject is not part of any octants!");
                return;
            }

            foreach (var node in obj.ParentNodes)
            {
                graph.RemoveConnectedEdges(node);
                graph.nodes.Remove(node);
                node.RemoveObject(obj);
            }
            foreach (var node in obj.ParentNodes)
            {
                if(node.ParentNode != null)
                    node.ParentNode.TryCollapse();
            }
            obj.ParentNodes.Clear();
            Root = Root.TryShrinking();
            var smallestContainingNode = GetSmallestContainingNode(obj);
            _lastLeafNodesFound.Clear();
            GetEmptyLeaves(smallestContainingNode);
            GetEdges(_lastLeafNodesFound);
        }

        private OctreeNode GetSmallestContainingNode(OctreeObject obj)
        {
            var current = Root;
            var previous = current;
            while(true)
            {
                previous = current;
                //if there are no children, we are already at the smallest possible node for this region
                if (current.children == null) break;

                //go over all the children and check which one fully contains the object
                for(int i = 0; i < 8; i++)
                {
                    //normally shjould not happen but for my sanity
                    if (current.children[i] == null) continue;

                    if (current.children[i].bounds.Contains(obj.bounds.min) && current.children[i].bounds.Contains(obj.bounds.max))
                    {
                        current = current.children[i]; 
                        break;
                    }
                }

                //if we have not found a child octant that contains the entire object, we break
                if(previous == current) break;
            }

            return current;
        }

        //not completely optimized, could do better by only recalculating the leafs and edges for the parts that got changed
        public void AddObject(OctreeObject obj)
        {
            var bounds = obj.bounds;
            //check if we need to expand the octree
            if (Root.bounds.Contains(bounds.min) && Root.bounds.Contains(bounds.max))
            {
                var smallestContainingNode = GetSmallestContainingNode(obj);
                smallestContainingNode.Divide(obj);
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
            graph.edges.Clear();
            _lastLeafNodesFound.Clear();
            _emptyLeafNodes.Clear();
            GetEmptyLeaves(Root);
            GetEdges(_emptyLeafNodes);
        }
    }
}
