
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LoadTests
{
    // ---- Shared creators (self-contained for this file) ----
    const string FX_NAME = "NF_Test FX";
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
            FX_NAME, PROJ_NAME, PROJ_CLONE, BOUNDS_NAME,
            "NF_Enemy", "NF_HitFX", "NF_Boss", "NF_Bonus"
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
    // LOAD TESTS
    // -------------------------------------------------------------------------

    [UnityTest, Category("Load")]
    public IEnumerator Load_Spawn1000Enemies_AllAliveNoExceptions()
    {
        const int N = 1000;
        for (int i = 0; i < N; i++) CreateMinimalEnemy();
        yield return null;

        var alive = Object.FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
        Assert.AreEqual(N, alive, $"Expected {N} Enemies, found {alive}");
    }

    [UnityTest, Category("Load")]
    public IEnumerator Load_BossFires500Projectiles_AllSpawned()
    {
        var boss = CreateMinimalBoss();
        yield return null;

        for (int i = 0; i < 500; i++) boss.ActivateShooting(true);
        yield return null;

        int projCount = FindAllByExactName(PROJ_CLONE).Length;
        Assert.GreaterOrEqual(projCount, 500, $"Expected >=500 projectiles, found {projCount}");
    }

    [UnityTest, Category("Load")]
    public IEnumerator Load_Spawn500Bonuses_WithTriggers_NoCrashes()
    {
        const int N = 500;
        for (int i = 0; i < N; i++)
        {
            var go = new GameObject("NF_Bonus", typeof(CircleCollider2D));
            var trig = go.GetComponent<CircleCollider2D>();
            trig.isTrigger = true;
            trig.radius = 0.5f;
        }
        yield return null;

        int bonusCount = Object.FindObjectsByType<CircleCollider2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                               .Count(c => c && c.isTrigger && c.gameObject.name == "NF_Bonus");
        Assert.AreEqual(N, bonusCount, $"Expected {N} Bonus triggers, found {bonusCount}");
    }
}
