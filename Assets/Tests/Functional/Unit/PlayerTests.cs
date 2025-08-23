using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class PlayerTests
{
	private const string TEST_FX = "Test Destruction FX", TEST_FX_CLONE = TEST_FX + "(Clone)", PLAYER_TAG = "Player";
	private static readonly System.Type[] PLAYER_COMPONENTS = { typeof(Player) };

	private Player _player;

	public static Player CreatePlayer()
	{
		GameObject go = new("Test Player", PLAYER_COMPONENTS);
		go.tag = PLAYER_TAG;
		var player = go.GetComponent<Player>();
		player.destructionFX = CreateDestructionFX();
		return player;
	}

	public static GameObject CreateDestructionFX()
	{
		return new(TEST_FX);
	}

	[UnitySetUp]
	public IEnumerator SetUp()
	{
		yield return null;
		_player = CreatePlayer();
	}

	[UnityTearDown]
	public IEnumerator TearDown()
	{
		yield return null;
		if (_player)
			Object.Destroy(_player.gameObject);
		var fx = GameObject.Find(TEST_FX);
		if (fx)
			Object.Destroy(fx);
		fx = GameObject.Find(TEST_FX_CLONE);
		if (fx)
			Object.Destroy(fx);
	}

	[UnityTest]
	public IEnumerator PlayerExists()
	{
		yield return null;
		Assert.IsTrue(_player, "Player doesn't instantiate after calling Instantiate.");
	}

	[UnityTest]
	public IEnumerator PlayerSingletonExists()
	{
		yield return null;
		Assert.IsTrue(Player.instance, "Player.instance doesn't exist after calling Instantiate.");
	}

	[UnityTest]
	public IEnumerator PlayerDestructionDestroysGameObject()
	{
		var go = _player.gameObject;
		_player.GetDamage(1);
		yield return null;
		Assert.IsFalse(go, "Player.gameObject wasn't destroyed after calling GetDamage.");
	}

	[UnityTest]
	public IEnumerator PlayerDoesNotSpawnFXBeforeDestruction()
	{
		var spawnedFx = GameObject.Find(TEST_FX_CLONE);
		yield return null;
		Assert.IsFalse(spawnedFx, "Destruction FX spawned before calling GetDamage.");
	}

	[UnityTest]
	public IEnumerator PlayerDestructionSpawnedFX()
	{
		_player.GetDamage(1);
		yield return null;
		var spawnedFx = GameObject.Find(TEST_FX_CLONE);
		Assert.IsTrue(spawnedFx, "Player didn't spawn FX after calling GetDamage.");
	}
}
