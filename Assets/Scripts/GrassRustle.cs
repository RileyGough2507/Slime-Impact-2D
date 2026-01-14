using UnityEngine;

public class GrassRustle : MonoBehaviour
{
    public Animator animator;
    public string rustleAnimationName = "GrassRustle";

    private bool isPlaying = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPlaying && collision.CompareTag("Player"))
        {
            StartCoroutine(Rustle());
        }
    }

    private System.Collections.IEnumerator Rustle()
    {
        isPlaying = true;

        animator.Play(rustleAnimationName, 0, 0f);

        // Wait for the animation length
        float length = animator.runtimeAnimatorController.animationClips[0].length;
        yield return new WaitForSeconds(length);

        isPlaying = false;
    }
}
