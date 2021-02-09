﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[Header("Loaded Scene")]
	[Tooltip("The name of the scene in the build settings that will load")]
	public string sceneName = "";

	[Header("Panels")]
	public GameObject[] menus;

	[Space]

	[Tooltip("The UI Panel parenting all sub menus")]
	public GameObject mainCanvas;

	[Header("Sound Effect")]
	public AudioSource hoverSound;
	public AudioSource swooshSound;

	[Header("LOADING SCREEN")]
	public GameObject loadingMenu;
	public Slider loadBar;
	public TMP_Text finishedLoadingText;

	public void Start()
	{
		DisableMenus();
	}

	public void PlayHover() => hoverSound.Play();
	public void PlaySwoosh() => swooshSound.Play();

	public void DisableMenus()
	{
		for (int i = 0; i < menus.Length; i++)
			menus[i].SetActive(false);
	}

	//Main Menu
	public void OpenMenu(int menuIndex)
	{
		DisableMenus();
		menus[menuIndex].SetActive(true);
	}

	public void NewGame()
	{
		if (sceneName != "")
			StartCoroutine(LoadAsynchronously(sceneName));
	}

	public void Support() => Application.OpenURL("http://unity3d.com/");

	public void Settings()
	{
		DisableMenus();
	}

	public void QuitGame()
	{
	#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
	#else
			Application.Quit();
	#endif
	}

	IEnumerator LoadAsynchronously(string sceneName) // scene name is just the name of the current scene being loaded
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
		operation.allowSceneActivation = false;
		mainCanvas.SetActive(false);
		loadingMenu.SetActive(true);

		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / .9f);
			loadBar.value = progress;

			if (operation.progress >= 0.9f)
			{
				finishedLoadingText.gameObject.SetActive(true);

				if (Input.anyKeyDown)
				{
					operation.allowSceneActivation = true;
				}
			}

			yield return null;
		}
	}
}