using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimationHandler : MonoBehaviour
{

    private Animator anim;
    [SerializeField] private Renderer _renderer;
    public bool animFinished = false;
    
    void Awake()
    {
        // needs to be set before room moves the hand on start
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

    public void SetVisible(bool visible)
    {
        _renderer.enabled = visible;
    }

}
