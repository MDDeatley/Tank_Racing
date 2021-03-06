﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MapGenerator3
public class AN_SimpleCityGenerator : MonoBehaviour
{
    public int CityZoneCount = 50, BigSectorCount = 10, SqareLength = 20;
    public Vector3Int CityCentre = new Vector3Int ( 0, 0, 0 );
    public AN_CitySample[] List1x1, List1x2, ListAngle, ListSqare;

    int MapLength;
    int [,] IntMap;
    List<Vector2Int> Vacant = new List<Vector2Int>();


    public GameObject start;
    public GameObject finish;

    public GameObject tank;
    public GameObject line;


    private int offset;

    private AudioSource[] aSource; //array of all AudioSource Components


    private void Start()
    {
        aSource = GetComponents<AudioSource>();

        offset = shareDataClass.offset;


		switch (offset)
		{
            case 1:
                aSource[0].Play();
                break;
            case 5:
                aSource[1].Play();
                break;
            case 10:
                aSource[2].Play();
                break;
            case 15:
                aSource[3].Play();
                break;
            case 20:
                aSource[4].Play();
                break;
            default:
                aSource[0].Play();
                break;
		}


        Debug.Log("Generating in game object : " + gameObject.name);
        MapLength = Mathf.FloorToInt(Mathf.Sqrt( CityZoneCount * 3) );
        IntMap = new int[MapLength, MapLength];
        IntMap[MapLength / 2, MapLength / 2] = 1;

        Debug.Log("Length of sqare map : " + MapLength + ", city centre : " + MapLength / 2 + " : " + MapLength / 2);

        CheckNeighbour(MapLength / 2, MapLength / 2);

        CalculateCityZone();
        CalculateBigSectors();
        StartCoroutine( BuildCity() );
    }

    public void CalculateCityZone() // array IntMap is a city territory
    {
        for (int i = 0; i < CityZoneCount; i++)
        {

            Vector2Int Pos = Vacant[Random.Range(0, Vacant.Count - 1)];
            Vacant.Remove(Pos);

            IntMap[Pos.x, Pos.y] = 1;
            CheckNeighbour(Pos.x, Pos.y);
        }
        Debug.Log("City territory calculated");
    }

    public void CalculateBigSectors()
    {
        // scaning city zone
        for (int x = 0; x < IntMap.GetLength(0); x++)
        {
            for (int y = 0; y < IntMap.GetLength(1); y++)
            {
                if (IntMap[x, y] == 1) Vacant.Add(new Vector2Int(x, y));
            }
        }

        // random instantiate big sectors in array
        int limit = CityZoneCount;
        while (BigSectorCount > 0 && limit > 0)
        {
            int i = Random.Range(0, 3);
            switch (i)
            {
                case (0):
                    {
                        InsertLong1x2();
                        break;
                    }
                case (1):
                    {
                        InsertAngle();
                        break;
                    }
                case (2):
                    {
                        InsertSqare();
                        break;
                    }
                default:
                    break;
            }
            limit--;
            if (limit < 1) Debug.Log("Big Sectors Limit achieved");
        }
        Debug.Log("Big sectors calculated");
    }

