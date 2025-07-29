using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool isFirstUpdate = true;

    private void Update()
    {
        // Making the callback slower than the first frame
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            SceneLoader.LoaderCallback();
        }
    }
}
