using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

    UIController ui;
    TraderController trader;

    List<ActorSpawnerController> spawners;
    List<ActorPoolController> actorPools;

    IDictionary<string, GameObject> enemyPrefabs;

    float timeBetweenStages;
    float timeBetweenWaves;
    float timeSinceLastStageChange;
    float timeSinceTraderDeath;

    int currentWave;
    int currentStageInWave;

    public bool isPlaying;
    bool spawnNewStage;
    bool preparingWave;
    bool waveCompositionGenerated;
    bool stageIsEmpty;
    public bool newWave;

    IDictionary<string, int> waveComposition;

    // Use this for initialization
    void Start () {
        initialise();
	}

    public void initialise() {
        initialiseVars();
        foreach (Transform spawner in GameObject.Find("Enemy Spawners").transform) {
            spawners.Add(spawner.gameObject.GetComponent<ActorSpawnerController>());
        }
        foreach (Transform actorPool in GameObject.Find("Pool Objects").transform) {
            if (actorPool.gameObject.name == "BulletPool"
                || actorPool.gameObject.name == "FireBallPool"
                || actorPool.gameObject.name == "ItemPool") {
                continue;
            }
            actorPools.Add(actorPool.gameObject.GetComponent<ActorPoolController>());
        }
    }

    void initialiseVars() {
        spawners = new List<ActorSpawnerController>();
        actorPools = new List<ActorPoolController>();
        timeBetweenStages = 0.5f;
        timeBetweenWaves = 2.5f;
        timeSinceLastStageChange = 0f;
        timeSinceTraderDeath = 0f;
        currentWave = 0;
        currentStageInWave = 1;
        isPlaying = true;
        spawnNewStage = false;
        preparingWave = true;
        waveCompositionGenerated = false;
        stageIsEmpty = false;
        newWave = true;
        waveComposition = new Dictionary<string, int>() {
            { "Damned", 0 },
            { "Bat", 0 },
            { "Bloater", 0 },
            { "Jumper", 0 },
            { "Shield", 0 }
        };
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        trader = GameObject.FindGameObjectWithTag("Trader").GetComponent<TraderController>();
        enemyPrefabs = new Dictionary<string, GameObject>() {
            { "Damned", (GameObject)Resources.Load("Damned") },
            { "Bat", (GameObject)Resources.Load("Bat") },
            { "Bloater", (GameObject)Resources.Load("Bloater") },
            { "Jumper", (GameObject)Resources.Load("Jumper") },
            { "Shield", (GameObject)Resources.Load("Shield") }
        };
    }

    // preparingWave - is spawning enemies
    // allStagesDead()
    // trader.isActivating()
    // trader.getActive()

	// Update is called once per frame
	void Update () {
        if (isPlaying) {
            timeSinceLastStageChange += Time.deltaTime;
            if (newWave) {
                timeSinceTraderDeath += Time.deltaTime;
                if (timeSinceTraderDeath > timeBetweenWaves) {
                    newWave = false;
                    timeSinceTraderDeath = 0f;
                    startNewWave();
                }
            }
            if (allStagesDead()
                && !trader.isAlive()
                && !preparingWave
                && !newWave) {
                trader.startActivation();
            }
            if (!waveCompositionGenerated) {
                generateNewWaveComposition();
            }
            manageCurrentWave();
        } else {
            killActiveActors();
        }
	}

    public void startNewWave() {
        currentWave++;
        currentStageInWave = 1;
        waveCompositionGenerated = false;
        preparingWave = true;
        timeSinceLastStageChange = 0;
        ui.updateWaveText(currentWave);
    }

    void manageCurrentWave() {
        switch (currentStageInWave) {
            case 1:
                //manageStage();
                checkChangeStage();
                break;
            case 2:
                manageStage("Damned");
                checkChangeStage();
                break;
            case 3:
                manageStage("Bat");
                checkChangeStage();
                break;
            case 4:
                manageStage("Bloater");
                checkChangeStage();
                break;
            case 5:
                manageStage("Jumper");
                checkChangeStage();
                break;
            case 6:
                manageStage("Shield");
                checkChangeStage();
                break;
            default:
                // Switch block defaults until the current wave is defeated
                if (preparingWave) {
                    preparingWave = false;
                }
                break;
        }
    }

    void manageStage(string enemyType) {
        if (spawnNewStage) {
            spawnStage(enemyType);
            spawnNewStage = false;
        }
        stageIsEmpty = (waveComposition[enemyType] == 0);
    }

    void checkChangeStage() {
        if (timeSinceLastStageChange > timeBetweenStages || stageIsEmpty) {
            timeSinceLastStageChange = 0;
            currentStageInWave++;
            spawnNewStage = true;
        }
    }

    void spawnStage(string enemyType) {
        int index;
        int enemyNum = waveComposition[enemyType];
        for (int i = 0; i < enemyNum; i++) {
            index = Random.Range(0, spawners.Count);
            spawners[index].setEnemyType(
                enemyPrefabs[enemyType]);
            spawners[index].spawnEnemy();
        }
    }

    void generateNewWaveComposition() {
        waveComposition["Damned"] = (int)Mathf.Ceil(currentWave * 10f);
        waveComposition["Bat"] = (int)Mathf.Ceil(currentWave * 3f);
        waveComposition["Bloater"] = (int)Mathf.Ceil(currentWave * 1f);
        waveComposition["Jumper"] = (int)Mathf.Ceil(currentWave * 1f);
        waveComposition["Shield"] = (int)Mathf.Floor(currentWave * 0.25f);
    }

    public int getCurrentStageInWave() {
        return currentStageInWave;
    }

    public List<ActorSpawnerController> getSpawners() {
        return spawners;
    }

    public List<ActorPoolController> getActorPools() {
        return actorPools;
    }

    public List<GameObject> getActiveActors() {
        List<GameObject> activeActors = new List<GameObject>();
        foreach (ActorPoolController actorPool in getActorPools()) {
            foreach (GameObject actor in actorPool.getActiveActors()) {
                activeActors.Add(actor);
            }
        }
        return activeActors;
    }

    void killActiveActors() {
        List<GameObject> activeActors = getActiveActors();
        foreach (GameObject actorObj in activeActors) {
            ActorInterface actor = actorObj.GetComponent<ActorInterface>();
            actor.removeHealth(actor.getHealth() + 1f);
        }
    }

    public bool allStagesDead() {
        List<ActorPoolController> actorPools = getActorPools();
        foreach (ActorPoolController actorPool in actorPools) {
            if (actorPool != null) {
                if (actorPool.getBoundActorNum() != 0
                    && actorPool.getActorType().GetType() != typeof(BloaterController)) {
                    return false;
                }
            }
        }
        return true;
    }
}
