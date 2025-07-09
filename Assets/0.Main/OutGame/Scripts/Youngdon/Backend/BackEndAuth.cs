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

    private void Start()
    {
        // [기능] 뒤끝 SDK 초기화
        var initBro = Backend.Initialize();
        if (initBro.IsSuccess())
        {
            Debug.Log("뒤끝 초기화 성공");

            customLoginButton.onClick.AddListener(OnCustomLogin);
            guestLoginButton.onClick.AddListener(OnGuestLogin);
            signUpButton.onClick.AddListener(OnSignUp);
        }
        else
        {
            Debug.LogError("뒤끝 초기화 실패: " + initBro);
        }
    }

    // [기능] 게스트 로그인 처리
    private void OnGuestLogin()
    {
        Backend.BMember.DeleteGuestInfo();
        var bro = Backend.BMember.GuestLogin();
        if (!bro.IsSuccess())
        {
            Debug.LogError("게스트 로그인 실패: " + bro);
            return;
        }

        Debug.Log("게스트 로그인 성공");
        var json    = bro.GetReturnValuetoJSON();
        string guestId = json?["row"]?["custom_id"]?.ToString() ?? System.Guid.NewGuid().ToString();
        UserData.Instance.SetUser(guestId, "Guest");

        // 로그인 후 항상 스테이지 정보 불러오기
        BackendStageGameData.Instance.InitalizeStage();
        BackendStageGameData.Instance.GetStage();

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
            Debug.Log("커스텀 로그인 성공");

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
            // [기능] 로그인 후 씬 전환
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.LogError("커스텀 로그인 실패: " + bro);
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
            Debug.Log("회원가입 성공");

            // [기능] 닉네임 설정
            var nickBro = Backend.BMember.UpdateNickname(nick);
            if (nickBro.IsSuccess())
            {
                Debug.Log("닉네임 설정 성공");

                // [기능] 닉네임 설정 성공 시 자동 로그인 실행
                BackendStageGameData.Instance.InitalizeStage();
                OnCustomLogin();
            }
            else
            {
                Debug.LogError("닉네임 설정 실패: " + nickBro);
            }
        }
        else
        {
            Debug.LogError("회원가입 실패: " + signUpBro);
        }
    }
}
