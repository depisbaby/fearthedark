using Unity.Netcode;
using UnityEngine;

public class Torch : Item
{
    public NetworkVariable<int> fuel = new NetworkVariable<int>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private Light light;
    [SerializeField] private FireSource fireSource;
    [SerializeField] private GameObject safetyArea;
    [SerializeField] private GameObject followingSafetyArea;
    public int maxFuel;
    int tick;
    

    protected override void ChildOnNetworkSpawn()
    {
        if (IsServer)
        {
            fuel.Value = maxFuel;
        }

        tick = Random.Range(0, 9999);
        safetyArea = Instantiate(followingSafetyArea);
        safetyArea.GetComponent<FollowingSafetyArea>().followedObject = this.gameObject;

    }
    
    protected override void ChildFixedUpdate()
    {
        tick++;
        if (IsServer)
        {

            if (fuel.Value > 0)
            {
                fireSource.hasFire = true;
            }
            else
            {
                fireSource.hasFire = false;
            }

            if (tick % (50*10) == 0 && fuel.Value > 0)
            {
                fuel.Value--;
            }
        }

        if (tick % 50 == 0)
        {
            if (safetyArea == null) return;
            if (fuel.Value <= 0 && safetyArea.activeSelf)
            {
                safetyArea.SetActive(false);
            }
            else if (fuel.Value > 0 && !safetyArea.activeSelf)
            {
                safetyArea.SetActive(true);
            }
        }
    }

    protected override void ChildUpdate()
    {
        if (IsServer)
        {
            if (fireSource.refreshed == true)
            {
                fireSource.refreshed = false;
                fuel.Value = maxFuel;
            }

        }

        if (!IsClient) return;

        float percent = (float)fuel.Value / (float)maxFuel;
        float target = percent;
        light.intensity = Mathf.Lerp(light.intensity, target, Time.deltaTime);
        
    }

    protected override void ChildOnDestroy()
    {
        Destroy(safetyArea);
    }

}
