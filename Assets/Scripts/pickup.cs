using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    public enum pickupType { coin, gem, health }

    public pickupType pt;
    [SerializeField] private GameObject PickupEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (pt)
            {
                case pickupType.coin:
                    GameManager.instance.IncrementCoinCount();
                    break;

                case pickupType.gem:
                    GameManager.instance.IncrementGemCount();
                    break;

                case pickupType.health:
                    // Nếu có health logic, gọi hàm tăng máu ở đây
                    break;
            }

            // Hiệu ứng pickup
            if (PickupEffect != null)
            {
                Instantiate(PickupEffect, transform.position, Quaternion.identity);
            }

            // Huỷ object sau 0.2s
            Destroy(gameObject, 0.2f);
        }
    }
}
