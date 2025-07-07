using UnityEngine;

public class TSVLoaderSample : MonoBehaviour
{
    public class SampleData
    {
        public int IntData { get; set; }
        public string StringData1 { get; set; }
        public string StringData2 { get; set; }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Unity의 StreamingAssets 경로, PersistentDataPath 경로 검색해서 스터디 할것.
        //아래 두 내용 주석 해제하면 어떤건지 나옴
        
        //Application.streamingAssetsPath
        //Application.persistentDataPath
        
        TSVLoader.LoadTableAsync<SampleData>("test_table", true).ContinueWith(
                (taskResult) =>
            {
                var list = taskResult.Result;

                foreach (var sampleData in list)
                {
                    Debug.Log($"Category : {sampleData.IntData}, Key : {sampleData.StringData1}, Kor : {sampleData.StringData2}");
                }
            }
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
