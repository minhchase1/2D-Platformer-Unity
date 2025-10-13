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
                    // THAY ĐỔI Ở ĐÂY: Gọi đúng tên hàm mới trong GameManager
                    GameManager.instance.CollectCoin();
                    break;

                case pickupType.gem:
                    // Hàm này vẫn giữ nguyên tên nên không cần sửa
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

            // Hủy object
            // Lưu ý: nên hủy ngay lập tức và tắt renderer/collider nếu muốn có hiệu ứng trễ
            // Destroy(gameObject, 0.2f); // Cách này có thể gây lỗi nhặt nhiều lần
            Destroy(gameObject); // Hủy ngay lập tức sẽ an toàn hơn
        }
    }
}