using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorcoll : MonoBehaviour
{
    [SerializeField]GameObject basketAgent;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            collision.gameObject.SetActive(false);
            StartCoroutine(basketAgent.GetComponent<BasketAgent>().SpawnBall());
            basketAgent.GetComponent<BasketAgent>().AddReward(-0.3f);
            basketAgent.GetComponent<BasketAgent>().missedCount++;
        }
    }

}
