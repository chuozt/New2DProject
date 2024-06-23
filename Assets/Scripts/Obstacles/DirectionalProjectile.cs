using System.Collections;
using UnityEngine;

public class DirectionalProjectile : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private float timeToShoot = 3;
    [SerializeField] private float bulletSpeed = 10;

    void Awake() => StartCoroutine(Shoot());

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(timeToShoot);
        
        //Create a bullet with a rotation that is similar to the projectile's rotation
        GameObject bul = Instantiate(bullet, gunTransform.position, transform.rotation);
        bul.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletSpeed);
        StartCoroutine(Shoot());
    }   
}
