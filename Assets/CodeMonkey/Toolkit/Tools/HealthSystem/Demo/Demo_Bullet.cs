using UnityEngine;

namespace CodeMonkey.Toolkit.THealthSystem.Demo {

    public class Demo_Bullet : MonoBehaviour {


        private void Start() {
            float bulletSpeed = 200f;
#if UNITY_6000_0_OR_NEWER
            GetComponent<Rigidbody2D>().linearVelocity = transform.right * bulletSpeed;
#else
            GetComponent<Rigidbody2D>().velocity = transform.right * bulletSpeed;
#endif
        }

        private void OnTriggerEnter2D(Collider2D collider2D) {
            if (collider2D.TryGetComponent(out Demo_Zombie zombie)) {
                zombie.Damage();
                Destroy(gameObject);
            }
        }

    }

}