using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class EnemyTests
{
	private const string	HIT_FX = "Test Hit FX",
							HIT_FX_CLONE = HIT_FX + "(Clone)",
							DESTRUCTION_FX = "Test Destruction FX",
							DESTRUCTION_FX_CLONE = DESTRUCTION_FX + "(Clone)";
	private static readonly System.Type[] ENEMY_COMPONENTS = { typeof(Enemy) };

	private Enemy _enemy;

	public static Enemy CreateEnemy()
	{
		GameObject go = new("Test Enemy", ENEMY_COMPONENTS);
		var enemy = go.GetComponent<Enemy>();
		enemy.destructionVFX = CreateDestructionFX();
		enemy.hitEffect = CreateHitFX();
		enemy.health = 10;
		return enemy;
	}

	public static GameObject CreateDestructionFX()
	{
		return new(DESTRUCTION_FX);
	}

	public static GameObject CreateHitFX()
	{
		return new(HIT_FX);
	}

	[UnitySetUp]
	public IEnumerator SetUp()
	{
		yield return null;
		_enemy = CreateEnemy();
	}

	[UnityTearDown]
	public IEnumerator TearDown()
	{
		yield return null;
		if (_enemy)
			Object.Destroy(_enemy.gameObject);
		var fx = GameObject.Find(DESTRUCTION_FX);
		if (fx)
			Object.Destroy(fx);
		fx = GameObject.Find(DESTRUCTION_FX_CLONE);
		if (fx)
			Object.Destroy(fx);
		fx = GameObject.Find(HIT_FX);
		if (fx)
			Object.Destroy(fx);
		fx = GameObject.Find(HIT_FX_CLONE);
		if (fx)
			Object.Destroy(fx);
	}

	[UnityTest]
	public IEnumerator EnemyExists()
	{
		yield return null;
		Assert.IsTrue(_enemy, "Enemy doesn't instantiate after calling Instantiate.");
	}

	[UnityTest]
	public IEnumerator EnemyDestructionDestroysGameObject()
	{
		var go = _enemy.gameObject;
		_enemy.GetDamage(int.MaxValue);
		yield return null;
		Assert.IsFalse(go, "Enemy.gameObject wasn't destroyed after calling GetDamage.");
	}

	[UnityTest]
	public IEnumerator EnemyDoesNotSpawnHitFXBeforeHit()
	{
		var spawnedFx = GameObject.Find(HIT_FX_CLONE);
		yield return null;
		Assert.IsFalse(spawnedFx, "Hit FX spawned before calling GetDamage.");
	}

	[UnityTest]
	public IEnumerator EnemyDoesNotSpawnDestructionFXBeforeDestruction()
	{
		var spawnedFx = GameObject.Find(DESTRUCTION_FX_CLONE);
		yield return null;
		Assert.IsFalse(spawnedFx, "Destruction FX spawned before calling GetDamage.");
	}

	[UnityTest]
	public IEnumerator EnemyHitSpawnedFX()
	{
		_enemy.GetDamage(1);
		yield return null;
		var spawnedFx = GameObject.Find(HIT_FX_CLONE);
		Assert.IsTrue(spawnedFx, "Enemy didn't spawn hit FX after calling GetDamage.");
	}

	[UnityTest]
	public IEnumerator EnemyDestructionSpawnedFX()
	{
		_enemy.GetDamage(int.MaxValue);
		yield return null;
		var spawnedFx = GameObject.Find(DESTRUCTION_FX_CLONE);
		Assert.IsTrue(spawnedFx, "Enemy didn't spawn destruction FX after calling destruction.");
	}
}
