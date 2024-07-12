using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialTwoHits : TwoHitDrone, ITutorial
{
    private TutorialManager manager;

    public void HandleFailedHit()
    {
        manager.NotEnoughForceToDestroy();
    }

    public override void HandleHit()
    {
        base.HandleHit();
        if(destroy)
        {
            Debug.Log("Reached");
            manager.OnDroneDestroyed();
        }
    }

    public void SetManager(TutorialManager manager)
    {
        this.manager = manager;
    }

    protected override void Start()
    {
        SpawnAnimationTime = 3.5f;
        tutorialDrone = true;
        base.Start();
    }
}
