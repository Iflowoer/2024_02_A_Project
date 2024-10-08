using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//추상 클래스 : 탈것

public abstract class Vehicle : MonoBehaviour
{
    public float speed = 10.0f;     //이동속도 선언

    //가상 메서드 : 이동

    public virtual void Move()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);      //앞쪽으로 이동
    }

    //추상 메서드 : 경적

    public abstract void Horn();
}
