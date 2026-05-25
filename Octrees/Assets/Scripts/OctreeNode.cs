using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Octrees {
    public class OctreeNode {
        public List<OctreeObject> Objects = new();

        static int NextId;
        public readonly int id;

        public Bounds Bounds;
        Bounds[] _childBounds = new Bounds[8];
        public OctreeNode[] children;
        public bool IsLeaf => children == null;
        float _minNodeSize;

        public OctreeNode(Bounds bounds, float minNodeSize) {
            id = NextId++;

            Bounds = bounds;
            _minNodeSize = minNodeSize;
            // its gonna be half size in all direction => total
            // size will be 1/8 (3 dimensions)
            Vector3 newSize = bounds.size * 0.5f;
            Vector3 centerOffset = bounds.size * 0.25f;
            Vector3 parentCenter = bounds.center;

            for (int i = 0; i < 8; ++i) {
                Vector3 childCenter = parentCenter;
                // if i is even subtract x, if uneven add x
                // (to go over all possible combinations
                childCenter.x += centerOffset.x * ((i & 1) == 0 ? -1 : 1);
                // if second bit is 1 then we add offset, otherwise we
                // subtract (to go over all possible combinations
                childCenter.y += centerOffset.y * ((i & 2) == 0 ? -1 : 1);
                // if third bit is 1 then we add offset, otherwise we
                // subtract (to go over all possible combinations
                childCenter.z += centerOffset.z * ((i & 4) == 0 ? -1 : 1);
                _childBounds[i] = new Bounds(childCenter, newSize);
            }
        }
        public void Divide(GameObject obj) => Divide(new OctreeObject(obj));
        public void Divide(OctreeObject octObj) {
            if (Bounds.size.x <= _minNodeSize) {
                AddObject(octObj);
                return;
            }

            children ??= new OctreeNode[8];

            bool intersectedChild = false;
            for (int i = 0; i < 8; i++) {

                children[i] ??= new OctreeNode(_childBounds[i], _minNodeSize);
                if (octObj.Intersects(_childBounds[i])) {
                children[i].Divide(octObj);
                intersectedChild = true;
                }
            }

            if (!intersectedChild) {
                AddObject(octObj);
            }
        }

        void AddObject(OctreeObject octObj) => Objects.Add(octObj);
        public void DrawNode() {
            if (/*Objects.Count > 0*/ false) 
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(Bounds.center, Bounds.size);
            } 
            else 
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(Bounds.center, Bounds.size);
            }

            if (children != null) 
            {
                foreach (var child in children) 
                {
                    if (child != null)
                        child.DrawNode(); // null guard needed
                }
            }
        }
    };
}
