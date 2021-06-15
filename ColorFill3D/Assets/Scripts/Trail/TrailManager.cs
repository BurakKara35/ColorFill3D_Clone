using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour
{
    [System.Serializable]
    public class Level
    {
        public int trailStartingInX;
        public int trailStartingInZ;

        public int trailLenghtInX;
        public int trailLenghtInZ;

        public ExtraTrail[] extraTrails;
    }

    [System.Serializable]
    public class ExtraTrail
    {
        public float ExtraTrailLineStartingInX;
        public float ExtraTrailLineStartingInZ;
        public int ExtraTrailLineLenghtInX;
    }

    [SerializeField] private List<Level> levels;

    [SerializeField] private GameObject trail;

    private GameObject[] trailPool = new GameObject[100];
    private int trailPoolCounter = 0;

    [SerializeField] private List<Vector3> trailsOpened = new List<Vector3>();

    private float position_y = 0.6f;
    private float scale_y = 0.2f;

    [SerializeField] private GameObject trailControllerGameObject;
    private TrailController trailController;

    private void Awake()
    {
        trailController = trailControllerGameObject.GetComponent<TrailController>();

        CreateTrailPool();
        CreateTrailMapForLevel(1);
    }

    private void CreateTrailMapForLevel(int currentLevel)
    {
        currentLevel--;

        CreateTrailMainSquare(currentLevel);

        if (levels[currentLevel].extraTrails.Length > 0)
            CreateExtraTrailLines(currentLevel);
    }

    private void CreateTrailMainSquare(int currentLevel)
    {
        var levelProperties = levels[currentLevel];

        float posZ = levelProperties.trailStartingInZ;

        for (int i = 0; i < levelProperties.trailLenghtInZ; i++)
        {
            float posX = levelProperties.trailStartingInX;

            for (int j = 0; j < levelProperties.trailLenghtInX; j++)
            {
                var obj = trailPool[trailPoolCounter];

                obj.transform.position = new Vector3(posX, position_y, posZ);
                obj.SetActive(true);
                trailPoolCounter++;

                posX++;
            }

            posZ++;
        }
    }

    private void CreateExtraTrailLines(int currentLevel)
    {
        var extraLevelProperties = levels[currentLevel].extraTrails;

        for (int i = 0; i < extraLevelProperties.Length; i++)
        {
            float ExtraPosX = extraLevelProperties[i].ExtraTrailLineStartingInX;

            for (int j = 0; j < extraLevelProperties[i].ExtraTrailLineLenghtInX; j++)
            {
                var obj = trailPool[trailPoolCounter];

                obj.transform.position = new Vector3(ExtraPosX, position_y, extraLevelProperties[i].ExtraTrailLineStartingInZ);
                obj.SetActive(true);
                trailPoolCounter++;

                ExtraPosX++;
            }
        }
    }

    private void CreateTrailPool()
    {
        for (int i = 0; i < trailPool.Length; i++)
        {
            GameObject obj = Instantiate(trail);

            obj.transform.parent = this.transform;
            obj.SetActive(false);

            trailPool[i] = obj;
        }
    }

    public void MakeTrailVisible(GameObject obj)
    {
        obj.tag = "OpenedTrail";
        obj.GetComponent<MeshRenderer>().enabled = true;
    }

    public void FillTrailsBetweenLeftToRight()
    {
        StartCoroutine(FillTrails());
    }

    IEnumerator FillTrails()
    {
        while (trailsOpened.Count > 0)
        {
            var trail = trailsOpened[0];
            var pos = new Vector3(trail.x, trail.y, trail.z);

            trailController.isLineEnd = false;

            for (; ; )
            {
                pos = new Vector3(pos.x + 1, pos.y, pos.z);
                trailControllerGameObject.transform.position = pos;

                if (trailController.isLineEnd)
                {
                    trailsOpened.Remove(trail);

                    if (trailController.lastObjectInLine != null)
                        trailsOpened.Remove(trailController.lastObjectInLine.transform.position);

                    break;
                }

                yield return new WaitForSeconds(0.02f);
            }
        }
    }

    public void FillTrailsBetweenRightToLeft()
    {

    }

    private void FillTrailsLeftToRight(int x, int z, int lenght)
    {
        for (int i = 0; i < lenght; i++)
        {
            OpenTheTrailLine(x, z);
            x++;
        }
    }

    public void FillTrailsOpenedList(Vector3 position)
    {
        trailsOpened.Add(position);
    }

    private void OpenTheTrailLine(int x, int z)
    {
        for (int i = 0; i < trailPool.Length; i++)
        {
            if (trailPool[i].transform.position == new Vector3(x, position_y, z))
            {
                trailPool[i].GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    public void RemoveInTrailsOpenedList(Vector3 pos)
    {
        trailsOpened.Remove(pos);
    }
}