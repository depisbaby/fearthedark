using Unity.Netcode;
using UnityEngine;

public class NestedNetworkObjectSpawner : MonoBehaviour
{
    public GameObject _gameObject;

    public void SpawnNestedObject()
    {
        GameObject go = Instantiate(_gameObject);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        NetworkObject no = go.GetComponent<NetworkObject>();
        no.Spawn();
    }
}
