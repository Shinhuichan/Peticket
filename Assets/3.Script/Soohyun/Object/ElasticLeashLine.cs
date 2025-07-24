using System.Collections;
using UnityEngine;

public class ElasticLeashLine : MonoBehaviour
{
    [Header("줄 연결")]
    public Transform neckAnchor;
    public Transform handAnchor;

    [Header("줄 시각화")]
    public LineRenderer line;
    public int segmentCount = 10;

    [Header("줄 동작")]
    public float minElasticity = 0.1f;
    public float maxElasticity = 1.0f;
    public float waveFrequency = 5f;
    public float waveAmplitude = 0.05f;
    public float smoothing = 10f;

    private Vector3[] currentPoints;
    private float stillnessThreshold = 0.001f;
    private Vector3 lastNeckPos;
    private Vector3 lastHandPos;

    private AnimalLogic animal;
    private IEnumerator Start()
    {
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
        }

        line.positionCount = segmentCount;
        line.widthMultiplier = 0.05f;
        line.material.color = Color.black;
        line.startColor = Color.black;
        line.endColor = Color.black;

        currentPoints = new Vector3[segmentCount];

        yield return null; // ✅ XR 오브젝트 초기화를 기다림

        // 강아지 자동 참조
        animal = FindObjectOfType<AnimalLogic>();
        if (animal != null && neckAnchor == null)
        {
            neckAnchor = animal.mouthPos != null ? animal.mouthPos : animal.transform;
        }

        // 오른손 자동 탐색
        if (handAnchor == null)
        {
            GameObject rightHandObj = GameObject.FindWithTag("Hand_Right");
            if (rightHandObj != null)
            {
                handAnchor = rightHandObj.transform;
                Debug.Log("[ElasticLeashLine] 오른손 컨트롤러 자동 연결됨");
            }
            else
            {
                Debug.LogError("[ElasticLeashLine] 태그 'Hand_Right'를 가진 오브젝트를 찾을 수 없습니다.");
                yield break;
            }
        }

        lastNeckPos = neckAnchor.position;
        lastHandPos = handAnchor.position;

        line.enabled = false;

        if (animal != null && handAnchor != null)
            animal.leashTargetTransform = handAnchor;
    }

    private void Update()
    {
        if (neckAnchor == null || handAnchor == null) return;

        if (!animal.IsLeashed)
        {
            line.enabled = false;
            return;
        }

        line.enabled = true;

        Vector3 start = neckAnchor.position;
        Vector3 end = handAnchor.position;

        float leashMax = animal.leashFollowDistance;
        float distance = Vector3.Distance(start, end);
        float tElastic = Mathf.Clamp01(distance / leashMax);
        float dynamicElasticity = Mathf.Lerp(minElasticity, maxElasticity, tElastic);

        bool isMoving =
            (Vector3.Distance(lastNeckPos, start) > stillnessThreshold ||
            Vector3.Distance(lastHandPos, end) > stillnessThreshold);

        Vector3 dir = end - start;
        Vector3 waveDir = Vector3.Cross(dir.normalized, Vector3.up).normalized;

        for(int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 straight = Vector3.Lerp(start, end, t);

            Vector3 offset = Vector3.zero;

            if (isMoving)
            {
                float wave = Mathf.Sin(Time.time * waveFrequency + t * Mathf.PI * 2) * waveAmplitude;
                offset = waveDir * wave * dynamicElasticity * Mathf.Sin(t * Mathf.PI);
            }

            currentPoints[i] = Vector3.Lerp(currentPoints[i], straight + offset, Time.deltaTime * smoothing);
        }

        line.SetPositions(currentPoints);

        float dist = Vector3.Distance(neckAnchor.position, handAnchor.position);
        if (dist > animal.leashFollowDistance * 1.2f)
        {
            animal.SetState(AnimalState.LeashFollow);
        }
    }
}