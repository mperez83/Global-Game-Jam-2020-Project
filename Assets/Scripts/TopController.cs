﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopController : MonoBehaviour
{
    public Camera mainCamera;

    // Horizontal movement
    Vector2 vel;
    float smoothAmount;

    // Vertical movement
    float jumpHeight = 0.05f;
    float timeToJumpApex = 0.5f;
    float gravity;
    float jumpVelocity;

    float moveAmountY;
    bool grounded;
    bool doubleJumpEnabled;
    bool doubleJumped;

    bool yeeted;

    public AudioClip jumpSound;
    public AudioClip yeetSound;
    public AudioSource audioSource;



    void Start()
    {
        // Horizontal stuff
        smoothAmount = Mathf.Clamp(((GameManager.instance.repairTime * 4f) / 100f) - 0.25f, 0.15f, 2f);

        // Vertical stuff
        timeToJumpApex = Mathf.Clamp(0.75f - ((GameManager.instance.repairTime * 2) / 100f), 0.25f, 0.75f);
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        // Check if we should enable double jump
        if (GameManager.instance.repairTime <= 16)
        {
            GetComponent<SpriteRenderer>().color = Color.cyan;
            doubleJumpEnabled = true;
        }
    }

    void Update()
    {
        if (!yeeted)
        {
            // Horizontal movement
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector2.SmoothDamp(transform.position, new Vector2(mousePos.x, transform.position.y), ref vel, smoothAmount);

            if (transform.position.x < GameManager.instance.screenLeftEdge) transform.position = new Vector2(GameManager.instance.screenLeftEdge, transform.position.y);
            if (transform.position.x > GameManager.instance.screenRightEdge) transform.position = new Vector2(GameManager.instance.screenRightEdge, transform.position.y);

            // Vertical movement
            if (Input.GetMouseButtonDown(0))
            {
                if (grounded)
                {
                    grounded = false;
                    moveAmountY = jumpVelocity;
                    audioSource.PlayOneShot(jumpSound);
                }
                else if (doubleJumpEnabled && !doubleJumped)
                {
                    doubleJumped = true;
                    moveAmountY = jumpVelocity;
                    audioSource.PlayOneShot(jumpSound);
                }
            }

            moveAmountY += gravity * Time.deltaTime;

            transform.Translate(new Vector2(0, moveAmountY));

            if (transform.position.y < -3.1f)
            {
                transform.position = new Vector2(transform.position.x, -3.1f);
                moveAmountY = 0;
                grounded = true;
                doubleJumped = false;
            }

            // Top spin time
            GameManager.instance.topSpinTime += Time.deltaTime;
        }
    }

    public void Yeet()
    {
        if (!yeeted)
        {
            yeeted = true;
            audioSource.PlayOneShot(yeetSound);
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = false;
            float xForce = Random.Range(-5f, 5f);
            float torque = Random.Range(40f, 80f);
            if (xForce > 0) torque *= -1;
            rb.AddForce(new Vector2(xForce, Random.Range(4f, 6f)), ForceMode2D.Impulse);
            rb.AddTorque(torque);
            LeanTween.delayedCall(gameObject, 3f, () =>
            {
                FadeOutSceneChange.instance.FadeOut("ResultsScene", 1f);
            });
        }
    }
}
