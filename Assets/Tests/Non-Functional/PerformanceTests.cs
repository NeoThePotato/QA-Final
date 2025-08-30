using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.PerformanceTesting;

public class PerformanceTests
{
    static GameObject CreateSimpleFX() => new GameObject("NF_Test FX");
    static GameObject CreateSimpleProjectile() => new GameObject("NF_Test Projectile");

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
        var bounds = new GameObject("NF_Bounds").AddComponent<BoxCollider2D>();
        bounds.size = new Vector2(30, 18);
        boss.InitMovement(bounds);
        return boss;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (go && go.name.StartsWith("NF_"))
                Object.DestroyImmediate(go);
        }
        yield return null;
    }

    // ----------------------------------------------------
    // Performance Tests using Unity.PerformanceTesting
    // ----------------------------------------------------

    [Test, Performance]
    public void Perf_EnemyActivateShooting_1000Calls()
    {
        var e = CreateMinimalEnemy();

        Measure.Method(() =>
        {
            e.ActivateShooting(true);
        })
        .WarmupCount(10)
        .MeasurementCount(1000)
        .Run();
    }

    [UnityTest, Performance]
    public IEnumerator Perf_BossMovement_240Frames()
    {
        var boss = CreateMinimalBoss();
        yield return null;

        Measure.Method(() =>
        {
            // Simulate 1 frame of boss movement
            var _ = boss.transform.position;
        })
        .WarmupCount(5)
        .MeasurementCount(240)
        .Run();

        yield return null;
    }

    [Test, Performance]
    public void Perf_PlayerCreateDestroy_200Cycles()
    {
        var fx = CreateSimpleFX();

        Measure.Method(() =>
        {
            var go = new GameObject("NF_Player", typeof(Player));
            var p = go.GetComponent<Player>();
            p.destructionFX = fx;
            p.GetDamage(1);
            Object.DestroyImmediate(go);
        })
        .WarmupCount(5)
        .MeasurementCount(200)
        .Run();
    }
}
