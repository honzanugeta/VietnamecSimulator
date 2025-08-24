using CodeMonkey.Toolkit.ShopSimulatorDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour, IInteractable
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    private bool isMuted = false;
    private int currentClipIndex = 0;
    private bool isInitialized = false;
    private bool wasPlayingLastFrame = false; // Nová promìnná pro sledování stavu

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioClips.Length > 0 && !isMuted)
        {
            PlayClip(currentClipIndex);
        }
    }

    private void Update()
    {
        // Vylepšená logika pro automatické pøehrávání další skladby
        if (!isMuted && audioClips.Length > 1 && audioSource.clip != null && isInitialized)
        {
            // Zkontroluj, zda se skladba právì dokonèila
            if (wasPlayingLastFrame && !audioSource.isPlaying)
            {
                PlayNextClip();
            }
            wasPlayingLastFrame = audioSource.isPlaying;
        }
    }

    public bool CanDoInteractAction(IInteractable.InteractAction interactAction)
    {
        return interactAction == IInteractable.InteractAction.ToggleRadio ||
               interactAction == IInteractable.InteractAction.Stock ||
               interactAction == IInteractable.InteractAction.Unstock;
    }

    public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary()
    {
        var dictionary = new Dictionary<IInteractable.InteractAction, string>
        {
            { IInteractable.InteractAction.ToggleRadio, isMuted ? "Zapnout rádio" : "Vypnout rádio" }
        };

        if (audioClips.Length > 1)
        {
            dictionary.Add(IInteractable.InteractAction.Stock, "Pøedchozí písnièka");
            dictionary.Add(IInteractable.InteractAction.Unstock, "Další písnièka");

        }

        return dictionary;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform)
    {
        switch (interactAction)
        {
            case IInteractable.InteractAction.ToggleRadio:
                ToggleRadio();
                break;
            case IInteractable.InteractAction.Stock:
                if (audioClips.Length > 1)
                    PlayPreviousClip();
                break;
            case IInteractable.InteractAction.Unstock:
                if (audioClips.Length > 1)

                    PlayNextClip();
                break;
        }
    }

    private void ToggleRadio()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            // Pause instead of stop to preserve position
            audioSource.Pause();
            wasPlayingLastFrame = false; // Reset sledování pøi vypnutí
            }
        else
        {
            if (audioSource.clip != null && isInitialized)
            {
                // Resume paused audio
                audioSource.UnPause();
            }
            else if (audioClips.Length > 0)
            {
                // Start fresh if no clip is loaded
                PlayClip(currentClipIndex);
            }
        }
    }

    private void PlayNextClip()
    {
        if (audioClips.Length > 0)
        {
            currentClipIndex = (currentClipIndex + 1) % audioClips.Length;
            PlayClip(currentClipIndex);
        }
    }

    private void PlayPreviousClip()
    {
        if (audioClips.Length > 0)
        {
            currentClipIndex = (currentClipIndex - 1 + audioClips.Length) % audioClips.Length;
            PlayClip(currentClipIndex);
        }
    }

    private void PlayClip(int index)
    {
        if (audioClips.Length > 0 && index >= 0 && index < audioClips.Length)
        {
            audioSource.clip = audioClips[index];
            if (!isMuted)
            {
                audioSource.Play();
                wasPlayingLastFrame = true; // Nastav sledování pøi spuštìní
            }
            isInitialized = true;
        }
    }
}
