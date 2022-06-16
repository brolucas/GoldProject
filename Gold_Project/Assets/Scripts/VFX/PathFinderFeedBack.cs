using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderFeedBack : MonoBehaviour
{
    private LineRenderer line;
    [SerializeField] private Texture[] textures;
    private int animStep;
    [SerializeField] float fps = 30f;
    private float fpsCounter;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        fpsCounter += Time.deltaTime;
        if(fpsCounter>= 1f / fps)
        {
            animStep++;
            if(animStep == textures.Length)
            {
                animStep = 0;
            }
            line.material.SetTexture("_MainTex", textures[animStep]);
            fpsCounter = 0f;
        }
    }
}
