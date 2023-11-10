using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{

    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow,
        Green
    }

    [SerializeField] private float floorHeight;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    [SerializeField] private MMSimpleObjectPooler lineRendererControllerPooler;
    [SerializeField] private MMSimpleObjectPooler gridSystemVisualPooler;

    private UnitActionSystem _unitActionSystem;
    private bool _actionIsBusy;
    private BaseAction _selectedAction;
    private Unit _selectedUnit;
    private List<LineRendererController> _outlineLines = new List<LineRendererController>();
    private List<GridSystemVisualSingle> _gridSystemVisuals = new List<GridSystemVisualSingle>();


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    private void Start()
    {
        _unitActionSystem = UnitActionSystem.Instance;

        _unitActionSystem.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        _unitActionSystem.OnSelectedEquipmentActionChanged += UnitActionSystem_OnSelectedEquipmentActionChanged;
        _unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        _unitActionSystem.OnSelectedActionUpdated += UnitActionSystem_OnSelectedActionUpdated;
        //LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

        UpdateGridVisual();
    }

    public void HideAllGridPosition()
    {
        _outlineLines.ForEach(l => l.gameObject.GetComponent<MMPoolableObject>().Destroy());
       _gridSystemVisuals.ForEach(g => g.gameObject.GetComponent<MMPoolableObject>().Destroy());
    }

    private List<GridPosition> GetGridPositionRange(GridPosition gridPosition, int range)
    {
        var gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        return gridPositionList;
    }

    private List<GridPosition> GetGridPositionRangeSquare(GridPosition gridPosition, int range)
    {
        var gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        return gridPositionList;
    }
    
    private void DrawGridSystemVisual(Vector3 worldPosition, GridVisualType gridVisualType)
    {
        var obj = gridSystemVisualPooler.GetPooledGameObject();
        obj.SetActive(true);
        obj.transform.position = worldPosition;
        var gridSystemVisual = obj.GetComponent<GridSystemVisualSingle>();
        gridSystemVisual.Show(GetGridVisualTypeMaterial(gridVisualType));
        _gridSystemVisuals.Add(gridSystemVisual);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        var gridPositions = ConvertGridPositionListToVector3(gridPositionList);
        
        foreach (var position in gridPositions)
        {
            DrawGridSystemVisual(position, gridVisualType);
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        _selectedUnit = _unitActionSystem.GetSelectedUnit();
        _selectedAction = _unitActionSystem.GetSelectedAction();
        _selectedAction.Setup();

        var gridVisualType = GridVisualType.White;
        var gridPositionList = new List<GridPosition>();
        var targetGridPositionList = new List<GridPosition>();

        switch (_selectedAction)
        {
            // default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.Green;
                gridPositionList = _selectedAction.GetValidActionGridPositionList();
                targetGridPositionList = moveAction.GetTargetGridPositionList();
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                gridPositionList = GetGridPositionRange(_selectedUnit.GetGridPosition(), shootAction.GetRange());
                targetGridPositionList = _selectedAction.GetValidActionGridPositionList();
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;
                gridPositionList = _selectedAction.GetValidActionGridPositionList();
                targetGridPositionList = grenadeAction.GetTargetGridPositionList();
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                gridPositionList = GetGridPositionRangeSquare(_selectedUnit.GetGridPosition(), swordAction.GetRange());
                targetGridPositionList = _selectedAction.GetValidActionGridPositionList();
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                gridPositionList = _selectedAction.GetValidActionGridPositionList();
                break;
        }
        
        DrawRangeLines(gridPositionList);

        ShowGridPositionList(targetGridPositionList, gridVisualType);
    }

    private void HighlightGridCells()
    {

    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool e)
    {
        _actionIsBusy = e;
        if (e)
        {
            HideAllGridPosition();
        }
        else
        {
            UpdateGridVisual();
        }
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedEquipmentActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedActionUpdated(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }

    private List<Vector3> ConvertGridPositionListToVector3(List<GridPosition> list)
    {
        var vectorList = new List<Vector3>();
        var yOffset = 0.03f;

        foreach (var gridPosition in list)
        {
            vectorList.Add(new Vector3(gridPosition.x, (gridPosition.floor * floorHeight) + yOffset, gridPosition.z));
        }

        return vectorList;
    }


    private void DrawLine(Vector3 start, Vector3 end)
    {
        var obj = lineRendererControllerPooler.GetPooledGameObject();
        obj.SetActive(true);
        var lineRendererController = obj.GetComponent<LineRendererController>();
        lineRendererController.Draw(start, end);
        _outlineLines.Add(lineRendererController);
    }

    private void DrawRangeLines(List<GridPosition> actionGridPositionList = null)
    {
        var gridPositionList = actionGridPositionList ?? _selectedAction.GetValidActionGridPositionList();
        
        var area = ConvertGridPositionListToVector3(gridPositionList);
        var cellSize = LevelGrid.Instance.GetCellSize();

        var offsetVector = new Vector3(-cellSize / 2, 0f, -cellSize / 2);

        foreach (var tilePos in area)
        {
            var tilePosWithOffset = tilePos + offsetVector;

            if (!area.Contains(tilePos + Vector3.left))
            {
                DrawLine(tilePosWithOffset, tilePosWithOffset + Vector3.forward);
            }

            if (!area.Contains(tilePos + Vector3.back))
            {
                DrawLine(tilePosWithOffset, tilePosWithOffset + Vector3.right);
            }

            if (!area.Contains(tilePos + Vector3.right))
            {
                DrawLine(tilePosWithOffset + Vector3.right, tilePosWithOffset + new Vector3(1, 0, 1));
            }

            if (!area.Contains(tilePos + Vector3.forward))
            {
                DrawLine(tilePosWithOffset + Vector3.forward, tilePosWithOffset + new Vector3(1, 0, 1));
            }
        }
    }
}