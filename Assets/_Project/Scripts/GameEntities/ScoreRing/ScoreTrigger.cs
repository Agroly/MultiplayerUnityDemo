using Unity.Netcode;
using UnityEngine;
using VContainer;

public class ScoreTrigger : MonoBehaviour
{
    [SerializeField] public Transform spawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        var playerScore = other.GetComponentInParent<PlayerScore>();

        if (playerScore != null)
        {
            if (playerScore.IsOwner)
            {
                gameObject.SetActive(false);

                GetComponent<Collider>().enabled = false;

                playerScore.AddScoreServerRpc();

                playerScore.UpdateCheckpointServerRpc(spawnPoint.position);
            }
        }
    }
}