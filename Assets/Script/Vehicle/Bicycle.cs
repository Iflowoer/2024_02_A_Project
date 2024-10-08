using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//구체 클래스 : 자전거
public class Bicycle : Vehicle
{
    //Move 메서드 재정의
    public override void Move()
    {
        base.Move();        //기본 이동

        //자전거만의 추가 동작
        transform.Rotate(0, 10, 0);
    }

    public override void Horn()
    {
        Debug.Log("자전거 경적 : 딩딩");
    }
}
