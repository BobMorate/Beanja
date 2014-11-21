using UnityEngine;
using System.Collections;

public class CloudDestroyer : MonoBehaviour {
	public float lastingTime;
	public float particleStopBeforeRemove;

	// Use this for initialization
	void Start () {
		Invoke("StopParticles", lastingTime - particleStopBeforeRemove);
		Destroy(gameObject, lastingTime);
	}
	void StopParticles()
	{
		particleSystem.Stop();
	}
	

}
