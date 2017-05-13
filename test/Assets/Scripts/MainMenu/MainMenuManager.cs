using UnityEngine;
using System.Collections;
using TMS.Common.Core;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviorBaseSingleton<MainMenuManager>
{
	public Text StartNewGameText;

	public Button StartNewGameButton;

	public Button ResumeGameButton;

	public Button SettingsButton;

	public void StartNewGame()
	{
		StartCoroutine(LoadRunnerScene());
	}

	private IEnumerator LoadRunnerScene()
	{
		StartNewGameButton.enabled = false;
		ResumeGameButton.enabled = false;
		SettingsButton.enabled = false;
		StartNewGameText.text = "Loading ...";

		var res = SceneManager.LoadSceneAsync("Runner", LoadSceneMode.Additive);
		res.allowSceneActivation = false;

		while (!res.isDone)
		{
			if (res.progress == 0.9f)
			{
				res.allowSceneActivation = true;
			}

			yield return null;
		}

		StartNewGameButton.enabled = true;
		ResumeGameButton.enabled = true;
		SettingsButton.enabled = true;
		gameObject.SetActive(false);
		StartNewGameText.text = "START";
	}

	public void ShowSettings()
	{
		
	}

	public void QuitGame()
	{
		
	}
}
