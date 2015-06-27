﻿using UnityEngine;
using System.Collections;

public static class SuperCollider {

    public static Vector3 ClosestPointOnSurface(Collider collider, Vector3 to, float radius)
    {
        if (collider is BoxCollider)
        {
            return SuperCollider.ClosestPointOnSurface((BoxCollider)collider, to);
        }
        else if (collider is SphereCollider)
        {
            return SuperCollider.ClosestPointOnSurface((SphereCollider)collider, to);
        }
        else if (collider is MeshCollider)
        {
            RPGMesh rpgMesh = collider.GetComponent<RPGMesh>();

            if (rpgMesh != null)
            {
                return rpgMesh.ClosestPointOn(to, radius, false, false);
            }
        }

        return Vector3.zero;
    }

    public static Vector3 ClosestPointOnSurface(SphereCollider collider, Vector3 to)
    {
        Vector3 p;

        p = to - collider.transform.position;
        p.Normalize();

        p *= collider.radius * collider.transform.localScale.x;
        p += collider.transform.position;

        return p;
    }

    public static Vector3 ClosestPointOnSurface(BoxCollider collider, Vector3 to)
    {
        // Cache the collider transform
        var ct = collider.transform;

        // Firstly, transform the point into the space of the collider
        var local = ct.InverseTransformPoint(to);

        // Now, shift it to be in the center of the box
        local -= collider.center;

        // Clamp the points to the collider's extents
        var localNorm =
            new Vector3(
                Mathf.Clamp(local.x, -collider.size.x * 0.5f, collider.size.x * 0.5f),
                Mathf.Clamp(local.y, -collider.size.y * 0.5f, collider.size.y * 0.5f),
                Mathf.Clamp(local.z, -collider.size.z * 0.5f, collider.size.z * 0.5f)
            );

        // Select a face to project on
        if (Mathf.Abs(localNorm.x) > Mathf.Abs(localNorm.y) && Mathf.Abs(localNorm.x) > Mathf.Abs(localNorm.z))
            localNorm.x = Mathf.Sign(localNorm.x) * collider.size.x * 0.5f;
        else if (Mathf.Abs(localNorm.y) > Mathf.Abs(localNorm.x) && Mathf.Abs(localNorm.y) > Mathf.Abs(localNorm.z))
            localNorm.y = Mathf.Sign(localNorm.y) * collider.size.y * 0.5f;
        else if (Mathf.Abs(localNorm.z) > Mathf.Abs(localNorm.x) && Mathf.Abs(localNorm.z) > Mathf.Abs(localNorm.y))
            localNorm.z = Mathf.Sign(localNorm.z) * collider.size.z * 0.5f;

        // Now we undo our transformations
        localNorm += collider.center;

        // Return resulting point
        return ct.TransformPoint(localNorm);
    }
}
