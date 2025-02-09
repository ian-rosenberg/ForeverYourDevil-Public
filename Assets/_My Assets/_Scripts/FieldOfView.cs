﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Original Credit to Johanne Assis - jaa67@njit.edu 

public class FieldOfView : MonoBehaviour
{
    //Handle rotation of fov towards target
    Quaternion original_Rotation;

    public bool canSee; /**True if seeing at least one target. False otherwise.*/
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float lookSpeed;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    // [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        original_Rotation = transform.parent.localRotation;
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    private void FixedUpdate()
    {
        if (visibleTargets.Count > 0)
        {
            Quaternion newRot = Quaternion.LookRotation(visibleTargets[visibleTargets.Count - 1].position - transform.parent.position);
            transform.parent.localRotation = Quaternion.RotateTowards(transform.parent.localRotation, newRot, lookSpeed);
        }

        else
        {
            transform.parent.localRotation = Quaternion.RotateTowards(transform.parent.localRotation, original_Rotation, lookSpeed);
        }
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        canSee = false;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    canSee = true;
                    // Debug.Log("sees a target");
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}