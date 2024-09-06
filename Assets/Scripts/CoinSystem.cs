using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSystem : MonoBehaviour
{
    [SerializeField] private int _moneyCount;

    private Rigidbody2D _rb;
    private GameManager _gameManager;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _gameManager = GameObject.Find("---GAME MANAGER---").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (transform.position.y < -15)
        {
            Destroy(gameObject);
        }
        _rb.velocity = new Vector3(0, -_gameManager.Speed, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            _gameManager.Money += _moneyCount;
            Destroy(gameObject);
        }
        else if (collision.transform.CompareTag("Obstacles"))
        {
            Destroy(gameObject);
        }
    }
}
