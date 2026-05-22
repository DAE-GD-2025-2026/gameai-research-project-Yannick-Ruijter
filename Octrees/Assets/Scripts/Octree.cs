using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Octrees {
public class Octree {
  public OctreeNode Root;
  public Bounds Bounds;

  public Octree(List<GameObject> objects, float minNodeSize) {
    CalculateBounds(objects);
    CreateTree(objects, minNodeSize);
  }

  private void CreateTree(List<GameObject> objects, float minNodeSize) {
    Root = new(Bounds, minNodeSize);
    foreach (var obj in objects) {
      Root.Divide(obj);
    }
  }

  void CalculateBounds(List<GameObject> objects) {
    foreach (var obj in objects) {
      Bounds.Encapsulate(obj.GetComponent<Collider>().bounds);
    }

    Vector3 size = Vector3.one *
                   Mathf.Max(Bounds.size.x, Bounds.size.y, Bounds.size.z) *
                   0.5f;
    Bounds.SetMinMax(Bounds.center - size, Bounds.center + size);
  }
}
}
