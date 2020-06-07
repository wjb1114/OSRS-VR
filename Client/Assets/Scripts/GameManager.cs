using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages current local game state
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<string, BasicObjectController> basicObjects = new Dictionary<string, BasicObjectController>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    /// <summary>
    /// Singleton initialization
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
    /// Spawn player in local scene
    /// </summary>
    /// <param name="_id">Player Id for reference by server logic</param>
    /// <param name="_username">Player entered username</param>
    /// <param name="_position">Position of player</param>
    /// <param name="_rotation">Rotation of player</param>

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if(_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    /// <summary>
    /// Sets initial state of objects in scene
    /// </summary>
    /// <param name="_id">Object unique string id</param>
    /// <param name="_isActive">Bool representing current state</param>
    /// <param name="_position">Position of object in initial state</param>
    public void SetInitialObjectPosition(string _id, bool _isActive, Vector3 _position)
    {
        if (basicObjects.Count < 1)
        {
            InitializeBasicObjects();
        }
        basicObjects[_id].transform.position = _position;
        basicObjects[_id].isActive = _isActive;
    }

    /// <summary>
    /// Sets initial state of objects in scene
    /// </summary>
    /// <param name="_id">Object unique string id</param>
    /// <param name="_isActive">Bool representing current state</param>
    /// <param name="_rotation">Rotation of object in initial state</param>

    public void SetInitialObjectRotation(string _id, bool _isActive, Quaternion _rotation)
    {
        if (basicObjects.Count < 1)
        {
            InitializeBasicObjects();
        }
        basicObjects[_id].transform.rotation = _rotation;
        basicObjects[_id].isActive = _isActive;
    }

    /// <summary>
    /// Populates dictionary of objects
    /// </summary>

    void InitializeBasicObjects()
    {
        foreach (BasicObjectController item in UnityEngine.Object.FindObjectsOfType(typeof(BasicObjectController)))
        {
            basicObjects.Add(item.identifier, item);
        }
    }
}
