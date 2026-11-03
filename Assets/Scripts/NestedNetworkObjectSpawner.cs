using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class NestedNetworkObjectSpawner : MonoBehaviour
{
    public GameObject _gameObject;

    public async void SpawnNestedObject()
    {
        if(_gameObject.GetComponent<Item>() != null)
        {
            //Debug.LogWarning("hep");
            await Task.Delay(1000);
        }

        GameObject go = Instantiate(_gameObject);
        NetworkObject no = go.GetComponent<NetworkObject>();
        no.Spawn();
        Vector3 pos = transform.position;
        while((pos-go.transform.position).magnitude > 1.0f)
        {
            go.transform.position = pos;
            await Task.Delay(100);
        }
        go.transform.rotation = transform.rotation;

        Destroy(this.gameObject);
    }
}
