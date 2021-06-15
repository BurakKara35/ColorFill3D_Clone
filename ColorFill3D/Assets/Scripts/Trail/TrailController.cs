using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    private TrailManager trailManager;

    [HideInInspector] public bool isLineEnd;
    [HideInInspector] public GameObject lastObjectInLine;

    private void Awake()
    {
        trailManager = GameObject.FindGameObjectWithTag("Trails").GetComponent<TrailManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("OpenedTrail"))
        {
            lastObjectInLine = other.gameObject;
            isLineEnd = true;
        }


        if (other.gameObject.CompareTag("Boundary"))
            isLineEnd = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("ClosedTrail"))
            trailManager.MakeTrailVisible(other.gameObject);
    }
}
