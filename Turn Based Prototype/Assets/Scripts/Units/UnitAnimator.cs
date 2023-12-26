using System;
using System.Collections;
using System.Collections.Generic;
using CoverSystem;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;


    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
            moveAction.OnChangedFloorsStarted += MoveAction_OnChangedFloorsStarted;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }

        if (TryGetComponent<CoverAbility>(out CoverAbility coverAbility))
        {
            coverAbility.OnInCover += CoverAbility_OnInCover;
            coverAbility.OnOutOfCover += CoverAbility_OnOutOfCover;
        }
    }

    private void MoveAction_OnChangedFloorsStarted(object sender, MoveAction.OnChangeFloorsStartedEventArgs e)
    {
        if (e.targetGridPosition.floor > e.unitGridPosition.floor)
        {
            // Jump
            animator.SetTrigger("JumpUp"); //climb up
        } else
        {
            // Drop
            animator.SetTrigger("JumpDown"); //climb down
        }
    }

    private void Start()
    {
        EquipRifle();
    }

    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        animator.SetTrigger("SwordSlash");
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");

        Transform bulletProjectileTransform = 
            Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);

        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();

        float unitShoulderHeight = 1.7f;
        targetUnitShootAtPosition.y += unitShoulderHeight;

        bulletProjectile.Setup(targetUnitShootAtPosition);
    }

    private void CoverAbility_OnInCover(object sender, EventArgs e)
    {
        
    }
    
    private void CoverAbility_OnOutOfCover(object sender, EventArgs e)
    {
        
    }

    private void EquipSword()
    {
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }

}
