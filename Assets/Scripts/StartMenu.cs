using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{

    public void changeScene(string sceneName) {
        Application.LoadLevel(sceneName);
    }


    public void startGame() {
        Debug.Log("Starting game...");
        // TODO call the scene game
        changeScene("game");
    }

}
