using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

namespace Octrees {
    public class OctreeObject {
        public Bounds bounds;
        Vector3 previousPos;
        public List<OctreeNode> ParentNodes = new();
        public OctreeObject(GameObject obj) {
            if (obj == null){
                bounds = new Bounds();
                bounds.size = Vector3.zero;
                return; 
            }
            bounds = obj.GetComponent<Collider>().bounds;
            previousPos = bounds.center;
        }

        void Update()
        {
            if(previousPos != bounds.center)
            {

            }
        }

        public bool Intersects(Bounds other) => bounds.Intersects(other);
    };
}
