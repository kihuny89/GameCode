using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public Color _color = Color.yellow;
    public float _radius = 0.5f;

    private void OnDrawGizmos()
    {
        //기지모 색상
        Gizmos.color = _color;
        //구체 모양의 기지모 생성
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
