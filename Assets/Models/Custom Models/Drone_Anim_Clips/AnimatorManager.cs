using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{

    private Animator animator;
    private int velocityHash;
    private int stunnedHash;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        velocityHash = Animator.StringToHash("Velocity");
        stunnedHash = Animator.StringToHash("Stunned");
    }

    // Update is called once per frame
    void Update()
    {
        float velocity = GetComponentInParent<BaseDroneController>().Velocity;
        animator.SetFloat(velocityHash, velocity);
    }

    public void PlayStunnedAnimation()
    {
        animator.SetTrigger(stunnedHash);
    }
}
