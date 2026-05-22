using NUnit.Framework;
using Octrees;
using System.Collections.Generic;
using UnityEngine;

public class OctreeGenerator : MonoBehaviour {
  [SerializeField]
  private List<GameObject> _objectsInOctree;
  [SerializeField]
  private float _minNodeSize = 1f;

  private Octree _octree;
  void Start() { _octree = new Octree(_objectsInOctree, _minNodeSize); }

  private void OnDrawGizmos() {
    if (!Application.isPlaying)
      return;

    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(_octree.Bounds.center, _octree.Bounds.size);

    _octree.Root.DrawNode();
  }
}
