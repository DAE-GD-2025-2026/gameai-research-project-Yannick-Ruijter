using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

namespace Octrees {
    public class OctreeObject {
        public Bounds bounds;
        public List<OctreeNode> ParentNodes = new();
        public OctreeObject(GameObject obj) {
            if (obj == null){
                bounds = new Bounds();
                bounds.size = Vector3.zero;
                return; 
            }
            bounds = obj.GetComponent<Collider>().bounds;
        }

        public bool Intersects(Bounds other) => bounds.Intersects(other);
    };
}
