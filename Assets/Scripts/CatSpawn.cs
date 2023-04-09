using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSpawn : MonoBehaviour
{
    public List<Material> catMats;
    public List<Transform> catPos;
    public GameObject catPrefab;
    public List<Material> catEyeMats;
    // Start is called before the first frame update
    void Start()
    {
        SpawnCats();
    }

    // Update is called once per frame
    void SpawnCats()
    {
        List<Material> tempMats = new List<Material>(catMats);
        List<Transform> tempPos = new List<Transform>(catPos);
        List<Material> tempEyeMats = new List<Material>(catEyeMats);
        for (int i = 0; i < catMats.Count; i++)
        {
            int matIndex = Random.Range(0, tempMats.Count);
            int posIndex = Random.Range(0, tempPos.Count);
            GameObject cat = Instantiate(catPrefab, tempPos[posIndex].position, Quaternion.identity);
            cat.transform.GetChild(3).GetComponent<Renderer>().material = tempMats[matIndex];
            cat.transform.GetChild(1).GetComponent<Renderer>().material = tempEyeMats[matIndex];
            tempMats.RemoveAt(matIndex);
            tempPos.RemoveAt(posIndex);
            tempEyeMats.RemoveAt(matIndex);
        }
    }

    public void cum()
    {
        print("UNG");
    }
}
