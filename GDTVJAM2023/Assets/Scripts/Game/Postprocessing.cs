using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

public class Postprocessing : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    private AutoExposure autoExposure;
    public float dimensionValue = 100; 

    private void Start()
    {
        postProcessVolume.profile.TryGetSettings(out autoExposure);
    }

    public void AutoExpOnOff(bool on)
    {
        if (on == true)
        {
            autoExposure.active = true;
        }
        else
        {
            autoExposure.active = false;
        }
    }

    private void ExpCompensation(float sliderValue)
    {
        autoExposure.keyValue.value = sliderValue;
    }

    public void GoToDimension()
    {
        ExpCompensation(dimensionValue);
        Invoke(nameof(GoToDimensionInvoke), 0.1f);
    }

    public void TakeDamage()
    {
        ExpCompensation(2.5f);
        Invoke(nameof(GoToDimensionInvoke), 0.1f);
    }

    private void GoToDimensionInvoke()
    {
        ExpCompensation(1);
    }

 
}
