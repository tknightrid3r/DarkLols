using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : MonoBehaviour
{
    public AudioClip collectSound;
    public float collectDistance = 2f;

    private AudioSource audioSource;
    private bool isPlayerNearby = false;
    private bool collected = false;
    private Transform player;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (collected) return;

        if (isPlayerNearby)
        {
            BonfirePromptUI.Instance.ShowPrompt();

            if (Input.GetKeyDown(KeyCode.E))
            {
                Collect();
                audioSource.PlayOneShot(collectSound);
            }
        }
        else
        {
            BonfirePromptUI.Instance.HidePrompt();
        }

        // Optional: check distance in case player walks away
        if (player != null && Vector3.Distance(transform.position, player.position) > collectDistance)
        {
            isPlayerNearby = false;
        }
    }

    private void Collect()
    {
        collected = true;
        BonfirePromptUI.Instance.HidePrompt();

        // Play sound
        if (collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }

        // Notify manager
        BonfireManager.Instance.CollectBonfire();

        Destroy(gameObject, 4f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            BonfirePromptUI.Instance.HidePrompt();
        }
    }
}