using UnityEngine;

public class UserData : MonoBehaviour
{
    // [기능] 내부 저장 인스턴스 (싱글톤 패턴)
    private static UserData _instance;

    /// [기능] UserData 싱글톤 인스턴스에 접근하는 전역 속성
    public static UserData Instance
    {
        get
        {
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
    public string UserID { get; private set; }
    public string Nickname { get; private set; }

    /// [사용처] 로그인 성공 직후 ID 및 닉네임 저장 시 사용
    public void SetUser(string userId, string nickname)
    {
        UserID = userId;
        Nickname = nickname;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        
        BackendGameData.Instance.GameDataGet(); // 데이터 삽입 함수
        //
        // [추가] 서버에 불러온 데이터가 존재하지 않을 경우, 데이터를 새로 생성하여 삽입
        if (BackendGameData.userData == null)
        {
            BackendGameData.Instance.GameDataInsert();
        }
        //
        // BackendGameData.Instance.LevelUp(); // [추가] 로컬에 저장된 데이터를 변경
        //
        BackendGameData.Instance.GameDataUpdate(); //[추가] 서버에 저장된 데이터를 덮어씌기(변경된 부분만)
    }
}