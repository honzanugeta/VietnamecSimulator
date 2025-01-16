using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interractionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactableMask;

    private readonly Collider[] _colliders = new Collider[3];

    [SerializeField] private int _numfound;

    private void Update()
    {
        _numfound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interractionPointRadius, _colliders,
            _interactableMask);
        if (_numfound > 0)
        {
            var interactable = _colliders[0].GetComponent<IInteractable>();
            if (interactable != null && Input.GetKeyDown(KeyCode.E)) 
            {
                Debug.Log("pressin e");
                interactable.Interact(this);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interractionPointRadius);
    }
}
