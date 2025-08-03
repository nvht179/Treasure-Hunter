using UnityEngine;

public class FlyingObject : MonoBehaviour {

    [SerializeField] protected FlyingObjectSO flyingObjectSO;
    [SerializeField] protected LayerMask collisionLayer;

    private Rigidbody2D rb;
    private Vector2 direction;
    private bool willDestroy;
    private float destroyTimer;
    protected float DestroyDelay;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.velocity = direction * flyingObjectSO.speed;
        }

        willDestroy = false;
    }

    private void Update() {
        if (willDestroy) {
            destroyTimer += Time.deltaTime;
            if (destroyTimer >= DestroyDelay) {
                Destroy(gameObject);
            }
        }
    }

    protected void StopFlying() {
        rb.velocity = Vector2.zero;
        willDestroy = true;
        destroyTimer = 0f;
    }

    protected void DamageObject(IDamageable damageable) {
        var offenderInfo = new IDamageable.DamageInfo
        {
            Damage = flyingObjectSO.damage,
            Force = flyingObjectSO.force,
            Velocity = rb.velocity
        };
        damageable?.TakeDamage(offenderInfo);
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction = direction.normalized;
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.velocity = velocity;
    }
}
