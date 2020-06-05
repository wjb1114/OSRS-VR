using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Primary manager for server setup and player spawning
/// </summary>

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;

    /// <summary>
    /// Singleton implementation
    /// </summary>

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    /// <summary>
    /// Starts server and sets timing
    /// </summary>

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Server.Start(16, 26950);
    }

    /// <summary>
    /// Safe shutdown of server environment
    /// </summary>

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    /// <summary>
    /// Creates new player based on incoming connection
    /// </summary>
    /// <returns>New player reference in server scene</returns>

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<Player>();
    }
}
