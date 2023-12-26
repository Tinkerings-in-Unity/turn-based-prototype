using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 9;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;


    [SerializeField] private bool isPlayer;
    [SerializeField] private bool isEnemy;
    [SerializeField] private CameraPointsManager cameraPointsManager;


    private Weapon _selectedWeapon;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    private int actionPoints = ACTION_POINTS_MAX;
    private Weapon _weaponOne;
    private Weapon _weaponTwo;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();

        _weaponOne = new Weapon("AK47", WeaponType.Projectile, 30, 15, 5);
        _weaponTwo = new Weapon("Vintorez", WeaponType.Projectile, 50, 10, 6);

        _selectedWeapon = _weaponOne;
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        healthSystem.OnDead += HealthSystem_OnDead;

        UnitActionSystem.Instance.OnSelectedEquipmentActionChanged += UnitActionSystem_OnSelectedEquipmentActionChanged;

        // UnitManager.Instance.OnUnitManagerReady += UnitManager_OnUnitManagerReady;
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void UnitManager_OnUnitManagerReady(object sender, EventArgs e)
    {
        Debug.Log("Unit manager ready received");
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            // Unit changed Grid Position
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        } else
        {
            return false;
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        } else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }
    
    public Weapon GetSelectedWeapon()
    {
        return _selectedWeapon;
    }
    
    public Weapon GetWeaponOne()
    {
        return _weaponOne;
    }
    
    public Weapon GetWeaponTwo()
    {
        return _weaponTwo;
    }

    public void SetSelectedWeapon(Weapon weapon)
    {
        _selectedWeapon = weapon;
    }
    
    public void SetWeaponOne(Weapon weapon)
    {
        _weaponOne = weapon;
    }
    
    public void SetWeaponTwo(Weapon weapon)
    {
        _weaponTwo = weapon;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void UnitActionSystem_OnSelectedEquipmentActionChanged(object sender, EventArgs e)
    {
        var selectedEquipmentAction = UnitActionSystem.Instance.GetSelectedEquipmentAction();

        switch (selectedEquipmentAction)
        {
            case EquipmentAction.WeaponOne:
                SetSelectedWeapon(_weaponOne);
                break;
            case EquipmentAction.WeaponTwo:
                SetSelectedWeapon(_weaponTwo);
                break;
            case EquipmentAction.Backpack:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool IsPlayer()
    {
        return isPlayer;
    }
    
    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);

        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }

    public Dictionary<int, CameraPoint> GetCameraPoints => cameraPointsManager.GetCameraPoints();

}