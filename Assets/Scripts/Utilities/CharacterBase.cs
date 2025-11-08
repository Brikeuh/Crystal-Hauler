using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    private string _characterName;
    private int _health;
    private int _attackPower;
    private float _movementSpeed;

    public string CharacterName
    {
        get { return _characterName; }
        set { _characterName = value; } // Simple assignment
    }

    public int Health
    {
        get { return _health; }
        set
        {
            // Example of validation in a setter
            if (value < 0)
            {
                _health = 0;
            }
            else if (value > 100) // Assuming max health is 100
            {
                _health = 100;
            }
            else
            {
                _health = value;
            }
        }
    }

    public int AttackPower
    {
        get { return _attackPower; }
        set { _attackPower = value; }
    }

    public float MovementSpeed
    {
        get { return _movementSpeed; }
        set { _movementSpeed = value; }
    }

    // Constructor (optional, but good practice)
    public CharacterBase(int health, int attack, float speed)
    {
        Health = health;
        AttackPower = attack;
        MovementSpeed = speed;
    }

    // Example of a method that might use these properties
    public void TakeDamage(int damageAmount)
    {
        Health -= damageAmount;
        if (Health <= 0)
        {
            Die();
        }
        Debug.Log($"{_characterName} took {damageAmount} damage. Current health: {Health}");
    }

    protected virtual void Die()
    {
        Debug.Log($"{_characterName} has died.");
        // Implement death logic (e.g., disable GameObject, play death animation)
        Destroy(gameObject);
    }
}
