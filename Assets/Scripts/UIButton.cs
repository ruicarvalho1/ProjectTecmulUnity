using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIInputReceiver))]
public class UIButton : Button
{
    private InputReciever receiver;

    protected override void Awake()
    {
        base.Awake();
        receiver = GetComponent<UIInputReceiver>();
        onClick.AddListener(() => receiver.OnInputRecieved());
    }

}
