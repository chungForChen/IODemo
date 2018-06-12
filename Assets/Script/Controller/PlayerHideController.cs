﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHideController : Photon.PunBehaviour {

    public float cooldown;//定义技能冷却时间 
    public float keepTime;//技能效果持续时间

    private float curCooldown;

    // Use this for initialization
    void Start()
    {
        curCooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //技能触发
        if (Input.GetKeyDown("space") && curCooldown <= 0)
        {
            curCooldown = cooldown;
            this.photonView.RPC("HidePlayer", PhotonTargets.AllViaServer, true);
            StartCoroutine("WaitForEndSkill");
        }

        if (curCooldown >= 0)
        {
            curCooldown -= Time.deltaTime;
        }

    }

    IEnumerator WaitForEndSkill()
    {
        yield return new WaitForSeconds(keepTime);
        this.photonView.RPC("HidePlayer", PhotonTargets.AllViaServer, false);
    }

    [PunRPC]
    void HidePlayer(bool isHide)
    {
        if (!this.photonView.isMine)
        {
            //在其他玩家的视口下隐藏本身
            this.gameObject.GetComponent<Renderer>().enabled = !isHide;
            this.gameObject.GetComponent<PlayerHealthUI>().getHealthCanvas().SetActive(!isHide);
   
        }
          
    }

}