using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSelect : MonoBehaviour
{

    [SerializeField] private GameObject StartButton, Checkmark, XMark;
    public bool IsAbleToSelect;
    private bool Checked;
    // Start is called before the first frame update
    void Start()
    {
        if (IsAbleToSelect)
        {
            XMark.SetActive(false);
        }
        StartButton.SetActive(false);
        Checkmark.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (IsAbleToSelect)
        {
            if (!Checked)
            {
                Checkmark.SetActive(true);
                StartButton.SetActive(true);
                Checked = true;
            }
        }
        
    }
}
