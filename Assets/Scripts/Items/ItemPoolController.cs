using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPoolController : MonoBehaviour {

    PowerUp mainPowerup;
    PowerUp backupPowerup;

    TraderController trader;

    List<string> powerupTypes = new List<string> {
        "Damage", "SlowMotion", "Invincibility"
    };

	// Use this for initialization
	void Awake () {
        mainPowerup = new PowerUp();
        backupPowerup = new PowerUp();

        trader = GameObject.FindGameObjectWithTag("Trader").GetComponent<TraderController>();

        //mainPowerup.setType(powerupTypes[0]);
        //backupPowerup.setType(powerupTypes[0]);
    }
	
    public PowerUp pickUpPowerup() {
        trader.startDeactivation();
        swapPowerups();
        return mainPowerup;
    }

    void swapPowerups() {
        PowerUp temp = backupPowerup;
        backupPowerup = mainPowerup;
        mainPowerup = temp;
    }

    public PowerUp getMainPowerup() {
        return mainPowerup;
    }

    public PowerUp getBackupPowerup() {
        return backupPowerup;
    }

    public string createPowerup() {
        string type = powerupTypes[Random.Range(0, powerupTypes.Count)];
        backupPowerup.setType(type);
        return type;
    }
}
