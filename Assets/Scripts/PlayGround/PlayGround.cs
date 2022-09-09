using System.Collections;
using System.Collections.Generic;
using BetterComponentHandling;
using BetterComponentHandling.CleanAssignment;
using BetterComponentHandling.PropertyCaching;
using Game;
using UnityEngine;

public class PlayGround : MonoBehaviour
{
    private Rigidbody rb;

    public Rigidbody RB => this.WithFunction(GetComponent<Rigidbody>).CacheAndReturnValue(ref rb);

    public Rigidbody RB2 => this.CustomGetCachedProperty(ref rb, GetComponent<Rigidbody>);
    
    
    public Rigidbody RB4 => this.GetCachedComponent(ref rb, GetComponent<Rigidbody>);
    
    void Start()
    {
        this.WithFunction(GetComponent<Rigidbody>).ReturnValue();

        this.GetComponentBetter(out var r, GetComponent<Rigidbody>);
    }
}
