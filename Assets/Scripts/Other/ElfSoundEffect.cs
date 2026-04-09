using UnityEngine;

public class ElfSoundEffect : MonoBehaviour
{
    public AudioSource sfxAudioSource;
    public AudioClip[] dirtStepClips;
    public AudioClip[] grassStepClips;
    [Range(0.8f, 1.2f)] public float minStepPitch = 0.9f; // 随机音高下限
    [Range(0.8f, 1.2f)] public float maxStepPitch = 1.1f; // 随机音高上限

    public void PlayFootstepSound(FootstepType type) {
        AudioClip[] currentClips = 
            (type == FootstepType.Dirt) ? dirtStepClips :
            (type == FootstepType.Grass) ? grassStepClips :
            null;

        if (currentClips != null && currentClips.Length > 0) {
            AudioClip clipToPlay = currentClips[Random.Range(0, currentClips.Length)];
            sfxAudioSource.pitch = Random.Range(minStepPitch, maxStepPitch);
            sfxAudioSource.PlayOneShot(clipToPlay);
        }
    }
}

// 当前踩的东西的枚举类型
public enum FootstepType
{
    Grass, Dirt
}