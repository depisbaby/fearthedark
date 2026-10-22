using AtomicConsole;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class ItemManager : NetworkBehaviour
{
    

    [SerializeField] private List<Item> itemLibrary = new List<Item>();
    [SerializeField] private List<Item> items = new List<Item>();


    public static ItemManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    [AtomicCommand(name: "spawnitem", description: "")]
    public void SpawnItem(string name, float x, float y, float z)
    {
        if (!IsServer)
        {
            SpawnItemServerRpc(name, x, y, z);
            return;
        }

        foreach (Item item in itemLibrary)
        {
            if(item.itemName == name)
            {
                GameObject go = Instantiate(item.gameObject);
                NetworkObject no = go.GetComponent<NetworkObject>();
                no.Spawn();
                no.gameObject.transform.position = new Vector3(x, y, z);
                Item newItem =go.GetComponent<Item>();
                items.Add(newItem);
            }
        }
    }

    [ServerRpc]
    public void SpawnItemServerRpc(string name, float x, float y, float z)
    {
        SpawnItem(name,x,y,z);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void PlayerPicksUpItemServerRpc(ulong clientId, ulong objectId)
    {
        NetworkObject no;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out no);

        
        if (no != null) {
            
            Item item = no.gameObject.GetComponent<Item>();
            if (item != null && !item.isPickedUp)
            {
                item.isPickedUp = true;
                no.ChangeOwnership(clientId);
                PlayerPicksUpItemClientRpc(clientId, objectId);
            }
        }
        else{
            Debug.LogError("NetworkObject null");
        }
    }

    [ClientRpc] void PlayerPicksUpItemClientRpc(ulong clientId, ulong objectId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        NetworkObject no;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out no);

        if (no != null)
        {
            Item item = no.gameObject.GetComponent<Item>();
            if (item != null)
            {
                GameManager.Instance.localPlayer.LocalPickUp(item);
            }
        }

    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void PlayerDroppedItemServerRpc(ulong objectId)
    {
        NetworkObject no;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out no);

        if (no != null)
        {
            Item item = no.gameObject.GetComponent<Item>();
            item.isPickedUp = false;
            
        }
    }

    public void DespawnItem(Item item)
    {
        items.Remove(item);
        NetworkObject no = item.GetComponent<NetworkObject>();
        no.Despawn();
        //Destroy(no.gameObject);
    }

}
