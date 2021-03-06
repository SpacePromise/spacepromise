﻿using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class TimeKeeper
    {
        private readonly Lazy<TimeKeeper> instanceLazy = new Lazy<TimeKeeper>(() => new TimeKeeper());
        public TimeKeeper Instance => this.instanceLazy.Value;
	
        public long UniverseTimeTicks = 0;

        private DateTime? universeDateTimeCached;
        public DateTime UniverseDateTime =>
            this.universeDateTimeCached ?? 
            (this.universeDateTimeCached = new DateTime(this.UniverseTimeTicks, DateTimeKind.Utc)).Value;

        public void SetUniverseTime(long ticks)
        {
            this.UniverseTimeTicks = ticks;
            this.universeDateTimeCached = null;
        }	
    }

    [DisallowMultipleComponent]
    public class Sun : MonoBehaviour
    {
        public AnimationCurve IntensityAnimation;
        private Light sun;

        void Start ()
        {
            this.sun = this.GetComponent<Light>();
            this.cameraReflectionProbe = Camera.main.gameObject.GetComponentInChildren<ReflectionProbe>();
        }

        private int reflectionRenderingId;
        private float counter = 0;
        private ReflectionProbe cameraReflectionProbe;

        void Update ()
        {
            if (this.sun == null)
                return;

            //return;

            this.counter += Time.deltaTime;

            // Rotate sun
            this.sun.transform.Rotate(Vector3.up, (360f / (/*15f * 60f*/60f)) * Time.deltaTime);

            // Trigger reflection rendering
            if (this.cameraReflectionProbe != null && 
                (this.reflectionRenderingId == 0 || 
                 this.cameraReflectionProbe.IsFinishedRendering(this.reflectionRenderingId)))
                this.reflectionRenderingId = this.cameraReflectionProbe.RenderProbe();

            // Update intensity
            this.sun.intensity = this.IntensityAnimation.Evaluate((this.sun.transform.rotation.eulerAngles.x)/180f);
        }
    }
}