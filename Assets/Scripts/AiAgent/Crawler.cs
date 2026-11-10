using UnityEngine;

public class Crawler : AiAgent
{
    public Player hunted;

    protected override void AiBehaviour()
    {

        CheckHunting();

        if(hunted != null)
        {
            SetDestination(hunted.transform.position);
            return;
        }

        //Debug.Log("Ai behavi");
        if (state == AiState.None)
        {
            
            if (Random.Range(0,4)==0) //25%
            {
                Roam();
            }
            else 
            {
                Idle(5*1000);
            }
        }
    }

    protected override void ChildStart()
    {
        anim.speed = 1.5f;
    }

    void CheckHunting()
    {
        if(AiManager.Instance.playerOutOfSafety.Count > 0)
        {
            if (AiManager.Instance.playerOutOfSafety.Contains(hunted))
            {
                return;
            }

            foreach(Player player in AiManager.Instance.playerOutOfSafety)
            {
                if (player.isDead.Value) continue;
                hunted = player;
            }
        }
        else
        {
            hunted = null;
        }
    }

}
