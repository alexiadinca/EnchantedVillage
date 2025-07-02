/*
using UnityEngine;
using System.Collections.Generic;

public class GameClock : MonoBehaviour
{
    [Header("Time Settings")]
    public int days = 1;  // day counter
    public int hours = 8;
    public int minutes = 0;
    public float timeSpeed = 3600f; // 1 in-game hour = 1 real second
    private float timer;

    [Header("Sun & Moon")]
    public Light sunLight;
    public Light moonLight;

    [Header("Skybox Settings")]
    public Material skyboxDay;
    public Material skyboxSunset;
    public Material skyboxNight;
    private Material currentSkybox;

    [Header("Village Lights")]
    public List<Light> villageLights = new List<Light>();

    [Header("Night Sounds")]
    public AudioSource nightAmbience;
    public AudioSource dayAmbience;


    void Start()
    {
        sunLight.enabled = true;
        sunLight.intensity = 1f;
        moonLight.enabled = false;

        UpdateSun();
        UpdateSkybox();
        UpdateVillageLights();
        UpdateNightAmbience();
    }

    void Update()
    {
        timer += Time.deltaTime * timeSpeed;
        if (timer >= 60f)
        {
            timer = 0f;
            minutes++;
            if (minutes >= 60)
            {
                minutes = 0;
                hours = (hours + 1) % 24;
            }
        }

        int previousHour = hours;
        hours = (hours + 1) % 24;

        // if we wrapped past midnight, increment the day counter
        if (hours == 0 && previousHour == 23)
        {
            days++;
            Debug.Log($"🌞 A new day begins: Day {days}");
        }

        UpdateSun();
        UpdateSkybox();
        UpdateAmbience();
        UpdateVillageLights();
        UpdateNightAmbience();

        Debug.Log($"🕒 Time: {hours:D2}:{minutes:D2}");
    }

    void UpdateSun()
    {
        float totalMinutes = hours * 60 + minutes;
        float sunrise = 6 * 60;
        float sunset = 19 * 60;

        if (totalMinutes >= sunrise && totalMinutes <= sunset)
        {
            float dayDuration = sunset - sunrise;
            float timeSinceSunrise = totalMinutes - sunrise;
            float normalizedTime = timeSinceSunrise / dayDuration;

            float sunAngle = Mathf.Lerp(0f, 180f, normalizedTime);
            sunLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle - 90f, 170f, 0f));

            sunLight.enabled = true;
            sunLight.intensity = 1f;
            moonLight.enabled = false;
        }
        else
        {
            sunLight.enabled = false;
            moonLight.enabled = true;
            moonLight.intensity = 0.3f;
        }
    }

    void UpdateSkybox()
    {
        Material selectedSkybox = null;

        if (hours >= 6 && hours < 18)
            selectedSkybox = skyboxDay;
        else if (hours >= 18 && hours < 19)
            selectedSkybox = skyboxSunset;
        else
            selectedSkybox = skyboxNight;

        if (selectedSkybox != currentSkybox && selectedSkybox != null)
        {
            RenderSettings.skybox = selectedSkybox;
            currentSkybox = selectedSkybox;
            DynamicGI.UpdateEnvironment();
        }
    }

    void UpdateAmbience()
    {
        bool isDay = hours >= 6 && hours < 18;

        if (dayAmbience != null)
        {
            if (isDay && !dayAmbience.isPlaying)
            {
                dayAmbience.time = 0f;
                dayAmbience.Play();
            }
            else if (!isDay && dayAmbience.isPlaying)
            {
                dayAmbience.Stop();
            }
        }
        Debug.Log($"🎵 Day Ambience Status | isDay: {isDay}, playing: {dayAmbience.isPlaying}");

    }


    void UpdateVillageLights()
    {
        bool isNight = hours < 6 || hours >= 18;

        foreach (Light light in villageLights)
        {
            if (light != null)
                light.enabled = isNight;
        }
    }

    void UpdateNightAmbience()
    {
        bool isNight = hours < 6 || hours >= 19;

        if (nightAmbience != null)
        {
            if (nightAmbience != null)
            {
                if (isNight)
                {
                    if (!nightAmbience.isPlaying)
                    {
                        nightAmbience.time = 0f; // reset playback position
                        nightAmbience.Play();
                    }
                }
                else
                {
                    if (nightAmbience.isPlaying)
                        nightAmbience.Stop();
                }
            }

        }
    }
}
*/


