using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobTang : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Sprite sprite = spriteRenderer.sprite;
        Debug.Log($"index {System.Array.IndexOf(sprites, sprite)} / rect {sprite.rect} / textureRect{sprite.textureRect}");
    }
}
