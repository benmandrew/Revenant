using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour, ActorInterface {

    bool free;
    //bool isProtectedByShield; // Redundant
    float health;
    float MAXHEALTH;

    GameObject player;
    Rigidbody2D rb2d;
    GameObject body;
    Animator anim;

    WaveController waveController;

    public int positionInRenderQueue;
    float arenaRadius;
    float boundsLength;

    bool teleport;
    float teleportRadiusMin;
    float timeSinceLastChange;

    int protectedEnemyNum;
    List<GameObject> protectedEnemies;
    List<ActorInterface> protectedEnemyScripts;

    List<ConnectingLine> lines;
    bool linesInitialised = false;

    float rotSpeed;

    // Use this for initialization
    void Awake() {
        free = true;
        //isProtectedByShield = false;
        MAXHEALTH = 1500f;
        player = GameObject.FindGameObjectWithTag("Player");
        rb2d = GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).gameObject;
        anim = body.GetComponent<Animator>();
        anim.SetTrigger("Teleport");

        waveController = GameObject.FindGameObjectWithTag("Wave Manager").GetComponent<WaveController>();

        teleport = false;
        teleportRadiusMin = 20;
        timeSinceLastChange = 7f;
        arenaRadius = GameObject.FindGameObjectWithTag("Walls").GetComponent<WallsController>().getRadius();
        boundsLength = body.GetComponent<SpriteRenderer>().bounds.extents.magnitude;

        rotSpeed = 50f;

        protectedEnemyNum = 10;
        createConnectingLines();
        linesInitialised = true;
    }

    List<GameObject> chooseProtectedEnemies(int enemyNum) {
        List<GameObject> activeActors = waveController.getActiveActors();
        List<int> protectedActorIndices = getProtectedActorIndices(activeActors);

        List<GameObject> protectedActors = new List<GameObject>();
        int counter = 0;
        foreach (int index in protectedActorIndices) {
            protectedActors.Add(activeActors[index]);
            protectedEnemyScripts.Add(activeActors[index].GetComponent<ActorInterface>());
            protectedEnemyScripts[counter].setProtected(true);
            counter++;
        }
        return protectedActors;
    }

    List<int> getProtectedActorIndices(List<GameObject> activeActors) {
        List<int> protectedActorIndices = new List<int>();
        if (activeActors.Count == 0) {
            return protectedActorIndices;
        }
        List<int> availableActorIndices = getAvailableActorIndices(activeActors);
        if (availableActorIndices.Count == 0) {
            return protectedActorIndices;
        }
        availableActorIndices = shuffle(availableActorIndices);
        int index = 0;
        while (protectedActorIndices.Count < protectedEnemyNum) {
            if (index >= availableActorIndices.Count) {
                print("Not enough available actors");
                break;
            }
            protectedActorIndices.Add(availableActorIndices[index]);
            index++;
        }
        return protectedActorIndices;
    }

    List<int> shuffle(List<int> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    List<int> getAvailableActorIndices(List<GameObject> activeActors) {
        List<int> availableActorIndices = new List<int>();
        for (int i = 0; i < activeActors.Count; i++) {
            if (!activeActors[i].GetComponent<ActorInterface>().isProtected()
                && !(activeActors[i].GetComponent<ActorInterface>().GetType() == typeof(ShieldController))) {
                availableActorIndices.Add(i);
            }
        }
        return availableActorIndices;
    }

    void createConnectingLines() {
        lines = new List<ConnectingLine>();
        GameObject linePrefab = (GameObject)Resources.Load("ConnectingLine");
        GameObject line;
        for (int i = 0; i < protectedEnemyNum; i++) {
            line = Instantiate(linePrefab);
            lines.Add(line.GetComponent<ConnectingLine>());
            line.transform.SetParent(transform);
        }
    }

    bool actorIsShield(GameObject actor) {
        return actor.GetComponent<ActorInterface>().GetType() == typeof(ShieldController);
    }

    // Update is called once per frame
    void Update () {
        rb2d.velocity = Vector2.zero;
        if (!free) {
            manageMovement();
            timeSinceLastChange += Time.deltaTime;
            //if (timeSinceLastChange > 8f) {
            //    setTeleport();
            //    timeSinceLastChange = 0f;
            //}
            if (health <= 0.0f) {
                killProtectedEnemies();
                freeActor();
            }
            drawLines();
        }
    }

    void drawLines() {
        for (int i = 0; i < protectedEnemies.Count; i++) {
            GameObject enemy = protectedEnemies[i];
            ConnectingLine line = lines[i];
            line.setTargetPos(enemy.transform);
        }
    }

    void manageMovement() {
        if (teleport) {
            Vector3 teleportCoords = calcTeleportCoords();
            transform.position = teleportCoords;
            teleport = false;

            foreach (ConnectingLine line in lines) {
                line.setShieldPos(transform);
            }
        }
        spin();
    }

    void spin() {
        Vector3 spinVector = new Vector3(0, 0, rotSpeed * Time.deltaTime);
        transform.Rotate(spinVector);
    }

    Vector3 calcTeleportCoords() {
        Vector3 newPos;
        // Keep generating new coordinates if it's outside the arena
        do {
            float angle = Random.Range(0, 2 * Mathf.PI);
            float radius = Random.Range(0, arenaRadius - boundsLength);
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            newPos = new Vector3(x, y, 0);
        } while (isInRangeOfPlayer(newPos));
        return newPos;
    }

    bool isInRangeOfPlayer(Vector3 position) {
        Vector3 difference = position - player.transform.position;
        return difference.magnitude < teleportRadiusMin;
    }

    // Animator event function
    void setTeleport() {
        teleport = true;
    }

    void killProtectedEnemies() {
        foreach (ActorInterface enemy in protectedEnemyScripts) {
            enemy.setProtected(false);
            enemy.removeHealth(enemy.getHealth() + 1f);
        }
    }

    public void createActor(GameObject parent) {
        health = MAXHEALTH;
        body.GetComponent<SpriteRenderer>().enabled = true;
        body.GetComponent<Collider2D>().enabled = true;
        body.GetComponent<Animator>().enabled = true;
        free = false;
        
        protectedEnemyScripts = new List<ActorInterface>();
        protectedEnemies = new List<GameObject>();
        protectedEnemies = chooseProtectedEnemies(protectedEnemyNum);
        
        for (int i = 0; i < protectedEnemies.Count; i++) {
            GameObject enemy = protectedEnemies[i];
            ConnectingLine line = lines[i];
            line.setTargetPos(enemy.transform);
            line.setShieldPos(transform);
            line.enable();
        }

        transform.position = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
    }

    public void freeActor() {
        rb2d.velocity = Vector2.zero;
        body.GetComponent<SpriteRenderer>().enabled = false;
        body.GetComponent<Collider2D>().enabled = false;
        body.GetComponent<Animator>().enabled = false;
        free = true;

        if (linesInitialised) {
            foreach (ConnectingLine line in lines) {
                line.disable();
            }
        }
    }

    public bool isFree() {
        return free;
    }

    public bool isProtected() {
        return false; // A shield can never be protected
    }

    public void setProtected(bool isProtected) {
        throw new System.Exception("Inner protection state of 'shield' cannot be changed");
    }

    public void setPosition(Vector3 newPosition) {
        transform.position = newPosition;
    }

    public void setRenderingOrder(int position) {
        SpriteRenderer sprite = body.GetComponent<SpriteRenderer>();
        sprite.sortingOrder += position;
    }

    public float getHealth() {
        return health;
    }

    public void removeHealth(float damage) {
        health -= damage;
    }
}
