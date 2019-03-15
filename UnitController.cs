using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitController : MonoBehaviour
{
    public Animator animator;
    private Sprite[] sub;
    public string skin = "warrior";
    void Start()
    {
        this.sub = Resources.LoadAll<Sprite>("Sprites/Units/"+ skin);
    }

    private void LateUpdate()
    {
        // change the rendered skin..
        foreach(var rend in GetComponentsInChildren<SpriteRenderer>())
        {
            string spname = rend.sprite.name.Replace("ninja_m", skin);
            rend.sprite = Array.Find(sub, item => item.name == spname);

        }
    }
}
