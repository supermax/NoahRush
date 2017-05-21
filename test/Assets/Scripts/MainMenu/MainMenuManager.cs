using System;
using UnityEngine;
using System.Collections;
using TMS.Common.Core;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : ViewModelSingleton<MainMenuManager>
{
	public Text StartNewGameText;

	public Button StartNewGameButton;

	public Button ResumeGameButton;

	public Button SettingsButton;

	public Button QuitButton;

	public Transform[] MainMenuObjects;

	public Transform[] RunnerObjects;

	public static void SetActive(Transform[] objects, bool isActive)
	{
		foreach (var obj in objects)
		{
			obj.gameObject.SetActive(isActive);
		}
	}

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
		SetActive(MainMenuObjects, true);
	}

	public void ResumeGame()
	{
		SetActive(MainMenuObjects, false);
	}

	public void StartNewGame()
	{
		SetActive(MainMenuObjects, false);
		SetActive(RunnerObjects, true);

		StartNewGameButton.enabled = true;
		ResumeGameButton.enabled = true;
		SettingsButton.enabled = true;
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
