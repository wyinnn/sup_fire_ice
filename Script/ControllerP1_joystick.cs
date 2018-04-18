﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class ControllerP1_joystick : MonoBehaviour {


    public float Accelrate;
    public float MaxSpeed;
    public bool isFireing;
    public bulletMove bullet;
    public MissileMove missile;
    public Transform firepoint;
    public float bulletSpeed;
    public AudioSource audioS;
    public AudioSource audioSB;
    public AudioSource audioR;
    public AudioSource audioM;

    public int special;

    public Animator anim;
    public Animator MoveAnim;

    public int maxAmmo;
    public float remainAmmo;
    public GameObject AmmoCount;
    public GameObject LifeCount;
    public GameObject SpeCount;


    public float timeBetweenShots;
    private float shotCounter;


    public bool isBig;
    public bool isMulti;
    public bool isMissile;

    public float maxLife;
    public float remainLife;


    private Rigidbody rigid;
    private float angle = 0f;

    private GameObject player;

    void SetBig()
    {
        isBig = true;
        isMulti = false;
        isMissile = false;
        audioR.Play();
        special = 5;
    }

    void SetMulti()
    {
        isBig = false;
        isMulti = true;
        isMissile = false;
        audioR.Play();
        special = 5;

    }

    void SetMissile()
    {
        isBig = false;
        isMulti = false;
        isMissile = true;
        audioR.Play();
        special = 5;

    }

    void SetLife(int change)
    {
        remainLife += change;
        if (remainLife > maxLife)
        {
            remainLife = maxLife;
        }
    }

    void SetAmmo(float change)
    {

        remainAmmo += change;


        if (remainAmmo > maxAmmo)
        {
            remainAmmo = maxAmmo;
        }
    }

    void Start()
    {
        rigid = this.GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {

        Vector3 pos = rigid.position;

        float v_dir = Input.GetAxis("J2-V-Direct");
        float h_dir = Input.GetAxis("J2-H-Direct");

        Vector3 direction = Vector3.zero;

        direction.x = -h_dir;
        direction.y = v_dir;

        angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, new Vector3(0f, 0f, -1f));

        if (direction.magnitude >= 0.9)
        {
            transform.GetChild(1).rotation = rotation;
        }


        float h_axis = Input.GetAxis("J2-Horizontal");

        if (h_axis != 0)
        {
            MoveAnim.Play("body Animation");
        }


        rigid.velocity = new Vector3(Accelrate * h_axis, 0f, 0f);

        if (Input.GetAxis("Fire2") < 0 && remainAmmo >= 1) //fire
        {
            isFireing = true;
        }
        else
        {
            isFireing = false;
        }

        if (isFireing)
        {
            shotCounter -= Time.deltaTime;
            if (shotCounter <= 0)
            {
                shotCounter = timeBetweenShots;
                audioS.volume = 0.3f;

                if (isMulti)
                {
                    special -= 1;
                    bulletMove newBullet1 = Instantiate(bullet, firepoint.position, firepoint.rotation) as bulletMove;
                    bulletMove newBullet2 = Instantiate(bullet, firepoint.position, firepoint.rotation) as bulletMove;

                    newBullet1.gameObject.SetActive(true);
                    newBullet1.transform.Translate(new Vector3(0.2f, 0f, 0f));
                    newBullet1.transform.Rotate(new Vector3(0f, 0f, -5f));
                    newBullet1.transform.Rotate(0f, 90f, 90f);
                    newBullet1.bulletSpeed = bulletSpeed;
                    newBullet1.SendMessage("SetMulti", true);

                    newBullet2.gameObject.SetActive(true);
                    newBullet2.transform.Translate(new Vector3(-0.2f, 0f, 0f));
                    newBullet2.transform.Rotate(new Vector3(0f, 0f, 5f));
                    newBullet2.transform.Rotate(0f, 90f, 90f);
                    newBullet2.bulletSpeed = bulletSpeed;
                    newBullet2.SendMessage("SetMulti", true);

                    CameraShaker.Instance.ShakeOnce(1.5f, 4f, 0f, 1.5f);
                    audioS.pitch = Random.Range(1f, 5f);

                }
                else
                {

                    if (isMissile)
                    {
                        special -= 1;
                        MissileMove newMissile = Instantiate(missile, firepoint.position, firepoint.rotation) as MissileMove;
                        newMissile.gameObject.SetActive(true);
                        //CameraShaker.Instance.ShakeOnce(2f, 4f, 0f, 1.5f);

                    }
                    else
                    {
                        bulletMove newBullet = Instantiate(bullet, firepoint.position, firepoint.rotation) as bulletMove;
                        newBullet.gameObject.SetActive(true);
                        newBullet.transform.Rotate(0f, 90f, 90f);
                        newBullet.bulletSpeed = bulletSpeed;
                        if (isBig)
                        {
                            audioSB.pitch = Random.Range(0.2f, 0.3f);
                            audioSB.volume = 1.0f;
                            special -= 1;
                            newBullet.transform.localScale = new Vector3(1f, 1f, 1f);
                            Animator a = newBullet.GetComponent<Animator>();
                            ParticleSystem p = newBullet.GetComponent<ParticleSystem>();
                            a.enabled = false;
                            newBullet.SendMessage("SetBig", true);
                            CameraShaker.Instance.ShakeOnce(2.5f, 4f, 0f, 3f);
                        }
                        else
                        {
                            CameraShaker.Instance.ShakeOnce(1.25f, 4f, 0f, 1.5f);
                            audioS.pitch = Random.Range(1f, 5f);

                        }
                    }

                }

                SetAmmo(-1);

                if (!isMissile)
                {
                    if (isBig)
                    {
                        audioSB.Play();
                    }
                    else
                    {
                        audioS.Play();
                    }
                }
                else
                {
                    audioM.pitch = Random.Range(0.8f, 1.2f);
                    audioM.Play();
                }
                anim.Play("Gun Animation");
            }
        }
        else
        {
            //shotCounter = 0;
            shotCounter -= Time.deltaTime;

        }
        AmmoCount.SendMessage("SetAmmo", Mathf.Floor(remainAmmo));

        float liftRatio = ((maxLife - 1) / maxLife) * remainLife / maxLife + 1f / maxLife;

        transform.GetChild(0).transform.localScale = new Vector3(1.5f * liftRatio, 0.3f, 0.5f);

        LifeCount.SendMessage("SetLife", remainLife);

        if (remainLife <= 0)
        {
            StartCoroutine(DelayTime(0.3f));
            Time.timeScale = 0.2f;
        }
        if (special <= 0)
        {
            isBig = false;
            isMulti = false;
            isMissile = false;
        }

        SpeCount.SendMessage("SetSpe", special);

    }
    IEnumerator DelayTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
