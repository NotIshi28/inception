using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{

    public Animation anim;
    public AnimationClip draw;


    public GameObject playerSetupView;

    private int selectedWeapon = 0;
    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
        playerSetupView.GetComponent<PhotonView>().RPC("setTPWeapon", RpcTarget.All, 0);
    }

    // Update is called once per frame
    void Update()
    {   
        int previousSelectedWeapon = selectedWeapon;

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
            SelectWeapon();
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
            SelectWeapon();
        }

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
            SelectWeapon();
        }

        if(previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0 )
        {
            if(selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
            SelectWeapon();
        }

        if(Input.GetAxis("Mouse ScrollWheel") < 0 )
        {
            if(selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }

            else
            {
                selectedWeapon--;
            }
        }
    }
    
    [PunRPC]
    void SelectWeapon()
    {

        playerSetupView.GetComponent<PhotonView>().RPC("setTPWeapon", RpcTarget.All, selectedWeapon);
        Debug.Log("Selected Weapon: " + selectedWeapon);

        if(selectedWeapon >= transform.childCount)
        {
            selectedWeapon = 0;
        }

        anim.Stop();
        anim.Play(draw.name);

        int i = 0;
        foreach(Transform weapon in transform)
        {
            if(i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
