using UnityEngine;

public class PlinkoBall : MonoBehaviour
{
    private PlinkoGame gameManager;

    public void Initialize(PlinkoGame manager)
    {
        gameManager = manager;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем столкновение с каждой из зон +10, +20 и +30
        foreach (Collider2D zone in gameManager.zone10)
        {
            if (collision == zone)
            {
                Debug.Log("Ball entered +10 zone!");
                gameManager.AddScore(10);
                return;
            }
        }

        foreach (Collider2D zone in gameManager.zone20)
        {
            if (collision == zone)
            {
                Debug.Log("Ball entered +20 zone!");
                gameManager.AddScore(20);
                return;
            }
        }

        foreach (Collider2D zone in gameManager.zone30)
        {
            if (collision == zone)
            {
                Debug.Log("Ball entered +30 zone!");
                gameManager.AddScore(30);
                return;
            }
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
        gameManager.OnBallDestroyed();
    }
}