using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveBehaviour
{
    void Init(Monster monster);                 // 초기화(레퍼런스/런타임값 세팅)
    void Tick(float time);                        // 매 프레임 이동
    void OnTriggerEnter2D(Collider2D col);      // 필요 없으면 빈 구현
}

public interface IAttackBehaviour
{
    void Init(Monster monster);                 // 초기화
    void Start();                               // 코루틴 시작 등
    void Tick(float time);                        // 필요 없으면 빈 구현
    void Stop();                                // 정리
    void OnTriggerEnter2D(Collider2D col);      // 근접 충돌형이면 여기서 처리
}