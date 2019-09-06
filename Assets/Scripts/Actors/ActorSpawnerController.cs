using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSpawnerController : MonoBehaviour {
    
    GameObject actorPrefab;
    ActorPoolController actorPoolController; // For actor requests

    IDictionary<string, ActorPoolController> actorPools;

    float spawnRadius;
    
    List<ActorInterface> spawnedActors;
    int currentActors;

    float spawnTime;
    float timeSinceLastSpawn;

    // Use this for initialization
    void Start () {
        actorPools = new Dictionary<string, ActorPoolController>();
        spawnRadius = 3.0f;
        spawnedActors = new List<ActorInterface>();
        currentActors = 0;
        spawnTime = 0.02f;
        timeSinceLastSpawn = 0.2f;

        actorPools.Add("Damned", GameObject.FindGameObjectWithTag(
            "DamnedPoolController").GetComponent<ActorPoolController>());
        actorPools.Add("Bats", GameObject.FindGameObjectWithTag(
            "BatPoolController").GetComponent<ActorPoolController>());
        actorPools.Add("Bloaters", GameObject.FindGameObjectWithTag(
            "BloaterPoolController").GetComponent<ActorPoolController>());
        actorPools.Add("Jumpers", GameObject.FindGameObjectWithTag(
            "JumperPoolController").GetComponent<ActorPoolController>());
        actorPools.Add("Shields", GameObject.FindGameObjectWithTag(
            "ShieldPoolController").GetComponent<ActorPoolController>());
        // Continue along
        //actorPools.Add("", GameObject.FindGameObjectWithTag(
        //    "").GetComponent<ActorPoolController>());
    }
	
	// Update is called once per frame
	void Update () {
        foreach (ActorInterface actor in spawnedActors) {
            if (actor != null) {
                if (actor.isFree()) {
                    removeActorFromList(actor);
                }
            }
        }
        //cullActorList();
    }

    public void setEnemyType(GameObject actorPrefab) {
        System.Type prefabType = actorPrefab.GetComponent<ActorInterface>().GetType();

        if (prefabType == typeof(DamnedController)) {
            actorPoolController = actorPools["Damned"];
        }
        else if (prefabType == typeof(BatController)) {
            actorPoolController = actorPools["Bats"];
        }
        else if (prefabType == typeof(BloaterController)) {
            actorPoolController = actorPools["Bloaters"];
        }
        else if (prefabType == typeof(JumperController)) {
            actorPoolController = actorPools["Jumpers"];
        }
        else if (prefabType == typeof(ShieldController)) {
            actorPoolController = actorPools["Shields"];
        }
    }

    public void spawnEnemy() {
        float distance = Random.Range(0, spawnRadius);
        float angle = Random.Range(0, 2*Mathf.PI);
        Vector3 positionModifier = new Vector3(
            distance * Mathf.Sin(angle),
            distance * Mathf.Cos(angle),
            0);
        ActorInterface newActor = actorPoolController.createActor(
            transform.position + positionModifier);
        if (newActor != null) {
            addActorToList(newActor);
        }
    }

    void addActorToList(ActorInterface actor) {
        currentActors++;
        for (int i = 0; i < spawnedActors.Count; i++) {
            if (spawnedActors[i] == null) {
                spawnedActors[i] = actor;
                return;
            }
        }
        spawnedActors.Add(actor);
    }

    void removeActorFromList(ActorInterface actor) {
        if (currentActors == 0) {
            throw new System.Exception("Cannot remove actor from empty list");
        }
        currentActors--;
        int index = getActorIndex(actor);
        spawnedActors[index] = null;
    }

    int getActorIndex(ActorInterface actor) {
        for (int i = 0; i < spawnedActors.Count; i++) {
            if (actor == spawnedActors[i]) {
                return i;
            }
        }
        return -1;
    }

    // TO DO: function not working (even needed?)
    void cullActorList() {
        if (spawnedActors.Count == 0) {
            return;
        }
        // Cull empty locations in the spawnedActor list,
        // from the end until an instance is found
        while (spawnedActors[spawnedActors.Count - 1] == null) {
            spawnedActors.RemoveAt(spawnedActors.Count - 1);
        }
    }

    public int getCurrentActors() {
        return currentActors;
    }
}
