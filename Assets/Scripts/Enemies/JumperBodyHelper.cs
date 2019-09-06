using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperBodyHelper : MonoBehaviour {

    JumperController parent;
    
	void Start () {
        parent = transform.parent.GetComponent<JumperController>();
	}
	
	void setFire() {
        parent.setFire();
    }

    void setTeleport() {
        parent.setTeleport();
    }
}
