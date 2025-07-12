using UnityEngine;
using UnityEngine.SceneManagement;
using BackEnd;
using TMPro;
using UnityEngine.UI;

public class BackEndAuth : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField idInput, pwInput, nicknameInput;

    [Header("Buttons")]
    public Button customLoginButton, guestLoginButton, signUpButton;

    [Header("Hash")]
    public Button hashGenerateButton;
    public TMP_InputField hashResultInput;
    
    private void Awake()
    {
        // [기능] 뒤끝 SDK 초기화
        var initBro = Backend.Initialize();
        if (initBro.IsSuccess())
        {
            Debug.Log("뒤끝 초기화 성공");

            customLoginButton.onClick.AddListener(OnCustomLogin);
            guestLoginButton.onClick.AddListener(OnGuestLogin);
            signUpButton.onClick.AddListener(OnSignUp);
            
            hashGenerateButton.onClick.AddListener(() =>
            {
                // [기능] 입력값을 해시로 변환
                string googleHash = Backend.Utils.GetGoogleHash();
                hashResultInput.text = googleHash;
                Debug.Log("해시 생성 성공: " + googleHash);
            });
        }
        else
        {
            Debug.LogError("뒤끝 초기화 실패: " + initBro);
        }
        // 비밀번호 가리기용
        pwInput.contentType = TMP_InputField.ContentType.Password;
        pwInput.ForceLabelUpdate();
    }

    // [기능] 게스트 로그인 처리
    private void OnGuestLogin()
    {
        Backend.BMember.DeleteGuestInfo();
        string guestID =  PlayerPrefs.GetString("GuestId", string.Empty);
        var bro = Backend.BMember.GuestLogin(guestID); // 게스트 로그인 시도 처음이라면 게스트 ID 생성해서 반환됨
        if (!bro.IsSuccess())
        {
            DebugScreen.LogError("게스트 로그인 실패: " + bro);
            return;
        }
        
        DebugScreen.Log("게스트 로그인 성공");
        var json    = bro.GetReturnValuetoJSON();
        guestID = json?["row"]?["custom_id"]?.ToString() ?? System.Guid.NewGuid().ToString();
        guestID = Backend.BMember.GetGuestID();
        PlayerPrefs.SetString("GuestId", guestID);
        UserData.Instance.SetUser(guestID, "Guest");

        // 로그인 후 항상 스테이지 정보 불러오기
        BackendStageGameData.Instance.InitalizeStage();
        BackendStageGameData.Instance.GetStage();
        BackendAugmentData.Instance.InitalizeAugmentData();
        BackendAugmentData.Instance.GetAugmentData();
        

        SceneManager.LoadScene(1);
    }

    // [기능] 커스텀 로그인 처리
    // [흐름] 로그인 시도 → 성공 시 닉네임 조회 → UserData 저장 → 씬 전환
    private void OnCustomLogin()
    {
        string id = idInput.text.Trim();
        string pw = pwInput.text.Trim();

        var bro = Backend.BMember.CustomLogin(id, pw);
        if (bro.IsSuccess())
        {
            DebugScreen.Log("커스텀 로그인 성공");

            // [기능] 로그인 성공 후 닉네임 가져오기
            var infoBro = Backend.BMember.GetUserInfo();
            if (infoBro.IsSuccess())
            {
                var json = infoBro.GetReturnValuetoJSON();
                var row = json["row"];

                // [주의] "nickname" 키가 없을 수 있으므로 조건 체크
                string nick = row.ContainsKey("nickname")
                    ? row["nickname"].ToString()
                    : string.Empty;

                // [기능] UserData에 ID, 닉네임 저장
                UserData.Instance.SetUser(id, nick);
            }
            BackendStageGameData.Instance.GetStage();
            BackendAugmentData.Instance.GetAugmentData();
            // [기능] 로그인 후 씬 전환
            SceneManager.LoadScene(1);
        }
        else
        {
            DebugScreen.LogError("커스텀 로그인 실패: " + bro);
        }
    }
    // [기능] 회원가입 처리
    // [흐름] 회원가입 요청 → 닉네임 설정 → 자동 로그인
    private void OnSignUp()
    {
        string id = idInput.text.Trim();
        string pw = pwInput.text.Trim();
        string nick = nicknameInput.text.Trim();

        var signUpBro = Backend.BMember.CustomSignUp(id, pw);
        if (signUpBro.IsSuccess())
        {
            DebugScreen.Log("회원가입 성공");

            // [기능] 닉네임 설정
            var nickBro = Backend.BMember.UpdateNickname(nick);
            if (nickBro.IsSuccess())
            {
                DebugScreen.Log("닉네임 설정 성공");

                // [기능] 닉네임 설정 성공 시 자동 로그인 실행
                BackendStageGameData.Instance.InitalizeStage();
                BackendAugmentData.Instance.InitalizeAugmentData();
                OnCustomLogin();
            }
            else
            {
                DebugScreen.LogError("닉네임 설정 실패: " + nickBro);
            }
        }
        else
        {
            DebugScreen.LogError("회원가입 실패: " + signUpBro);
        }
    }
}
