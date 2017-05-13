using System;
using UnityEngine;
using System.Collections;
using TMS.Common.Core;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : ViewModelSingleton<MainMenuManager>
{
	public Text StartNewGameText;

	public Button StartNewGameButton;

	public Button ResumeGameButton;

	public Button SettingsButton;

	public Button QuitButton;

	public GameObject EventSystemHolder;

	protected override void Start()
	{
		base.Start();

#if !UNITY_ANDROID
		QuitButton.gameObject.SetActive(false);
#endif

		Subscribe<UIActionPayload>(OnUIAction);
	}

	private void OnUIAction(UIActionPayload payload)
	{
		switch (payload.Action)
		{
			case UIActionType.StartGame:
				StartNewGame();
				break;

			case UIActionType.PauseGame:
				PauseGame();
				break;

			case UIActionType.RestartGame:
				// TODO
				break;

			case UIActionType.ResumeGame:
				ResumeGame();
				break;

			case UIActionType.QuitGame:
				QuitGame();
				break;

			case UIActionType.ShowSettings:
				ShowSettings();
				break;
		}
	}

	public void PauseGame()
	{
		gameObject.SetActive(true);
	}

	public void ResumeGame()
	{
		gameObject.SetActive(false);
	}

	public void StartNewGame()
	{
		var scene = SceneManager.GetSceneByName(SceneNames.Runner);
		if (scene.isLoaded)
		{
			gameObject.SetActive(false);
			return;
		}

		StartCoroutine(LoadRunnerScene());
	}

	private IEnumerator LoadRunnerScene()
	{
		StartNewGameButton.enabled = false;
		ResumeGameButton.enabled = false;
		SettingsButton.enabled = false;
		StartNewGameText.text = "Loading ...";
		DestroyImmediate(EventSystemHolder);

		var res = SceneManager.LoadSceneAsync("Runner", LoadSceneMode.Additive);
		res.allowSceneActivation = false;

		while (!res.isDone)
		{
			if (Math.Abs(res.progress - 0.9f) <= 0.1f)
			{
				res.allowSceneActivation = true;
			}

			yield return null;
		}

		StartNewGameButton.enabled = true;
		ResumeGameButton.enabled = true;
		SettingsButton.enabled = true;
		gameObject.SetActive(false);
		StartNewGameText.text = "RESTART";
	}

	public void ShowSettings()
	{
		
	}

	public void QuitGame()
	{
#if UNITY_ANDROID
		Application.Quit();
#endif
	}
}
