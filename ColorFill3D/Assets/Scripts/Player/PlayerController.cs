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

    private Rigidbody rigidbody;

    private TrailManager trailManager;

    private float speed = 100;

    private int playerGoingToRight;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        trailManager = GameObject.FindGameObjectWithTag("Trails").GetComponent<TrailManager>();
    }

    private void Start()
    {
        playerDirection = PlayerDirection.None;
    }

    private void Update()
    {
        InputControl();
    }

    private void FixedUpdate()
    {
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
        rigidbody.velocity = new Vector3(direction, 0, 0) * Time.fixedDeltaTime * speed;
        MoveToAppropriateTileInZ();

        if (direction < 0)
            playerGoingToRight--;
        else
            playerGoingToRight++;
    }

    private void VerticalMovement(float direction)
    {
        rigidbody.velocity = new Vector3(0, 0, direction) * Time.fixedDeltaTime * speed;
        MoveToAppropriateTileInX();
    }

    private void MoveToAppropriateTileInX()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);

        transform.position = pos;
    }

    private void MoveToAppropriateTileInZ()
    {
        Vector3 pos = transform.position;
        pos.z = Mathf.Round(pos.z);

        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ClosedTrail"))
        {
            var obj = other.gameObject;

            trailManager.MakeTrailVisible(obj);

            if (playerDirection == PlayerDirection.Forward || playerDirection == PlayerDirection.Backward || playerDirection == PlayerDirection.None)
                trailManager.FillTrailsOpenedList(obj.transform.position);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.CompareTag("OpenedTrail"))
    //    {
    //        if (playerDirection == PlayerDirection.Forward || playerDirection == PlayerDirection.Backward || playerDirection == PlayerDirection.None)
    //            trailManager.FillTrailsOpenedList(other.gameObject.transform.position);
    //        else if (playerDirection == PlayerDirection.Left || playerDirection == PlayerDirection.Right)
    //            trailManager.RemoveInTrailsOpenedList(other.gameObject.transform.position);
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            playerDirection = PlayerDirection.None;
            MoveToAppropriateTileInX();
            MoveToAppropriateTileInZ();

            FillTrailsBetween();

            playerGoingToRight = 0;
        }
    }

    private void FillTrailsBetween()
    {
        if (playerGoingToRight > 0)
            trailManager.FillTrailsBetweenLeftToRight();
        else
            trailManager.FillTrailsBetweenRightToLeft();
    }
}