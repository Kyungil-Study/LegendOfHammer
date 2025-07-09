using UnityEngine;


// 뒤끝 SDK namespace 추가
using BackEnd;

public class BackendManager : MonoBehaviour
{
    void Start()
    {
        var bro = Backend.Initialize(); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        }

        Test();
    }

    // 동기 함수를 비동기에서 호출하게 해주는 함수(유니티 UI 접근 불가)
    void Test()
    {
        //BackendLogin.Instance.CustomSignUp("user1","1234");
        BackendLogin.Instance.CustomLogin("user1", "1234"); // 뒤끝 로그인 함수

        #region 게임 데이터
        
        Debug.Log("----------게임데이터----------");
        
        BackendGameData.Instance.GameDataGet(); // 데이터 삽입 함수

        // [추가] 서버에 불러온 데이터가 존재하지 않을 경우, 데이터를 새로 생성하여 삽입
        if (BackendGameData.userData == null)
        {
            BackendGameData.Instance.GameDataInsert();
        }

        BackendGameData.Instance.LevelUp(); // [추가] 로컬에 저장된 데이터를 변경

        BackendGameData.Instance.GameDataUpdate(); //[추가] 서버에 저장된 데이터를 덮어씌기(변경된 부분만)
        
        #endregion

        #region 랭킹
        
        Debug.Log("----------랭킹----------");
        
        BackendRank.Instance.RankInsert(100); // [추가] 랭킹 등록하기 함수
        BackendRank.Instance.RankGet(); // [추가] 랭킹 불러오기 함수
        
        #endregion
        
        #region 차트
        
        Debug.Log("----------차트----------");
        
        // [추가] chartId의 차트 정보 불러오기
        // [변경 필요] '파일 ID'을 '뒤끝 콘솔 > 차트 관리 > 아이템차트'에서 등록한 차트의 파일 ID값으로 변경해주세요.
        BackendChart.Instance.ChartGet("192078"); // [추가] chartId의 차트 정보 불러오기
        
        #endregion
        
        #region 우편
        
        Debug.Log("----------우편----------");

        // 우편 리스트를 불러와 우편의 정보와 inDate값들을 로컬에 저장합니다.  
        BackendPost.Instance.PostListGet(PostType.Rank);

        // 저장된 우편의 위치를 읽어 우편을 수령합니다. 여기서 index는 우편의 순서. 0이면 제일 윗 우편, 1이면 그 다음 우편
        BackendPost.Instance.PostReceive(PostType.Rank, 0);

        // 조회된 모든 우편을 수령합니다.  
        BackendPost.Instance.PostReceiveAll(PostType.Rank);
        
        #endregion
        
        Debug.Log("테스트를 종료합니다.");
    }
}