using UnityEngine;

public class WorldCamera : MonoBehaviour
{
    public static WorldCamera Instance;
    private void Awake()
    {
        Instance = this;
    }

    public GameObject playerPov;

    public void BindTo(GameObject _gameObject)
    {
        gameObject.transform.parent = _gameObject.transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }

    public void BindToLocalPOV()
    {
        gameObject.transform.parent = playerPov.transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }
}