using UnityEngine;
using System.Collections.Generic;

public class GameClock : MonoBehaviour
{
    [Header("Time Settings")]
    public int days = 1;             // track day count
    public int hours = 8;
    public int minutes = 0;

    [Tooltip("How many real-world seconds it takes to advance one in-game minute.")]
    public float secondsPerGameMinute = 1f;

    private float timer = 0f;

    [Header("Sun & Moon")]
    public Light sunLight;
    public Light moonLight;

    [Header("Skybox Settings")]
    public Material skyboxDay;
    public Material skyboxSunset;
    public Material skyboxNight;
    private Material currentSkybox;

    [Header("Village Lights")]
    public List<Light> villageLights = new List<Light>();

    [Header("Ambience")]
    public AudioSource dayAmbience;
    public AudioSource nightAmbience;

    void Start()
    {
        // initial setup
        sunLight.enabled = true;
        sunLight.intensity = 1f;
        moonLight.enabled = false;

        UpdateSun();
        UpdateSkybox();
        UpdateVillageLights();
        UpdateAmbience();
    }

    void Update()
    {
        // --- TIME PROGRESSION ---
        timer += Time.deltaTime;
        // once we've accumulated enough real seconds for 1 in-game minute:
        while (timer >= secondsPerGameMinute)
        {
            timer -= secondsPerGameMinute;  // keep any extra
            AdvanceMinute();
        }

        // --- EVERYTHING ELSE ---
        UpdateSun();
        UpdateSkybox();
        UpdateVillageLights();
        UpdateAmbience();
    }

    private void AdvanceMinute()
    {
        minutes++;
        if (minutes >= 60)
        {
            minutes = 0;
            hours++;

            if (hours >= 24)
            {
                hours = 0;
                days++;
                Debug.Log($"🌞 New Day: {days}");
            }
        }

        Debug.Log($"🕒 Day {days} - {hours:D2}:{minutes:D2}");
    }

    void UpdateSun()
    {
        float totalMins = hours * 60 + minutes;
        float sunrise = 6 * 60;
        float sunset = 19 * 60;

        if (totalMins >= sunrise && totalMins <= sunset)
        {
            // daytime sun arc
            float normalized = (totalMins - sunrise) / (sunset - sunrise);
            float angle = Mathf.Lerp(0, 180, normalized) - 90f;
            sunLight.transform.rotation = Quaternion.Euler(angle, 170, 0);
            sunLight.enabled = true;
            moonLight.enabled = false;
        }
        else
        {
            sunLight.enabled = false;
            moonLight.enabled = true;
            moonLight.intensity = 0.3f;
        }
    }

    void UpdateSkybox()
    {
        Material next = (hours >= 6 && hours < 18)
            ? skyboxDay
            : (hours >= 18 && hours < 19)
                ? skyboxSunset
                : skyboxNight;

        if (next != currentSkybox && next != null)
        {
            RenderSettings.skybox = next;
            currentSkybox = next;
            DynamicGI.UpdateEnvironment();
        }
    }

    void UpdateVillageLights()
    {
        bool isNight = hours < 6 || hours >= 18;
        foreach (var l in villageLights) if (l) l.enabled = isNight;
    }

    void UpdateAmbience()
    {
        bool isDay = hours >= 6 && hours < 18;

        if (dayAmbience)
        {
            if (isDay && !dayAmbience.isPlaying) dayAmbience.Play();
            else if (!isDay && dayAmbience.isPlaying) dayAmbience.Stop();
        }

        if (nightAmbience)
        {
            if (!isDay && !nightAmbience.isPlaying) nightAmbience.Play();
            else if (isDay && nightAmbience.isPlaying) nightAmbience.Stop();
        }
    }
}
