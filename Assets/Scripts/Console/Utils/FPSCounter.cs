using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Console.Utils
{
    public class FPSCounter
    {
        public float current = 0.0f;
        public float updateInterval = 0.5f;
        // FPS accumulated over the interval
        float accum = 0;
        // Frames drawn over the interval
        int frames = 1;
        // Left time for current interval
        float timeleft;
        float delta;

        public FPSCounter()
        {
            timeleft = updateInterval;
        }

        public IEnumerator Update()
        {
            // skip the first frame where everything is initializing.
            yield return null;

            while (true)
            {
                delta = Time.deltaTime;

                timeleft -= delta;
                accum += Time.timeScale / delta;
                ++frames;

                // Interval ended - update GUI text and start new interval
                if (timeleft <= 0.0f)
                {
                    current = accum / frames;
                    timeleft = updateInterval;
                    accum = 0.0f;
                    frames = 0;
                }

                yield return null;
            }
        }
    }
}