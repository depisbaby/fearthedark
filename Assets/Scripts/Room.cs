using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.UIElements;
using System.Linq;

public class Room : NetworkBehaviour
{
    public ConnectionType[] originalConnections;
    public ConnectionType[] connections;

    public Room()
    {
        connections = originalConnections.ToArray();
    }

    public enum ConnectionType
    {
        None = 0,
        Mansion = 1,
    }

    public Vector2 DirectionOfSide(int side)
    {
        switch (side) { 
            case 0:
                return new Vector2(1, 0);
                break;
            case 1:
                return new Vector2(0, 1);
                break;
            case 2:
                return new Vector2(-1, 0);
                break;
            case 3:
                return new Vector2(0, -1);
                break;
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

}
