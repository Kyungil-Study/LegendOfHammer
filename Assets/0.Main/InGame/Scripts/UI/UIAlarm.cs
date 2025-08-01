using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAlarm : MonoBehaviour
{
    [SerializeField] float duration = 1.0f; // 알람 지속 시간
    [SerializeField] float flickerInterval = 0.3f; // 알람 간격
    
    [SerializeField] Image[] images; // 알람 이미지
    float[] imagesDefaultAlpha; // 이미지 알파값 저장용
    [SerializeField] TMP_Text[] texts; // 알람 텍스트
    float[] textsDefaultAlpha; // 텍스트 알파값 저장용

    public void ExecuteAlarm()
    {
        SoundManager.Instance.PlayWarning();
        gameObject.SetActive(true);
        StopCoroutine(AlarmCoroutine());
        StartCoroutine(AlarmCoroutine());
    }

    private IEnumerator AlarmCoroutine()
    {
        Ready();
        
        if (images == null || images.Length == 0 || texts == null || texts.Length == 0)
        {
            Debug.Log("UIAlarm: Images or Texts are not set.");

            yield break;
        }
        
        // 알람 시작
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // 이미지와 텍스트의 알파값을 토글
            for (int i = 0; i < images.Length; i++)
            {
                Color color = images[i].color;
                color.a = Mathf.PingPong(elapsedTime / flickerInterval, 1.0f) * imagesDefaultAlpha[i];
                images[i].color = color;
            }

            for (int i = 0; i < texts.Length; i++)
            {
                Color color = texts[i].color;
                color.a = Mathf.PingPong(elapsedTime / flickerInterval, 1.0f) * textsDefaultAlpha[i];
                texts[i].color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 알람 종료 후 기본 상태로 복원
        for (int i = 0; i < images.Length; i++)
        {
            Color color = images[i].color;
            color.a = imagesDefaultAlpha[i];
            images[i].color = color;
        }

        for (int i = 0; i < texts.Length; i++)
        {
            Color color = texts[i].color;
            color.a = textsDefaultAlpha[i];
            texts[i].color = color;
        }
        
        gameObject.SetActive(false);

    }

    private void Ready()
    {
        var images = GetComponentsInChildren<Image>();
        
        if (images == null || images.Length == 0)
        {
            Debug.LogError("UIAlarm: No Image components found in children.");
            return;
        }
        
        imagesDefaultAlpha = new float[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            imagesDefaultAlpha[i] = images[i].color.a; // 이미지의 기본 알파값 저장
        }
        
        
        
        texts = GetComponentsInChildren<TMP_Text>();
        if (texts == null || texts.Length == 0)
        {
            Debug.LogError("UIAlarm: No TMP_Text components found in children.");
            return;
        }
        
        textsDefaultAlpha = new float[texts.Length];
        for (int i = 0; i < texts.Length; i++)
        {
            textsDefaultAlpha[i] = texts[i].color.a; // 텍스트의 기본 알파값 저장
        }
            
    }
}
