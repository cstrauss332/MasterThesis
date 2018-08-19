using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour {

    const int CORNERMAXAMOUNT = 50;

    public static GameObject[] CheckForCorners(Vector3 centerPoint ,float radius, string layerName)
    {
        Collider[] colliders = new Collider[CORNERMAXAMOUNT];
        Physics.OverlapSphereNonAlloc(centerPoint, radius, colliders,(1 << LayerMask.NameToLayer(layerName)));

        List<GameObject> retCorners = new List<GameObject>();

        foreach (Collider target in colliders)
        {
            if (target == null)
                break;

            retCorners.Add(target.gameObject);
        }

        return retCorners.ToArray();
    }

    public static GameObject[] CheckForCorners(Vector3 centerPoint, float radius)
    {
        Collider[] colliders = new Collider[CORNERMAXAMOUNT];
        Physics.OverlapSphereNonAlloc(centerPoint, radius, colliders);

        List<GameObject> retCorners = new List<GameObject>();

        foreach (Collider target in colliders)
        {
            if (target == null)
                break;

            retCorners.Add(target.gameObject);
        }

        return retCorners.ToArray();
    }
}
