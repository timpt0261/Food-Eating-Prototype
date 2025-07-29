using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class QuickTimeEvent : MonoBehaviour
{
    [SerializeField] private GameObject go_QTE;
    [SerializeField] private Slider qte_slider;
    [SerializeField] private float speed = 1f;

    [SerializeField] private TextMeshProUGUI score_TMP;

    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] int score;

    [SerializeField] private KeyCode key = KeyCode.E;
    [SerializeField] private bool pressedOnFrame;


    private bool isActive = false;

    public UnityEvent OnSuccess;
    public UnityEvent OnFailure;


    void Awake()
    {
        go_QTE.SetActive(false);

    }
    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        UpdateQTE();
        pressedOnFrame = Input.GetKey(key);
        if (pressedOnFrame) StopQTE();

    }

    public void ActivateQTE()
    {
        go_QTE.SetActive(true);
        qte_slider.enabled = true;
        qte_slider.value = 0;
        isActive = true;
    }


    private void UpdateQTE()
    {
        qte_slider.value = Mathf.PingPong(Time.time * speed, qte_slider.maxValue);

    }

    private void StopQTE()
    {
        go_QTE.SetActive(false);
        isActive = false;

        bool success = IsBetween(qte_slider.value, 30, 70);
        if (success)
        {
            OnSuccess?.Invoke();
            return;
        }

        OnFailure?.Invoke();
    }

    private static bool IsBetween(float value, float a, float b, bool inclusive = true)
    {
        float min = Mathf.Min(a, b);
        float max = Mathf.Max(a, b);

        return inclusive
            ? value >= min && value <= max   // [min, max]
            : value > min && value < max;  // (min, max)
    }

    public void UpdateScore()
    {
        score++;
        score_TMP.text = $"{score:D8}";
        finalScoreText.text = $"Score : {score}";
    }
}
