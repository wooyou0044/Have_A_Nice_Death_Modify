using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ObjectPooler : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform {
        get
        {
            if (_hasTransform == false)
            {
                _hasTransform = true;
                _transform = transform;
            }
            return _transform;
        }
    }

    //효과 오브젝트들을 관리하는 딕셔너리
    private Dictionary<GameObject, List<GameObject>> _gameObjects = new Dictionary<GameObject, List<GameObject>>();

    //미사일 오브젝트들을 관리하는 딕셔너리
    private Dictionary<Projectile, List<Projectile>> _projectiles = new Dictionary<Projectile, List<Projectile>>();

    /// <summary>
    /// 게임에서 일어날 효과 이펙트들을 풀링해주는 메서드
    /// </summary>
    /// <param name="original"></param>
    /// <param name="position"></param>
    /// <param name="transform"></param>
    public void ShowEffect(GameObject original, Vector2 position, Transform transform = null)
    {
        if(original != null)
        {
            bool parent = transform != null;
            //해당 프리팹을 키 값으로 보유하고 있다면
            if (_gameObjects.ContainsKey(original) == true)
            {
                //키의 리스트 목록을 순회하면서 비활성화 된 오브젝트를 찾는다.
                foreach(GameObject gameObject in _gameObjects[original])
                {
                    if(gameObject.activeInHierarchy == false)
                    {
                        gameObject.transform.position = position;
                        if(parent == true)
                        {
                            gameObject.transform.parent = transform;
                            gameObject.transform.rotation = transform.rotation;
                        }
                        else
                        {
                            gameObject.transform.parent = getTransform;
                            gameObject.transform.rotation = Quaternion.identity;
                        }
                        gameObject.SetActive(true);
                        return;
                    }
                }
                GameObject value = Instantiate(original, position, parent ? transform.rotation : Quaternion.identity, parent ? transform : getTransform);
                _gameObjects[original].Add(value);
            }
            //그게 아니라면 키 값을 설정하고 객체를 생성한다.
            else
            {
                GameObject value = Instantiate(original, position, parent ? transform.rotation: Quaternion.identity, parent ? transform: getTransform);
                _gameObjects.Add(original, new List<GameObject>() { value });
            }
        }
    }

    public bool IsPickUp()
    {
        foreach (KeyValuePair<GameObject, List<GameObject>> kvp in _gameObjects)
        {
            Coffee coffee = kvp.Key.GetComponent<Coffee>();
            if(coffee != null)
            {
                List<GameObject> list = kvp.Value;
                foreach (GameObject item in list)
                {
                    if (item.activeInHierarchy == true && item.GetComponent<Coffee>().GetCoffee())
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 해당 프리팹 클론을 반환하는 함수
    /// </summary>
    /// <param name="original"></param>
    /// <param name="adhesion"></param>
    /// <returns></returns>
    public Projectile GetProjectile(Projectile original)
    {
        if (original != null)
        {
            if(_projectiles.ContainsKey(original) == true)
            {
                //키의 리스트 목록을 순회하면서 비활성화 된 오브젝트를 찾는다.
                foreach (Projectile projectile in _projectiles[original])
                {
                    if (projectile.gameObject.activeInHierarchy == false)
                    {
                        projectile.transform.parent = getTransform;
                        projectile.gameObject.SetActive(true);
                        return projectile;
                    }
                }
                Projectile value = Instantiate(original, getTransform);
                _projectiles[original].Add(value);
                return value;
            }
            else
            {
                Projectile value = Instantiate(original, getTransform);
                _projectiles.Add(original, new List<Projectile>() { value });
                return value;
            }
        }
        return null;
    }
}