using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessingController : MonoBehaviour {

    public PostProcessingProfile profile;

    ColorGradingModel.Settings colorGradingSettings;
    ChromaticAberrationModel.Settings chromaticAbberationSettings;

    ColorGradingModel.Settings colorGradingBaseSettings;
    ChromaticAberrationModel.Settings chromaticAbberationBaseSettings;

    // Use this for initialization
    void Start () {
        colorGradingBaseSettings = profile.colorGrading.settings;
        chromaticAbberationBaseSettings = profile.chromaticAberration.settings;

        colorGradingSettings = profile.colorGrading.settings;
        chromaticAbberationSettings = profile.chromaticAberration.settings;
        resetPostProcessingStack();
    }
	
    void OnApplicationQuit() {
        profile.colorGrading.settings = colorGradingBaseSettings;
        profile.chromaticAberration.settings = chromaticAbberationBaseSettings;
        resetPostProcessingStack();
    }

    void resetPostProcessingStack() {
        profile.fog.enabled = false;
        profile.antialiasing.enabled = false;
        profile.ambientOcclusion.enabled = false;
        profile.screenSpaceReflection.enabled = false;
        profile.depthOfField.enabled = false;
        profile.motionBlur.enabled = true;
        profile.eyeAdaptation.enabled = false;
        profile.bloom.enabled = false;
        profile.colorGrading.enabled = true;
        profile.userLut.enabled = false;
        profile.chromaticAberration.enabled = false;
        profile.grain.enabled = false;
        profile.vignette.enabled = true;
        profile.dithering.enabled = false;

        disableDamagePowerupPostProcessing();
        disableSlowMotionPowerupPostProcessing();
        disableInvincibilityPowerupPostProcessing();
    }

    public void enableDamagePowerupPostProcessing() {
        colorGradingSettings.basic.postExposure = 1.5f;
        colorGradingSettings.basic.contrast = 1.5f;

        profile.colorGrading.settings = colorGradingSettings;
    }

    public void disableDamagePowerupPostProcessing() {
        colorGradingSettings.basic.postExposure = 0f;
        colorGradingSettings.basic.contrast = 1f;

        profile.colorGrading.settings = colorGradingSettings;
    }

    public void enableSlowMotionPowerupPostProcessing() {
        profile.chromaticAberration.enabled = true;
    }

    public void disableSlowMotionPowerupPostProcessing() {
        profile.chromaticAberration.enabled = false;
    }

    public void enableInvincibilityPowerupPostProcessing() {
        
    }

    public void disableInvincibilityPowerupPostProcessing() {
        
    }

}
