using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Start : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 LoadPosition = new Vector3(-1.4104867f, 3.72687531f, 5.35491753f);
    public void OnMouseDown()
    {
        Vector3 cameraPos = Camera.main.transform.position;

        if ((cameraPos - transform.position).magnitude < 1)
        {
            this.GetComponent<Collider>().enabled = false;
            Camera.main.transform.position = LoadPosition;
            StartCoroutine(Load());
        }

    }

    private IEnumerator Load()
    {

        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Granny's House");

    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
