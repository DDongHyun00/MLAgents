using System.Runtime.InteropServices;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.EditorTools;
using UnityEngine;
public class MoveToBallAgent : Agent
{
    [SerializeField]
    private Transform targetTransform;

    void Start()
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent에게 Vector Observation 정보를 전달하는 함수

        // Observe가 2개 세팅되었기에 Behavior Parameters의 Space Size는 6이 되어야 한다.
        // 옵저브 1개당 float 인자 3개 = Space Size 3
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }
    

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Agent가 결정한 행동을 전달, 보상 업데이트, 에피소드 종료

        // 2개의 좌표
        float x = actions.ContinuousActions[0];
        float y = actions.ContinuousActions[1];

        float moveSpeed = 3f; // 움직임 속도

        // Agent의 움직임 부여
        transform.Translate(new Vector3(x, 0, y) * Time.deltaTime * moveSpeed);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 개발자가 직접 명령을 내리는 휴리스틱 모드에서 사용
        // 주로 테스트용 또는 모방 학습에 사용
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;

        continuousAction[0] = Input.GetAxisRaw("Horizontal");
        continuousAction[1] = Input.GetAxisRaw("Vertical");
    }
    
    [SerializeField]
    Renderer floorRenderer;
    [SerializeField]
    Material winMaterial;
    [SerializeField]
    Material loseMaterial;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            Debug.Log("Gooood");
            SetReward(1f); // 성공
            floorRenderer.material = winMaterial;
            EndEpisode(); // 에피소드 끝
        }
        else if (other.tag == "Wall")
        {
            Debug.Log("Baaaad");
            SetReward(-1f); // 실패
            floorRenderer.material = loseMaterial;
            EndEpisode(); // 에피소드 끝
        }
    }


    public override void OnEpisodeBegin()
    {
        // 각 에피소드가 시작될 때 호출되는 함수

        // Agent의 초기 위치값
        transform.localPosition = new Vector3(0, 0.5f, 0);
    }
    
}


