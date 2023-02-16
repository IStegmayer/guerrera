using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dzajna
{
public class StatusBar : MonoBehaviour
{
    Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }


    public void SetMaxStat(float _maxStat)
    {
        slider.maxValue = _maxStat;
        slider.value = _maxStat;
    }

    public void SetMaxStat(int _maxStat)
    {
        slider.maxValue = _maxStat;
        slider.value = _maxStat;
    }

    public void SetCurrentStat(float _currentStat)
    {
        slider.value = _currentStat;
    }

    public void SetCurrentStat(int _currentStat)
    {
        slider.value = _currentStat;
    }
}
}