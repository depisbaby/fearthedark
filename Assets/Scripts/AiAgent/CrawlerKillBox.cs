using UnityEngine;

public class CrawlerKillBox : MonoBehaviour
{
    Crawler crawler;

    private void Awake()
    {
        crawler = transform.parent.gameObject.GetComponent<Crawler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!crawler.IsServer) return;

        if (other.gameObject.layer == 3)//is player
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player.isDead.Value != true)
            {
                //Debug.Log("TAPETTU");
                player.KillClientRPC();
            }
        }
    }

}
