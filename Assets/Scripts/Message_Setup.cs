using UnityEngine;
using UnityEngine.UIElements;

public class MessageSetup : MonoBehaviour
{
    private void OnEnable()
    {
        UIDocument uiDoc = GetComponent<UIDocument>();
        VisualElement root = uiDoc.rootVisualElement;

        MessageWindow messageWindow = new MessageWindow();
        root.Add(messageWindow);

        messageWindow.confirmed += () => Debug.Log("Confirmed");
        messageWindow.cancelled += () => Debug.Log("Cancelled");
        messageWindow.confirmed += () => root.Remove(messageWindow);
        messageWindow.cancelled += () => root.Remove(messageWindow);

        root.Add(messageWindow);
    }
}