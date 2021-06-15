using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    [SerializeField] private int trailStartingInX;
    [SerializeField] private int trailStartingInY;

    [SerializeField] private int trailCountInX;
    [SerializeField] private int trailCountInY;

    [SerializeField] private Vector3[] ExtraTrails;
    [SerializeField] private int[] ExtraTrailCount;
}
