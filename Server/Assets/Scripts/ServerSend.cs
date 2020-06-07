using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sends data to one or more clients
/// </summary>

public class ServerSend
{
    /// <summary>
    /// Sends a TCP packet to a specific client
    /// </summary>
    /// <param name="_toClient">Id of client that is being sent the packet</param>
    /// <param name="_packet">Packet containing data to be sent</param>

    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>
    /// Sends a UDP packet to a specific client
    /// </summary>
    /// <param name="_toClient">Id of client that is being sent the packet</param>
    /// <param name="_packet">Packet containing data to be sent</param>

    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>
    /// Sends a TCP Packet to all connected clients
    /// </summary>
    /// <param name="_packet">Packet containing data to be sent</param>

    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }

    /// <summary>
    /// Sends a TCP Packet to all connected clients except one, usually a player that initiated the action
    /// </summary>
    /// <param name="_exceptClient">Id of client that will NOT be sent this packet</param>
    /// <param name="_packet">Packet containing data to be sent</param>

    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    /// <summary>
    /// Sends a UDP Packet to all connected clients
    /// </summary>
    /// <param name="_packet">Packet containing data to be sent</param>

    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }

    /// <summary>
    /// Sends a UDP Packet to all connected clients except one, usually a player that initiated the action
    /// </summary>
    /// <param name="_exceptClient">Id of client that will NOT be sent this packet</param>
    /// <param name="_packet">Packet containing data to be sent</param>

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    /// <summary>
    /// Builds packet for a confirmation of user login
    /// </summary>
    /// <param name="_toClient">Client that will receive the packet</param>
    /// <param name="_msg">Message to be sent</param>

    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>
    /// Builds packet used to spawn player into client game scene
    /// </summary>
    /// <param name="_toClient">Client that will receive the packet, the player that is being spawned</param>
    /// <param name="_player">Player object to be spawned</param>

    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>
    /// Builds packet used for chainging player position across all clients
    /// </summary>
    /// <param name="_player">Player whose movement is being updated</param>

    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    /// <summary>
    /// Builds packet used for chainging player position across all clients except player that is rotating, to prevent jittering. Rotation is client authoritative
    /// </summary>
    /// <param name="_player">Player whose rotation is being updated</param>

    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    /// <summary>
    /// Set position of object
    /// </summary>
    /// <param name="_object">Object to reposition</param>

    public static void ObjectPosition(BasicObject _object)
    {
        using (Packet _packet = new Packet((int)ServerPackets.objectPosition))
        {
            _packet.Write(_object.id);
            _packet.Write(_object.currentPosition);

            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>
    /// Set rotation of object
    /// </summary>
    /// <param name="_object">Object to rotate</param>

    public static void ObjectRotation(BasicObject _object)
    {
        using (Packet _packet = new Packet((int)ServerPackets.objectRotation))
        {
            _packet.Write(_object.id);
            _packet.Write(_object.currentRotation);

            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>
    /// Builds packet to indicate a player has disconnected
    /// </summary>
    /// <param name="_playerId">Id of disconnected player</param>

    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);
            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>
    /// Builds packet to update player's health value
    /// </summary>
    /// <param name="_player">Player whose health has bene modified</param>

    public static void PlayerHealth(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerHealth))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.health);
            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>
    /// Builds packet to respawn player
    /// </summary>
    /// <param name="_player">Player being respawned</param>

    public static void PlayerRespawned(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRespawned))
        {
            _packet.Write(_player.id);

            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>
    /// Set initial position values for all objects in scene for newly connected client
    /// </summary>
    /// <param name="_id">Id of client to receive packets</param>
    /// <param name="_objects">Dictionary of object whose states need to be sent</param>

    public static void AllObjectPosition(int _id, Dictionary<string, BasicObject> _objects)
    {
        foreach (var item in _objects)
        {
            using (Packet _packet = new Packet((int)ServerPackets.objectInitialPosition))
            {
                _packet.Write(item.Key);
                _packet.Write(item.Value.isActive);
                _packet.Write(item.Value.currentPosition);

                SendTCPData(_id, _packet);
            }
        }
    }

    /// <summary>
    /// Set initial rotation values for all objects in scene for newly connected client
    /// </summary>
    /// <param name="_id">Id of client to receive packets</param>
    /// <param name="_objects">Dictionary of object whose states need to be sent</param>

    public static void AllObjectRotation(int _id, Dictionary<string, BasicObject> _objects)
    {
        foreach (var item in _objects)
        {
            using (Packet _packet = new Packet((int)ServerPackets.objectInitialRotation))
            {
                _packet.Write(item.Key);
                _packet.Write(item.Value.isActive);
                _packet.Write(item.Value.currentRotation);

                SendTCPData(_id, _packet);
            }
        }
    }
}
