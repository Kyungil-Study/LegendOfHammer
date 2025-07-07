using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1.0f;
    [SerializeField] private float scrollDistance = 10.0f;
    [SerializeField] private Transform[] pages;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var page in pages)
        {
            var topPosY = page.transform.position.y + scrollDistance;
            if (topPosY <= -scrollDistance)
            {
                page.transform.position = new Vector3(
                    page.transform.position.x,
                    page.transform.position.y + scrollDistance*2*2,
                    0);
            }
                
            page.transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
        }
        
    }
}
