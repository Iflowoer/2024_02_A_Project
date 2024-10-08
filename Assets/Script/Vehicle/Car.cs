using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//구체 클래스 : 자동차
public class Car : Vehicle
{
    public override void Horn()
    {
        Debug.Log("자동차 경적 : 빵빵");
    }
}
