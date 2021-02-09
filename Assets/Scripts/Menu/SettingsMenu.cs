using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	[Header("Settings Panels")]
	public GameObject PanelGame;
	public GameObject PanelControls;
	public GameObject PanelVideo;
	public GameObject PanelKeyBindings;
	public GameObject PanelMovement;
	public GameObject PanelCombat;
	public GameObject PanelGeneral;

	[Header("Highlight Effects")]
	public GameObject lineGame;
	public GameObject lineControls;
	public GameObject lineVideo;
	public GameObject lineKeyBindings;
	public GameObject lineMovement;
	public GameObject lineCombat;
	public GameObject lineGeneral;

	[Header("GAME SETTINGS")]
	public GameObject lineDifficultyNormal;
	public GameObject lineDifficultyHardcore;
	public GameObject musicSlider;
	public AudioSource music;
	public GameObject showHudText;
	public GameObject tooltipsText;

	[Header("CONTROLS SETTINGS")]
	public GameObject sensitivitySlider;
	public GameObject mouseSmoothSlider;
	public GameObject invertMouseText;

	[Header("VIDEO SETTINGS")]
	public GameObject fullscreenText;
	public GameObject ambientOcclusionText;
	public GameObject shadowofftextLine;
	public GameObject shadowlowtextLine;
	public GameObject shadowhightextLine;
	public GameObject aaOffTextLine;
	public GameObject aa2xTextLine;
	public GameObject aa4xTextLine;
	public GameObject aa8xTextLine;
	public GameObject vsyncText;
	public GameObject motionBlurText;
	public GameObject textureLowTextLine;
	public GameObject textureMedTextLine;
	public GameObject textureHighTextLine;
	public GameObject cameraEffectsText;

	[Space]

	[Header("VALUES")]
	public int difficulty;
	public float sliderValue;
	public int showHud;
	public int tooltips;

	public float sliderValueSensitivity;
	public float sliderValueSmoothing;
	public int invertMouse;

	public int shadows;
	public int antiAliasing;
	public int vsync;
	public int motionBlur;
	public int ambientOcclusion;
	public int textures;
	public int cameraEffects;

	void Start()
    {
		Load();
	}

    void Update()
    {
		sliderValue = musicSlider.GetComponent<Slider>().value;
		sliderValueSensitivity = sensitivitySlider.GetComponent<Slider>().value;
		sliderValueSmoothing = mouseSmoothSlider.GetComponent<Slider>().value;
	}

	public void Load()
	{
		SettingsData data = SavingSystem.LoadSettings();

		difficulty = data.difficulty;
		sliderValue = data.sliderValue;
		musicSlider.GetComponent<Slider>().value = data.sliderValue;
		showHud = data.showHud;
		tooltips = data.tooltips;
		Difficulty();
		MusicSlider();
		UpdateShowHud();
		UpdateToolTips();

		sliderValueSensitivity = data.sliderValueSensitivity;
		sensitivitySlider.GetComponent<Slider>().value = data.sliderValueSensitivity;
		sliderValueSmoothing = data.sliderValueSmoothing;
		mouseSmoothSlider.GetComponent<Slider>().value = data.sliderValueSmoothing;
		invertMouse = data.invertMouse;
		//SensitivitySlider();
		//SensitivitySmoothing();
		UpdateInvertMouse();

		shadows = data.shadows;
		antiAliasing = data.antiAliasing;
		vsync = data.vsync;
		motionBlur = data.motionBlur;
		textures = data.textures;
		cameraEffects = data.cameraEffects;
		Shadows();
		AntiAliasing();
		UpdateVsync();
		UpdateMotionBlur();
		UpdateAmbientOcclusion();
		Textures();
		UpdateCameraEffects();
	}

	public void Save() => SavingSystem.SaveSettings(this);

	void DisablePanels()
	{
		PanelGame.SetActive(false);
		PanelControls.SetActive(false);
		PanelVideo.SetActive(false);
		PanelKeyBindings.SetActive(false);

		lineGame.SetActive(false);
		lineControls.SetActive(false);
		lineVideo.SetActive(false);
		lineKeyBindings.SetActive(false);

		PanelMovement.SetActive(false);
		lineMovement.SetActive(false);
		PanelCombat.SetActive(false);
		lineCombat.SetActive(false);
		PanelGeneral.SetActive(false);
		lineGeneral.SetActive(false);
	}

	public void GamePanel()
	{
		DisablePanels();
		PanelGame.SetActive(true);
		lineGame.SetActive(true);
	}

	public void VideoPanel()
	{
		DisablePanels();
		PanelVideo.SetActive(true);
		lineVideo.SetActive(true);
	}

	public void ControlsPanel()
	{
		DisablePanels();
		PanelControls.SetActive(true);
		lineControls.SetActive(true);
	}

	public void KeyBindingsPanel()
	{
		DisablePanels();
		MovementPanel();
		PanelKeyBindings.SetActive(true);
		lineKeyBindings.SetActive(true);
	}

	public void MovementPanel()
	{
		DisablePanels();
		PanelKeyBindings.SetActive(true);
		lineKeyBindings.SetActive(true);
		PanelMovement.SetActive(true);
		lineMovement.SetActive(true);
	}

	public void CombatPanel()
	{
		DisablePanels();
		PanelKeyBindings.SetActive(true);
		lineKeyBindings.SetActive(true);
		PanelCombat.SetActive(true);
		lineCombat.SetActive(true);
	}

	public void GeneralPanel()
	{
		DisablePanels();
		PanelKeyBindings.SetActive(true);
		lineKeyBindings.SetActive(true);
		PanelGeneral.SetActive(true);
		lineGeneral.SetActive(true);
	}

	// Game //
	public void DifficultyNormal()
	{
		difficulty = 0;
		lineDifficultyHardcore.gameObject.SetActive(false);
		lineDifficultyNormal.gameObject.SetActive(true);
	}

	public void DifficultyHardcore()
	{
		difficulty = 1;
		lineDifficultyHardcore.gameObject.SetActive(true);
		lineDifficultyNormal.gameObject.SetActive(false);
	}

	public void Difficulty()
	{
		if (difficulty == 0)
			DifficultyNormal();
		else if (difficulty == 1)
			DifficultyHardcore();
	}

	public void MusicSlider() => GetComponent<AudioSource>().volume = sliderValue;

	public void UpdateShowHud() => showHudText.GetComponent<TMP_Text>().text = showHud == 0 ? "Off" : "On";

	public void ShowHud()
	{
		showHud = showHud == 0 ? 1 : 0;
		showHudText.GetComponent<TMP_Text>().text = showHud == 0 ? "Off" : "On";
	}

	public void UpdateToolTips() => tooltipsText.GetComponent<TMP_Text>().text = tooltips == 0 ? "Off" : "On";

	public void ToolTips()
	{
		tooltips = tooltips == 0 ? 1 : 0;
		tooltipsText.GetComponent<TMP_Text>().text = tooltips == 0 ? "Off" : "On";
	}

	// Controlls //
	//public void SensitivitySlider() => GetComponent<AudioSource>().volume = sliderValueXSensitivity;
	public void SensitivitySlider()
    {

    }

	//public void SensitivitySmoothing() => GetComponent<AudioSource>().volume = sliderValueSmoothing;
	public void SensitivitySmoothing()
    {

    }

	public void UpdateInvertMouse() => invertMouseText.GetComponent<TMP_Text>().text = invertMouse == 0 ? "Off" : "On";

	public void InvertMouse()
	{
		invertMouse = invertMouse == 0 ? 1 : 0;
		invertMouseText.GetComponent<TMP_Text>().text = invertMouse == 0 ? "Off" : "On";
	}

	// Videos //
	public void FullScreen()
	{
		Screen.fullScreen = !Screen.fullScreen;

		if (Screen.fullScreen == true)
			fullscreenText.GetComponent<TMP_Text>().text = "Off";
		else if (Screen.fullScreen == false)
			fullscreenText.GetComponent<TMP_Text>().text = "On";
	}

	public void UpdateAmbientOcclusion() => ambientOcclusionText.GetComponent<TMP_Text>().text = ambientOcclusion == 0 ? "Off" : "On";

	public void AmbientOcclusion()
	{
		ambientOcclusion = ambientOcclusion == 0 ? 1 : 0;
		ambientOcclusionText.GetComponent<TMP_Text>().text = ambientOcclusion == 0 ? "Off" : "On";
	}

	public void ShadowsOff()
	{
		shadows = 0;
		//QualitySettings.shadowCascades = 0;
		//QualitySettings.shadowDistance = 0;
		shadowofftextLine.gameObject.SetActive(true);
		shadowlowtextLine.gameObject.SetActive(false);
		shadowhightextLine.gameObject.SetActive(false);
	}

	public void ShadowsLow()
	{
		shadows = 1;
		//QualitySettings.shadowCascades = 2;
		//QualitySettings.shadowDistance = 75;
		shadowofftextLine.gameObject.SetActive(false);
		shadowlowtextLine.gameObject.SetActive(true);
		shadowhightextLine.gameObject.SetActive(false);
	}

	public void ShadowsHigh()
	{
		shadows = 2;
		//QualitySettings.shadowCascades = 4;
		//QualitySettings.shadowDistance = 500;
		shadowofftextLine.gameObject.SetActive(false);
		shadowlowtextLine.gameObject.SetActive(false);
		shadowhightextLine.gameObject.SetActive(true);
	}

	public void Shadows()
	{
		if (shadows == 0)
			ShadowsOff();
		else if (shadows == 1)
			ShadowsLow();
		else if (shadows == 2)
			ShadowsHigh();
	}

	public void AntiAliasingOff()
	{
		antiAliasing = 0;
		//QualitySettings.antiAliasing = 0;
		aaOffTextLine.gameObject.SetActive(true);
		aa2xTextLine.gameObject.SetActive(false);
		aa4xTextLine.gameObject.SetActive(false);
		aa8xTextLine.gameObject.SetActive(false);
	}

	public void AntiAliasingx2()
	{
		antiAliasing = 1;
		//QualitySettings.antiAliasing = 2;
		aaOffTextLine.gameObject.SetActive(false);
		aa2xTextLine.gameObject.SetActive(true);
		aa4xTextLine.gameObject.SetActive(false);
		aa8xTextLine.gameObject.SetActive(false);
	}

	public void AntiAliasingx4()
	{
		antiAliasing = 2;
		//QualitySettings.antiAliasing = 4;
		aaOffTextLine.gameObject.SetActive(false);
		aa2xTextLine.gameObject.SetActive(false);
		aa4xTextLine.gameObject.SetActive(true);
		aa8xTextLine.gameObject.SetActive(false);
	}

	public void AntiAliasingx8()
	{
		antiAliasing = 3;
		//QualitySettings.antiAliasing = 8;
		aaOffTextLine.gameObject.SetActive(false);
		aa2xTextLine.gameObject.SetActive(false);
		aa4xTextLine.gameObject.SetActive(false);
		aa8xTextLine.gameObject.SetActive(true);
	}

	public void AntiAliasing()
	{
		if (antiAliasing == 0)
			AntiAliasingOff();
		else if (antiAliasing == 1)
			AntiAliasingx2();
		else if (antiAliasing == 2)
			AntiAliasingx4();
		else if (antiAliasing == 3)
			AntiAliasingx8();
	}

	public void UpdateVsync() => vsyncText.GetComponent<TMP_Text>().text = vsync == 0 ? "Off" : "On";

	public void Vsync()
	{
		vsync = vsync == 0 ? 1 : 0;
		vsyncText.GetComponent<TMP_Text>().text = vsync == 0 ? "Off" : "On";
		//QualitySettings.vSyncCount = 0;
		//QualitySettings.vSyncCount = 1;
	}

	public void UpdateMotionBlur() => motionBlurText.GetComponent<TMP_Text>().text = motionBlur == 0 ? "Off" : "On";

	public void MotionBlur()
	{
		motionBlur = motionBlur == 0 ? 1 : 0;
		motionBlurText.GetComponent<TMP_Text>().text = motionBlur == 0 ? "Off" : "On";
	}

	public void TexturesLow()
	{
		textures = 0;
		//QualitySettings.masterTextureLimit = 2;
		textureLowTextLine.gameObject.SetActive(true);
		textureMedTextLine.gameObject.SetActive(false);
		textureHighTextLine.gameObject.SetActive(false);
	}

	public void TexturesMed()
	{
		textures = 1;
		//QualitySettings.masterTextureLimit = 1;
		textureLowTextLine.gameObject.SetActive(false);
		textureMedTextLine.gameObject.SetActive(true);
		textureHighTextLine.gameObject.SetActive(false);
	}

	public void TexturesHigh()
	{
		textures = 2;
		//QualitySettings.masterTextureLimit = 0;
		textureLowTextLine.gameObject.SetActive(false);
		textureMedTextLine.gameObject.SetActive(false);
		textureHighTextLine.gameObject.SetActive(true);
	}

	public void Textures()
	{
		if (textures == 0)
			TexturesLow();
		else if (textures == 1)
			TexturesMed();
		else if (textures == 2)
			TexturesHigh();
	}

	public void UpdateCameraEffects() => cameraEffectsText.GetComponent<TMP_Text>().text = cameraEffects == 0 ? "Off" : "On";

	public void CameraEffects()
	{
		cameraEffects = cameraEffects == 0 ? 1 : 0;
		cameraEffectsText.GetComponent<TMP_Text>().text = cameraEffects == 0 ? "Off" : "On";
	}
}