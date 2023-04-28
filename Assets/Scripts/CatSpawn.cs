using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSpawn : MonoBehaviour
{
    [Header ("Materials")]
    public List<Material> catMats;
    public List<Material> catEyeMats;

    [Header ("Waypoints")]
    public List<CatWaypoint> CatWaypoints;

    public List<GameObject> catPrefabs;

    public List<Transform> catPos;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var cat in CatWaypoints)
        {
            catPos.Add(cat.catPosition);
        }
        SpawnCats();
    }

    // Update is called once per frame
    void SpawnCats()
    {
        List<Material> tempMats = new List<Material>(catMats);
        List<Transform> tempPos = new List<Transform>(catPos);
        List<GameObject> tempPrefabs = new List<GameObject>();
        foreach(var cat in CatWaypoints)
        {
            if (cat.UseSittingCat) { tempPrefabs.Add(tempPrefabs[0]); }
            if(cat.UseLayingCat) { tempPrefabs.Add(tempPrefabs[1]); }
            
        }
        List<Material> tempEyeMats = new List<Material>(catEyeMats);
        for (int i = 0; i < catMats.Count; i++)
        {
            int matIndex = UnityEngine.Random.Range(0, tempMats.Count);
            int posIndex = UnityEngine.Random.Range(0, tempPos.Count);
            GameObject cat = Instantiate(tempPrefabs[UnityEngine.Random.Range(0,tempPrefabs.Count -1)], tempPos[posIndex].position, tempPos[posIndex].rotation);
            cat.transform.GetChild(3).GetComponent<Renderer>().material = tempMats[matIndex];
            cat.transform.GetChild(1).GetComponent<Renderer>().material = tempEyeMats[matIndex];
            tempMats.RemoveAt(matIndex);
            tempPos.RemoveAt(posIndex);
            tempEyeMats.RemoveAt(matIndex);
        }

        
    }

    [Serializable]
    public class CatWaypoint
    {
        public Transform catPosition;
        public bool UseSittingCat;
        public bool UseLayingCat;
    }

}
