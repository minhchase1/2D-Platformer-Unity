using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHP = 3;
    public int currentHP;
    public UnityEvent onHurt;
    public UnityEvent onDie;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        onHurt?.Invoke();
        if (currentHP <= 0)
        {
            currentHP = 0;
            onDie?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
    }
}
