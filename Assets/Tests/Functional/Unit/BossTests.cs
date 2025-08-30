using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class BossTests
{
	private const string TEST_FX = "Test Destruction FX", TEST_FX_CLONE = TEST_FX + "(Clone)", BOSS_TAG = "BossEnemy";
	private const string TEST_PROJECTILE = "Test Boss Projectile", TEST_PROJECTILE_CLONE = TEST_PROJECTILE + "(Clone)";
	private const string TEST_BOUNDARIES = "Test Boundaries";
	private static readonly System.Type[] BOSS_COMPONENTS = { typeof(BossEnemy), typeof(Rigidbody2D), typeof(CircleCollider2D) };
	private static readonly System.Type[] BOUNDARIES_COMPONENTS = { typeof(BoxCollider2D) };

	private BossEnemy _boss;

	public static BossEnemy CreateBoss()
	{
		GameObject go = new("Test Boss", BOSS_COMPONENTS)
		{
			tag = BOSS_TAG,
			transform = { position = Vector3.zero }
		};
		var boss = go.GetComponent<BossEnemy>();
		var boundaries = CreateBounds().GetComponent<BoxCollider2D>();
		boss.InitMovement(boundaries);
		boss.destructionVFX = CreateDestructionFX();
		boss.Projectile = CreateProjectile();
		return boss;
	}

	public static GameObject CreateDestructionFX()
	{
		return new(TEST_FX);
	}
	
	public static GameObject CreateProjectile()
	{
		return new(TEST_PROJECTILE);
	}
	
	public static GameObject CreateBounds()
	{
		return new(TEST_BOUNDARIES, BOUNDARIES_COMPONENTS);
	}

	[UnitySetUp]
	public IEnumerator SetUp()
	{
		yield return null;
		_boss = CreateBoss();
	}

	[UnityTearDown]
	public IEnumerator TearDown()
	{
		yield return null;
		if (_boss)
			Object.Destroy(_boss.gameObject);
		var fx = GameObject.Find(TEST_FX);
		if (fx)
			Object.Destroy(fx);
		fx = GameObject.Find(TEST_FX_CLONE);
		if (fx)
			Object.Destroy(fx);
		fx = GameObject.Find(TEST_PROJECTILE);
		if (fx)
			Object.Destroy(fx);
		fx = GameObject.Find(TEST_PROJECTILE_CLONE);
		if (fx)
			Object.Destroy(fx);
		fx = GameObject.Find(TEST_BOUNDARIES);
		if (fx)
			Object.Destroy(fx);
	}

	[UnityTest]
	public IEnumerator BossExists()
	{
		yield return null;
		Assert.IsTrue(_boss, "BossEnemy doesn't instantiate after calling Instantiate.");
	}

	[UnityTest]
	public IEnumerator BossSingletonExists()
	{
		yield return null;
		Assert.IsTrue(BossEnemy.instance, "BossEnemy.instance doesn't exist after calling Instantiate.");
	}

	[UnityTest]
	public IEnumerator BossDestructionDestroysGameObject()
	{
		var go = _boss.gameObject;
		_boss.GetDamage(99999999);
		yield return null;
		Assert.IsFalse(go, "BossEnemy.gameObject wasn't destroyed after calling GetDamage.");
	}

	[UnityTest]
	public IEnumerator BossDoesNotSpawnFXBeforeDestruction()
	{
		var spawnedFx = GameObject.Find(TEST_FX_CLONE);
		yield return null;
		Assert.IsFalse(spawnedFx, "Destruction FX spawned before calling GetDamage.");
	}

	[UnityTest]
	public IEnumerator BossDestructionSpawnedFX()
	{
		_boss.GetDamage(99999999);
		yield return null;
		var spawnedFx = GameObject.Find(TEST_FX_CLONE);
		Assert.IsTrue(spawnedFx, "Boss didn't spawn FX after calling GetDamage.");
	}
	
	[UnityTest]
	public IEnumerator BossActivateShootingSpawnsProjectileProperly()
	{
		_boss.ActivateShooting(true);
		yield return null;
		var spawnedProjectile = GameObject.Find(TEST_PROJECTILE_CLONE);
		Assert.IsTrue(spawnedProjectile, "Boss didn't spawn Projectile after calling ActivateShooting.");
	}
	
	[UnityTest]
	public IEnumerator BossCanMove()
	{
		yield return null;
		Assert.IsTrue(_boss.transform.position != Vector3.zero, "Boss didn't move from the initial position.");
	}
}
