using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ActorInterface {
	void createActor(GameObject parent);
    void freeActor();
    bool isFree();
    bool isProtected();
    void setProtected(bool isProtected);
    void setPosition(Vector3 newPosition);
    void setRenderingOrder(int position);
    float getHealth();
    void removeHealth(float damage);
}
