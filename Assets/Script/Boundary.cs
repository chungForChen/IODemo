﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : Photon.PunBehaviour{

    public enum Axis { X, Y, Z,None };
    public Axis verticalAxis=Axis.None;//表示边界所在的面与哪一个坐标轴垂直

    public bool blockFood = true;//边界是否限制可移动食物（AI）的移动
    public bool blockPlayer = true;//边界是否限制玩家角色（玩家或AI控制）的移动

    public enum ForceDirection { Positive, Negative };//在触发器内的玩家强制更改的方向
    public ForceDirection forceDirection=ForceDirection.Positive;

	void Start () {

	}

    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        string tag = other.gameObject.tag;
        if ((tag == "player" && blockPlayer)
            || ((tag == "food" || tag == "poison") && blockFood))
        {
            ObjectBehaviour objectBehaviour = other.gameObject.GetComponent<ObjectBehaviour>();
            if (objectBehaviour != null)
            {
                Vector3 direction = objectBehaviour.GetForwardDirection();
                bool positiveLimit = (forceDirection == ForceDirection.Positive);
                bool negativeLimit = (forceDirection == ForceDirection.Negative);
                float theta = Random.Range(0.0f, 2 * Mathf.PI);
                float phi = Random.Range(Mathf.PI / 3, Mathf.PI / 2);
                float n1 = Mathf.Cos(theta) * Mathf.Cos(phi);
                float n2 = Mathf.Sin(theta) * Mathf.Cos(phi);
                float n3 = Mathf.Sin(phi);
                if (forceDirection == ForceDirection.Negative) n3 *= -1;
                switch (verticalAxis)
                {
                    case Axis.X:
                        if ((positiveLimit && direction.x < 0.0f) || (negativeLimit && direction.x > 0.0f))
                            objectBehaviour.SetForwardDirecion(new Vector3(n3, n1, n2));
                        break;
                    case Axis.Y:
                        if ((positiveLimit && direction.y < 0.0f) || (negativeLimit && direction.y > 0.0f))
                            objectBehaviour.SetForwardDirecion(new Vector3(n2, n3, n1));
                        break;
                    case Axis.Z:
                        if ((positiveLimit && direction.z < 0.0f) || (negativeLimit && direction.z > 0.0f))
                            objectBehaviour.SetForwardDirecion(new Vector3(n1, n2, n3));
                        break;
                }
            }
        }
    }
}