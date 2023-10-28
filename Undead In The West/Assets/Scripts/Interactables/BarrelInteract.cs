using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelInteract : Destructible, IInteractable
{

    [SerializeField] private GameObject visualGameObject;


    private GridPosition gridPosition;
    private Action onInteractionComplete;
    private bool isActive;
    private float timer;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            isActive = false;

            LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
            Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);

            Destroy(gameObject);
            onInteractionComplete();
        }
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;
        timer = .5f;

        visualGameObject.SetActive(false);

        Transform barrelDestroyedTransform = Instantiate(destroyedPrefab, transform.position, transform.rotation);
        ApplyExplosionToChildren(barrelDestroyedTransform, 250f, transform.position, 10f);
    }
}
