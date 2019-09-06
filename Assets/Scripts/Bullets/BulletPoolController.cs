using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolController : MonoBehaviour {

    public int bulletPoolSize;
    public GameObject bulletPrefab;
    List<GameObject> bullets;
    List<BulletController> bulletControllers;

    [SerializeField]
    float bulletSpeed;
    [SerializeField]
    float bulletDamage;

	// Use this for initialization
	void Start () {
        //bulletPrefab = (GameObject)Resources.Load("Bullet");

        //bulletPoolSize = 50;
        //bulletSpeed = 50.0f;
        bullets = new List<GameObject>();
        bulletControllers = new List<BulletController>();
        for (int i = 0; i < bulletPoolSize; i++) {
            instantiateNewBullet();
        }
	}

    public void createBullet(GameObject parent, string parentWeapon) {
        int index = getAvailableBullet();
        if (index == -1) {
            instantiateNewBullet();
            index = bullets.Count - 1;
            bulletPoolSize++;
        }
        bulletControllers[index].setParent(parent);
        bulletControllers[index].resetToParent();
        bulletControllers[index].setParentWeapon(parentWeapon);
    }

    int getAvailableBullet() {
        int index = -1;
        bool freeBulletFound = false;
        while (!freeBulletFound) {
            index++;
            if (index >= bulletPoolSize) {
                index = -1;
                break;
            }
            if (bullets[index].GetComponent<BulletController>().isFree()) {
                freeBulletFound = true;
            }
        }
        return index;
    }

    void instantiateNewBullet() {
        int newIndex = bullets.Count;
        GameObject bullet = Instantiate(
            bulletPrefab,
            Vector2.zero,
            Quaternion.identity);
        bullets.Add(bullet);
        bulletControllers.Add(bullet.GetComponent<BulletController>());

        bullet.transform.SetParent(transform);
        bulletControllers[newIndex].setBulletContainer(this);
        bulletControllers[newIndex].freeBullet();
        bullet.transform.SetParent(transform);
    }

    public float getBulletSpeed() {
        return bulletSpeed;
    }

    public float getBulletDamage() {
        return bulletDamage;
    }

    public void setBulletDamage(float newBulletDamage) {
        bulletDamage = newBulletDamage;
    }
}
