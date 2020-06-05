using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Reference to a connected client application
/// </summary>

public class Client
{
    public static int dataBufferSize = 4096;

    public int id;
    public TCP tcp;
    public UDP udp;
    public Player player;

    public Client(int _clientId)
    {
        id = _clientId;
        tcp = new TCP(id);
        udp = new UDP(id);
    }

    /// <summary>
    /// Represents a TCP connection attached to the parent Client object
    /// </summary>

    public class TCP
    {
        public TcpClient socket;

        private readonly int id;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public TCP(int _id)
        {
            id = _id;
        }

        /// <summary>
        /// Set up initial TCP connection 
        /// </summary>
        /// <param name="_socket">TCP Client to be used for connection</param>

        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();

            receivedData = new Packet();
            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            ServerSend.Welcome(id, "Welcome to the server.");
        }

        /// <summary>
        /// Send a packet via TCP using a Network Stream
        /// </summary>
        /// <param name="_packet">Packet to be sent via TCP</param>

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to player {id} via TCP: {_ex}");
            }
        }

        /// <summary>
        /// Called whenever a TCP packet is received, begings read actions
        /// </summary>
        /// <param name="_result">Incoming stream passed from callback</param>

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Server.clients[id].Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiving TCP data: {_ex}");
                Server.clients[id].Disconnect();
            }
        }

        /// <summary>
        /// Handles incoming TCP packets using packet dictionary and methods in Packet.cs
        /// </summary>
        /// <param name="_data">Byte array containing contents of the packet</param>
        /// <returns></returns>

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Terminates TCP connection when a player disconnects, allowing a new player to take the connection slot
        /// </summary>

        public void Disconnect()
        {
            socket.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    /// <summary>
    /// represents a UDP connection attached to the parent Client object
    /// </summary>

    public class UDP
    {
        public IPEndPoint endPoint;
        private int id;

        public UDP(int _id)
        {
            id = _id;
        }

        /// <summary>
        /// Initializes UDP connection
        /// </summary>
        /// <param name="_endPoint">Endpoint used for the duration of the connection</param>

        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
        }

        /// <summary>
        /// Sends data via UDP packet to client
        /// </summary>
        /// <param name="_packet">Data to be sent</param>

        public void SendData(Packet _packet)
        {
            Server.SendUDPData(endPoint, _packet);
        }

        /// <summary>
        /// Handles initial read of incoming packet
        /// </summary>
        /// <param name="_packetData">Data received from client</param>

        public void HandleData(Packet _packetData)
        {
            int _packetLength = _packetData.ReadInt();
            byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    Server.packetHandlers[_packetId](id, _packet);
                }
            });
        }

        /// <summary>
        /// terminates the current UDP connection
        /// </summary>

        public void Disconnect()
        {
            endPoint = null;
        }
    }

    /// <summary>
    /// Initializes new player in game environment and sets up client initialization packet
    /// </summary>
    /// <param name="_playerName"></param>

    public void SendIntoGame(string _playerName)
    {
        player = NetworkManager.instance.InstantiatePlayer();
        player.Initialize(id, _playerName);

        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                if (_client.id != id)
                {
                    ServerSend.SpawnPlayer(id, _client.player);
                }
            }
        }

        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                ServerSend.SpawnPlayer(_client.id, player);
            }
        }
    }

    /// <summary>
    /// Terminates TCP and UDP connections and sends disconnect packet to all other connected players
    /// </summary>

    private void Disconnect()
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(player.gameObject);
            player = null;
        });

        tcp.Disconnect();
        udp.Disconnect();

        ServerSend.PlayerDisconnected(id);
    }
}
