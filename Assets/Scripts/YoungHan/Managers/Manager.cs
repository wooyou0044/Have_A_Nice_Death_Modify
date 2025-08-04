using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �� Ŭ������ ��ӹ��� ��ü���� ��� �̱������� �ϳ��� �� �ȿ��� ���� �ϳ��� ������Ʈ�θ� �����ϰ� �����.
/// </summary>
[DisallowMultipleComponent]
public abstract class Manager<T> : MonoBehaviour where T: MonoBehaviour
{
    //DontDestroyOnLoad���� �̸� �ؽ�Ʈ
    private static readonly string TextDontDestroyOnLoad = "DontDestroyOnLoad";

    //�̱��� ��ü
    private static T _instance = null;

    protected static T instance {
        get
        {
            //���� �̱����� ȣ�� �ߴµ� ������ ���� ��� ���ο� ���� ������Ʈ�� ���� ���빰�� �־��ش�.
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.AddComponent<T>();
                    gameObject.name = _instance.GetType().Name;
                }
            }
            return _instance;
        }
    }

    //�ش� ��ü�� �� ��ȯ �� ���� ���θ� �Ǵ��ϴ� ����
    protected bool _destroyOnLoad;

    //���� ���� �ڷ�ƾ�� Ű ���� ��� ���� ���� ����ü
    private struct Coroutine
    {
        public IEnumerator key {
            get;
            private set;
        }

        public IEnumerator value {
            get;
            private set;
        }

        public Coroutine(IEnumerator key, IEnumerator value)
        {
            this.key = key;
            this.value = value;
        }
    }

    //���� ���� �ڷ�ƾ ��ü���� ���� ����Ʈ
    private List<Coroutine> _coroutineList = new List<Coroutine>();

    /// <summary>
    /// �� �Լ��� ��ũ��Ʈ�� �ε�ǰų� �˻�⿡�� ���� ����� �� ȣ��ȴ�.(�����⿡���� ȣ���)
    /// </summary>
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (gameObject.scene == SceneManager.GetActiveScene())
        {
            //��ǥ�ϴ� �̱��� ��ü�� ���ٸ� ������ ��ü�� �ش��ϴ� ������Ʈ�� ã�´�.
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
            }
            //�� ����� ��ǥ�ϴ� �̱��� ��ü�� �ƴ϶�� �ݹ� �Լ��� �������ش�.
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != _instance && this != null)
                {
                    UnityEditor.Undo.DestroyObjectImmediate(this);
                }
            };
        }
    }
#endif

    private void Awake()
    {
        //��ǥ�ϴ� �̱��� ��ü�� �� ��ü�� �ƴ� ���
        if (_instance != this)
        {
            //��ǥ�ϴ� �̱��� ��ü�� ���� ��� ������ ��ü�� �ش��ϴ� ������Ʈ�� ã�´�.
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
            }
            //��ǥ�ϴ� �̱��� ��ü�� �ִ� ���
            else
            {
                //��ǥ�ϴ� �̱��� ��ü�� DontDestroyOnLoad ���� �ִ� ���
                if (_instance.gameObject.scene.name == TextDontDestroyOnLoad)
                {
                    Manager<T> manager = _instance.GetComponent<Manager<T>>();
                    //�� ��ȯ �� ��� ������Ʈ�� �������־�� �Ѵٸ� �ٸ� ��ü���� ��ǥ �ڸ��� �絵�ϰ� �������� ������ �����Ѵ�.
                    if (manager._destroyOnLoad == true)
                    {
                        T[] ts = FindObjectsOfType<T>();
                        T t = null;
                        int length = ts != null ? ts.Length : 0;
                        for(int i = 0; i < length; i++)
                        {
                            if(t == null && ts[i] != _instance)
                            {
                                t = ts[i];
                                break;
                            }
                        }
                        for (int i = 0; i < length; i++)
                        {
                            if (ts[i] != t)
                            {
                                Destroy(ts[i]);
                            }
                        }
                        _instance = t;
                        return;
                    }
                }
                Destroy(this);
            }
        }
        //��ǥ�ϴ� �̱��� ��ü�� �� ��ü�� ���
        if (_instance == this)
        {
            Initialize();
            //�� ��ȯ �� ��� ������Ʈ�� �� ��ȯ���� �������� �ʴ´�.
            if (_destroyOnLoad == false)
            {
                DontDestroyOnLoad(this);
            }
            //�� ��ȯ �� ��� ������Ʈ�� �� ��ȯ���� �����Ѵ�.
            else
            {
                SceneManager.sceneUnloaded += (scene) =>
                {
                    Destroy(this);
                };
            }
        }
    }

    /// <summary>
    /// ����Ʈ�� �ڷ�ƾ ������ �����ϰ� �ڷ�ƾ�� ������ �Ŀ� �۵��� ������ �ڷ�ƾ ������ �������ִ� �Լ�
    /// </summary>
    /// <param name="key"></param>
    /// <param name="force"></param>
    protected void StartCoroutine(IEnumerator enumerator, bool force)
    {
        if(force == false)
        {
            int count = _coroutineList.Count;
            for(int i = 0; i < count; i++)
            {
                if (string.Equals(enumerator.ToString(), _coroutineList[i].key.ToString()) == true)
                {
                    return;
                }
            }
        }
        IEnumerator value = DoWaitingForEnd();
        StartCoroutine(value);
        _coroutineList.Add(new Coroutine(enumerator, value));
        IEnumerator DoWaitingForEnd()
        {
            yield return StartCoroutine(enumerator);
            int count = _coroutineList.Count;
            for(int i = 0; i < count; i++)
            {
                if(enumerator == _coroutineList[i].key)
                {
                    _coroutineList.RemoveAt(i);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// ����Ʈ�� Ư�� �ڷ�ƾ ������ ���� ���̶�� �����ϰ� �ڷ�ƾ ������ �������ִ� �Լ�
    /// </summary>
    /// <param name="enumerator"></param>
    /// <param name="all"></param>
    protected void StopCoroutine(IEnumerator enumerator, bool all)
    {
        do
        {
            bool done = true;
            int count = _coroutineList.Count;
            for (int i = 0; i < count; i++)
            {
                if (string.Equals(enumerator.ToString(), _coroutineList[i].key.ToString()) == true)
                {
                    StopCoroutine(_coroutineList[i].value);
                    _coroutineList.RemoveAt(i);
                    done = false;
                    break;
                }
            }
            if (done == true)
            {
                break;
            }
        } while (all == true);
    }

    /// <summary>
    /// �ڷ�ƾ �Լ��� ���� �ϴ��� ���θ� �˷��ִ� �Լ�
    /// </summary>
    /// <returns>�ڷ�ƾ�� �ϳ��� �����ϸ� ���� ��ȯ�Ѵ�.</returns>
    protected bool IsCoroutineWorking()
    {
        if(_coroutineList.Count > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Ư�� �ڷ�ƾ �Լ��� ���� �ϴ��� ���θ� �˷��ִ� �Լ�
    /// </summary>
    /// <returns>Ư�� �ڷ�ƾ�� �����ϰ� �ִٸ� ���� ��ȯ�Ѵ�.</returns>
    protected bool IsCoroutineWorking(IEnumerator enumerator)
    {
        int count = _coroutineList.Count;
        for(int i = 0; i < count; i++)
        {
            if(string.Equals(enumerator.ToString(), _coroutineList[i].key.ToString()) == true)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// �ʱ�ȭ �߻� �Լ�
    /// </summary>
    protected abstract void Initialize();
}