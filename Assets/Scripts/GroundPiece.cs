using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPiece : MonoBehaviour
{
    public bool isColoured;

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeColour(Color colour)
    {
        GetComponent<MeshRenderer>().material.color = colour;
        isColoured = true;

        GameManager.singleton.CheckComplete();
    }
}