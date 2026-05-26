using NUnit.Framework;
using Octrees;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    // Start is called once before the first execution of Update after the
    // MonoBehaviour is created
    [SerializeField] private OctreeGenerator _generator;
    [SerializeField] private OctreeAgent _agent;
    [SerializeField] private List<GameObject> _objectsToAdd = new();
    private List<OctreeObject> _objectsToRemove = new();
    private List<OctreeObject> _octreeObjectsToAdd = new();
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        foreach (var item in _objectsToAdd)
        {
            _octreeObjectsToAdd.Add(new OctreeObject(item));
        }
    }

    // Update is called once per frame
    void Update() {}

    public void OnAddObject(InputAction.CallbackContext context)
    {
        if (!context.performed || _octreeObjectsToAdd.Count <= 0) return;
        _generator.AddGameObject(_octreeObjectsToAdd.First());
        _objectsToRemove.Add(_octreeObjectsToAdd[0]);
        _octreeObjectsToAdd.RemoveAt(0);
        _agent.GetRandomDestination();
    }

    public void OnRemoveObject(InputAction.CallbackContext context)
    {
        if (!context.performed || _objectsToRemove.Count <= 0) return;
        _generator.RemoveObject(_objectsToRemove.Last());
        _octreeObjectsToAdd.Insert(0, _objectsToRemove.Last());
        _objectsToRemove.Remove(_objectsToRemove.Last());
        _agent.GetRandomDestination();
    }
}
