using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderChildItemController : MonoBehaviour {

    TraderController parent;
    string type;
    
    void Start() {
        parent = transform.parent.GetComponent<TraderController>();
        type = gameObject.name;
    }

	void OnTriggerEnter2D(Collider2D col) {
        if (col.transform.parent.tag != "Player" || !parent.getActive()) {
            return;
        }
        switch (type) {
            case "Powerup":
                parent.OnCollisionEnter2DPowerup(col);
                break;
            case "Health":
                parent.OnCollisionEnter2DHealth(col);
                break;
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.transform.parent.tag != "Player" || !parent.getActive()) {
            return;
        }
        switch (type) {
            case "Powerup":
                parent.OnCollisionExit2DPowerup(col);
                break;
            case "Health":
                parent.OnCollisionExit2DHealth(col);
                break;
        }
    }
}
