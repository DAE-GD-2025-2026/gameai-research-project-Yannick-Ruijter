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
        public Octree(List<GameObject> objects, float minNodeSize, Graph graph) {
            this.graph = graph;
            CalculateBounds(objects);
            CreateTree(objects, minNodeSize);
            GetEmptyLeaves(Root);
            GetEdges();
            Debug.Log(graph.edges.Count);
        }

        private void GetEdges()
        {
            foreach (var leafNode in _emptyLeafNodes)
            {
                foreach (var otherLeafNode in _emptyLeafNodes)
                {
                    if (leafNode != otherLeafNode && leafNode.bounds.Intersects(otherLeafNode.bounds))
                        graph.AddEdge(leafNode, otherLeafNode);
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
    }
}
