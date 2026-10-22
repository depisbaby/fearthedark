using Unity.Netcode;
using UnityEngine;

public class MapTorch : NetworkBehaviour
{
    public NetworkVariable<int> fuel = new NetworkVariable<int>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private Light light;
    [SerializeField] private FireSource fireSource;
    [SerializeField] private GameObject safetyArea;
    private bool inRange;
    int tick;
    public int maxFuel;
    public bool infiniteFuel;
    public bool fuelOnStart;
    public float renderDistance;




    private void Start()
    {
        tick = Random.Range(0, 9999);
    }

    public override void OnNetworkSpawn()
    {
        
        if (IsServer)
        {
            if (fuelOnStart)
            {
                fuel.Value = maxFuel;
            }
        }
    }

    private void FixedUpdate()
    {

        tick++;
        if (IsServer)
        {
            if(fuel.Value > 0)
            {
                fireSource.hasFire = true;
            }
            else
            {
                fireSource.hasFire = false;
            }

            if (tick % (50 * 10) == 0 && fuel.Value > 0)
            {

                if (!infiniteFuel)
                { 
                    fuel.Value--;
                }
            }
            
        }

        if(tick % 50 == 0)
        {
            if(fuel.Value <= 0 && safetyArea.activeSelf)
            {
                safetyArea.SetActive(false);
            }
            else if (fuel.Value > 0 && !safetyArea.activeSelf)
            {
                safetyArea.SetActive(true);
            }

            if (GameManager.Instance.localPlayer.transform != null)
            {
                float distance = (GameManager.Instance.localPlayer.transform.position- transform.position).magnitude;
                if((distance < renderDistance))
                {
                    inRange = true;
                }
                else
                {
                    inRange = false;
                }
            }

        }

        if (light.intensity < 0.1f && light.enabled)
        {
            light.enabled = false;
        }
        else if(light.intensity > 0.1f && !light.enabled)
        {
            light.enabled = true;
        }

    }

    private void Update()
    {
        if (IsServer)
        {
            if(fireSource.refreshed == true)
            {
                fireSource.refreshed = false;
                fuel.Value = maxFuel;
            }

        }

        if (!IsClient) return;

        if (!inRange)
        {
            float target = 0.0f;
            light.intensity = Mathf.Lerp(light.intensity, target, Time.deltaTime);
        }
        else
        {
            float percent = (float)fuel.Value / (float)maxFuel;
            float target = percent;
            light.intensity = Mathf.Lerp(light.intensity, target, Time.deltaTime);
        }

        
    }
}
