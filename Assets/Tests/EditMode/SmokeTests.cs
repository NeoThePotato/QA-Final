using UnityEngine;
using UnityEditor.SceneManagement;
using NUnit.Framework;

public class SmokeTests
{
	private const string TEST_SCENE = "Assets\\Space Shooter Template FREE\\Scenes\\Demo_Scene.unity";

	[OneTimeSetUp]
	public void SetUp()
	{
		EditorSceneManager.OpenScene(TEST_SCENE);
	}

	[OneTimeTearDown]
	public void TearDown()
	{
		EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
	}

	[Test]
	public void PlayerInLevel()
	{
		Assert.NotNull(Object.FindAnyObjectByType<Player>(), "No player found in scene \"{0}\".", EditorSceneManager.GetActiveScene().name);
		Assert.NotNull(Object.FindAnyObjectByType<LevelController>(), "No LevelController found in scene \"{0}\".", EditorSceneManager.GetActiveScene().name);
	}
}
