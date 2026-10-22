using UnityEngine;

public class Crawler : AiAgent
{
    protected override void AiBehaviour()
    {
        //Debug.Log("Ai behavi");
        if (state == AiState.None)
        {
            if(AiManager.Instance.playerOutOfSafety.Count != 0 && AiManager.Instance.playerOutOfSafety[0] != null)
            {
                SetDestination(AiManager.Instance.playerOutOfSafety[0].transform.position);
            }
        }
    }

    protected override void ChildStart()
    {
        anim.speed = 1.5f;
    }
}
