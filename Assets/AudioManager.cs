using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    [SerializeField] List<Sound> _sounds;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }
    public Sound PlaySound(SoundType soundType)
    {
        Sound sound = _sounds.Where(s => s.Type.Equals(soundType)).ToArray()[0];
        AudioClip clip = sound.Clips[Random.Range(0, sound.Clips.Count - 1)];
        GameObject newSound = new GameObject(soundType.ToString());
        newSound.transform.parent = transform;
        newSound.AddComponent<AudioSource>().clip = clip;
        newSound.GetComponent<AudioSource>().Play();
        Destroy(newSound, clip.length);
        return sound;
    }
}
[System.Serializable]
public struct Sound
{
    public SoundType Type;
    public List<AudioClip> Clips;
}
public enum SoundType
{
    ImpactLight,
    ImpactMedium,
    ImpactHeavy,
    MovementFailure,
}