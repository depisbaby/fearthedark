using AtomicConsole;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public float tileSize;
    public float mapOffset;
    [HideInInspector]public Room[,] baseMap;
    public int mapSize;
    public List<Room> roomLibrary;
    public List<Room> startingRooms;
    public bool isMapGenerated;
    bool isGenerating;
    List<Vector2> openPositions = new List<Vector2>();
    public Room startingRoom;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitLibrary();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    [AtomicCommand(name: "genmap", description: "")]
    public async void GenerateMap()
    {
        if (!IsServer) return;
        if (isMapGenerated) return;
        if (isGenerating) return;

        isGenerating = true;

        baseMap = new Room[mapSize, mapSize];
        startingRoom = PlaceRoom(startingRooms[0], mapSize / 2, 0, mapSize / 2, Random.Range(0,4));

        Debug.Log("Placing rooms...");
        while (openPositions.Count > 0)
        {
            await Task.Delay(1);
            Debug.Log("Number of open positions: "+ openPositions.Count);
            Vector2 pos = openPositions[Random.Range(0,openPositions.Count-1)];
            ResolveOpenPosition(pos);
        }

        Debug.Log("Map generated!");

        isMapGenerated = true;
    }

    Room PlaceRoom(Room roomReference, int x, int y, int z, int rotation)
    {
        Room newRoom = Instantiate(roomReference.gameObject).GetComponent<Room>();
        newRoom.Init();
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
            Debug.Log("hep");
            if(connection != Room.ConnectionType.NoConnection)
            {
                Vector2 pos = new Vector2(x, z) + newRoom.DirectionOfSide(i);
                if (baseMap[(int)pos.x, (int)pos.y] == null && !openPositions.Contains(pos) && !PositionOutOfBounds(pos))
                {
                    openPositions.Add(pos);
                }
            }
            i++;
        }

        NetworkObject no = newRoom.gameObject.GetComponent<NetworkObject>();
        no.Spawn();
        newRoom.NetworkInit();

        Debug.Log("Placed a room.");
        return newRoom;
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

                if (neighbors[0] != Room.ConnectionType.ET) {
                    if (neighbors[0] != room.connections[0]) continue;
                }
                if (neighbors[1] != Room.ConnectionType.ET)
                {
                    if (neighbors[1] != room.connections[1]) continue;
                }
                if (neighbors[2] != Room.ConnectionType.ET)
                {
                    if (neighbors[2] != room.connections[2]) continue;
                }
                if (neighbors[3] != Room.ConnectionType.ET)
                {
                    if (neighbors[3] != room.connections[3]) continue;
                }

                fittingRooms.Add(room);
                fittingRotations.Add(i);
                
            }
        }

        if(fittingRooms.Count == 0)
        {
            Debug.LogError("There is a position with fitting tiles configured. Tile with following sides required: " + neighbors[0] + ", " + neighbors[1] + ", " + neighbors[2] + ", " + neighbors[3]);
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
        Vector2[] vectors = { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };
        int[] oppositeSideMapping = { 2, 3, 0, 1 };
        for (int i = 0; i < 4; i++)
        {
            Vector2 neighborPos = pos + vectors[i];
            if (PositionOutOfBounds(neighborPos))
            {
                neighbors[i] = Room.ConnectionType.NoConnection;
            }
            else if (baseMap[(int)neighborPos.x, (int)neighborPos.y] == null)
            {
                neighbors[i] = Room.ConnectionType.ET;
            }
            else
            {
                neighbors[i] = baseMap[(int)neighborPos.x, (int)neighborPos.y].connections[oppositeSideMapping[i]];
            }
        }
        return neighbors;
    }

    void InitLibrary()
    {
        foreach (Room room in roomLibrary)
        {
            room.Init();
        }

        foreach (Room room in startingRooms)
        {
            room.Init();
        }
    }

}
