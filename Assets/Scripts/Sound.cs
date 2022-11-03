using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Sound", menuName = "Sound", order = 1)]
public class Sound : ScriptableObject
{
    public string Name;
    public AudioClip Clip;
    [Range(0f, 1f)]
    public float Volume;
    [Range(.1f, 1f)]
    public float Pitch;
    [Range(1, 100)]
    public int NoiseAmt;
}
