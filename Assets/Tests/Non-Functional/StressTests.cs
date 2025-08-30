
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StressTests
{
    // ---- Shared creators (self-contained for this file) ----
    const string FX_NAME = "NF_Test FX";
    const string FX_CLONE = FX_NAME + "(Clone)";
    const string PROJ_NAME = "NF_Test Projectile";
    const string PROJ_CLONE = PROJ_NAME + "(Clone)";
    const string BOUNDS_NAME = "NF_Test Bounds";

    static GameObject CreateSimpleFX() => new GameObject(FX_NAME);
    static GameObject CreateSimpleProjectile() => new GameObject(PROJ_NAME);

    static BoxCollider2D CreateBounds(float w = 30f, float h = 18f)
    {
        var go = new GameObject(BOUNDS_NAME);
        var box = go.AddComponent<BoxCollider2D>();
        box.size = new Vector2(w, h);
        return box;
    }

    static Enemy CreateMinimalEnemy()
    {
        var go = new GameObject("NF_Enemy", typeof(Enemy));
        var e = go.GetComponent<Enemy>();
        e.health = 1;
        e.Projectile = CreateSimpleProjectile();
        e.destructionVFX = CreateSimpleFX();
        e.hitEffect = new GameObject("NF_HitFX");
        return e;
    }

    static BossEnemy CreateMinimalBoss()
    {
        var go = new GameObject("NF_Boss", typeof(BossEnemy), typeof(Rigidbody2D), typeof(CircleCollider2D));
        var boss = go.GetComponent<BossEnemy>();
        boss.destructionVFX = CreateSimpleFX();
        boss.Projectile = CreateSimpleProjectile();
        var bounds = CreateBounds();
        boss.InitMovement(bounds);
        return boss;
    }

    static GameObject[] FindAllByExactName(string exact) =>
        Object.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
              .Where(g => g && g.name == exact).ToArray();

    // ---- Cleanup after each test ----
    [UnityTearDown]
    public IEnumerator GlobalTearDown()
    {
        var names = new[]
        {
            FX_NAME, FX_CLONE, PROJ_NAME, PROJ_CLONE, BOUNDS_NAME,
            "NF_Enemy", "NF_HitFX", "NF_Boss"
        };

        foreach (var n in names)
        {
            foreach (var go in FindAllByExactName(n))
                if (go) Object.Destroy(go);
        }

        yield return null;
        foreach (var g in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (g && (g.name.Contains("NF_") || g.name.Contains("(Clone)")))
                Object.DestroyImmediate(g);
        }
        yield return null;
    }

    // -------------------------------------------------------------------------
    // STRESS TESTS
    // -------------------------------------------------------------------------

    [UnityTest, Category("Stress")]
    public IEnumerator Stress_BossRapidDamage_OnlyOneDestructionFX()
    {
        var boss = CreateMinimalBoss();
        yield return null;

        for (int i = 0; i < 50; i++) boss.GetDamage(999999);
        yield return null;

        var bossAlive = Object.FindObjectsByType<BossEnemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Any();
        Assert.IsFalse(bossAlive, "Boss should be destroyed after rapid damage.");

        int fxCount = FindAllByExactName(FX_CLONE).Length;
        Assert.AreEqual(1, fxCount, $"Expected 1 destruction FX, found {fxCount}");
    }

    [UnityTest, Category("Stress")]
    public IEnumerator Stress_EnemyShootForever_For3Seconds_ReasonableProjectileCount()
    {
        var e = CreateMinimalEnemy();
        e.shootForever = true;
        e.shotChance = 100;
        yield return null;

        float t = 0f;
        while (t < 3.25f)
        {
            t += Time.deltaTime;
            yield return null;
        }

        int projCount = FindAllByExactName(PROJ_CLONE).Length;
        Assert.LessOrEqual(projCount, 5, $"Unexpected projectile spam: {projCount}");
        Assert.GreaterOrEqual(projCount, 2, $"Too few projectiles: {projCount}");
    }

    [UnityTest, Category("Stress")]
    public IEnumerator Stress_CreateAndDestroy_1000Enemies_NoLingeringObjects()
    {
        const int N = 1000;

        for (int i = 0; i < N; i++)
        {
            var e = CreateMinimalEnemy();
            if (i % 50 == 0) yield return null;
            if (i % 2 == 0) Object.Destroy(e.gameObject);
        }

        for (int i = 0; i < 5; i++) yield return null;

        foreach (var e in Object.FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (e) Object.DestroyImmediate(e.gameObject);

        yield return null;

        var leftovers = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                              .Count(g => g && g.name == "NF_Enemy");
        Assert.AreEqual(0, leftovers, $"Found lingering NF_Enemy objects: {leftovers}");
    }
}
