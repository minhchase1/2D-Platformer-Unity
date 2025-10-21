using UnityEngine;

public class PortalAnimation : MonoBehaviour
{
    public float rotationSpeed = 100f; // Tốc độ xoay (độ trên giây)

    void Update()
    {
        // Xoay GameObject quanh trục Z
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}