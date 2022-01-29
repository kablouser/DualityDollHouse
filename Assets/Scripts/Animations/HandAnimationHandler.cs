using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimationHandler : MonoBehaviour
{

    private Animator anim;
    public bool animFinished = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //play animation and wait for it to finish
    public void PlayAnimSynchronous(string key)
    {
        anim.Play(key, 0, .0f);
        animFinished = false;
    }

    public void FinishHandAnimation(int i)
    {
        animFinished = true;
    }

}
