﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Audio;

namespace Yangtze.Audio.Editor
{
    /// <summary>
    /// A minimal version of an ActiveEvent only containing what is needed for the profiler
    /// </summary>
    public class ProfilerEvent
    {
        /// <summary>
        /// The name of the ActiveEvent being profiled
        /// </summary>
        public string eventName = "";
        /// <summary>
        /// The name of the audio file being played in the event
        /// </summary>
        public AudioClip clip;
        /// <summary>
        /// The GameObject containing the AudioSource component playing the event
        /// </summary>
        public GameObject emitterObject;
        /// <summary>
        /// The audio bus the event is routed to
        /// </summary>
        public AudioMixerGroup bus;
    }
}