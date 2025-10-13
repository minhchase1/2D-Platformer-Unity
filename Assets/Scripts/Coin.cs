using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // THAY ĐỔI: Gọi hàm mới để đếm coin trong màn hiện tại
            GameManager.instance.CollectCoin();
            Destroy(gameObject);
        }
    }
}