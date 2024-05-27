using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallProperties : MonoBehaviour
{
    public bool isClimbable;
    public bool isWallRunnable;

    public float GetBuildingHeight()
    {
        Collider collider = GetComponent<Collider>();
        
        if (collider != null)
        {
            return collider.bounds.size.y;
        }
        
        return 0f;
    }
}