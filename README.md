
# OSRS-VR

## OSRS VR Server and Client applications built in Unity 2019.2.9f1

### When committing code, please create a new feature branch. When you are ready to merge, please merge into the staging branch via Pull Request. Staging will be merged into Master after testing has confirmed completeness of new functionality.

### Please ensure all high importance data is sent via TCP and all data that is constinuously sent (ie. player position) is sent via UDP

### All scene specific items MUST be saved as a prefab to prevent merging issues. Please contact me if you need a non-prefab object added to the repository scenes

### All geometry must be places in both the client and server  scenes, and must have the EXACT SAME coordinates and rotation values

# Breakdown of current scripts (more information available in code comments)

## Server

### Client.cs
Main reference point to client application instance
Contains TCP and UDP connections and their implementations

### Constants.cs
Server timing variables. must be the same across server and client code

### NetworkManager.cs
Primary manager for server setup and player spawning

### Packet.cs
Helper methods for reading from and writing to packets, as well as enums containing packet types

### Player.cs
Helper methods for reading and applying player inputs and movement
Handles player damage and respawning

### Server.cs
Handles server state and client packet definitions

### ServerHandle.cs
Defines methods used for handing incoming data

### ServerSend.cs
Defines methods used for handling outgoing data

### ThreadManager.cs
Sets up actions that need to be executed on the main thread

## Client

### CameraController.cs
Unity Camera Controller for first person view

### Client.cs
Handles connection and disconnection logic
Initializes Server Packet dictionary for handling data

### ClientHandle.cs
Handles incoming traffic from server

### ClientSend.cs
Handles outgoing data to server

### GameManager.cs
Handles client scene management

### Packet.cs
Helper methods for reading from and writing to packets, as well as enums containing packet types

### PlayerController.cs
Handles player inputs for movement and shooting

### PlayerManager.cs
Handles player state

### ThreadManager.cs
Sets up actions that need to be executed on the main thread

### UIManager.cs
Handles changes to user interface