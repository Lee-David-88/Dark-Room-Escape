using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public Transform muzzle;
    public Projectile projectile;
    public float msBtwShots = 100f;
    public float muzzleVelocity = 35f;

    public Transform shell;
    public Transform shellEjectionPoint;
    public bool EjectShell;

    MuzzleFlash muzzleFlash;

    float nextShotTime;

    public void Start() {
        muzzleFlash = GetComponent<MuzzleFlash>();
    }
    public void Shoot() {

        if (Time.time > nextShotTime) {
            nextShotTime = Time.time + msBtwShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);

            if (EjectShell) {
                Instantiate(shell, shellEjectionPoint.position, shellEjectionPoint.rotation);
            }
            muzzleFlash.Activate();
            
        }
        
    }

    public void Aim(Vector3 aimPoint) {
        transform.LookAt(aimPoint);
    }
}
