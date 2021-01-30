using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainGame : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitToLoad());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator waitToLoad() 
    {
        yield return new WaitForSeconds(43f);
        SceneManager.LoadScene("MainScene");
    }
}
