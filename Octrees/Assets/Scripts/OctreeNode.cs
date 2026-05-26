using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Octrees {
    public class OctreeNode {
        public List<OctreeObject> Objects = new();
        public List<OctreeObject> ObjectsInChildren
        {
            get
            {
                List<OctreeObject> current = new(Objects);
                if (children != null)
                {
                    for (int i = 0; i < 8; i++)
                        current.AddRange(children[i].ObjectsInChildren);
                }
                return current;
            }
        }
        public bool IsEmpty => ObjectsInChildren.Count == 0;
        static int NextId;
        public readonly int id;

        public Bounds bounds;
        Bounds[] _childBounds = new Bounds[8];
        public OctreeNode[] children;
        public bool IsLeaf => children == null;
        float _minNodeSize;

        public OctreeNode(OctreeNode[] newChildren, float minNodeSize)
        {
            id = NextId++;
            children = newChildren;
            this.bounds = new();

            for (int i = 0; i < children.Length; i++)
            {
                bounds.Encapsulate(children[i].bounds);
            }

        }

        public OctreeNode(Bounds bounds, float minNodeSize) {
            id = NextId++;

            this.bounds = bounds;
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
            if (bounds.size.x <= _minNodeSize) {
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
        public void DrawNode()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            if (children != null)
            {
                foreach (OctreeNode child in children)
                {
                    if (child != null) child.DrawNode();
                }
            }
        }

        public void RemoveObject(OctreeObject obj)
        {
            if(Objects.Contains(obj))
            {
                Objects.Remove(obj);
                Debug.Log("Removed object from octant");
                return;
            }
            if (children == null) return;
            //check how many objects are left in the current octant
            int numberOfObjectsLeft = 0;
            
            for (int i = 0; i < 8; i++)
            {
                //if the current child or one of its children contains the object, remove it
                if (children[i].ObjectsInChildren.Contains(obj))
                    children[i].RemoveObject(obj);

                numberOfObjectsLeft += children[i].ObjectsInChildren.Count;
            }

            //collapsing the current octant of the octree if every child is empty
            if (numberOfObjectsLeft == 0)
            {
                Debug.Log("All octants empty, deleting octants...");
                children = null; 
            }
        }

        public OctreeNode TryCollapsing()
        {
            //we can't collapse without any children
            if (children == null) return this;

            //check how many empty children we have
            int nrNonEmptyChildren = 0;
            int nonEmptyIndex = -1;
            for(int i = 0; i < 8; i++)
            {
                //if we find a non empty child
                if (!children[i].IsEmpty)
                {
                    //save its index and increment the counter
                    nonEmptyIndex = i;
                    nrNonEmptyChildren++;
                }
                //if we have more than 1 non empty child, we can not collapse so we just return the current node (will become new root)
                if (nrNonEmptyChildren > 1) return this;
            }
            //if there's only 1 child non-empty, we see if that child can collapse
            return children[nonEmptyIndex].TryCollapsing();
        }
    };
}
