using System.Collections;
using UnityEngine;

public class TemporaryBehavior : MonoBehaviour
{
    [SerializeField, Range(0, int.MaxValue)]
    private float _duration = 3;

    private void OnEnable()
    {
        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            yield return new WaitForSeconds(_duration);
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
