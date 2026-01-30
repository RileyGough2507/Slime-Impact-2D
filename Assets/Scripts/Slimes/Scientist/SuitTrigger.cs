using UnityEngine;

public class SuitTrigger : MonoBehaviour
{
    public ScientistBossIntro bossIntro;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Scientist")
        {
            bossIntro.OnScientistHitSuit();
        }
    }
}
