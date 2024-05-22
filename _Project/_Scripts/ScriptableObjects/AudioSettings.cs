using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "ScriptableObjects/AudioSettings")]
public class AudioSettings : ScriptableObject
{
    [Range(-80, 20)]
    public float masterVolume = 1f;
    [Range(0, 1)]
    public float musicVolume = 1f;
    [Range(0, 1)]
    public float sfxVolume = 1f;
}
