﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Util : MonoBehaviour{

    public static Vector3 lerp(GameObject[] points, float u, int i0 = 0, int i1 = -1) {
        Vector3[] vpoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; ++i) {
            vpoints[i] = points[i].transform.position;
        }
        return lerp(vpoints, u, i0, i1);
    }

    public static Vector3 lerp(Vector3[] points, float u, int i0 = 0, int i1 = -1) {
        if (i1 == -1) {
            i1 = points.Length - 1;
        }
        if (i0 == i1) {
            return points[i0];
        }

        Vector3 point1 = lerp(points, u, i0, i1 - 1);
        Vector3 point2 = lerp(points, u, i0 + 1, i1);
        Vector3 point12 = (1 - u) * point1 + u * point2;
        return point12;
    }

    public static bool colliderIntersects(Collider2D coll, List<Collider2D> intersecting_colls) {
        foreach (Collider2D c in intersecting_colls) {
            if (c.bounds.Intersects(coll.bounds)) {
                return true;
            }
        }
        return false;
    }

    public static float getAngleInRads(Vector2 angle_vector) {
        float angle = Vector2.Angle(new Vector2(1f, 0f), angle_vector.normalized);
        if (Vector2.Angle(new Vector2(0f, 1f), angle_vector.normalized) > 90f) {
            angle = (-angle) + 360;
        }
        return (angle * Mathf.PI) / 180f;
    }

    public static int getCloserDirection(float from_angle, float to_angle) {
        float positive_distance, negative_distance;
        if (to_angle < from_angle) {
            negative_distance = from_angle - to_angle;
            positive_distance = (to_angle + 2 * Mathf.PI) - from_angle;
        } else {
            negative_distance = (from_angle + 2 * Mathf.PI) - to_angle;
            positive_distance = to_angle - from_angle;
        }

        if (positive_distance > negative_distance) {
            return 1;
        } else {
            return -1;
        }
    }
}
