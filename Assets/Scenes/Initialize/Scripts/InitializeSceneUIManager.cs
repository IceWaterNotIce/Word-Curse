using UnityEngine;
using UnityEngine.UIElements;

public class InitializeSceneUIManager : MonoBehaviour
{
    VisualElement root;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the root visual element
        root = GetComponent<UIDocument>().rootVisualElement;
        // Get the version label
        Label versionLabel = root.Q<Label>("LabelVersion");
        // Set the version label text
        Debug.Log("Version: " + Application.version);
        versionLabel.text = "Version: " + Application.version.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
