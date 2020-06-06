using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles states for simple objects, ie. doors 
/// </summary>

public class BasicObjectController : MonoBehaviour
{
    public string identifier;
    [HideInInspector]
    public bool isActive;

    /// <summary>
    /// updates client side state of objects - not currently in use, but useful to have for client event specific actions - ie. NPC dialogue
    /// </summary>

    public void UpdateState(Vector3 pos, Quaternion rot)
    {
        isActive = !isActive;

        transform.position = pos;
        transform.rotation = rot;
    }
}
