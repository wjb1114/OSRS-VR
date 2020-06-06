using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents any object with default and active states - ie. doors
/// </summary>

public class BasicObject : MonoBehaviour
{
    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public Vector3 currentPosition;
    [HideInInspector]
    public Quaternion currentRotation;
    public bool startActive;
    public string id;
    public Vector3 basePosition;
    public Vector3 baseRotation;
    public Vector3 activePosition;
    public Vector3 activeRotation;

    private Quaternion trueBaseRotation;
    private Quaternion trueActiveRotation;

    /// <summary>
    /// Sets initial values based on server inspector values. currentPosition and currentRotation values used due to limits of what can be accessed in non-main threads
    /// </summary>

    void Start()
    {
        trueBaseRotation = Quaternion.Euler(baseRotation.x, baseRotation.y, baseRotation.z);
        trueActiveRotation = Quaternion.Euler(activeRotation.x, activeRotation.y, activeRotation.z);
        isActive = startActive;

        if (isActive)
        {
            transform.position = activePosition;
            transform.rotation = trueActiveRotation;
        }
        else
        {
            transform.position = basePosition;
            transform.rotation = trueBaseRotation;
        }

        currentPosition = transform.position;
        currentRotation = transform.rotation;
    }

    /// <summary>
    /// Toggles object state, changing position and rotation values
    /// </summary>

    public void ToggleObjectState()
    {
        isActive = !isActive;

        if (isActive)
        {
            transform.position = activePosition;
            transform.rotation = trueActiveRotation;
        }
        else
        {
            transform.position = basePosition;
            transform.rotation = trueBaseRotation;
        }

        currentPosition = transform.position;
        currentRotation = transform.rotation;

        ServerSend.ObjectPosition(this);
        ServerSend.ObjectRotation(this);
    }
}
