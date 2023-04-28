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

    private List<Transform> catPos;
    // Start is called before the first frame update
    void Start()
    {
        catPos= new List<Transform>(); 
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
        
        List<Material> tempEyeMats = new List<Material>(catEyeMats);
        for (int i = 0; i < catMats.Count; i++)
        {
            foreach (var cats in CatWaypoints)
            {
                if (cats.UseSittingCat) { tempPrefabs.Add(catPrefabs[0]);}
                if (cats.UseLayingCat) { tempPrefabs.Add(catPrefabs[1]);}

            }
            int matIndex = UnityEngine.Random.Range(0, tempMats.Count);
            int posIndex = UnityEngine.Random.Range(0, tempPos.Count);
            int prefabIndex = UnityEngine.Random.Range(0, tempPrefabs.Count);
            if(prefabIndex < 0) { prefabIndex= 0; } 
            GameObject cat = Instantiate(tempPrefabs[prefabIndex], tempPos[posIndex].position, tempPos[posIndex].rotation);
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
