using UnityEngine;

public class DirectionalProjectileBullet : MonoBehaviour
{
    void Awake()
    {
        //Destroy in an amount of time if not collide with anything
        Destroy(this.gameObject, 7);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
            PlayerScript.Instance.TakeDamage();
        else
        {
            //Collide with objects in Ground (6) and Wall (7) layers
            if(col.gameObject.layer == 6 || col.gameObject.layer == 7)
                Destroy(this.gameObject);
        }
    }
}