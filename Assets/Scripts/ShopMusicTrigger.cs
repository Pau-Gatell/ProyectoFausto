using UnityEngine;

public class ShopMusicTrigger : MonoBehaviour
{
    public AudioSource shopMusic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!shopMusic.isPlaying)
                shopMusic.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopMusic.Stop();
        }
    }
}