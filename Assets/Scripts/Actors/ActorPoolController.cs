using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorPoolController : MonoBehaviour {

    public int poolSize;
    int nextFreeActor;
    public GameObject actorPrefab;
    List<GameObject> actors;
    List<ActorInterface> actorControllers;

    float timeSinceLastSpawn;
    float spawnTime;

	// Use this for initialization
	void Awake () {
        poolSize = 0;
        nextFreeActor = 0;
        actors = new List<GameObject>();
        actorControllers = new List<ActorInterface>();

        initialiseActors();

        spawnTime = 1;
        timeSinceLastSpawn = spawnTime;
    }
	
    void initialiseActors() {
        for (int i = 0; i < poolSize; i++) {
            instantiateNewActor();
        }
    }

	// Update is called once per frame
	void Update () {

    }
    
    public ActorInterface createActor(Vector3 newPosition = new Vector3()) {
        int index = getAvailableActor();
        if (index == -1) {
            instantiateNewActor();
            index = actors.Count - 1;
            poolSize++;
        }
        actorControllers[index].createActor(gameObject); // Redundant argument
        actorControllers[index].setPosition(newPosition);

        return actorControllers[index];
    }

    int getAvailableActor() {
        bool foundFreeActor = false;
        int index = 0;
        for (int i = 0; i < poolSize; i++) {
            index = (i + nextFreeActor) % poolSize;
            if (actorControllers[index].isFree()) {
                foundFreeActor = true;
                break;
            }
        }
        nextFreeActor++;
        if (!foundFreeActor) {
            return -1;
        }
        return index;
    }

    void instantiateNewActor() {
        int newIndex = actors.Count;
        GameObject actor = Instantiate(
            actorPrefab,
            Vector2.zero,
            Quaternion.identity);
        actors.Add(actor);
        actorControllers.Add(actor.GetComponent<ActorInterface>());
        actorControllers[newIndex].freeActor();
        actorControllers[newIndex].setRenderingOrder(newIndex);
        actor.transform.SetParent(transform);
    }

    public int getBoundActorNum() {
        int num = 0;
        for (int i = 0; i < actors.Count; i++) {
            if (!actors[i].GetComponent<ActorInterface>().isFree()) {
                num++;
            }
        }
        return num;
    }

    public ActorInterface getActorType() {
        return actorPrefab.GetComponent<ActorInterface>();
    }

    public List<GameObject> getActiveActors() {
        List<GameObject> activeActors = new List<GameObject>();
        foreach (GameObject actor in actors) {
            if (!actor.GetComponent<ActorInterface>().isFree()) {
                activeActors.Add(actor);
            }
        }
        return activeActors;
    }
}
