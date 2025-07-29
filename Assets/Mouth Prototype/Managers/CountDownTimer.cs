using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountDownTimer : MonoBehaviour
{
    [Tooltip("Duration in Seconds")]
    [SerializeField] private float duration;

    static int milliseconds = 1000;
    private float timeRemaining;
    private bool isCountingDown;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private UnityEvent OnCountDownStart;
    [SerializeField] private UnityEvent OnStopCountDown;

    public void Awake()
    {
        // StartCountDown();
        Time.timeScale = 0f;
    }

    public void Update()
    {
        UpdateCountDown();
    }

    public void StartCountDown()
    {
        Time.timeScale = 1;
        isCountingDown = true;
        timeRemaining = duration * milliseconds;
        OnCountDownStart?.Invoke();
    }

    public void StopCountDown()
    {
        Time.timeScale = 0;
        isCountingDown = false;
        timeRemaining = 0;
        DisplayTime(timeRemaining);
        OnStopCountDown?.Invoke();

    }

    public void UpdateCountDown()
    {
        if (isCountingDown && timeRemaining > 0)
        {
            timeRemaining = Mathf.Clamp(timeRemaining - Time.fixedDeltaTime, 0, duration);
            DisplayTime(timeRemaining);
            return;

        }

        StopCountDown();

    }


    private void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1f;
        int minutes = Mathf.FloorToInt(timeToDisplay / 60f);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60f);
        int milliseconds = Mathf.FloorToInt(timeToDisplay % 1f * 1000f);

        timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
}
