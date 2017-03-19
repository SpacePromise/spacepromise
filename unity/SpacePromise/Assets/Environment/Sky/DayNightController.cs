using Assets.Engine.Unity;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Environment.Sky
{
    /// <summary>
    /// Implements a Day/Night cycle relative to the game world, with a World-Time clock, and optional Direcitonal Light control.
    /// </summary>
    /// <!-- 
    /// Version 0.0.1.0 (beta)
    /// By Reed Kimble
    /// Last Revision 5/19/2011
    /// -->
    /// <remarks>
    /// Add this script to a new GameObject to create a Day/Night cycle for the scene. The day/night cycle effect is achieved by modifying the
    /// scene ambient light color, fog color, and skybox material.  The script will also rotate, fade, and enable/disable a directional
    /// light if one is attached to the same GameObject as the DayNightController script.  The length of a complete day (in seconds) and the number of
    /// hours per day are modifiable in the script fields and allow calculation of the World-time hour-of-day.  Each 'phase' of the day is considered
    /// to be 1/4 of the dayCycleLength.
    /// 
    /// Note that the script will rotate the GameObject transform it is attached to, even if no directional light is attached. You will probably want to 
    /// use a dedicated GameObject for this script and any attached directional light.
    /// 
    /// The GameObject with this script should be placed roughly in the center of your scene, with a height of about 2/3 the length (or width) of the terrain.
    /// If that GameObject has a light, it should be a directional light pointed straight down (x:90, y:0, z:0).  This light will then be rotated around its
    /// x-axis (relative to the scene; eg. as if you used the rotation tool locked on the green x axis) and will reach its horizontal peeks during the
    /// end of dusk and beginning of dawn, turning off during the night (upside-down rotation).
    /// 
    /// The reset command will attempt to use the default skybox assets DawnDusk, Sunny2, and StarryNight if that package has been imported.  The
    /// command will also choose acceptable color values and set the day cycle to two minutes. It is suggested that the directional light be a light-
    /// yellow or peach in color with a roughly 0.33f intensity.  The script will not set any default values for the light, if one exists, so the light
    /// must be configured manually.
    /// </remarks>
    public class DayNightController : MonoBehaviour
    {
        /// <summary>
        /// The number of real-world seconds in one game day.
        /// </summary>
        [SerializeField]
        private float dayCycleLength;

        /// <summary>
        /// The number of hours per day used in the WorldHour time calculation.
        /// </summary>
        [SerializeField]
        private float hoursPerDay;

        /// <summary>
        /// Dawn occurs at currentCycleTime = 0.0f, so this offsets the WorldHour time to make
        /// dawn occur at a specified hour. A value of 3 results in a 5am dawn for a 24 hour world clock.
        /// </summary>
        [SerializeField]
        private float dawnTimeOffset;

        [SerializeField]
        private Gradient dawnGradient;

        [SerializeField]
        private Gradient duskGradient;

        [SerializeField]
        private AnimationCurve sunIntensity;

        /// <summary>
        /// The current time within the day cycle. Modify to change the World Time.
        /// </summary>
        [ExposeProperty]
        public float CurrentCycleTime { get; private set; }

        /// <summary>
        /// The current 'phase' of the day; Dawn, Day, Dusk, or Night
        /// </summary>
        [ExposeProperty]
        public DayPhase CurrentPhase { get; private set; }

        [ExposeProperty]
        public int WorldTimeHour
        {
            get { return this.worldTimeHour; }
        }

        [ExposeProperty]
        public int WorldTimeMinute
        {
            get { return this.worldTimeMinute; }
        }

        private int worldTimeHour;
        private int worldTimeMinute;

        private float dawnTime;
        private float dayTime;
        private float duskTime;
        private float nightTime;
        private float quarterDay;
        private float lightIntensity;
        private Light lightSource;

        /// <summary>
        /// Initializes working variables and performs starting calculations.
        /// </summary>
        void Initialize()
        {
            this.quarterDay = dayCycleLength * 0.25f;
            this.dawnTime = 0.0f;
            this.dayTime = dawnTime + quarterDay;
            this.duskTime = dayTime + quarterDay;
            this.nightTime = duskTime + quarterDay;
            this.lightSource = this.GetComponentInChildren<Light>();
            this.lightIntensity = this.lightSource.intensity;
        }

        /// <summary>
        /// Sets the script control fields to reasonable default values for an acceptable day/night cycle effect.
        /// </summary>
        void Reset()
        {
            this.dayCycleLength = 1200.0f;
            this.hoursPerDay = 24.0f;
            this.dawnTimeOffset = 3.0f;
            this.dawnGradient = new Gradient
            {
                mode = GradientMode.Blend,
                colorKeys = new[]
                {
                    new GradientColorKey(new Color(0, 0, 0), 0),
                    new GradientColorKey(new Color(25/255f, 38/255f, 53/255f), 0.26f),
                    new GradientColorKey(new Color(62/255f, 59/255f, 39/255f), 0.53f),
                    new GradientColorKey(new Color(83/255f, 83/255f, 83/255f), 1f)
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                }
            };
            this.duskGradient = new Gradient
            {
                mode = GradientMode.Blend,
                colorKeys = new[]
                {
                    new GradientColorKey(new Color(99/255f, 99/255f, 99/255f), 0),
                    new GradientColorKey(new Color(98/255f, 76/255f, 0), 0.26f),
                    new GradientColorKey(new Color(73/255f, 47/255f, 15/255f), 0.56f),
                    new GradientColorKey(new Color(0, 0, 0), 1f)
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1)
                }
            };
            this.sunIntensity = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(0.27f, 0.00f, 0.03f, 0.03f),
                new Keyframe(0.42f, 0.81f, 2.80f, 2.80f),
                new Keyframe(0.56f, 1.00f),
                new Keyframe(0.72f, 0.76f, -4.5f, -4.5f),
                new Keyframe(0.75f, 0.00f, 0.01f, 0.01f),
                new Keyframe(1, 0));
        }

        void Start()
        {
            this.Initialize();
        }

        void Update()
        {
            // Rudementary phase-check algorithm:
            if (CurrentCycleTime > nightTime && CurrentPhase == DayPhase.Dusk)
            {
                SetNight();
            }
            else if (CurrentCycleTime > duskTime && CurrentPhase == DayPhase.Day)
            {
                SetDusk();
            }
            else if (CurrentCycleTime > dayTime && CurrentPhase == DayPhase.Dawn)
            {
                SetDay();
            }
            else if (CurrentCycleTime > dawnTime && CurrentCycleTime < dayTime && CurrentPhase == DayPhase.Night)
            {
                SetDawn();
            }

            // Perform standard updates:
            UpdateWorldTime();
            UpdateDaylight();

            // Update the current cycle time:
            CurrentCycleTime += Time.deltaTime;
            CurrentCycleTime = CurrentCycleTime % dayCycleLength;
        }

        /// <summary>
        /// Sets the currentPhase to Dawn, turning on the directional light, if any.
        /// </summary>
        public void SetDawn()
        {
            this.lightSource.enabled = true;
            //RenderSettings.ambientMode = AmbientMode.Flat;
            this.CurrentPhase = DayPhase.Dawn;
        }

        /// <summary>
        /// Sets the currentPhase to Day, ensuring full day color ambient light, and full
        /// directional light intensity, if any.
        /// </summary>
        public void SetDay()
        {
            this.lightSource.enabled = true;
            this.lightSource.intensity = lightIntensity;
            CurrentPhase = DayPhase.Day;
        }

        /// <summary>
        /// Sets the currentPhase to Dusk.
        /// </summary>
        public void SetDusk()
        {
            this.lightSource.enabled = true;
            CurrentPhase = DayPhase.Dusk;
        }

        /// <summary>
        /// Sets the currentPhase to Night, ensuring full night color ambient light, and
        /// turning off the directional light, if any.
        /// </summary>
        public void SetNight()
        {
            this.lightSource.enabled = false;
            CurrentPhase = DayPhase.Night;
        }

        /// <summary>
        /// If the currentPhase is dawn or dusk, this method adjusts the ambient light color and direcitonal
        /// light intensity (if any) to a percentage of full dark or full light as appropriate. Regardless
        /// of currentPhase, the method also rotates the transform of this component, thereby rotating the
        /// directional light, if any.
        /// </summary>
        private void UpdateDaylight()
        {
            if (CurrentPhase == DayPhase.Dawn)
            {
                var relativeTime = CurrentCycleTime - dawnTime;
                var relative = relativeTime / quarterDay;
                this.lightSource.intensity = Mathf.Clamp01(this.lightIntensity * this.sunIntensity.Evaluate(relative / 2f));
                //RenderSettings.ambientLight = this.dawnGradient.Evaluate(relative);
            }
            else if (CurrentPhase == DayPhase.Dusk)
            {
                float relativeTime = CurrentCycleTime - duskTime;
                var relative = (quarterDay - relativeTime) / quarterDay;
                this.lightSource.intensity = Mathf.Clamp01(this.lightIntensity * this.sunIntensity.Evaluate((1f - relative) / 2f + 0.5f));
                //RenderSettings.ambientLight = this.duskGradient.Evaluate(1f - relative);
            }

            this.transform.rotation = Quaternion.Euler(CurrentCycleTime / dayCycleLength * 360f + 90, -90, -90);
        }

        /// <summary>
        /// Updates the World-time hour based on the current time of day.
        /// </summary>
        private void UpdateWorldTime()
        {
            this.worldTimeHour = (int)((Mathf.Ceil(this.CurrentCycleTime / this.dayCycleLength * this.hoursPerDay) + this.dawnTimeOffset) % this.hoursPerDay) + 1;
            this.worldTimeMinute = Mathf.FloorToInt(this.CurrentCycleTime / this.dayCycleLength * this.hoursPerDay % 1f / (1f / 60f));
        }

        public enum DayPhase
        {
            Night = 0,
            Dawn = 1,
            Day = 2,
            Dusk = 3
        }
    }
}