using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialExplosive : ExplosiveDrone, ITutorial
{
    private TutorialManager manager;

    public void HandleFailedHit()
    {
        Debug.Log("??");
    }

    public override void HandleHit()
    {
        base.HandleHit();
        manager.OnDroneDestroyed();
    }

    public void SetManager(TutorialManager manager)
    {
        this.manager = manager;
    }

    protected override void Start()
    {
        SpawnAnimationTime = 3f;
        tutorialDrone = true;
        base.Start();
    }
}
