using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public enum TimePeriod { Morning, Afternoon, Evening }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI dayOfWeekText;
    [SerializeField] private Image timeDial;        
    [SerializeField] private Image timeImage;       

    [Header("Sprites")]
    [SerializeField] private Sprite daySprite;
    [SerializeField] private Sprite afternoonSprite;
    [SerializeField] private Sprite nightSprite;

    [Header("Settings")]
    [SerializeField] private float secondsPerDay = 180f; 
    private float fillAmountPerSecond;
    private float fillPerPeriod;

    private float currentFill = 0f;

    private int currentDay = 0;
    private TimePeriod currentPeriod = TimePeriod.Morning;

    private string[] weekDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    public static TimeManager Instance;

    private Tween timeTween;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        fillAmountPerSecond = 1f / secondsPerDay;  
        fillPerPeriod = 1f / 3f;                  
    }
    private void OnEnable()
    {
        PlayerStat.OnDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        PlayerStat.OnDeath -= OnPlayerDeath;
    }

    void Start()
    {
        UpdateUI();
        StartTimeTween(); 
    }

    void StartTimeTween()
    {
        timeTween?.Kill(); 

        timeTween = DOTween.To(() => currentFill, x => {
            currentFill = x;
            timeDial.fillAmount = currentFill;

            if (currentFill >= ((int)currentPeriod + 1) * fillPerPeriod)
                AdvancePeriod();

            if (currentFill >= 1f)
                StartNewDay();

        }, 1f, secondsPerDay).SetEase(Ease.Linear);
    }


    private void AdvancePeriod()
    {
        currentPeriod++;
        if ((int)currentPeriod > 2)
        {
            StartNewDay();
            return;
        }

        UpdateUI();
    }

    private void StartNewDay()
    {
        currentFill = 0f;
        currentPeriod = TimePeriod.Morning;
        currentDay++;

        timeDial.fillAmount = currentFill;
        UpdateUI();

        StartTimeTween(); 
    }

    private void UpdateUI()
    {
        if (dayText) dayText.text = $"DAY {currentDay}";
        if (dayOfWeekText) dayOfWeekText.text = weekDays[currentDay % 7];

        if (timeImage)
        {
            switch (currentPeriod)
            {
                case TimePeriod.Morning: timeImage.sprite = daySprite; break;
                case TimePeriod.Afternoon: timeImage.sprite = afternoonSprite; break;
                case TimePeriod.Evening: timeImage.sprite = nightSprite; break;
            }
        }
    }

    public void ForceAdvanceTime()
    {
        timeTween?.Kill();

        currentPeriod++;
        if ((int)currentPeriod > 2)
        {
            StartNewDay();
            return;
        }

        float target = ((int)currentPeriod) * fillPerPeriod;

        DOTween.To(() => currentFill, x => {
            currentFill = x;
            timeDial.fillAmount = x;
        }, target, 0.5f)
        .SetEase(Ease.OutSine)
        .OnComplete(() =>
        {
            UpdateUI();
            StartTimeTween();
        });
    }


    private void OnPlayerDeath()
    {
        timeTween?.Kill(); 

        currentFill = 0f; 
        currentPeriod = TimePeriod.Morning;

        timeDial.fillAmount = currentFill;

        DOTween.To(() => timeDial.fillAmount, x => timeDial.fillAmount = x, 0f, 1f)

        .SetEase(Ease.OutSine).OnUpdate(() => currentFill = timeDial.fillAmount)
               .OnComplete(() =>
               {
                   UpdateUI();
                   StartTimeTween(); 
               });
    }

}
