using UnityEngine;

public class SoundMouse : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;
    public AudioClip mouseClick;


    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            audioSource.PlayOneShot(mouseClick, 1);
        }
    }
}
