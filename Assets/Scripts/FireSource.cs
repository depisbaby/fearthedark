using UnityEngine;

public class FireSource : MonoBehaviour
{
    public bool hasFire;
    public bool refreshed;

    private void OnTriggerEnter(Collider other)
    {
        FireSource fire = other.transform.gameObject.GetComponent<FireSource>();
        if(fire != null)
        {
            if(hasFire)
            {
                fire.refreshed = true;
            }
        }

    }
}
