using System.Collections;
using Assets.Scripts.UI.LoadScreen;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public enum InputDevice
    {
        Mouse_Keyboard,
        Controller
    }

    public class GlobalUIController : MonoBehaviour
    {
        public GameObject eventSystem;
        public GameObject loadingScreen;

        public GameObject graphicsUI;
        public GameObject optionsUI;
        public GameObject pauseUI;
        public GameObject cameraFader;
        public GameObject playerHUD;
        public GameObject fpsCounter;
        public GameObject joystickConnectUI;

        [Header("Boot Settings")]
        [Tooltip("Should the given scene name below be loaded on start?")]
        public bool bootOnLoad = true;
        public string bootSceneName = string.Empty;

        // The current input device being used by the player (Based on last input).
        private InputDevice currentInputDevice = InputDevice.Controller;

        public InputDevice CurrentInputDevice
        {
            get
            {
                return currentInputDevice;
            }
        }

        private int currentJoystickCount;
        private string loadedScene;

        private void Start()
        {
            SceneManager.sceneLoaded -= OnNewSceneLoaded;
            SceneManager.sceneLoaded += OnNewSceneLoaded;

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(graphicsUI);
            DontDestroyOnLoad(loadingScreen);
            DontDestroyOnLoad(eventSystem);
            DontDestroyOnLoad(playerHUD);
            DontDestroyOnLoad(optionsUI);
            DontDestroyOnLoad(pauseUI);
            DontDestroyOnLoad(cameraFader);
            DontDestroyOnLoad(fpsCounter);
            DontDestroyOnLoad(joystickConnectUI);

            if (bootOnLoad)
            {
                LoadLevel(bootSceneName);
            }
        }

        public void LoadLevel(string _levelName)
        {
            loadingScreen.SetActive(true);
            loadingScreen.GetComponentInChildren<LoadingScreenController>().StartAsyncLoad(_levelName);
        }

        private void OnNewSceneLoaded(Scene _sceneLoaded, LoadSceneMode _loadMode)
        {
            loadingScreen.SetActive(false);

            playerHUD.SetActive(_sceneLoaded.name != "MainMenu");
            var fadeCol = cameraFader.GetComponentInChildren<Image>().color;
            fadeCol.a = 0.0f;
            cameraFader.GetComponentInChildren<Image>().color = fadeCol;
        }

        private int joysticksLastFrame;

        private void Update()
        {
            currentJoystickCount = Input.GetJoystickNames().Length;

            if (currentJoystickCount > joysticksLastFrame)
            {
                StopAllCoroutines();
                StartCoroutine(NotifyJoystickConnected());
            }
            else if (currentJoystickCount < joysticksLastFrame)
            {
                StopAllCoroutines();
                StartCoroutine(NotifyJoystickDisconnected());
            }

            joysticksLastFrame = currentJoystickCount;

            Cursor.visible = currentInputDevice == InputDevice.Mouse_Keyboard;

            if (Input.GetKeyDown(KeyCode.F1))
            {
                fpsCounter.SetActive(!fpsCounter.activeSelf);
            }

            loadedScene = SceneManager.GetActiveScene().name;
            if (loadedScene.Equals("MainMenu") || loadedScene.Equals("CreditsScene"))
            {
                playerHUD.SetActive(false);
            }
        }

        private IEnumerator NotifyJoystickConnected()
        {
            yield return null;
        }

        private IEnumerator NotifyJoystickDisconnected()
        {
            yield return null;
        }

        private void OnGUI()
        {
            // Check if last input was a keyboard click or mouse click.
            if (Event.current.isKey || Event.current.isMouse)
            {
                //Debug.Log("Mouse/Keyboard input");
                currentInputDevice = InputDevice.Mouse_Keyboard;
                return;
            }

            // Check if latest input was a mouse move event.
            if (Input.GetAxis("Mouse X") != 0.0f || Input.GetAxis("Mouse Y") != 0.0f)
            {
                //Debug.Log("Mouse move input");
                currentInputDevice = InputDevice.Mouse_Keyboard;
                return;
            }

            // Check if input was a joystick button.
            if (Input.GetKey(KeyCode.Joystick1Button0) ||
                Input.GetKey(KeyCode.Joystick1Button1) ||
                Input.GetKey(KeyCode.Joystick1Button3) ||
                Input.GetKey(KeyCode.Joystick1Button4) ||
                Input.GetKey(KeyCode.Joystick1Button2) ||
                Input.GetKey(KeyCode.Joystick1Button5) ||
                Input.GetKey(KeyCode.Joystick1Button6) ||
                Input.GetKey(KeyCode.Joystick1Button7) ||
                Input.GetKey(KeyCode.Joystick1Button8) ||
                Input.GetKey(KeyCode.Joystick1Button9) ||
                Input.GetKey(KeyCode.Joystick1Button10) ||
                Input.GetKey(KeyCode.Joystick1Button11) ||
                Input.GetKey(KeyCode.Joystick1Button12) ||
                Input.GetKey(KeyCode.Joystick1Button13) ||
                Input.GetKey(KeyCode.Joystick1Button14) ||
                Input.GetKey(KeyCode.Joystick1Button15) ||
                Input.GetKey(KeyCode.Joystick1Button16) ||
                Input.GetKey(KeyCode.Joystick1Button17) ||
                Input.GetKey(KeyCode.Joystick1Button18) ||
                Input.GetKey(KeyCode.Joystick1Button19))
            {
                //Debug.Log("Controller button input");
                currentInputDevice = InputDevice.Controller;
                return;
            }

            // Check if input was the left joystick moving.
            if (Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f)
            {
                //Debug.Log("Controller left joy input");
                currentInputDevice = InputDevice.Controller;
                return;
            }

            if (Input.GetAxis("DPadX") != 0.0f || Input.GetAxis("DPadY") != 0.0f)
            {
                //Debug.Log("Controller DPAD input.");
                currentInputDevice = InputDevice.Controller;
            }
        }
    }
}
