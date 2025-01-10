using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MessageWindow : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<MessageWindow> { }

    
    private const string styleResource = "MessageWindowStyleSheet";
    private const string ussMessageWindow = "message_window";
    private const string ussMessageContainer = "message_container";
    private const string ussHorContainer = "horizontal_container";
    private const string ussMessageLabel = "message_label";
    private const string ussMessageButton = "message_button";
    private const string ussBtnCancel = "btn_cancel";
    private const string ussBtnConfirm = "btn_confirm";
    public MessageWindow()
    {
        styleSheets.Add(Resources.Load<StyleSheet>(styleResource));
        AddToClassList(ussMessageContainer);

        VisualElement window = new VisualElement();
        window.AddToClassList(ussMessageWindow);
        hierarchy.Add(window);

        // Text
        VisualElement horizontalContainerText = new VisualElement();
        horizontalContainerText.AddToClassList(ussHorContainer);
        window.Add(horizontalContainerText);
        
        Label messageLabel = new Label();
        messageLabel.text = "Hello World!";
        messageLabel.AddToClassList(ussMessageLabel);
        horizontalContainerText.Add(messageLabel);

        // Button
        VisualElement horizontalContainerButton = new VisualElement();
        horizontalContainerButton.AddToClassList(ussHorContainer);
        window.Add(horizontalContainerButton);

        Button btnCancel = new Button(){ text = "Cancel" };
        btnCancel.AddToClassList(ussMessageButton);
        btnCancel.AddToClassList(ussBtnCancel);
        horizontalContainerButton.Add(btnCancel);

        Button btnConfirm = new Button(){ text = "Confirm" };
        btnConfirm.AddToClassList(ussMessageButton);
        btnConfirm.AddToClassList(ussBtnConfirm);
        horizontalContainerButton.Add(btnConfirm);

        btnCancel.clicked += OnCancel;
        btnConfirm.clicked += OnConfirm;
    }

    public event Action confirmed;
    public event Action cancelled;

    private void OnConfirm()
    {
        confirmed?.Invoke();
    }

    private void OnCancel()
    {
        cancelled?.Invoke();
    }
}
