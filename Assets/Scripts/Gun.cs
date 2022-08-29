using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    private float range = 100f;
    [SerializeField] private float damage = 20f;
    public Camera fpsCam;
    private AudioSource gunSound;
    private Ammo ammo;
    private bool isFiring = false;
    [SerializeField] private float fireRate = 0.1f;
    private float attackTime;
    [SerializeField] private Transform cameraRoot;
    private float recoilSpeed = -2f;

    private void Awake()
    {
        InputManager inputManager = new InputManager();
        inputManager.Player.Enable();
        inputManager.Player.Shoot.performed += Shoot;
        inputManager.Player.Shoot.canceled += Shoot_canceled;
        gunSound = GetComponent<AudioSource>();
        ammo = GetComponent<Ammo>();
    }

    private void Update()
    {
        if (isFiring && ammo.GetCurrentAmmo() > 0)
        {
            attackTime += Time.deltaTime;

            if (attackTime >= fireRate)
            {
                attackTime = 0f;
                IsShooting();
            }
        }
        else
        {
            isFiring = false;
        }
    }

    private void IsShooting()
    {
        ammo.ReduceCurrentAmmo();
        gunSound.Play();
        CinemachineShake.instance.ShakeCamera(1f, .25f);
        Recoil();
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Component damageable = hit.transform.GetComponent(typeof(IDamageable));

                if (damageable)
                {
                    GameFunctions.Attack(damageable, damage);
                    hit.transform.gameObject.GetComponentInChildren<ParticleSystem>().Play();
                    hit.transform.gameObject.GetComponent<EnemyAI>().OnDamageTaken();
                }
            }
        }
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (ammo.GetCurrentAmmo() > 0)
        {
            isFiring = true;
        }
    }

    private void Shoot_canceled(InputAction.CallbackContext obj)
    {
        isFiring = false;
    }

    private void Recoil()
    {
        cameraRoot.Rotate(recoilSpeed, 0, 0);
    }
}
