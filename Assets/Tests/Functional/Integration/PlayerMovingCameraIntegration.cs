using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class PlayerMovingCameraIntegration
{
	private const string MAIN_CAMERA = "MainCamera";
	private const float MARGIN = 5f;
	private PlayerMoving _player;
	private Camera _camera;

	public static PlayerMoving CreatePlayer()
	{
		var player = PlayerTests.CreatePlayer();
		var playerMoving = player.gameObject.AddComponent<PlayerMoving>();
		playerMoving.borders = new Borders();
		return playerMoving;
	}

	public static Camera CreateMainCamera()
	{
		var camera = new GameObject().AddComponent<Camera>();
		camera.tag = MAIN_CAMERA;
		return camera;
	}

	[UnitySetUp]
	public IEnumerator SetUp()
	{
		yield return null;
		_player = CreatePlayer();
		_camera = CreateMainCamera();
	}

	[UnityTearDown]
	public IEnumerator TearDown()
	{
		yield return null;
		if (_player)
			Object.Destroy(_player.gameObject);
		if (_camera)
			Object.Destroy(_camera.gameObject);
	}

	[UnityTest]
	public IEnumerator PlayerMovingSingletonExists()
	{
		yield return null;
		Assert.IsTrue(PlayerMoving.instance, "PlayerMoving.instance doesn't exist after calling Instantiate.");
	}

	[UnityTest]
	public IEnumerator MainCameraExists()
	{
		yield return null;
		Assert.IsTrue(Camera.main, "Camera.main doesn't exist after calling Instantiate.");
	}

	[UnityTest]
	public IEnumerator PositionClampedRightTest()
	{
		var position = _player.transform.position;
		position.x = _player.borders.maxX + MARGIN;
		_player.transform.position = position;
		yield return null;
		AssertWithinBounds();
	}

	[UnityTest]
	public IEnumerator PositionClampedLeftTest()
	{
		var position = _player.transform.position;
		position.x = _player.borders.minX - MARGIN;
		_player.transform.position = position;
		yield return null;
		AssertWithinBounds();
	}

	[UnityTest]
	public IEnumerator PositionClampedUpTest()
	{
		var position = _player.transform.position;
		position.y = _player.borders.maxY + MARGIN;
		_player.transform.position = position;
		yield return null;
		AssertWithinBounds();
	}

	[UnityTest]
	public IEnumerator PositionClampedDownTest()
	{
		var position = _player.transform.position;
		position.y = _player.borders.minY - MARGIN;
		_player.transform.position = position;
		yield return null;
		AssertWithinBounds();
	}

	[UnityTest]
	public IEnumerator PositionClampedDiagonalTest()
	{
		var position = _player.transform.position;
		position.y = _player.borders.maxY + MARGIN;
		position.x = _player.borders.maxX + MARGIN;
		_player.transform.position = position;
		yield return null;
		AssertWithinBounds();
	}

	[UnityTest]
	public IEnumerator PositionClampedInfinityTest()
	{
		var position = _player.transform.position;
		position.y = float.MaxValue;
		position.x = float.MaxValue;
		_player.transform.position = position;
		yield return null;
		AssertWithinBounds();
	}

	public void AssertWithinBounds() => AssertWithinBounds(_player);

	public static void AssertWithinBounds(PlayerMoving player) => AssertWithinBounds(player.borders, player.transform.position);

	public static void AssertWithinBounds(Borders borders, Vector2 position)
	{
		const string OUTSIDE_BOUNDS = "Player position is outside the bounds of borders.";

		var minX = borders.minX;
		var maxX = borders.maxX;
		var minY = borders.minY;
		var maxY = borders.maxY;
		if (minX > maxX)
			(minX, maxX) = (maxX, minX);
		if (minY > maxY)
			(minY, maxY) = (maxY, minY);
		Assert.GreaterOrEqual(position.x, minX, OUTSIDE_BOUNDS);
		Assert.LessOrEqual(position.x, maxX, OUTSIDE_BOUNDS);
		Assert.GreaterOrEqual(position.y, minY, OUTSIDE_BOUNDS);
		Assert.LessOrEqual(position.y, maxY, OUTSIDE_BOUNDS);
	}
}
