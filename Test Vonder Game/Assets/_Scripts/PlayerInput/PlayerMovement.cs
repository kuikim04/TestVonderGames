using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    public SPUM_Prefabs animation_cha;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Fire")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool jumpPressed = false;
    private bool isFiring = false;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        PlayerStat.OnDeath += Death;
    }

    private void OnDisable()
    {
        PlayerStat.OnDeath -= Death;
    }
    private void Update()
    {
        animation_cha.PlayAnimation(Mathf.Abs(horizontalInput) > 0.01f ? 1 : 0);
        FlipCharacter();
    }

    private void FixedUpdate()
    {
        CheckGround();

        if (GameManager.Instance.isInteraction)
        {
            StopMovement();
            return;
        }

        HandleMovement();
        ApplyGravity();
        HandleJump();
    }

    private void StopMovement()
    {
        horizontalInput = 0;
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    private void HandleMovement()
    {
        float targetSpeed = horizontalInput * moveSpeed;
        float maxXSpeed = moveSpeed * (isGrounded ? 1f : 0.5f);
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            float acceleration = isGrounded ? 50f : 25f;
            float newX = Mathf.MoveTowards(rb.velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(Mathf.Clamp(newX, -maxXSpeed, maxXSpeed), rb.velocity.y);
        }
        else if (isGrounded)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void ApplyGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * (Physics2D.gravity.y * Time.fixedDeltaTime);
        }
    }

    private void HandleJump()
    {
        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false;
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if(!GameManager.Instance.isInteraction)
            horizontalInput = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !GameManager.Instance.isInteraction)
        {
            jumpPressed = true;
        }
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed && !isFiring && !GameManager.Instance.isInteraction)
        {
            isFiring = true;
            animation_cha.PlayAnimation(9);
            StartCoroutine(FireAfterDelay());
        }
    }

    private IEnumerator FireAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Shoot();
        yield return new WaitForSeconds(0.2f);
        isFiring = false;
    }

    private void Shoot()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.transform.rotation = Quaternion.Euler(0f, isFacingRight ? 0f : 180f, 0f);
        bullet.GetComponent<Bullet>().Shoot(direction);
    }

    private void FlipCharacter()
    {
        if ((horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    private void Death()
    {
        GameManager.Instance.isInteraction = true;
        animation_cha.EditChk = false;
        animation_cha.PlayAnimation(2);

        StartCoroutine(FadeAndRespawn());
    }

    IEnumerator FadeAndRespawn()
    {
        yield return new WaitForSeconds(1f);
        fadeImage.DOFade(1f, 0.5f).OnComplete(() =>
        {
            transform.position = startPoint.position;
            animation_cha.EditChk = true;
            animation_cha.PlayAnimation(2);
        });


        yield return new WaitForSeconds(3f);

        GameManager.Instance.isInteraction = false;

        yield return fadeImage.DOFade(0f, 0.5f).WaitForCompletion();
    }

}

