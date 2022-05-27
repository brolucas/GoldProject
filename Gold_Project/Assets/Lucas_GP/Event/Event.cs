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
    public float Event_Spawn_Rate;
    public float Event_DisabledTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
