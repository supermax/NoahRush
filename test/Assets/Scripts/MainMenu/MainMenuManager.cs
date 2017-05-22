#region Usings

using TMS.Common.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

#endregion

public class MainMenuManager : ViewModelSingleton<MainMenuManager>
{
	public Transform[] MainMenuObjects;

	public Button QuitButton;

	public Button ResumeGameButton;

	public Transform[] RunnerInGameObjects;

	public Transform[] RunnerObjects;

	public Button SettingsButton;

	public Button StartNewGameButton;

	public Text StartNewGameText;

	public static void SetActive(Transform[] objects, bool isActive)
	{
		foreach (var obj in objects)
			obj.gameObject.SetActive(isActive);
	}

	protected override void Start()
	{
		base.Start();

#if !UNITY_ANDROID
		QuitButton.gameObject.SetActive(false);
#endif
		ResumeGameButton.enabled = true;

		Subscribe<UIActionPayload>(OnUIAction);
	}

	private void OnUIAction(UIActionPayload payload)
	{
		switch (payload.Action)
		{
			case UIActionType.StartGame:
			case UIActionType.RestartGame:
				StartNewGame();
				break;

			case UIActionType.PauseGame:
				PauseGame();
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
		SetActive(RunnerInGameObjects, false);
	}

	public void ResumeGame()
	{
		SetActive(MainMenuObjects, false);
		SetActive(RunnerInGameObjects, true);
	}

	public void StartNewGame()
	{
		SetActive(MainMenuObjects, false);
		SetActive(RunnerInGameObjects, true);
		SetActive(RunnerObjects, true);

		ResumeGameButton.enabled = true;
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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Publish(new UIActionPayload { Action = UIActionType.QuitGame });
		}
	}
}