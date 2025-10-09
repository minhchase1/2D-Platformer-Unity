using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FloatingObject : MonoBehaviour
{
	public float floatStrength = 2f;      // lực nổi
	public float damping = 0.1f;          // làm trôi mượt hơn
	private Rigidbody2D rb;
	private bool isInWater = false;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (isInWater)
		{
			// Tác dụng lực nổi lên trên
			rb.AddForce(Vector2.up * floatStrength, ForceMode2D.Force);

			// Giảm vận tốc rơi để mô phỏng nước
			rb.velocity = new Vector2(rb.velocity.x * (1 - damping), rb.velocity.y * 0.8f);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Water"))
		{
			isInWater = true;
			rb.gravityScale = 0.2f; // nhẹ hơn khi trong nước
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Water"))
		{
			isInWater = false;
			rb.gravityScale = 1f;
		}
	}
}
