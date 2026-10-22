using UnityEngine;

public class FollowingSafetyArea : MonoBehaviour
{
    public GameObject followedObject;

    // Update is called once per frame
    void Update()
    {
        transform.position = followedObject.transform.position;
    }
}
