using System;
using UnityEngine;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    public enum Audios
    {
        Ambiance,
        Begin,
        Button,
        CharacterDie,
        CharacterHurt,
        Dash,
        EnemyDie,
        EnemyHurt1,
        EnemyHurt2,
        FootStep,
        Lose,
        Menu,
        Pickup,
        PowerOff,
        PowerOn,
        Shield,
        Sword,
        Trumpet,
        Win
    }

    private static Audio instance;
    public AudioMixerGroup mixer;
    public AudioClips[] audioClips;
    private GameObject ambianceGameObject;
    private float lastFootStep;
    private AudioSource menuAudioSource;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Play(Audios selectedAudio, bool loop)
    {
        var a = Array.Find(audioClips, sound => sound.audioName == selectedAudio);
        if (a != null)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            if (a.audioName == Audios.Menu)
            {
                if (menuAudioSource == null) menuAudioSource = audioSource;
                else Destroy(menuAudioSource);
            }

            audioSource.clip = a.audioClip;
            audioSource.outputAudioMixerGroup = mixer;
            audioSource.volume = a.volume;
            if (loop) audioSource.loop = true;
            audioSource.Play();
            if (!loop) Destroy(audioSource, audioSource.clip.length);
        }
    }

    public void PlayOnce(Audios selectedAudio)
    {
        Play(selectedAudio, false);
    }

    public AudioSource PlayWalk(GameObject walkObject)
    {
        var a = Array.Find(audioClips, sound => sound.audioName == Audios.FootStep);
        if (a != null)
        {
            var audioSource = walkObject.AddComponent<AudioSource>();
            audioSource.clip = a.audioClip;
            audioSource.outputAudioMixerGroup = mixer;
            audioSource.volume = a.volume;
            audioSource.Play();
            return audioSource;
        }

        return null;
    }

    public void PlayAmbiance()
    {
        var a = Array.Find(audioClips, sound => sound.audioName == Audios.Ambiance);
        if (a != null)
            if (ambianceGameObject == null)
            {
                ambianceGameObject = new GameObject("Ambiance");
                ambianceGameObject.transform.localPosition = new Vector3(769.8f, 1.215772f, 317.73f);
                var audioSource = ambianceGameObject.AddComponent<AudioSource>();
                var keys = new Keyframe[18]; // Key Frames For Ambiance 3D Custom Rolloff
                keys[0] = new Keyframe(0f, 1f);
                keys[1] = new Keyframe(2.5f, 0.5230858f);
                keys[2] = new Keyframe(5.6f, 0.7568662f);
                keys[3] = new Keyframe(7.1f, 0.6156462f);
                keys[4] = new Keyframe(8.35f, 0.4480292f);
                keys[5] = new Keyframe(11.6f, 0.6005238f);
                keys[6] = new Keyframe(17.3f, 0.2945787f);
                keys[7] = new Keyframe(25.4f, 0.3936036f);
                keys[8] = new Keyframe(32.05f, 0.3828415f);
                keys[9] = new Keyframe(38.5f, 0.5516127f);
                keys[10] = new Keyframe(47.65f, 0.6252577f);
                keys[11] = new Keyframe(57.2f, 0.6098276f);
                keys[12] = new Keyframe(64.8f, 0.437909f);
                keys[13] = new Keyframe(73.35f, 0.6636088f);
                keys[14] = new Keyframe(85.72f, 0.4147403f);
                keys[15] = new Keyframe(89.2f, 0.6121156f);
                keys[16] = new Keyframe(93.95f, 0.3307125f);
                keys[17] = new Keyframe(95f, 0f);

                audioSource.clip = a.audioClip;
                audioSource.outputAudioMixerGroup = mixer;
                audioSource.loop = true;
                audioSource.volume = a.volume;
                audioSource.spatialBlend = 1f;
                audioSource.minDistance = 1;
                audioSource.maxDistance = 95;
                audioSource.rolloffMode = AudioRolloffMode.Custom;
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, new AnimationCurve(keys));
                audioSource.Play();
            }
            else
            {
                ambianceGameObject.SetActive(true);
            }
    }

    public AudioSource EnemyWalk(GameObject walkObject)
    {
        var a = Array.Find(audioClips, sound => sound.audioName == Audios.FootStep);
        if (a != null)
        {
            var audioSource = walkObject.AddComponent<AudioSource>();
            audioSource.clip = a.audioClip;
            audioSource.outputAudioMixerGroup = mixer;
            audioSource.volume = a.volume;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 1;
            audioSource.maxDistance = 3;
            audioSource.Play();
            return audioSource;
        }

        return null;
    }

    public void PlayEnemy(GameObject enemyObject, Audios selectedAudio)
    {
        var a = Array.Find(audioClips, sound => sound.audioName == selectedAudio);
        if (a != null)
        {
            var audioSource = enemyObject.AddComponent<AudioSource>();
            audioSource.clip = a.audioClip;
            audioSource.outputAudioMixerGroup = mixer;
            audioSource.volume = a.volume;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 1;
            audioSource.maxDistance = 3;
            audioSource.Play();
            Destroy(audioSource, audioSource.clip.length);
        }
    }

    public void StopAmbiance()
    {
        ambianceGameObject.SetActive(false);
    }

    public void StopMenu()
    {
        Destroy(menuAudioSource);
    }

    public void ChangeMixerVolume(float volume)
    {
        mixer.audioMixer.SetFloat("volume", volume);
    }

    [Serializable]
    public class AudioClips
    {
        public Audios audioName;
        public AudioClip audioClip;

        [Range(0f, 1f)] public float volume = 1f;
    }
}