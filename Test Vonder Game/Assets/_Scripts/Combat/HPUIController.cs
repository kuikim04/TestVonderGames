using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPUIController : MonoBehaviour
{
    public Image mainHPBar;
    public Image damageEffectBar;

    private float lastFillAmount = 1f;
    private void OnEnable()
    {
        PlayerStat.OnHPChanged += UpdateHPUI;
    }

    private void OnDisable()
    {
        PlayerStat.OnHPChanged -= UpdateHPUI;
    }

    public void UpdateHPUI(float currentHP, float maxHP)
    {
        float fillAmount = currentHP / maxHP;

        mainHPBar.DOKill();
        damageEffectBar.DOKill();

        if (fillAmount < lastFillAmount)
        {
            mainHPBar.fillAmount = fillAmount;

            damageEffectBar
                .DOFillAmount(fillAmount, 0.4f)
                .SetDelay(0.1f)
                .SetEase(Ease.OutQuad);
        }
        else
        {
            damageEffectBar.fillAmount = fillAmount;

            mainHPBar
                .DOFillAmount(fillAmount, 0.4f)
                .SetDelay(0.1f)
                .SetEase(Ease.OutQuad);
        }

        lastFillAmount = fillAmount;
    }

}
