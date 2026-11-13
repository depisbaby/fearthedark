using UnityEngine;

public class WorldCamera : MonoBehaviour
{
    public static WorldCamera Instance;
    private void Awake()
    {
        Instance = this;
    }

    public GameObject playerPov;
    public Player playerSpectated;

    private void Update()
    {
        if(playerSpectated != null)
        {
            gameObject.transform.position = playerSpectated.cameraPosition.position;
            gameObject.transform.rotation = Quaternion.LookRotation(playerSpectated.lookingDirection.Value);
        }
    }

    public void BindTo(GameObject _gameObject)
    {
        gameObject.transform.parent = _gameObject.transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }

    public void BindToLocalPOV()
    {
        if (playerSpectated)
        {
            playerSpectated = null;
        }

        gameObject.transform.parent = playerPov.transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }

    public void StartSpectating(Player player)
    {
        playerSpectated = player;
        gameObject.transform.position = playerSpectated.cameraPosition.position;
        gameObject.transform.rotation = Quaternion.LookRotation(playerSpectated.lookingDirection.Value);
    }
}
