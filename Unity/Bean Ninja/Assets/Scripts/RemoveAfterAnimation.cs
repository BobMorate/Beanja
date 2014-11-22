using UnityEngine;
using System.Collections;

public class RemoveAfterAnimation : MonoBehaviour {
	public Animation anim;
	// Use this for initialization
	void Start () {
		if(anim == null)
			anim = animation;
	}
	
	// Update is called once per frame
	void Update () {
		if(!anim.isPlaying)
			Destroy(gameObject);
	}
}
