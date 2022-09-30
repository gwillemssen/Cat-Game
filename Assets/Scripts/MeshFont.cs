using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MeshFont : MonoBehaviour
{

    public Font mFont;
    public List<MeshFilter> mPlaneFilters;
    public int mAmmoCount = 60;

    private void Awake()
    {
        //uses material and updates the main texture
        foreach (MeshFilter meshFilter in mPlaneFilters)
        {
            meshFilter.GetComponent<MeshRenderer>().material.mainTexture = mFont.material.mainTexture;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AmmoCountDown());
    }

    private void CreateFont(string output)
    {
        //get the texture based on the font and characters needed
        mFont.RequestCharactersInTexture(output);
        
        //for each character in the string
        for (int i = 0; i < output.Length; i++)
        {
            //get character data
            CharacterInfo character;
            mFont.GetCharacterInfo(output[i], out character);
            
            //set UVs
            Vector2[] uvs = new Vector2[4];
            uvs[0] = character.uvBottomLeft;
            uvs[1] = character.uvTopRight;
            uvs[2] = character.uvBottomRight;
            uvs[3] = character.uvTopLeft;

            //apply uvs
            mPlaneFilters[i].mesh.uv = uvs;
            
            //get scale
            Vector3 newScale = mPlaneFilters[i].transform.localScale;
            newScale.x = character.glyphWidth * 0.02f;
            
            //set
            mPlaneFilters[i].transform.localScale = newScale;
        }
    }
    
    private IEnumerator AmmoCountDown()
    {
        while (mAmmoCount > 0)
        {
            DisplayAmmo(mAmmoCount);
            mAmmoCount--;

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void DisplayAmmo(int ammoCount)
    {
        string output = ammoCount.ToString();
        if (ammoCount < 10)
            output = "0" + output;
        
        CreateFont(output);
    }
}
