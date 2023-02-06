using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class BasketAgent : Agent
{
    [SerializeField] GameObject dropper;
    int count;
    Transform tr;
    Vector3 initialPosition;
    Rigidbody2D rb;
    [SerializeField]Renderer indicatorRd;

    Material originMt;
    [SerializeField] GameObject ball;
    Transform ballTr;

    [SerializeField] Material badMt;
    [SerializeField] Material completeMt;
    [SerializeField] Material goodMt;
    RaycastHit hit;
    public LayerMask layerMask;
    int lazyPoint;

    public int missedCount;


    public override void Initialize(){
        //initialize agent
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        originMt = indicatorRd.material;
        initialPosition = new Vector3 (tr.localPosition.x, tr.localPosition.y, tr.localPosition.z);
        ballTr = ball.GetComponent<Transform>();

    }
        
    
    public override void OnEpisodeBegin(){
        count = 0;
        missedCount = 0;
        //reset agent
        rb.velocity = Vector3.zero;
        tr.localPosition = initialPosition;

        StartCoroutine(RevertMaterial());
        StartCoroutine(SpawnBall());

    }

    IEnumerator RevertMaterial(){
        yield return new WaitForSeconds(0.5f);
        indicatorRd.material = originMt;
    }

    public IEnumerator SpawnBall(){
        yield return new WaitForSeconds(1f);
        ball.transform.localPosition = new Vector3(Random.Range(-9f,9f),dropper.transform.localPosition.y,dropper.transform.localPosition.z);
        ball.SetActive(true);
    }
    
    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor){
        sensor.AddObservation(tr.localPosition);
        sensor.AddObservation(count);
        sensor.AddObservation(missedCount);
        sensor.AddObservation(ballTr.localPosition.x);
        sensor.AddObservation(rb.velocity.x);
    }
    // private void Update() {
    //     if(Mathf.Floor(Time.time ) % 2 == 1){
    //         ++lazyPoint;
    //     }
    // }
    
    public override void OnActionReceived(ActionBuffers actions){
        
        float h = Mathf.Clamp(actions.ContinuousActions[0], -1.0f, 1.0f);
        Vector3 dir = new Vector3(h, 0, 0);
        rb.AddForce(dir * 50f);

        if(missedCount >= 5){
            indicatorRd.material = badMt;
            SetReward(-1f);
            ball.SetActive(false);
            EndEpisode();
        }
        SetReward(-0.005f);

        // if(count >= missedCount){
        //     SetReward(lazyPoint * count * 0.001f);
        // }
        // else{
        //     SetReward(lazyPoint * missedCount * 0.003f);
        // }
    }
    public override void Heuristic(in ActionBuffers actionsOut){
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        // Debug.Log($"[0] = {continuousActions[0]}");
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Ball")){
            other.gameObject.SetActive(false);
            // AddReward(0.3f);
            count++;
            lazyPoint = 0;
            if(count >= 5){
                indicatorRd.material = completeMt;
                SetReward(1f);
                EndEpisode();
            }
            else{
                indicatorRd.material = goodMt;
                StartCoroutine(RevertMaterial());
                StartCoroutine(SpawnBall());
            }
        }
        if(other.gameObject.CompareTag("Dead")){
            indicatorRd.material = badMt;
            SetReward(-1f);
            ball.SetActive(false);
            EndEpisode();
        }
    }
}
