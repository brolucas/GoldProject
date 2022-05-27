using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Element
{
    Fire,
    Earth,
    Ice,
    Wind,
    Poison
};
public enum Type
{
    Damage,
    Disable,
    TerrainChange
}

public class Event : MonoBehaviour
{
    public Element Event_Element;
    public Type Event_Type;
    public int Event_Nb_Touched_Case;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
