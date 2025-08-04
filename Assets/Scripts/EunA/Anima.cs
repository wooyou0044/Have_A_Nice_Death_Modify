using System.Collections;
using UnityEngine;

public class Anima : MonoBehaviour
{
    [SerializeField]
    private byte _healValue = 1;
    [SerializeField]
    private float _dropWaitingTime = 0.5f;
    [SerializeField]
    private float _hideWaitingTime = 1.0f;
    [SerializeField]
    private GameObject animaImage;
    [SerializeField]
    private GameObject getItemEffect;


    private bool _activation = false;
    private Player _player = null;

    private void OnEnable()
    {
        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            _activation = false;
            yield return new WaitForSeconds(_dropWaitingTime);
            _activation = true;
            while(_player == null)
            {
                yield return null;
            }
            _player.Heal(_healValue);
            animaImage.SetActive(false);
            getItemEffect.SetActive(true);
            yield return new WaitForSeconds(_hideWaitingTime);
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void OnTriggerStay2D(Collider2D collision)
    {        
        if (_activation == true && _player == null && collision.CompareTag("Player"))
        {
            _player = collision.gameObject.GetComponent<Player>();
        }     
    }
}