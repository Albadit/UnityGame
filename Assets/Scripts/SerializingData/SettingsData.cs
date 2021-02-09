using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
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

    public SettingsData(SettingsMenu settings)
    {
        difficulty = settings.difficulty;
        sliderValue = settings.sliderValue;
        showHud = settings.showHud;
        tooltips = settings.tooltips;

        sliderValueSensitivity = settings.sliderValueSensitivity;
        sliderValueSmoothing = settings.sliderValueSmoothing;
        invertMouse = settings.invertMouse;

        shadows = settings.shadows;
        antiAliasing = settings.antiAliasing;
        vsync = settings.vsync;
        motionBlur = settings.motionBlur;
        ambientOcclusion = settings.ambientOcclusion;
        textures = settings.textures;
        cameraEffects = settings.cameraEffects;
    }
}