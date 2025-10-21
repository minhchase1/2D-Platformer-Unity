// CoinMagnet.cs
using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    // Khi có một coin đi vào vùng trigger của nam châm
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            // Ra lệnh cho coin đó bắt đầu bay về phía Player
            collision.GetComponent<Coin>().AttractToPlayer(transform.parent);
        }
    }
}