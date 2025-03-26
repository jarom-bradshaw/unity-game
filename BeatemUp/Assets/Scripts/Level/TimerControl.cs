using UnityEngine;
using TMPro;
public class TimerControl : MonoBehaviour
{
    public TMP_Text timerText;         // UI Text to display the timer
    private float timeElapsed = 0f;
    private bool isRunning = true;
    [SerializeField] private Transform target;

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            timeElapsed += Time.deltaTime;  // Add deltaTime to keep it frame-independent
            UpdateTimerDisplay();
        }
        if (target.position.x >= 99.51)
        {
            StopTimer();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeElapsed / 60);
        int seconds = Mathf.FloorToInt(timeElapsed % 60);
        int milliseconds = Mathf.FloorToInt(timeElapsed * 100 % 100);

        timerText.text = $"{minutes:D2}:{seconds:D2}.{milliseconds:D2}";
    }

    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;
    public void ResetTimer()
    {
        timeElapsed = 0f;
        UpdateTimerDisplay();
    }
}