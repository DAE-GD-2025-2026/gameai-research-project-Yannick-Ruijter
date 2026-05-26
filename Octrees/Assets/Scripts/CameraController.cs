using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    // Start is called once before the first execution of Update after the
    // MonoBehaviour is created
    [SerializeField] private OctreeGenerator _generator;
    [SerializeField] private List<GameObject> _objectsToAdd = new();
    void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {}

    public void OnSpacePressed(InputAction.CallbackContext context)
    {
        if (!context.performed || _objectsToAdd.Count <= 0) return;
        _generator.AddGameObject(_objectsToAdd.First());
        _objectsToAdd.RemoveAt(0);
    }
}
