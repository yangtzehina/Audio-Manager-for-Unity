// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Yangtze.Audio;
using UnityEngine;
using Yangtze.Audio.Runtime;

public class AudioManagerDemo : MonoBehaviour
{
    [SerializeField]
    private KeyCode testKey = KeyCode.F;
    [SerializeField]
    private AudioEvent testEvent = null;

    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            AudioManager.PlayEvent(testEvent, gameObject);
        }
    }
}