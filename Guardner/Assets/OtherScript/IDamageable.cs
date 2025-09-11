using UnityEngine;

public interface IDamageable
{
    void Ondamage(float damage, Vector2 hitPoint, Vector2 hitNormal);

    void Die();
}
