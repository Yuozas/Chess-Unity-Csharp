using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Toggle autoRotation, rotationAnimation;
    private void Start()
    {
        autoRotation.isOn = PlayerPrefs.GetInt("AutoRotation") == 1;
        rotationAnimation.isOn = PlayerPrefs.GetInt("RotationAnimation") == 1;
    }
    public void SetAutoRotation(Toggle toggle) => PlayerPrefs.SetInt("AutoRotation", Convert.ToInt32(toggle.isOn));
    public void SetRotationAnimation(Toggle toggle) => PlayerPrefs.SetInt("RotationAnimation", Convert.ToInt32(toggle.isOn));

}
