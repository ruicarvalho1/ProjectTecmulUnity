using UnityEngine;
using UnityEngine.Events;

public class UIInputReceiver : InputReciever
{
    [SerializeField] private UnityEvent clickEvent;

    public override void OnInputRecieved()
    {
        foreach (var handler in inputHandlers)
        {
            handler.ProcessInput(Input.mousePosition, gameObject, () => clickEvent.Invoke());
        }
    }

}
