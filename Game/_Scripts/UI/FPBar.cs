using UnityEngine;
using UnityEngine.UI;

namespace Dzajna {
public class FPBar : MonoBehaviour {
    
    private Slider slider;

    private void Awake() {
        slider = GetComponent<Slider>();
    }

    public void SetMaxFP(float maxFP) {
        slider.maxValue = maxFP;
        slider.value = maxFP * 0.652f;
    }

    public void SetCurrentFP(float currentFP) {
        slider.value = currentFP * 0.652f;
    }
}
}