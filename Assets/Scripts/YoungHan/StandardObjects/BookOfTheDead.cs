using UnityEngine;

[DisallowMultipleComponent]
public class BookOfTheDead : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform
    {
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

    public Transform target;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private Vector2 _offset;

    private void Update()
    {
        if(target != null && target.gameObject.activeInHierarchy == true)
        {
            Vector2 offset = new Vector2(_offset.x * target.forward.z, _offset.y);
            getTransform.rotation = target.rotation;
            getTransform.position = Vector2.Lerp(getTransform.position, offset + (Vector2)target.position, Time.deltaTime * _speed);
        }
    }
}