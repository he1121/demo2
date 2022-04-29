using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioSource : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WalkAudioPlay()
    {
        AudioManager.PlayWalkAudio();
    }
    public void JumpAudioPlay()
    {
        AudioManager.PlayJumpAudio();
    }
    public void FallAudioPlay()
    {
        AudioManager.PlayFallAudio();
    }

    public void DoubleJumpAudioPlay()
    {
        AudioManager.PlayDoubleJumpAudio();
    }

    public void AttackAudioPlay()
    {
        AudioManager.PlayAttackJumpAudio();
    }

}
