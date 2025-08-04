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

    //ȿ�� ������Ʈ���� �����ϴ� ��ųʸ�
    private Dictionary<GameObject, List<GameObject>> _gameObjects = new Dictionary<GameObject, List<GameObject>>();

    //�̻��� ������Ʈ���� �����ϴ� ��ųʸ�
    private Dictionary<Projectile, List<Projectile>> _projectiles = new Dictionary<Projectile, List<Projectile>>();

    /// <summary>
    /// ���ӿ��� �Ͼ ȿ�� ����Ʈ���� Ǯ�����ִ� �޼���
    /// </summary>
    /// <param name="original"></param>
    /// <param name="position"></param>
    /// <param name="transform"></param>
    public void ShowEffect(GameObject original, Vector2 position, Transform transform = null)
    {
        if(original != null)
        {
            bool parent = transform != null;
            //�ش� �������� Ű ������ �����ϰ� �ִٸ�
            if (_gameObjects.ContainsKey(original) == true)
            {
                //Ű�� ����Ʈ ����� ��ȸ�ϸ鼭 ��Ȱ��ȭ �� ������Ʈ�� ã�´�.
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
            //�װ� �ƴ϶�� Ű ���� �����ϰ� ��ü�� �����Ѵ�.
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
    /// �ش� ������ Ŭ���� ��ȯ�ϴ� �Լ�
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
                //Ű�� ����Ʈ ����� ��ȸ�ϸ鼭 ��Ȱ��ȭ �� ������Ʈ�� ã�´�.
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