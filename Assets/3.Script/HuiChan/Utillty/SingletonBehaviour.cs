using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            _instance = FindFirstObjectByType<T>();

            if (_instance != null)
                Debug.Log($"기존 인스턴스 '{typeof(T).Name}'을(를) 찾아서 사용합니다.");
            else
            {
                GameObject singletonObject = new GameObject(typeof(T).Name);
                _instance = singletonObject.AddComponent<T>();
                Debug.Log($"Scene에 '{typeof(T).Name}'이(가) 없어 새로 생성합니다.");
            }

            return _instance;
        }
    }

    protected abstract bool IsDontDestroy();

    protected virtual void Awake()
    {
        if (_instance == null || _instance == this)
        {
            _instance = this as T; // 현재 오브젝트를 _instance로 설정합니다.
            Debug.Log($"'{typeof(T).Name}'의 인스턴스가 Awake에서 설정되었습니다.");

            if (IsDontDestroy())
            {
                DontDestroyOnLoad(this.gameObject);
                Debug.Log($"'{typeof(T).Name}'은(는) 씬 전환 시 파괴되지 않습니다.");
            }
        }
        else Debug.LogWarning($"'{typeof(T).Name}'의 중복 인스턴스가 감지되었습니다. 기존 인스턴스({_instance.name})를 유지하고, 이 인스턴스({this.name})는 파괴하지 않습니다. 씬에 여러 개의 '{typeof(T).Name}' 인스턴스가 존재할 수 있습니다.");
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            Debug.Log($"'{typeof(T).Name}'이(가) 파괴되어 _instance를 null로 설정했습니다.");
        }
    }
} 