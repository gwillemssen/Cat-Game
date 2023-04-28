using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameAnimation : MonoBehaviour
{
    [SerializeField] private float frameTime = 0.2f;
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] sprites;

    private int index;
    private float frameTimer;

    private void Start()
    {
        image.sprite = sprites[0];
    }

    void Update()
    {
        frameTimer += Time.deltaTime;

        if(frameTimer > frameTime)
        {
            frameTimer = 0f;
            index++;
            if(index >= sprites.Length)
            { index = 0; }

            image.sprite = sprites[index];
        }
    }
}
