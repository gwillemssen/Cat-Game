using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Sound", menuName = "Sound", order = 1)]
public class Sound : ScriptableObject
{
    public AudioClip Clip;
    [Range(0f, 3f)]
    public float Volume = 1f;
    [Range(-3f, 3f)]
    public float Pitch = 1f;
    [Range(0, 1)]
    public float NoisePercentage;
    public bool IncreasesNoiseMeter = false;
    [Range(0, 1f)]
    public float SpatialBlend = 1f;

}