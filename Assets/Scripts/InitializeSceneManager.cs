using UnityEngine;
using UnityEngine.SceneManagement;

namespace InternetEmpire
{
    public class InitializeSceneManager : MonoBehaviour
    {
        public bool isNeedNetwork = false;
        public bool isAssetBundleReady = false;
        public bool isVersionChecked = false;

        void Start()
        {
            CheckNetwork();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Check()
        {
            if (isAssetBundleReady && isVersionChecked)
            {
                // Load the lobby scene
                SceneManager.LoadScene("Lobby");
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        void CheckNetwork()
        {
            if (isNeedNetwork)
            {
                // Check network
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    // Show network error panel
                    MessageManager.Instance.CreateCloseMessage("No network connection. Please check your network settings first.", QuitGame);
                }
            }
        }


    }
}