using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    Image image;
    private int clicks = 0;
    private float randomStart = 0;
    float lastClickTime = 0;
    float lastActivateTime;

    [SerializeField] private int clicksRequired = 5;        // NEW — change in Inspector
    [SerializeField] ParticleSystem click5Particles = null;
    [SerializeField] ParticleSystem[] click5ParticleOutlines = null;

    public void Click()
    {
        lastClickTime = Time.time;
        clicks++;

        if (clicks >= clicksRequired)
        {
            if (clicks == clicksRequired)
            {
                randomStart = Random.Range(0f, 100f);
            }

            speed = speedMax;
            lastActivateTime = Time.time;
            CameraShake.Shake(.15f, .3f);

            // -----------------------------
            // MAIN PARTICLE PREFAB
            // -----------------------------
            ParticleSystem newParticles = Instantiate(click5Particles);

            // FIXED: assign seed to the *clone*, not the prefab
            newParticles.randomSeed = (uint)Random.Range(1, 999999);

            SetGradientKeys(newParticles.colorOverLifetime);
            newParticles.Play();

            // -----------------------------
            // OUTLINE PARTICLES
            // -----------------------------
            foreach (var particle in click5ParticleOutlines)
            {
                ParticleSystem outlineClone = Instantiate(particle);

                // FIXED: set seed to the clone
                outlineClone.randomSeed = (uint)Random.Range(1, 999999);

                outlineClone.Play();
            }
        }
    }

    private void SetGradientKeys(ParticleSystem.ColorOverLifetimeModule colorOverLifetime)
    {
        var color = colorOverLifetime.color;
        var gradient = color.gradient;
        var colorKeys = gradient.colorKeys;

        for (int i = 0; i < 8; i++)
        {
            colorKeys[i].color = HSBColor.ToColor(
                new HSBColor(Mathf.PingPong(randomStart + i * .125f, 1), 1, 1));
        }

        gradient.colorKeys = colorKeys;
        color.gradient = gradient;
        colorOverLifetime.color = color;
    }

    private void Awake()
    {
        clicks = 0;
        image = GetComponent<Image>();
    }

    float speedMax = 0.5f;
    float speed = 0.5f;
    float speedMin = 0.2f;

    void Update()
    {
        if (clicks >= clicksRequired)
        {
            randomStart += Time.deltaTime * speed;
            float pingPong = Mathf.PingPong(randomStart, 1);

            if (Time.time >= lastActivateTime + 6)
            {
                speed = Mathf.Max(speed - Time.deltaTime, speedMin);

                if (speed == speedMin)
                {
                    if (pingPong >= 70f / 255f && pingPong <= 80f / 255f)
                    {
                        pingPong = 75f / 255f;
                        clicks = 0;
                    }
                }
            }

            image.color = HSBColor.ToColor(
                new HSBColor(Mathf.PingPong(randomStart, 1), 1, 1));
        }
        else if (clicks > 0)
        {
            if (Time.time >= lastClickTime + 1)
            {
                clicks = 0;
            }
        }
    }
}
