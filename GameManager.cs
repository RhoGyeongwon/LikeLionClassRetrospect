using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[Serializable]
public class PlayerData
{
    public string playerName;
    
    [NonSerialized]
    public float Distance;
}

public class GameManager : MonoBehaviour
{
    public float battleTime = 30.0f;
    
    // 경마에 참여할 플레이어 리스트
    public List<PlayerData> Players = new List<PlayerData>();
    
    // ui에 표현 될 버튼 프리팹
    public RaceButton templateButton; //"템플릿" 역할을 하도록 설정된 변수 이름이라 프리팹 알아두자!
    
    // 버튼들이 붙을 부모오브젝트
    public Transform RaceButtonParent;  // [★ 이 transform 값은 랜덤인가? 순차적으로 쌓이는가? 어떻게 올라가는지 궁금하다. 아래부터 스택같이 쌓이는가? ] 
    
    // 생성된 버튼들 관리
    private List<RaceButton> raceButtons = new List<RaceButton>();
    
    // 코루틴(Coroutine):
    // 유니티의 특별한 함수 실행 방식으로, 한 번에 모든 코드를 실행하지 않고, 여러 프레임에 걸쳐 나눠 실행해.
    // 비유: "길거리 가수 공연을 보면서, 노래가 끝날 때까지 기다리는 것"처럼 특정 조건(시간, 이벤트)이 만족될 때까지 대기.
    // IEnumerator:
    // 코루틴의 반환 타입.
    // 코루틴 함수에서 발생하는 이벤트를 순차적으로 처리하는 방식"을 정의
    IEnumerator BattlerTimer()
    {
        for (var i = 0; i < Players.Count; i++)
        {
            // 오브젝트 생성하기
            var newObj = Instantiate(templateButton.gameObject, RaceButtonParent);
            // 첫 번째 매개변수: 만들 자식 오브젝트(templateButton.gameObject).
            // 두 번째 매개변수: 새로 생성된 부모 오브젝트 (여기의 하위로 들어간다.)
            // Instantiate 함수의 세 번째 매개변수를 사용하면, 월드 좌표를 유지될 수 있다.
            // templateButton 프리팹을 복제하여 각 플레이어의 UI 버튼 생성
            // 부모 오브젝트에 Layout Group이 없다면, 생성된 버튼들의 위치가 고정되므로 주의!
            
            // RaceButton 컴포넌트 캐싱하기
            raceButtons.Add(newObj.GetComponent<RaceButton>());
            
            //그러니까 개체 생성을 통해 오브젝트를 생성해서
            //게임 오브젝트의 컴포넌트도 세팅을 하겠다는거군. 왜냐면 컴포넌트 값은 아직 안 붙여져있으니까.
        }
        // 코루틴 내부에 더 이상 실행할 코드가 없으므로 코루틴은 종료된다!
        while (battleTime >= 0.0f) //battleTime이 시작하면
        {
            Debug.Log(battleTime);
            
            yield return new WaitForSeconds(1.0f);
            // 이 함수는 1초동안 쉰다.
            // yield return:
            // 중간에 멈추고 대기하는 키워드.
            // 비유: "공연 중간에 다른 볼거리를 보여주며 기다리게 하는 것".
            //코루틴은 한 번에 모든 코드를 실행하지 않고, yield return 키워드에서 중단한 뒤 다음 프레임 이후에 다시 실행
            
            foreach (var playerData in Players)
            {
                playerData.Distance += Random.Range(0.0f, 1.0f); // 각자의 거리에 랜덤값을 준다. 기존 거리에 더해야하니 += 사용
            }
            
            var ranks = (from p in Players orderby p.Distance select p).ToList ();
            //[★ players 리스트 안에 있는 플레이어 데이터 하나를 골라 거리순에 따라 오름차순으로 나열하고 그것들을 골라 리스트 형태로 반환한다
            for (var i = 0; i < ranks.Count; i++)
            {
                Debug.Log($"Rank {i+1} : {ranks[i].playerName} / distance : {ranks[i].Distance}");
                raceButtons[i].text.text = ranks[i].playerName;
            }
            
            // [ ★ 어떠한 값이 참이 될때가지 기다리는 YieldInstruction ] , yield return new WaitUntil(() => 조건);
            
            // yield return new WaitUntil();

            // [ ★ 물리 적용이 끝난 시점까지 기다리는 코루틴 ] , 물리 적용이 끝날 때까지 기다린다, 유니티의 물리 연산은 FixedUpdate 주기에 맞춰 실행 / Rigidbody 물리 계산 이후 특정 작업을 수행해야 할 때 사용
            // yield return new FixedUpdate();
            
            battleTime -= 1.0f; // 
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // 코루틴 함수를 시작한다.
        StartCoroutine(BattlerTimer()); //코루틴 함수는 여러개가 있는건가?
    }

    //private float _stepBattleDuration = 1.0f;
    
    // Update is called once per frame
    void Update()
    {
        //Time.realtimeSinceStartup;

        // 1초당 60프레임이다 1/60 = time.deltaTime이 된다.
        // 1초당 120프레임이면 1/120 = time.deltaTime이 된다.
        // Time.deltaTime;

        // 업데이트를 이용한 방법
        // if (0 >= battleTime)
        //     return;
        //
        // if (_stepBattleDuration >= 1.0f)
        // {
        //     Debug.Log(battleTime);
        //     
        //     battleTime -= 1.0f;
        //     _stepBattleDuration = 0.0f;
        // }
        //
        // _stepBattleDuration += Time.deltaTime;
    }
}
