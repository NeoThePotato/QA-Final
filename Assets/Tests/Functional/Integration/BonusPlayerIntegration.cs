using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class BonusPlayerIntegration
{
	private Player _player;
	private Bonus _bonus;

	public static Player CreatePlayer()
	{
		var player = PlayerTests.CreatePlayer();
		var collider = player.gameObject.AddComponent<CircleCollider2D>();
		collider.radius = 1f;
		var shooting = player.gameObject.AddComponent<PlayerShooting>();
		shooting.enabled = false;
		player.gameObject.AddComponent<Rigidbody2D>();
		return player;
	}

	public static Bonus CreateBonus()
	{
		var bonus = BonusTests.CreateBonus();
		bonus.transform.position = new(0f, 10f, 0);
		return bonus;
	}

	private IEnumerator TriggerPickup()
	{
		_bonus.transform.position = _player.transform.position;
		yield return new WaitForFixedUpdate();
		yield return null;
	}

	[UnitySetUp]
	public IEnumerator SetUp()
	{
		yield return null;
		_player = CreatePlayer();
		_bonus = CreateBonus();
	}

	[UnityTearDown]
	public IEnumerator TearDown()
	{
		yield return null;
		if (_player)
			Object.Destroy(_player.gameObject);
		if (_bonus)
			Object.Destroy(_bonus.gameObject);
	}

	[UnityTest]
	public IEnumerator BonusIsNotPickedUpAutomatically()
	{
		yield return new WaitForFixedUpdate();
		yield return null;
		Assert.IsTrue(_bonus, "Bonus was destroyed or failed to instantiate.");
	}

	[UnityTest]
	public IEnumerator BonusIsDestroyedByPickup()
	{
		yield return TriggerPickup();
		Assert.IsFalse(_bonus, "Bonus wasn't destroyed when picked up.");
	}

	[UnityTest]
	public IEnumerator PlayerWeaponLevelIncreasesByPickup()
	{
		var previousPower = PlayerShooting.instance.weaponPower;
		yield return TriggerPickup();
		Assert.IsTrue(PlayerShooting.instance.weaponPower > previousPower, "Weapon power didn't increase when bonus was picked up.");
	}

	[UnityTest]
	public IEnumerator PlayerWeaponLevelDoesNotOverflow()
	{
		PlayerShooting.instance.weaponPower = PlayerShooting.instance.maxweaponPower;
		yield return TriggerPickup();
		Assert.AreEqual(PlayerShooting.instance.weaponPower, PlayerShooting.instance.maxweaponPower, "Weapon power didn't stay the same when at max weapon level.");
	}
}
