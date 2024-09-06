using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSystem : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Rigidbody2D _rb;
    private Vector2 _startPos;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        _rb.velocity = new Vector3(horizontal * _speed, 0, 0);

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _startPos = transform.position;
                    break;
                case TouchPhase.Moved:
                    Vector3 dir = Camera.main.ScreenToWorldPoint(touch.position - _startPos);
                    Vector3 newPos = new Vector3(dir.x, -4.5f, 0);
                    transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * _speed);
                    break;
            }
        }
    }
}
