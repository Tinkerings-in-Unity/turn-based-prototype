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

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    [SerializeField] private LineRendererController lineRendererControllerPrefab;
    [SerializeField] private MMSimpleObjectPooler lineRendererControllerPooler;

    private UnitActionSystem _unitActionSystem;
    private GridSystemVisualSingle[,,] gridSystemVisualSingleArray;
    private bool _actionIsBusy;
    private BaseAction _selectedAction;
    private Unit _selectedUnit;
    private List<LineRendererController> _outlineLines = new List<LineRendererController>();
   

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
        gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight(),
            LevelGrid.Instance.GetFloorAmount()
        ];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                for (int floor = 0; floor < LevelGrid.Instance.GetFloorAmount(); floor++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);

                    Transform gridSystemVisualSingleTransform =
                        Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                    gridSystemVisualSingleArray[x, z, floor] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                }
            }
        }
        
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
        
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                for (int floor = 0; floor < LevelGrid.Instance.GetFloorAmount(); floor++)
                {
                    gridSystemVisualSingleArray[x, z, floor].Hide();
                }
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

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

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

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

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z, gridPosition.floor].
                Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        _selectedUnit = _unitActionSystem.GetSelectedUnit();
        _selectedAction = _unitActionSystem.GetSelectedAction();
        _selectedAction.Setup();

        var gridVisualType = GridVisualType.White;

        switch (_selectedAction)
        {
            // default:
            case MoveAction moveAction:
                DrawRangeLines();
                ShowGridPositionList(moveAction.GetTargetGridPositionList(), GridVisualType.Green);
                return;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(_selectedUnit.GetGridPosition(), shootAction.GetRange(), GridVisualType.RedSoft);
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRangeSquare(_selectedUnit.GetGridPosition(), swordAction.GetRange(), GridVisualType.RedSoft);
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;
        }

        ShowGridPositionList(_selectedAction.GetValidActionGridPositionList(), gridVisualType);
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
        } else
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
        
        foreach (var gridPosition in list)
        {
            vectorList.Add(new Vector3(gridPosition.x, 0.03f,  gridPosition.z));
        }

        return vectorList;
    }
    
    // private void OnDrawGizmos()
    // {
    //     if(_actionIsBusy || _selectedAction == null)
    //     {
    //         return;
    //     }
    //
    //
    //     var area = ConvertGridPositionListToVector3(_selectedAction.GetValidActionGridPositionList());
    //     var cellSize = LevelGrid.Instance.GetCellSize();
    //
    //     var offsetVector = new Vector3(-cellSize / 2, 0f, -cellSize / 2);
    //
    //     Gizmos.color = Color.white;
    //     foreach (var tilePos in area)
    //     {
    //         var tilePosWithOffset = tilePos + offsetVector;
    //         
    //         if (!area.Contains(tilePos + Vector3.left))
    //         {
    //             Gizmos.DrawLine(tilePosWithOffset, tilePosWithOffset + Vector3.forward);
    //         }
    //
    //         if (!area.Contains(tilePos + Vector3.back))
    //         {
    //             Gizmos.DrawLine(tilePosWithOffset, tilePosWithOffset + Vector3.right);
    //         }
    //
    //         if (!area.Contains(tilePos + Vector3.right))
    //         {
    //             Gizmos.DrawLine(tilePosWithOffset+ Vector3.right, tilePosWithOffset + new Vector3(1, 0, 1));
    //         }
    //
    //         if (!area.Contains(tilePos + Vector3.forward))
    //         {
    //             Gizmos.DrawLine(tilePosWithOffset + Vector3.forward, tilePosWithOffset + new Vector3(1, 0, 1));
    //         }
    //     }
    //     
    // }

    private void DrawLine(Vector3 start, Vector3 end)
    {
        var obj = lineRendererControllerPooler.GetPooledGameObject();
        obj.SetActive(true);
        var lineRendererController = obj.GetComponent<LineRendererController>();
        lineRendererController.Draw(start, end);
        _outlineLines.Add(lineRendererController);
    }
    
    private void DrawRangeLines()
    {
        var area = ConvertGridPositionListToVector3(_selectedAction.GetValidActionGridPositionList());
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
                DrawLine(tilePosWithOffset+ Vector3.right, tilePosWithOffset + new Vector3(1, 0, 1));
            }

            if (!area.Contains(tilePos + Vector3.forward))
            {
                DrawLine(tilePosWithOffset + Vector3.forward, tilePosWithOffset + new Vector3(1, 0, 1));
            }
        }
    }
    
    // private void LateUpdate() {
    //     if (_outlinePoints.Count >= 2)
    //     {
    //         _lineRenderer.positionCount = _outlinePoints.Count;
    //         
    //         var points = _outlinePoints.ToList();
    //         for (int i = 0; i < points.Count; i++) {
    //             Vector3 position = points[i];
    //             // position += new Vector3(0, 0, 5);
    //
    //             _lineRenderer.SetPosition(i, position);
    //         }
    //     }
    // }
}