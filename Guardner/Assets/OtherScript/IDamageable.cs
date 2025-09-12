using UnityEngine;

public interface IDamageable
{
    void Ondamage(int damage, Vector2 hitPoint, Vector2 hitNormal);

    void Die();
}
