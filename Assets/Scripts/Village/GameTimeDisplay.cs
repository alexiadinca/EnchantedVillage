using UnityEngine;
using TMPro;

public class GameTimeDisplay : MonoBehaviour
{
    [Header("References")]
    public GameClock gameClock;
    public TMP_Text dayText;
    public TMP_Text timeText;

    void Start()
    {
        // initialize day counter if needed
        if (gameClock.days <= 0)
            gameClock.days = 1;
    }

    void Update()
    {
        // — Increment days when GameClock rolls over past midnight —
        if (gameClock.hours == 0 && gameClock.minutes == 0 && !m_countedMidnight)
        {
            gameClock.days++;
            m_countedMidnight = true;
        }
        else if (gameClock.hours != 0 || gameClock.minutes != 0)
        {
            m_countedMidnight = false;
        }

        // — Update UI —
        dayText.text = $"Day {gameClock.days}";

        // convert to 12-hour + AM/PM
        int h = gameClock.hours % 12;
        if (h == 0) h = 12;
        string ampm = gameClock.hours < 12 ? "AM" : "PM";
        timeText.text = $"{h}:{gameClock.minutes:00} {ampm}";
    }

    // private flag so we only count midnight once
    private bool m_countedMidnight = false;
}
