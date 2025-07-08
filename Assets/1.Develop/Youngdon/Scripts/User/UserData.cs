using UnityEngine;

/// [기능] 로그인한 유저의 ID 및 닉네임 정보를 전역에서 관리하는 싱글톤 클래스
public class UserData : MonoBehaviour
{
    // [기능] 내부 저장 인스턴스 (싱글톤 패턴)
    // [주의] 외부에서 직접 접근 불가
    private static UserData _instance;

    /// [기능] UserData 싱글톤 인스턴스에 접근하는 전역 속성
    /// [흐름] 처음 접근 시 오브젝트 생성 후 DontDestroyOnLoad 설정
    public static UserData Instance
    {
        get
        {
            // [주의] 인스턴스가 없을 경우 런타임 중 자동 생성
            if (_instance == null)
            {
                GameObject go = new GameObject(nameof(UserData));
                _instance = go.AddComponent<UserData>();
                DontDestroyOnLoad(go); // 씬 전환 시에도 파괴되지 않도록 설정
            }
            return _instance;
        }
    }

    // [기능] 로그인 유저 정보
    // [주의] 읽기 전용 (SetUser를 통해서만 값 설정 가능)
    public string UserID { get; private set; }
    public string Nickname { get; private set; }

    /// [기능] 유저 정보를 설정하는 함수
    /// [사용처] 로그인 성공 직후 ID 및 닉네임 저장 시 사용
    public void SetUser(string userId, string nickname)
    {
        UserID = userId;
        Nickname = nickname;
    }

    /// [기능] 유니티 생명주기 함수
    /// [역할] 싱글톤 중복 생성을 방지하고 DontDestroyOnLoad 설정
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // [주의] 중복 생성 방지
        }
    }
}