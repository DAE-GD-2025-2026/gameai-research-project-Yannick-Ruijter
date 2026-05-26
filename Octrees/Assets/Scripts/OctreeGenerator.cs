using NUnit.Framework;
using Octrees;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class OctreeGenerator : MonoBehaviour 
{
    [SerializeField]
    private List<GameObject> _objectsInOctree;
    [SerializeField]
    private float _minNodeSize = 1f;

    private Octree _octree;

    public readonly Graph waypoints = new();
    void Awake() { _octree = new Octree(_objectsInOctree, _minNodeSize, waypoints); }

    public void AddGameObject(GameObject obj)
    {
        _octree.AddObject(obj);
    }

    public void RemoveObject(GameObject obj)
    {
        _octree.RemoveObject(obj);
    }

    private void OnDrawGizmos() 
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.green;

        _octree.Root.DrawNode();
        //_octree.graph.DrawGraph();
    }
}
