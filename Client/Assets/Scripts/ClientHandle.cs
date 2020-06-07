using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

/// <summary>
/// Handles incoming packets from server
/// </summary>

public class ClientHandle : MonoBehaviour
{
    /// <summary>
    /// Sends message indicating server connection was successful
    /// </summary>
    /// <param name="_packet">Packet from server</param>

    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    /// <summary>
    /// Spawns player in local scene
    /// </summary>
    /// <param name="_packet">Packet containing initialization data</param>

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    /// <summary>
    /// Handles updates to player position from server for all players
    /// </summary>
    /// <param name="_packet">Packet containing new position information</param>

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.players[_id].transform.position = _position;
    }

    /// <summary>
    /// Updates position of object on state change
    /// </summary>
    /// <param name="_packet">Packet containing new state information</param>

    public static void ObjectPosition(Packet _packet)
    {
        string _id = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();

        GameManager.basicObjects[_id].transform.position = _position;
    }

    /// <summary>
    /// Updates rotation of object on state change
    /// </summary>
    /// <param name="_packet">Packet containing new state information</param>

    public static void ObjectRotation(Packet _packet)
    {
        string _id = _packet.ReadString();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.basicObjects[_id].transform.rotation = _rotation;
    }

    /// <summary>
    /// Handles updates to player rotation from server for all players except one sending rotation
    /// </summary>
    /// <param name="_packet">Packet containing new rotation information</param>

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.players[_id].transform.rotation = _rotation;
    }

    /// <summary>
    /// Handles packets conatining disconnect data from other clients
    /// </summary>
    /// <param name="_packet">Packet containing data with disconnected player information</param>

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    /// <summary>
    /// Updates player health for all other players
    /// </summary>
    /// <param name="_packet">Packet conatining updated health information</param>

    public static void PlayerHealth(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.players[_id].SetHealth(_health);
    }

    /// <summary>
    /// Updates other player data when a player respawns
    /// </summary>
    /// <param name="_packet">Packet containing respawn data</param>

    public static void PlayerRespawned(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.players[_id].Respawn();
    }

    /// <summary>
    /// Sets initial position of all objects in client scene when a new client connects
    /// </summary>
    /// <param name="_packet">Packet containing object position data</param>

    public static void InitialObjectPosition(Packet _packet)
    {
        string _id = _packet.ReadString();
        bool _isActive = _packet.ReadBool();
        Vector3 _position = _packet.ReadVector3();
        GameManager.instance.SetInitialObjectPosition(_id, _isActive, _position);
    }

    /// <summary>
    /// Sets initial rotation of all objects in client scene when a new client connects
    /// </summary>
    /// <param name="_packet">Packet containing object rotation data</param>

    public static void InitialObjectRotation(Packet _packet)
    {
        string _id = _packet.ReadString();
        bool _isActive = _packet.ReadBool();
        Quaternion _rotation = _packet.ReadQuaternion();
        GameManager.instance.SetInitialObjectRotation(_id, _isActive, _rotation);
    }
}
