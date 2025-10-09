using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[DeathZone] Player chạm DeathZone → trừ máu");
            other.GetComponent<PlayerController>()?.Die();
        }
    }
}
