using AtomicConsole;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class MapManager : NetworkBehaviour
{
    public static MapManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public NetworkVariable<FixedString4096Bytes> seed = new NetworkVariable<FixedString4096Bytes>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public float tileSize;
    public float mapOffset;
    [HideInInspector]public Room[,] baseMap;
    public int mapSize;
    public List<Room> roomLibrary;
    public List<Room> startingRooms;
    bool isMapGenerated;
    bool isGenerating;
    List<Vector2> openPositions = new List<Vector2>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterMainPortal(bool fromHubSide)
    {
        if (seed.Value == "")
        {
            if (IsHost)
            {
                seed.Value = GenerateRandomString(50);
            }
            else
            {
                Debug.LogError("Seed not set.");
                return;
            }
        }

        if (fromHubSide) {
            if (!isMapGenerated)
            {
                GenerateMap(seed.Value.ToString());
            }
        }
    }

    string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string result = "";

        for (int i = 0; i < length; i++)
        {
            result += chars[Random.Range(0, chars.Length)];
        }

        return result;
    }

    [AtomicCommand(name: "genmap", description: "")]
    public async void GenerateMap(string seed)
    {
        if (!IsServer) return;
        if (isMapGenerated) return;
        if (isGenerating) return;
        isGenerating = true;
        baseMap = new Room[mapSize, mapSize];
        PlaceRoom(startingRooms[0], mapSize / 2, 0, mapSize / 2, 0);

        Random.InitState(seed.GetHashCode());

        while (openPositions.Count > 0)
        {
            Vector2 pos = openPositions[Random.Range(0,openPositions.Count-1)];
            ResolveOpenPosition(pos);
        }

    }

    void PlaceRoom(Room roomReference, int x, int y, int z, int rotation)
    {
        Room newRoom = Instantiate(roomReference.gameObject).GetComponent<Room>();
        NetworkObject no = newRoom.gameObject.GetComponent<NetworkObject>();
        no.Spawn();
        newRoom.transform.position = new Vector3(x*tileSize + mapOffset, y * tileSize, z * tileSize + mapOffset);
        newRoom.transform.rotation = Quaternion.Euler(0, rotation * 90, 0);
        newRoom.SetRotation(rotation);
        baseMap[x,z] = newRoom;
        
        if(openPositions.Contains(new Vector2(x, z)))
        {
            openPositions.Remove(new Vector2(x, z));
        }

        int i = 0;
        foreach(Room.ConnectionType connection in newRoom.connections)
        {
            if(connection != Room.ConnectionType.None)
            {
                Vector2 pos = new Vector2(x, z) + newRoom.DirectionOfSide(i);
                if (baseMap[(int)pos.x, (int)pos.y]!= null && !openPositions.Contains(pos) && !PositionOutOfBounds(pos))
                {
                    openPositions.Add(pos);
                }
            }
            i++;
        }
    }

    void ResolveOpenPosition(Vector2 pos)
    {
        List<Room> fittingRooms = new List<Room>();
        List<int> fittingRotations = new List<int>();
        Room.ConnectionType[] neighbors = GetNeighbors(pos);
        
        foreach (Room room in roomLibrary)
        {
            for (int i = 0; i < 4; i++)
            {
                room.SetRotation(i);
                if(neighbors.SequenceEqual(room.connections))
                {
                    fittingRooms.Add(room);
                    fittingRotations.Add(i);
                }
            }
        }

        int rndm = Random.Range(0, fittingRooms.Count - 1);
        Room chosen = fittingRooms[rndm];
        int chosenRotation = fittingRotations[rndm];

        PlaceRoom(chosen, (int)pos.x, 0, (int)pos.y, chosenRotation);


    }

    bool PositionOutOfBounds(Vector2 pos) { 
        if(pos.x < 0 || pos.x >= mapSize)
        {
            return true;
        }
        if(pos.y < 0 || pos.y >= mapSize)
        {
            return true;
        }
        return false;
    }

    Room.ConnectionType[] GetNeighbors(Vector2 pos)
    {
        Room.ConnectionType[] neighbors = new Room.ConnectionType[4];
        Vector2[] vectors = { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) };
        int[] oppositeSideMapping = { 2, 3, 0, 1 };
        for (int i = 0; i < 4; i++)
        {
            Vector2 neighborPos = pos + vectors[i];
            if (baseMap[(int)neighborPos.x, (int)neighborPos.y] == null || PositionOutOfBounds(neighborPos))
            {
                neighbors[i] = Room.ConnectionType.None;
            }
            else
            {
                neighbors[i] = baseMap[(int)neighborPos.x, (int)neighborPos.y].connections[oppositeSideMapping[i]];
            }
        }
        return neighbors;
    }

}
