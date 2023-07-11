using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentActionButtonUI : MonoBehaviour
{

    [SerializeField] private EquipmentAction equipmentAction;
    [SerializeField] protected TextMeshProUGUI textMeshPro;
    [SerializeField] protected Button button;
    [SerializeField] protected GameObject selectedGameObject;

    public void Setup()
    {
        var player = UnitManager.Instance.GetPlayer();
        
        switch (equipmentAction)
        {
            case EquipmentAction.WeaponOne:
                textMeshPro.text = player.GetWeaponOne().GetWeaponName();
                break;
            case EquipmentAction.WeaponTwo:
                textMeshPro.text = player.GetWeaponTwo().GetWeaponName();
                break;
            case EquipmentAction.Backpack:
                textMeshPro.text = equipmentAction.ToString().ToUpper();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        UnitActionSystemUI.Instance.RegisterEquipmentActionButton(this);
        
        button.onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedEquipmentAction(equipmentAction);
        });
    }

    public void UpdateSelectedVisual()
    {
        var selectedEquipmentAction = UnitActionSystem.Instance.GetSelectedEquipmentAction();
        selectedGameObject.SetActive(selectedEquipmentAction == equipmentAction);
    }

}
