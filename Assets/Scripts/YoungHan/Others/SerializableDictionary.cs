using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ν����Ϳ��� ���̴� ��ųʸ�
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class SerializableDictionary<T> : ISerializationCallbackReceiver
{
    [Serializable]
    public struct Data<U>
    {
        public string key;
        public U value;

        public Data(string key, U value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [SerializeField]
    private List<Data<T>> list = new List<Data<T>>();

    //����ȭ �� ȣ��Ǵ� �ݹ� �Լ�
    public void OnBeforeSerialize()
    {
    }

    //������ȭ �� ȣ��Ǵ� �ݹ� �Լ�
    public void OnAfterDeserialize()
    {
        List<string> key = new List<string>();
        List<T> value = new List<T>();
        int count = 0;
        for (int i = 0; i < list.Count; i++)
        {
            Data<T> data = list[i];
            if (key.Contains(data.key) == false && string.IsNullOrWhiteSpace(data.key) == false)
            {
                key.Add(data.key);
                value.Add(data.value);
                count++;
            }
            else if (i == list.Count - 1)
            {
                key.Add(null);
                value.Add(data.value);
                count++;
            }
        }
        list.Clear();
        for (int i = 0; i < count; i++)
        {
            list.Add(new Data<T>(key[i], value[i]));
        }
    }

    /// <summary>
    /// Ű���� ������ ���� ����� ��ȯ�ϴ� �Լ� 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Data<T>> GetDatas()
    {
        return list;
    }
}