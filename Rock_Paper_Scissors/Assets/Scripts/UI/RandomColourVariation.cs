using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColourVariation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float minColorValue = 100f;
    [SerializeField] private float maxColorValue = 170f;
    [SerializeField] private float greenOffset = 40f;
    [SerializeField] private float blueOffset = 80f;

    void Awake()
    {
        float randomColourNumber = Random.Range(minColorValue, maxColorValue);
        Color hexColor = new Color(randomColourNumber / 255f, (randomColourNumber + greenOffset ) / 255f, (randomColourNumber + blueOffset) / 255f);
        spriteRenderer.color = hexColor;
    }
}
