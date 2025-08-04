using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인스펙터에서 보이는 딕셔너리
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

    //직렬화 전 호출되는 콜백 함수
    public void OnBeforeSerialize()
    {
    }

    //역직렬화 후 호출되는 콜백 함수
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
    /// 키값이 정리된 내용 목록을 반환하는 함수 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Data<T>> GetDatas()
    {
        return list;
    }
}