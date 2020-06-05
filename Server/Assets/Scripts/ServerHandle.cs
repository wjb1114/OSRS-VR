using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logic for handling incoming network data
/// </summary>

public class ServerHandle
{
    /// <summary>
    /// Handles initial connection by sending message received packet to newly connected client
    /// </summary>
    /// <param name="_fromClient">Id of client message is sent to</param>
    /// <param name="_packet">Packet containing data from client</param>

    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }

    /// <summary>
    /// Handles player movement inputs
    /// </summary>
    /// <param name="_fromClient">Client whose player needs to be moved</param>
    /// <param name="_packet">Packet containing movement data</param>

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
    }

    /// <summary>
    /// Handles players shooting another player
    /// </summary>
    /// <param name="_fromClient">Client whose player has fired a shot</param>
    /// <param name="_packet">Packet containing data needed to process shot</param>

    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.Shoot(_shootDirection);
    }
}
