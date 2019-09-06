using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingLine : MonoBehaviour {

    Transform shieldPos;
    Transform targetPos;

    bool changeShieldPosTrigger;
    bool changeTargetPosTrigger;

    LineRenderer lineRend;

	// Use this for initialization
	void Awake () {
        lineRend = GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	void Update () {
		if (changeShieldPosTrigger) {
            changeShieldPosTrigger = false;
            lineRend.SetPosition(0, shieldPos.position);
        }
        if (changeTargetPosTrigger) {
            changeTargetPosTrigger = false;
            lineRend.SetPosition(1, targetPos.position);
        }
    }

    public void setShieldPos(Transform newShieldPos) {
        shieldPos = newShieldPos;
        changeShieldPosTrigger = true;
    }

    public void setTargetPos(Transform newTargetPos) {
        targetPos = newTargetPos;
        changeTargetPosTrigger = true;
    }

    public void disable() {
        lineRend.enabled = false;
    }

    public void enable() {
        lineRend.enabled = true;
    }
}
