using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using NUnit.Framework;

[TestFixture]
public class PlayerMovingInputIntegration
{
	private const float MARGIN = 50f;

	private PlayerMoving _player;
	private Camera _camera;
	private InputTestFixture _fixture;

	public static PlayerMoving CreatePlayer() => PlayerMovingCameraIntegration.CreatePlayer();

	public static Camera CreateMainCamera() => PlayerMovingCameraIntegration.CreateMainCamera();

	[UnitySetUp]
	public IEnumerator SetUp()
	{
		yield return null;
		_player = CreatePlayer();
		_camera = CreateMainCamera();
		_fixture = new InputTestFixture();
		_fixture.Setup();
	}

	[UnityTearDown]
	public IEnumerator TearDown()
	{
		yield return null;
		_fixture.TearDown();
		if (_player)
			Object.Destroy(_player.gameObject);
		if (_camera)
			Object.Destroy(_camera.gameObject);
	}

	[UnityTest]
	public IEnumerator MouseExists()
	{
		var mouse = InputSystem.AddDevice<Mouse>();
		Assert.NotNull(Mouse.current, "Mouse was added to InputSystem but not registered for Mouse.current.");
		InputSystem.RemoveDevice(mouse);
		yield return null;
	}

#if UNITY_STANDALONE || UNITY_EDITOR
	[UnityTest]
	public IEnumerator MouseMovesPlayer()
	{
		const string PLAYER_NOT_RESPONDING = "Mouse was pressed and moved but player didn't respond.";

		var mouse = InputSystem.AddDevice<Mouse>();

		// Move To Center
		Vector2 mousePosition = new(_camera.pixelWidth / 2, _camera.pixelHeight / 2);
		_fixture.Press(mouse.leftButton);
		_fixture.Move(mouse.position, mousePosition);
		yield return null;

		// Move Right
		mousePosition.x += MARGIN;
		_fixture.Move(mouse.position, mousePosition);
		Vector2 previousPlayerPosition = _player.transform.position;
		yield return null;
		Assert.Greater(_player.transform.position.x, previousPlayerPosition.x, PLAYER_NOT_RESPONDING);
		// Move Left
		mousePosition.x -= MARGIN;
		_fixture.Move(mouse.position, mousePosition);
		previousPlayerPosition = _player.transform.position;
		yield return null;
		Assert.Less(_player.transform.position.x, previousPlayerPosition.x, PLAYER_NOT_RESPONDING);
		// Move Up
		mousePosition.y += MARGIN;
		_fixture.Move(mouse.position, mousePosition);
		previousPlayerPosition = _player.transform.position;
		yield return null;
		Assert.Greater(_player.transform.position.y, previousPlayerPosition.y, PLAYER_NOT_RESPONDING);
		// Move Down
		mousePosition.y -= MARGIN;
		_fixture.Move(mouse.position, mousePosition);
		previousPlayerPosition = _player.transform.position;
		yield return null;
		Assert.Less(_player.transform.position.y, previousPlayerPosition.y, PLAYER_NOT_RESPONDING);

		_fixture.Release(mouse.leftButton);
		InputSystem.RemoveDevice(mouse);
	}
#endif

#if UNITY_IOS || UNITY_ANDROID
	[UnityTest]
	public IEnumerator TouchMovesPlayer()
	{
		const string PLAYER_NOT_RESPONDING = "Touch was moved but player didn't respond.";

		var touchscreen = InputSystem.AddDevice<Touchscreen>();

		// Move To Center
		Vector2 touchPosition = new(_camera.pixelWidth / 2, _camera.pixelHeight / 2);
		_fixture.BeginTouch(0, touchPosition);
		_fixture.Move(touchscreen.position, touchPosition);
		yield return null;

		// Move Right
		touchPosition.x += MARGIN;
		_fixture.Move(touchscreen.position, touchPosition);
		Vector2 previousPlayerPosition = _player.transform.position;
		yield return null;
		Assert.Greater(_player.transform.position.x, previousPlayerPosition.x, PLAYER_NOT_RESPONDING);
		// Move Left
		touchPosition.x -= MARGIN;
		_fixture.Move(touchscreen.position, touchPosition);
		previousPlayerPosition = _player.transform.position;
		yield return null;
		Assert.Less(_player.transform.position.x, previousPlayerPosition.x, PLAYER_NOT_RESPONDING);
		// Move Up
		touchPosition.y += MARGIN;
		_fixture.Move(touchscreen.position, touchPosition);
		previousPlayerPosition = _player.transform.position;
		yield return null;
		Assert.Greater(_player.transform.position.y, previousPlayerPosition.y, PLAYER_NOT_RESPONDING);
		// Move Down
		touchPosition.y -= MARGIN;
		_fixture.Move(touchscreen.position, touchPosition);
		previousPlayerPosition = _player.transform.position;
		yield return null;
		Assert.Less(_player.transform.position.y, previousPlayerPosition.y, PLAYER_NOT_RESPONDING);

		_fixture.EndTouch(0, touchPosition);
		InputSystem.RemoveDevice(touchscreen);
	}
#endif
}
