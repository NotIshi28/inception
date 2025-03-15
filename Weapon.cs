using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Pun.UtilityScripts;

public class Weapon : MonoBehaviour
{
    public float damage;
    public Camera camera;
    public float range;
    public float fireRate;

    public bool isShotgun = false;
    
    [Header("vfx")]
    public ParticleSystem hitFx;
    
    private float nextFire;

    [Header("ammo")]
    public int mag=5;
    public int ammo=45;
    public int magAmmo=45;
    public int maxAmmo=90;
    //added to decrease damage with range
    public float rangeDamageConst = 0.05f;

    [Header("UI")]
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI magText;

    [Header("Sound")]
    public int ShootSoundIndex = 0;
    public PlayerPhotonSoundManager playerPhotonSoundManager;


    [Header("Animation")]
    public Animation anim;
    public AnimationClip reload;

    [Header("Recoil")]

    [Range(0,2)]
    public float recoverPercent = 0.7f;

    public float recoilUp = 0.02f;
    public float recoilBack = 0.08f;

    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;

    private float recoilLength;
    private float recoverLength;

    private bool recoiling;
    public bool recovering;

    void Start()
    {
        magText.text = "Mag: " + mag.ToString();
        ammoText.text = "Ammo: " +  ammo.ToString() + '/' + magAmmo.ToString();

        originalPosition = transform.localPosition;

        recoilLength = 0;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    void Update()
    {

        if(nextFire > 0)
        {
            nextFire -= Time.deltaTime;
        }

        if(Input.GetButton("Fire1") && nextFire <= 0 && anim.isPlaying == false)
        {
            if(ammo > 0 )
            {

                nextFire = 1 / fireRate;
                Shoot();
                
                ammo--;

                magText.text = "Mags Left: " + mag.ToString();
                ammoText.text = "Ammo: " +  ammo.ToString() + '/' + magAmmo.ToString();

            }

            else if(!isShotgun)
            {
                // Reload();
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if(recoiling){
            Recoil();
        }

        if(recovering){
            Recover();
        }

        

    }

    [PunRPC]
    void Shoot()
    {

        recoiling = true;
        recovering = false;

        playerPhotonSoundManager.PlayShootSound(ShootSoundIndex);

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        RaycastHit hit;

        if(Physics.Raycast(ray.origin, ray.direction, out hit, range))
        {
            
            PhotonNetwork.Instantiate(hitFx.name, hit.point, Quaternion.identity);

            if(hit.transform.gameObject.GetComponent<Health>())
            {

                if(damage >= hit.transform.gameObject.GetComponent<Health>().health)
                {

                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();
                    
                    PhotonNetwork.LocalPlayer.AddScore(100);
                }
                float hitDistance = hit.distance;

                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, Mathf.Abs(damage-(hitDistance*rangeDamageConst)));
                Debug.Log("Hit: " + hit.transform.name);
            }
            
        }
    }

    public void Reload()
    {
        if(mag > 0 && ammo < magAmmo)
        {
            anim.Play(reload.name);
        }

    }

    public void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);
        
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);

        if(transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = true;
        }
    }

    public void Recover()
    {
        Vector3 finalPosition = originalPosition;
        
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);

        if(transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = false;
        }
    }

    public void AddAmmo()
    {
        mag--;
        ammo = magAmmo;

        magText.text = "Mags Left: " + mag.ToString();
        ammoText.text = "Ammo: " +  ammo.ToString() + '/' + magAmmo.ToString();
    }
}
