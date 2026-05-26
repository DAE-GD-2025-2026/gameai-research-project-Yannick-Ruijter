using Unity.VisualScripting;
using UnityEngine;

namespace Octrees {
    public class OctreeObject {
        public Bounds bounds;
        Vector3 previousPos;
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
