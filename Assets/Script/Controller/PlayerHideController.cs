﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHideController : PlayerSkillController {

    public override SkillType GetSkillType()
    {
        return PlayerSkillController.SkillType.HIDE;
    }

    //public ParticleSystem effect;
    public GameObject particleEffect;

    void Awake()
    {
        //effect = gameObject.GetComponentInChildren<ParticleSystem>();
        ParticleSystem[] systems = particleEffect.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Pause();
        }
 
    }

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

            //开启粒子效果
            this.photonView.RPC("EnableParticle", PhotonTargets.AllViaServer); 

        }
        if (curCooldown > 0)
        {
            curCooldown -= Time.deltaTime;
            curCooldown = curCooldown < 0 ? 0 : curCooldown;
        }

    }

    IEnumerator WaitForEndSkill()
    {
        yield return new WaitForSeconds(keepTime);
        this.photonView.RPC("HidePlayer", PhotonTargets.AllViaServer, false);

        //关闭粒子效果
        this.photonView.RPC("DisableParticle", PhotonTargets.AllViaServer);    
    }

    [PunRPC]
    void HidePlayer(bool isHide)
    {
        if (!this.photonView.isMine)
        {
            //在其他玩家的视口下隐藏本身 
            this.gameObject.GetComponent<PlayerHealthUI>().getHealthCanvas().SetActive(!isHide);
            Renderer[] renders = this.gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer m in renders)
            {
                m.enabled = !isHide;
            }

            //显示粒子效果
            if (isHide)
            {
                renders = this.GetComponentInChildren<ParticleSystem>().GetComponents<Renderer>();
                foreach (Renderer m in renders)
                {
                    m.enabled = true;
                }
            }

        }      
    }

    [PunRPC]
    protected void EnableParticle()
    {
        //effect.Play();
        //effect.transform.parent = null;
        ParticleSystem[] systems = particleEffect.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Play();
        }
        particleEffect.transform.parent = null;

    }

    [PunRPC]
    protected void DisableParticle()
    {

        particleEffect.transform.parent = this.transform;
        particleEffect.transform.localPosition = Vector3.zero;
      
    }

}
