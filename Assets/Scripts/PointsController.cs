using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsController : MonoBehaviour {

    PlayerController parentPlayer;
    UIController ui;

    int points;

	// Use this for initialization
	void Start () {
        parentPlayer = transform.parent.gameObject.GetComponent<PlayerController>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        points = 0;

    }
	
    public void killedEnemy() {
        points += 10;
        ui.updatePointsText(points);
    }
    
	// Update is called once per frame
	void Update () {
        
	}
}
