using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPracticeController : MonoBehaviour
{
    public GameObject BlackBasket;
    public GameObject WhiteBasket;
    public GameObject Map;

    public void Awake()
    {
        Init();
    }

    protected void Init()
    {
        var bb = Instantiate(BlackBasket);
        var bw = Instantiate(WhiteBasket);
        var map = Instantiate(Map);
    }




}
