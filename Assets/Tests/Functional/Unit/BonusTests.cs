using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class BonusTests
{
	private static readonly System.Type[] BONUS_COMPONENTS = { typeof(Bonus), typeof(CircleCollider2D) };

	private Bonus _bonus;

	public static Bonus CreateBonus()
	{
		GameObject go = new("Test Bonus", BONUS_COMPONENTS);
		var bonus = go.GetComponent<Bonus>();
		var trigger = go.GetComponent<CircleCollider2D>();
		trigger.radius = 1f;
		trigger.isTrigger = true;
		return bonus;
	}

	[UnitySetUp]
	public IEnumerator SetUp()
	{
		yield return null;
		_bonus = CreateBonus();
	}

	[UnityTearDown]
	public IEnumerator TearDown()
	{
		yield return null;
		if (_bonus)
			Object.Destroy(_bonus.gameObject);
	}

	[UnityTest]
	public IEnumerator BonusExists()
	{
		yield return null;
		Assert.IsTrue(_bonus, "Bonus doesn't instantiate after calling Instantiate.");
	}
}