    #region InsertBigSectorsVoids
    void InsertLong1x2()
    {
        int k = Random.Range(0, Vacant.Count - 1);
        Vector2Int Pos = Vacant[k];
        bool rotate = Random.Range(0, 2) == 1; // 90 degrees if true
        if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x + 1, Pos.y] == 1 && !rotate)
        {
            IntMap[Pos.x, Pos.y] = 21;
            IntMap[Pos.x + 1, Pos.y] = 9;
            Vacant.Remove(Pos);
            Vacant.Remove(new Vector2Int(Pos.x + 1, Pos.y));
            BigSectorCount--;
        }
        else if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x, Pos.y + 1] == 1 && rotate)
        {
            IntMap[Pos.x, Pos.y] = 22;
            IntMap[Pos.x, Pos.y + 1] = 0;
            Vacant.Remove(Pos);
            Vacant.Remove(new Vector2Int(Pos.x, Pos.y + 1));
            BigSectorCount--;
        }
    }

    void InsertAngle()
    {
        int k = Random.Range(0, Vacant.Count - 1);
        Vector2Int Pos = Vacant[k];
        bool rotate = Random.Range(0, 2) == 1; // -90 degrees if true
        if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x + 1, Pos.y] == 1 && IntMap[Pos.x, Pos.y + 1] == 1 && !rotate)
        {
            IntMap[Pos.x, Pos.y] = 31;
            IntMap[Pos.x + 1, Pos.y] = 0;
            IntMap[Pos.x, Pos.y + 1] = 0;
            Vacant.Remove(Pos);
            Vacant.Remove(new Vector2Int(Pos.x + 1, Pos.y));
            Vacant.Remove(new Vector2Int(Pos.x, Pos.y + 1));
            BigSectorCount--;
        }
        else if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x, Pos.y + 1] == 1 && IntMap[Pos.x - 1, Pos.y] == 1 && rotate)
        {
            IntMap[Pos.x, Pos.y] = 32;
            IntMap[Pos.x, Pos.y + 1] = 0;
            IntMap[Pos.x - 1, Pos.y] = 0;
            Vacant.Remove(Pos);
            Vacant.Remove(new Vector2Int(Pos.x, Pos.y + 1));
            Vacant.Remove(new Vector2Int(Pos.x - 1, Pos.y));
            BigSectorCount--;
        }
    }

    void InsertSqare()
    {
        int k = Random.Range(0, Vacant.Count - 1);
        Vector2Int Pos = Vacant[k];
        bool rotate = Random.Range(0, 2) == 1; // -90 degrees if true
        if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x + 1, Pos.y] == 1 && IntMap[Pos.x, Pos.y + 1] == 1 && IntMap[Pos.x + 1, Pos.y + 1] == 1 && !rotate)
        {
            IntMap[Pos.x, Pos.y] = 41;
            IntMap[Pos.x + 1, Pos.y] = 0;
            IntMap[Pos.x, Pos.y + 1] = 0;
            IntMap[Pos.x + 1, Pos.y + 1] = 0;
            Vacant.Remove(Pos);
            Vacant.Remove(new Vector2Int(Pos.x + 1, Pos.y));
            Vacant.Remove(new Vector2Int(Pos.x, Pos.y + 1));
            Vacant.Remove(new Vector2Int(Pos.x + 1, Pos.y + 1));
            BigSectorCount--;
        }
        else if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x - 1, Pos.y] == 1 && IntMap[Pos.x, Pos.y - 1] == 1 && IntMap[Pos.x - 1, Pos.y - 1] == 1 && rotate)
        {
            IntMap[Pos.x, Pos.y] = 42;
            IntMap[Pos.x - 1, Pos.y] = 0;
            IntMap[Pos.x, Pos.y - 1] = 0;
            IntMap[Pos.x - 1, Pos.y - 1] = 0;
            Vacant.Remove(Pos);
            Vacant.Remove(new Vector2Int(Pos.x, Pos.y - 1));
            Vacant.Remove(new Vector2Int(Pos.x - 1, Pos.y));
            Vacant.Remove(new Vector2Int(Pos.x - 1, Pos.y - 1));
            BigSectorCount--;
        }
    }
    #endregion

    IEnumerator BuildCity() // instantiate finished city version
    {

        bool createdStart = false;
        bool createdFinish = false;


        var YN = Mathf.Pow(-1,Random.Range(1, 3));
        var XN = Mathf.Pow(-1, Random.Range(1, 3));


        for (int x = 1; x < IntMap.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < IntMap.GetLength(1) - 1; y++)
            {



                int selected;
                switch (IntMap[x, y])
                {
                    case (1): // 1x1
                        {
                            if (createdStart == false && (x >= MapLength / 2) && (y >= MapLength / 2)) {
                                Debug.Log("CREATED START!");

                                Instantiate(start, new Vector3Int(x - MapLength / 2 + CityCentre.x, 0, y - MapLength / 2 + CityCentre.y) * SqareLength, Quaternion.Euler(0, 90 * Random.Range(0, 4), 0));

                                tank.transform.position = new Vector3Int(x - MapLength / 2 + CityCentre.x, 0, y - MapLength / 2 + CityCentre.y) * SqareLength;

                                createdStart = true;
                            }
                            else if (createdFinish == false && (x >= MapLength / 2 + offset * XN) && (y >= MapLength / 2 + offset * YN))
                            {
                                Debug.Log("CREATED finish!");

                                Instantiate(finish, new Vector3Int(x - MapLength / 2 + CityCentre.x, 0, y - MapLength / 2 + CityCentre.y) * SqareLength, Quaternion.Euler(0, 90 * Random.Range(0, 4), 0));

                                line.transform.position = new Vector3Int(x - MapLength / 2 + CityCentre.x, 0, y - MapLength / 2 + CityCentre.y) * SqareLength;

                                createdFinish = true;
                            }
                            else {
                                selected = SelectPrefab(List1x1);
                                Instantiate( List1x1[selected].gameObject, new Vector3Int(x - MapLength / 2 + CityCentre.x, 0, y - MapLength / 2 + CityCentre.y) * SqareLength, Quaternion.Euler(0, 90 * Random.Range(0,4), 0) );
                            }

                            break;
                        }
                    case (21): // 1x2
                        {
                            selected = SelectPrefab(List1x2);
                            Instantiate(List1x2[selected].gameObject, new Vector3Int(x - MapLength / 2 + CityCentre.x, 0, y - MapLength / 2 + CityCentre.y) * SqareLength, Quaternion.Euler(0, 0, 0));
                            break;
                        }
                    case (22): // 1x2 rotate -90
                        {
                            selected = SelectPrefab(List1x2);
                            Instantiate(List1x2[selected].gameObject, new Vector3Int(x - MapLength / 2 + CityCentre.x, 0, y - MapLength / 2 + CityCentre.y) * SqareLength, Quaternion.Euler(0, -90, 0));
                            break;
                        }
                    case (31): // angle
                        {
                            selected = SelectPrefab(ListAngle);
                            Instantiate(ListAngle[selected].gameObject, new Vector3Int(x - MapLength / 2 + CityCentre.x, 0, y - MapLength / 2 + CityCentre.y) * SqareLength, Quaternion.Euler(0, 0, 0));
                            break;
                        }
                    case (32): // angle rotate -90
                        {
                            selected = SelectPrefab(ListAngle);
                            Instantiate(ListAngle[selected].gameObject, new Vector3Int(x - MapLength / 2 + CityCentre.x, 0, y - MapLength / 2 + CityCentre.y) * SqareLength, Quaternion.Euler(0, -90, 0));
                            break;
                        }
                    case (41): // sqare
                        {
                            selected = SelectPrefab(ListSqare);
                            Instantiate(ListSqare[selected].gameObject, new Vector3Int(x - MapLength / 2 + CityCentre.x, 0, y - MapLength / 2 + CityCentre.y) * SqareLength, Quaternion.Euler(0, 0, 0));
                            break;
                        }
                    case (42): // sqare shift
                        {
                            selected = SelectPrefab(ListSqare);
                            Instantiate(ListSqare[selected].gameObject, new Vector3Int(x - MapLength / 2 + CityCentre.x, 0, y - MapLength / 2 + CityCentre.y) * SqareLength, Quaternion.Euler(0, 180, 0));
                            break;
                        }
                    default:
                        break;
                }
                //yield return new WaitForSeconds(.001f);
            }
            yield return new WaitForSeconds(.001f);
        }
        Debug.Log("City was built");
    }

    void CheckNeighbour(int x, int y)
    {
        if (IntMap[x, y + 1] == 0) Vacant.Add(new Vector2Int(x, y + 1)); else Vacant.Remove(new Vector2Int(x, y + 1));
        if (IntMap[x, y - 1] == 0) Vacant.Add(new Vector2Int(x, y - 1)); else Vacant.Remove(new Vector2Int(x, y - 1));
        if (IntMap[x + 1, y] == 0) Vacant.Add(new Vector2Int(x + 1, y)); else Vacant.Remove(new Vector2Int(x + 1, y));
        if (IntMap[x - 1, y] == 0) Vacant.Add(new Vector2Int(x - 1, y)); else Vacant.Remove(new Vector2Int(x - 1, y));
    }

    public int SelectPrefab(AN_CitySample[] List)
    {
        int VeritySumm = 0;
        for (int k = 0; k < List.Length; k++)
        {
            VeritySumm += List[k].Verity;
        }

        int CheckSumm = 0, i = 0;
        int IntRandom = Random.Range(1, VeritySumm);
        while (CheckSumm < IntRandom)
        {
            CheckSumm += List[i].Verity;
            i++;
        }
        i--;
        return i;
    
    }
}