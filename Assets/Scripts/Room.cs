using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

public class Room : NetworkBehaviour
{
    public ConnectionType[] originalConnections;
    [HideInInspector]public ConnectionType[] connections;

    public List<NestedNetworkObjectSpawner> nestedNetworkObjects;

    public enum ConnectionType
    {
        ET = 0, //Empty tile. Do not use this when assigning side connections of a room in the inspector.
        NoConnection = 1,
        Mansion = 2,
    }

    public Vector2 DirectionOfSide(int side)
    {
        switch (side) { 
            case 0:
                return new Vector2(0, 1);
            case 1:
                return new Vector2(1, 0);
            case 2:
                return new Vector2(0, -1);
            case 3:
                return new Vector2(-1, 0);
            default: return new Vector2(0, 0);
        }
        
    }

    public void SetRotation(int rotation)
    {
        switch (rotation)
        {
            case 1:
                connections[0] = originalConnections[3];
                connections[1] = originalConnections[0];
                connections[2] = originalConnections[1];
                connections[3] = originalConnections[2];
                break;
            case 2:
                connections[0] = originalConnections[2];
                connections[1] = originalConnections[3];
                connections[2] = originalConnections[0];
                connections[3] = originalConnections[1];
                break;
            case 3:
                connections[0] = originalConnections[1];
                connections[1] = originalConnections[2];
                connections[2] = originalConnections[3];
                connections[3] = originalConnections[0];
                break;
            default:
                connections[0] = originalConnections[0];
                connections[1] = originalConnections[1];
                connections[2] = originalConnections[2];
                connections[3] = originalConnections[3];
                break;
        }
    }

    public void Init()
    {
        connections = new ConnectionType[4];
        connections[0] = originalConnections[0];
        connections[1] = originalConnections[1];
        connections[2] = originalConnections[2];
        connections[3] = originalConnections[3];
    }

    public void NetworkInit()
    {
        if (IsServer)
        {
            foreach (NestedNetworkObjectSpawner networkObject in nestedNetworkObjects)
            {
                networkObject.SpawnNestedObject();
                //networkObject.TrySetParent(GetComponent<NetworkObject>(), true);
            }
        }
    }

}
