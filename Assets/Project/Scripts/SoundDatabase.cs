using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "ScriptableObjects/SoundDatabase", order = 2)]

public class SoundDatabase : ScriptableObject
{
    public List<AudioClip>  buttonClickSound = new List<AudioClip>();
    public AudioClip matchSound;
    public AudioClip backgroundMusic;
}
