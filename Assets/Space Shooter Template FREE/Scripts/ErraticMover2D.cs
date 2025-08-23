using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ErraticMover2D : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Units per second")]
    public float speed = 3f;

    [Tooltip("How often to pick a new random direction (seconds).")]
    public float changeInterval = 2f;

    [Header("Bounds")]
    [Tooltip("A BoxCollider2D that defines the playable rectangle (the border of the level).")]
    public BoxCollider2D levelBounds;

    [Tooltip("Extra inset from the bounds edges (useful if your border graphics overlap the collider).")]
    public float edgeInset = 0f;

    private Rigidbody2D rb;
    private Vector2 dir;
    private float timer;

    // Cached sizes
    private float radius;         // approximate radius of this object
    private Bounds worldBounds;   // level bounds in world space

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Estimate radius from a Collider2D if present, otherwise from Renderer bounds
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            var e = col.bounds.extents;
            radius = Mathf.Max(e.x, e.y);
        }
        else
        {
            var rend = GetComponent<Renderer>();
            if (rend != null)
            {
                var e = rend.bounds.extents;
                radius = Mathf.Max(e.x, e.y);
            }
            else radius = 0.25f; // fallback
        }

        PickNewDirection(true);
        RefreshWorldBounds();
    }

    void Update()
    {
        if (levelBounds == null) return;

        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            PickNewDirection(false);
            timer = 0f;
        }
    }

    void FixedUpdate()
    {
        if (levelBounds == null) return;

        // Predict next position
        Vector2 pos = rb.position;
        Vector2 vel = dir * speed;
        Vector2 nextPos = pos + vel * Time.fixedDeltaTime;

        // Clamp reflect against bounds (immediate switch on touch)
        Vector2 reflected = dir;
        bool touched = false;

        float minX = worldBounds.min.x + edgeInset + radius;
        float maxX = worldBounds.max.x - edgeInset - radius;
        float minY = worldBounds.min.y + edgeInset + radius;
        float maxY = worldBounds.max.y - edgeInset - radius;

        // If the next step would cross the border, flip that axis now
        if (nextPos.x < minX || nextPos.x > maxX)
        {
            reflected.x = -reflected.x;
            touched = true;
            // Keep within bounds
            nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
        }
        if (nextPos.y < minY || nextPos.y > maxY)
        {
            reflected.y = -reflected.y;
            touched = true;
            nextPos.y = Mathf.Clamp(nextPos.y, minY, maxY);
        }

        if (touched)
        {
            dir = reflected.normalized;
            vel = dir * speed;
        }

        // Apply velocity
        rb.linearVelocity = vel;

        // Keep the rigidbody inside in case of drift
        rb.position = new Vector2(
            Mathf.Clamp(rb.position.x, minX, maxX),
            Mathf.Clamp(rb.position.y, minY, maxY)
        );
    }

    private void PickNewDirection(bool allowAny)
    {
        // Random non-zero direction; avoid extremely tiny vectors
        Vector2 newDir;
        int safety = 0;
        do
        {
            newDir = Random.insideUnitCircle.normalized;
            safety++;
        } while (newDir.sqrMagnitude < 0.01f && safety < 8);

        // Optional: avoid picking a direction almost identical to current, for “erratic” feel
        if (!allowAny && Vector2.Dot(newDir, dir) > 0.95f)
        {
            newDir = Quaternion.Euler(0, 0, Random.Range(60f, 180f)) * newDir;
        }

        dir = newDir.normalized;
    }

    private void RefreshWorldBounds()
    {
        if (levelBounds != null)
            worldBounds = levelBounds.bounds; // converts collider local size to world space
    }

    void OnValidate()
    {
        changeInterval = Mathf.Max(0.05f, changeInterval);
        speed = Mathf.Max(0f, speed);
        edgeInset = Mathf.Max(0f, edgeInset);
        // Try to update cached bounds in edit-time
        if (levelBounds != null) worldBounds = levelBounds.bounds;
    }

    // Optional: reflect on collision normals as a backup if you also use physical borders
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contactCount > 0)
        {
            Vector2 normal = collision.GetContact(0).normal;
            dir = Vector2.Reflect(dir, normal).normalized;
            rb.linearVelocity = dir * speed;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize current direction and bounds edges considered for the center
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(dir.normalized * (radius + 0.5f)));

        if (levelBounds != null)
        {
            var b = levelBounds.bounds;
            float minX = b.min.x + edgeInset + radius;
            float maxX = b.max.x - edgeInset - radius;
            float minY = b.min.y + edgeInset + radius;
            float maxY = b.max.y - edgeInset - radius;

            // Draw inner rectangle where the object's center is allowed
            Vector3 a = new Vector3(minX, minY, 0);
            Vector3 c = new Vector3(maxX, maxY, 0);
            Vector3 b1 = new Vector3(c.x, a.y, 0);
            Vector3 d = new Vector3(a.x, c.y, 0);
            Gizmos.DrawLine(a, b1);
            Gizmos.DrawLine(b1, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }
    }
}
