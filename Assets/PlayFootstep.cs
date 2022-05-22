using UnityEngine;

public class PlayFootstep : MonoBehaviour
{
    void PlayFootstepSound()
    {
        FindObjectOfType<AudioManager>().Play("Footstep");
    }
}
