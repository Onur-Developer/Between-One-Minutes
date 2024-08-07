using UnityEngine;

public class JoystickNullCheck : MonoBehaviour
{
    public MainPlayer mainPlayer;

    private void OnEnable()
    {
        if (GameManager.instance != null)
            GameManager.instance.isJoyStick = true;
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
        {
            if(mainPlayer!=null)
                mainPlayer.StopPlayer();
            GameManager.instance.isJoyStick = false;
        }
    }
}
