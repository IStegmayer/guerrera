using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI valueText;
    private Slider slider;
    private AudioSystem audioSystem;
    
    private void Start() {
        slider = GetComponent<Slider>();
        audioSystem = AudioSystem.Instance;
    }

    public void UpdateVolume() {
        valueText.text = slider.value.ToString();
        audioSystem.SetMasterVolume(slider.value);
    }
}
