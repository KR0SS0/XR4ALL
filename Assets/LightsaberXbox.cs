using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class LightsaberXbox : LightsaberController
{
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger1 = "attack1";
    [SerializeField] private string attackTrigger2 = "attack2";
    [SerializeField] private string idleTrigger = "idle";

    [SerializeField] private float attackRange = 5f;

    private int attackQueue = 0;
    private bool isAttacking = false;

    private InputAction anyInputAction;

    private void Start()
    {
        anyInputAction = new InputAction();
        anyInputAction.AddBinding("<Gamepad>/buttonWest");
        anyInputAction.performed += ctx => OnAnyInput();
        anyInputAction.Enable();
    }

    private void OnAnyInput()
    {
        QueueAttack();
    }

    void Update()
    {
        if (Keyboard.current.ctrlKey.wasPressedThisFrame)
        {
            QueueAttack();
        }
    }

    private void QueueAttack()
    {
        if (isAttacking)
        {
            attackQueue = 1; // Set to 1 since only one extra attack is needed after the first
        } else
        {
            StartCoroutine(PerformAttackAnimation());
        }
    }

    private IEnumerator PerformAttackAnimation()
    {
        isAttacking = true;

        // Play the first attack animation
        animator.SetTrigger(attackTrigger1);
        Attack();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("lightsaber_attack_1"));
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // If another attack is queued, play the second attack animation
        if (attackQueue > 0)
        {
            attackQueue = 0;
            animator.SetTrigger(attackTrigger2);
            Attack();
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("lightsaber_attack_2"));
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }

        // Return to idle state
        animator.SetTrigger(idleTrigger);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("lightsaber_idle"));

        isAttacking = false;

        // If another attack was queued during the second attack, restart the attack sequence
        if (attackQueue > 0)
        {
            QueueAttack();
        }
    }

    private void Attack()
    {
        BaseDroneController drone = FindNearestDrone();
        if (drone != null)
        {
            float distanceToDrone = Vector3.Distance(transform.position, drone.transform.position);
            if (distanceToDrone <= attackRange)
            {
                drone.HandleHit();
                PlayStrikeSound();
                Debug.Log("Drone hit!");
            } else
            {
                PlaySwingSound();
                Debug.Log("Drone is out of attack range.");
            }
        } else
        {
            Debug.Log("No drones found.");
        }
    }

    private BaseDroneController FindNearestDrone()
    {
        BaseDroneController[] drones = FindObjectsOfType<BaseDroneController>();
        BaseDroneController nearestDrone = null;
        float nearestDistance = Mathf.Infinity;

        foreach (BaseDroneController drone in drones)
        {
            float distance = Vector3.Distance(transform.position, drone.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestDrone = drone;
            }
        }

        return nearestDrone;
    }
}
