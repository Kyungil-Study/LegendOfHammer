using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using TMPro;

public class BackEndAuth : MonoBehaviour
{
    [Header("InputFields")]
    public TMP_InputField idInput;
    public TMP_InputField pwInput;
    public TMP_InputField nicknameInput;

    [Header("Buttons")]
    public Button customLoginButton;
    public Button guestLoginButton;
    public Button signUpButton;

    private void Start()
    {
        var bro = Backend.Initialize(); // 뒤끝 초기화

        if (bro.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
            // 버튼 이벤트 연결
            customLoginButton.onClick.AddListener(OnCustomLogin);
            guestLoginButton.onClick.AddListener(OnGuestLogin);
            signUpButton.onClick.AddListener(OnSignUp);
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        }
    }
    
    #region 회원가입 및 로그인
    private void OnGuestLogin()
    {
        // 게스트 로그인 테스트용으로 추가해둠
        Backend.BMember.DeleteGuestInfo();
        
        var bro = Backend.BMember.GuestLogin();
        if (bro.IsSuccess())
        {
            Debug.Log("게스트 로그인 성공");
        }
        else
        {
            Debug.LogError("게스트 로그인 실패: " + bro);
        }
    }
    private void OnCustomLogin()
    {
        string id = idInput.text.Trim();
        string pw = pwInput.text.Trim();
        var bro = Backend.BMember.CustomLogin(id, pw);
        if (bro.IsSuccess())
        {
            Debug.Log("커스텀 로그인 성공");
        }
        else
        {
            Debug.LogError("커스텀 로그인 실패: " + bro);
        }
    }
    private void OnSignUp()
    {
        string id = idInput.text.Trim();
        string pw = pwInput.text.Trim();
        string nickname = nicknameInput.text.Trim();
        var signUpBro = Backend.BMember.CustomSignUp(id, pw);
        if (signUpBro.IsSuccess())
        {
            Debug.Log("회원가입 성공");
            var nicknameBro = Backend.BMember.UpdateNickname(nickname);
            if (nicknameBro.IsSuccess())
            {
                Debug.Log("닉네임 설정 성공");
            }
            else
            {
                Debug.LogError("닉네임 설정 실패: " + nicknameBro);
            }
        }
        else
        {
            Debug.LogError("회원가입 실패: " + signUpBro);
        }
    }
    #endregion
}