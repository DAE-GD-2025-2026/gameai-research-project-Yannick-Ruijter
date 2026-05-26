using UnityEngine;

namespace Octrees {
    public class OctreeObject {
        Bounds bounds;

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
