using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsController : MonoBehaviour {

    int resolution;
    float radius;

    float circumference;
    float wallLength;
    Vector3 wallScale;

    List<GameObject> walls;

	// Use this for initialization
	void Awake () {
        radius = 60f;
        resolution = 32;
        circumference = Mathf.PI * radius * radius;
        wallLength = (circumference / resolution) * 0.04f;
        wallScale = new Vector3(wallLength, 1, 1);
        walls = new List<GameObject>();

        createWalls();
    }
	
	void createWalls() {
        float angle;
        Vector3 position;
        Vector3 rotation;
        float x;
        float y;

        GameObject wall;

        for (int i = 0; i < resolution; i++) {
            angle = i * ((float)360 / resolution) * Mathf.Deg2Rad;
            position = calcPosition(angle);
            rotation = calcRotation(angle);
            wall = createWall(position, rotation);
            remove3DCollider(wall);

            walls.Add(wall);
        }
        // Delay the addition of 2D colliders because the 3D colliders take time to be removed
        StartCoroutine(add2DColliders(walls));
    }

    Vector3 calcPosition(float angle) {
        float x = radius * Mathf.Sin(angle);
        float y = radius * Mathf.Cos(angle);
        return new Vector3(x, y, 0);
    }

    Vector3 calcRotation(float angle) {
        return new Vector3(0, 0, -angle * Mathf.Rad2Deg);
    }

    GameObject createWall(Vector3 position, Vector3 rotation) {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(transform);
        wall.transform.position = position;
        wall.transform.eulerAngles = rotation;
        wall.transform.localScale = wallScale;
        return wall;
    }

    void remove3DCollider(GameObject wall) {
        Destroy(wall.GetComponent<BoxCollider>());
    }

    IEnumerator add2DColliders(List<GameObject> walls) {
        yield return new WaitForSeconds(0.1f);
        foreach (GameObject wall in walls) {
            wall.AddComponent(typeof(BoxCollider2D));
        }
    }

    public float getRadius() {
        return radius;
    }
}
