using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour {

    public KeyCode Menukey = KeyCode.Escape;
    private PlayerUI playerUI;
    private Player player;
    [Header("Player Movement")]
    public KeyCode Forward = KeyCode.W;
    public KeyCode Backward = KeyCode.S;
    public bool LeftHeld = false;
    public KeyCode Left = KeyCode.A;
    public bool RightHeld = false;
    public KeyCode Right = KeyCode.D;
    public KeyCode JumpKey = KeyCode.Space;
    public bool JumpDown = false;
    public bool JumpUp = false;
    public KeyCode SprintKey = KeyCode.LeftShift;

    [Header("Player Interaction")]
    public KeyCode AttackKey = KeyCode.Q;

    public int VerticallMotion { get { return Input.GetKey(Backward) ? Input.GetKey(Forward) ? 0 : -1 : Input.GetKey(Forward) ? 1 : 0; } }
    public int HorizontalMotion { get { return Input.GetKey(Left) || LeftHeld ? Input.GetKey(Right) || RightHeld ? 0 : -1 : Input.GetKey(Right) || RightHeld ? 1 : 0; } }
    public bool JumpKeyDown { get { return Input.GetKeyDown(JumpKey) || JumpDown; } }
    public bool JumpKeyHeld { get { return Input.GetKey(JumpKey); } }
    public bool JumpKeyUp { get { return Input.GetKeyUp(JumpKey) || JumpUp; } }
    public bool Sprint { get { return Input.GetKey(SprintKey); } }

    void Start()
    {
        player = FindObjectOfType<Player>();
        playerUI = FindObjectOfType<PlayerUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(Menukey))
        {
            MenuPressed();
        }
        if(Input.GetKeyDown(AttackKey))
        {
            AttackPressed();
        }
    }

    public void MenuPressed()
    {
        playerUI.MenuPressed();
    }

    public void AttackPressed()
    {
        player.Attack();
    }

    public void SetLeft(bool set)
    {
        LeftHeld = set;
    }

    public void SetRight(bool set)
    {
        RightHeld = set;
    }

    public void SetJumpDown() {
        JumpDown = true;
    }

    public void SetJumpUp()
    {
        JumpUp = true;
    }

    public void ResetJump()
    {
        JumpDown = false;
        JumpUp = false;
    }
}
