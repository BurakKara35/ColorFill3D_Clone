using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private bool swipe = false;
    private bool swipeFinished = true;
    private Vector2 swipeFirstPosition;
    private Vector2 differenceBetweenSwipePositions;
    private IEnumerator swipeCoroutine;

    private enum PlayerDirection { None, Left, Right, Forward, Backward }
    private PlayerDirection playerDirection;

    private float swipingInSeconds = 0.1f;

    private void Start()
    {
        playerDirection = PlayerDirection.None;
    }

    private void Update()
    {
        InputControl();
        Movement();
    }

    private void InputControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipe = true;
            swipeFirstPosition.x = Input.mousePosition.x;
            swipeFirstPosition.y = Input.mousePosition.y;
            swipeCoroutine = Swiping();
            StartCoroutine(swipeCoroutine);

            if (playerDirection != PlayerDirection.None)
                MoveToAppropriateTile();
        }
        if (Input.GetMouseButton(0) && swipe)
        {
            swipeFinished = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            swipe = false;
            swipeFinished = true;
            StopCoroutine(swipeCoroutine);
        }
    }

    public IEnumerator Swiping()
    {
        yield return new WaitForSeconds(swipingInSeconds);
        if (swipe)
        {
            differenceBetweenSwipePositions.x = Input.mousePosition.x - swipeFirstPosition.x;
            differenceBetweenSwipePositions.y = Input.mousePosition.y - swipeFirstPosition.y;

            CalculateDirection();

            if (!swipeFinished)
            {
                swipeCoroutine = Swiping();
                StartCoroutine(swipeCoroutine);
            }
        }
    }

    private void CalculateDirection()
    {
        if (Mathf.Abs(differenceBetweenSwipePositions.x) >= Mathf.Abs(differenceBetweenSwipePositions.y))
        {
            if (differenceBetweenSwipePositions.x < 0)
                playerDirection = PlayerDirection.Left;
            else if (differenceBetweenSwipePositions.x > 0)
                playerDirection = PlayerDirection.Right;
        }
        else
        {
            if (differenceBetweenSwipePositions.y < 0)
                playerDirection = PlayerDirection.Backward;
            else if (differenceBetweenSwipePositions.y > 0)
                playerDirection = PlayerDirection.Forward;
        }
    }

    private void Movement()
    {
        if (playerDirection == PlayerDirection.Left)
            HorizontalMovement(-1);
        else if (playerDirection == PlayerDirection.Right)
            HorizontalMovement(1);
        else if (playerDirection == PlayerDirection.Forward)
            VerticalMovement(1);
        else if (playerDirection == PlayerDirection.Backward)
            VerticalMovement(-1);
    }

    private void HorizontalMovement(float direction)
    {
        transform.Translate(new Vector3(direction, 0, 0) * Time.fixedDeltaTime);
    }

    private void VerticalMovement(float direction)
    {
        transform.Translate(new Vector3(0, 0, direction) * Time.fixedDeltaTime);
    }

    private void MoveToAppropriateTile()
    {
        if(playerDirection == PlayerDirection.Forward || playerDirection == PlayerDirection.Backward)
        {
            transform.position = new Vector3(transform.position.x,
                                             transform.position.y,
                                             Mathf.Round(transform.position.z));
            Debug.Log(transform.position.z);
        }
        else
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x),
                                             transform.position.y,
                                             transform.position.z);
            Debug.Log(transform.position.x);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            playerDirection = PlayerDirection.None;
        }
    }
}