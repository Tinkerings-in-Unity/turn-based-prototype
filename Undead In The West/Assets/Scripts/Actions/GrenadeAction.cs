using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{

    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }

    public override void Setup()
    {
    }


    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }
                
                var unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                var targetTileWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition );
                var throwDir = (targetTileWorldPosition - unitWorldPosition).normalized;

                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(
                        unitWorldPosition + Vector3.up * unitShoulderHeight,
                        throwDir,
                        Vector3.Distance(unitWorldPosition, targetTileWorldPosition),
                        obstaclesLayerMask))
                {
                    // Blocked by an Obstacle
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    } 
    
    public List<GridPosition> GetTargetGridPositionList()
    {
        var validGridPositionList = GetValidActionGridPositionList();
        var targetGridPositionList = new List<GridPosition>();
        
        foreach (var gridPosition in validGridPositionList)
        {
            if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
            {
                // Grid Position is empty, no Unit
                continue;
            }

            Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

            if (targetUnit.IsEnemy() == unit.IsEnemy())
            {
                // Both Units on same 'team'
                continue;
            }
            
            targetGridPositionList.Add(gridPosition);
        }

        return targetGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourComplete);

        ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }

}
