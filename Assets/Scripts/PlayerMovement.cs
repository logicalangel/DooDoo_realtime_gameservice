using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject Bullet;
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private float MoveSpeed;
    private bool FacingRight;
    private bool Jump;
    private bool Fire;
    // Start is called before the first frame update
    void Start()
    {
        FacingRight = true; // for player1 & negative for player2
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        HandleMovement(horizontal);
        Flip(horizontal);
        HandleFire();
        ResetValues();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire = true;
        }
    }

    private void HandleFire()
    {
        if (Fire)
        {
            GameObject obj = (GameObject)Instantiate(Bullet, new Vector3(transform.position.x + 1, transform.position.y), Quaternion.identity);
            obj.GetComponent<Rigidbody2D>().velocity = new Vector2((FacingRight ? 6:-6), 0);
        }
    }

    private void HandleMovement(float horizontal)
    {
        _rigidbody.velocity = new Vector2(horizontal * MoveSpeed, _rigidbody.velocity.y);
        if (Jump)
        {
            _rigidbody.AddForce(new Vector2(0, 200));
        }
    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !FacingRight || horizontal < 0 && FacingRight)
        {
            FacingRight = !FacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void ResetValues()
    {
        Jump = false;
        Fire = false;
    }
}
