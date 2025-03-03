﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Yangtze.Audio.Runtime
{
    /// <summary>
    /// The final node in an audio event
    /// </summary>
    public class AudioOutput : AudioNode
    {
        /// <summary>
        /// The audio bus to route this event to
        /// </summary>
        [SerializeField]
        public AudioMixerGroup mixerGroup = null;
        /// <summary>
        /// The low end of the random volume assigned when playing the event
        /// </summary>
        [Range(0, 1)]
        public float MinVolume = 1;
        /// <summary>
        /// The high end of the random volume assigned when playing the event
        /// </summary>
        [Range(0, 1)]
        public float MaxVolume = 1;
        /// <summary>
        /// The low end of the random pitch assigned when playing the event
        /// </summary>
        [Range(-3, 3)]
        public float MinPitch = 1;
        /// <summary>
        /// The high end of the random pitch assigned when playing the event
        /// </summary>
        [Range(-3, 3)]
        public float MaxPitch = 1;
        /// <summary>
        /// Whether to make the sound seamlessly loop
        /// </summary>
        [SerializeField]
        public bool loop = false;
        /// <summary>
        /// Amount of spatialization applied to the AudioSource
        /// </summary>
        [SerializeField]
        public float spatialBlend = 0;
        /// <summary>
        /// Whether to use the spatializer assigned in the project's audio settings
        /// </summary>
        [SerializeField]
        public bool HRTF = false;
        /// <summary>
        /// 音效能够听到的最短路径
        /// </summary>
        [SerializeField]
        public float MinDistance = 1;
        /// <summary>
        /// The distance beyond which the sound can no longer be heard
        /// </summary>
        [SerializeField]
        public float MaxDistance = 10;
        /// <summary>
        /// 音效的曲线模式
        /// </summary>
        [SerializeField]
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Custom;
        /// <summary>
        /// The response curve for how loud the sound will be at different distances
        /// </summary>
        [SerializeField]
        public AnimationCurve attenuationCurve = new AnimationCurve();
        /// <summary>
        /// The amount of doppler effect applied to the sound when moving relative to the listener
        /// </summary>
        [SerializeField]
        public float dopplerLevel = 1;

        /// <summary>
        /// The width in pixels for the node's window in the graph
        /// </summary>
        private const float NodeWidth = 300;
        /// <summary>
        /// The height in pixels for the node's window in the graph
        /// </summary>
        private const float NodeHeight = 185;

        /// <summary>
        /// Apply all of the properties to the ActiveEvent and start processing the rest of the event's nodes
        /// </summary>
        /// <param name="activeEvent"></param>
        public override void ProcessNode(ActiveEvent activeEvent)
        {
            if (this.input.ConnectedNodes == null || this.input.ConnectedNodes.Length == 0)
            {
                Debug.LogWarningFormat("No connected nodes for {0}", this.name);
                return;
            }

            activeEvent.SetVolume(Random.Range(this.MinVolume, this.MaxVolume));
            activeEvent.SetPitch(Random.Range(this.MinPitch, this.MaxPitch));

            AudioSource eventSource = activeEvent.source;
            eventSource.outputAudioMixerGroup = this.mixerGroup;
            eventSource.loop = this.loop;
            eventSource.spatialBlend = this.spatialBlend;
            if (this.spatialBlend > 0)
            {
                eventSource.spatialize = this.HRTF;
                eventSource.maxDistance = this.MaxDistance;
                if (this.rolloffMode == AudioRolloffMode.Custom)
                {
                    eventSource.rolloffMode = AudioRolloffMode.Custom;
                    eventSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, this.attenuationCurve);
                }
                else
                {
                    eventSource.rolloffMode = this.rolloffMode;
                    eventSource.minDistance = this.MinDistance;
                }
                eventSource.dopplerLevel = this.dopplerLevel;
            }

            ProcessConnectedNode(0, activeEvent);
        }

#if UNITY_EDITOR

        /// <summary>
        /// EDITOR: Initialize variables for output settings
        /// </summary>
        /// <param name="position">The position of the node window in the graph</param>
        public override void InitializeNode(Vector2 position)
        {
            this.name = "Output";
            this.nodeRect.position = position;
            this.nodeRect.width = NodeWidth;
            this.nodeRect.height = NodeHeight;
            AddInput();
        }

        /// <summary>
        /// EDITOR: Draw the node's properties in the node window in the graph
        /// </summary>
        protected override void DrawProperties()
        {
            this.mixerGroup = EditorGUILayout.ObjectField("Mixer Group", this.mixerGroup, typeof(AudioMixerGroup), false) as AudioMixerGroup;
            EditorGUILayout.MinMaxSlider("Volume", ref this.MinVolume, ref this.MaxVolume, Volume_Min, Volume_Max);
            EditorGUILayout.MinMaxSlider("Pitch", ref this.MinPitch, ref this.MaxPitch, Pitch_Min, Pitch_Max);
            this.loop = EditorGUILayout.Toggle("Loop", this.loop);
            this.spatialBlend = EditorGUILayout.Slider("Spatial Blend", this.spatialBlend, 0, 1);

            EditorGUI.BeginDisabledGroup(this.spatialBlend == 0);
            this.HRTF = EditorGUILayout.Toggle("HRTF", this.HRTF);
            this.rolloffMode = (AudioRolloffMode)EditorGUILayout.EnumPopup("AudioRolloffMode", this.rolloffMode);
            EditorGUI.BeginDisabledGroup(this.rolloffMode == AudioRolloffMode.Custom);
            this.MinDistance = EditorGUILayout.FloatField("Min Distance", this.MinDistance);
            EditorGUI.EndDisabledGroup();
            this.MaxDistance = EditorGUILayout.FloatField("Max Distance", this.MaxDistance);
            EditorGUI.BeginDisabledGroup(this.rolloffMode != AudioRolloffMode.Custom);
            this.attenuationCurve = EditorGUILayout.CurveField("Attenuation", this.attenuationCurve);
            EditorGUI.EndDisabledGroup();
            this.dopplerLevel = EditorGUILayout.FloatField("Doppler Level", this.dopplerLevel);
            EditorGUI.EndDisabledGroup();
        }

#endif
    }
}