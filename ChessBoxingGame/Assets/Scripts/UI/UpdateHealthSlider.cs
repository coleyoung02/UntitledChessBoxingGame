using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateHealthSlider : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    #region to be removed later
    [SerializeField] private float hp;
    private void Update()
    {
        updateSlider(hp);
    }

    #endregion

    // When passing in sliderPercent, make sure to divide currentHP/maxHP for proper display
    public void updateSlider(float sliderPercent)
    {
        // 100 value should be removed later
        hpSlider.value = sliderPercent / 100;
    }
}
