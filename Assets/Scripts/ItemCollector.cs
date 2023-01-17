using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    private int pineapplesCount = 0;
    [SerializeField] private Text pineappleText;
    [SerializeField] private AudioSource collectionSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pineapple"))
        {
            collectionSound.Play();
            Destroy(collision.gameObject);
            pineapplesCount++;
            pineappleText.text = "Pineapples: " + pineapplesCount;
        }


    }
}
