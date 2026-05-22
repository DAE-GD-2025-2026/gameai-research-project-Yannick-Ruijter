using UnityEngine;

namespace Octrees {
  public class OctreeObject {
    Bounds bounds;

    public OctreeObject(GameObject obj) {
      bounds = obj.GetComponent<Collider>().bounds;
    }

    public bool Intersects(Bounds other) => bounds.Intersects(other);
  };
}
