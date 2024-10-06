using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerUI : MonoBehaviour {

    private GameState lastGameState;

    [SerializeField] private Animator animator = default;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Renderer playerRenderer;

    [SerializeField] private GameObject uiInterface;

    public void MenuPressed() {
        if (GameManager.Instance.gameState == GameState.Paused) {
            GameManager.Instance.gameState = lastGameState;
            animator.SetBool("pause", false);
            playerAnimator.speed = 1;
            GameManager.Instance.ToggleCursorVisibility(false);
        }
        else if (!GameManager.Instance.inTitleScreen) {
            lastGameState = GameManager.Instance.gameState;
            GameManager.Instance.gameState = GameState.Paused;
            animator.gameObject.SetActive(true);
            animator.SetBool("pause", true);
            playerAnimator.speed = 0;
            GameManager.Instance.ToggleCursorVisibility(true);
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && GameManager.Instance.gameState != GameState.Regular)
        {
            Quit();
        }
    }

    public void StartGame()
    {
        StartCoroutine(StartGameOverTime());
    }

    private IEnumerator StartGameOverTime()
    {
        animator.SetTrigger("toGame");
        yield return new WaitForSeconds(1);
        GameManager.Instance.StartGame();
        yield return new WaitForSeconds(1);
        animator.SetTrigger("inGame");
        uiInterface.SetActive(true);

    }

    public void ToUpgrades()
    {
        animator.SetBool("upgrades", true);
    }

    public void ToHome()
    {
        animator.SetBool("home", true);
        animator.SetBool("upgrades", false);
    }

    public void ReturnToHome()
    {
        StartCoroutine(ReturnToHomeOverTime());
    }

    private IEnumerator ReturnToHomeOverTime()
    {
        animator.SetTrigger("toHome");
        yield return new WaitForSeconds(1);
        GameManager.Instance.ReturnToHome();
        yield return new WaitForSeconds(1);
        animator.SetTrigger("inHome");

    }

    public void Quit()
    {
        GameManager.Instance.FullGameQuit();
    }

}
