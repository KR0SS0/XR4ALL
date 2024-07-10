using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOneHit : OneHitDrone, ITutorial
{
    private TutorialManager manager;

    public override void HandleHit()
    {
        base.HandleHit();
        manager.OnDroneDestroyed();
    }

    public void HandleFailedHit()
    {
        manager.NotEnoughForceToDestroy();
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
