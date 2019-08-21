using System.Collections;
using UnityEngine.UI;
using System.Text;
using UnityEngine;

public class ConsoleView : MonoBehaviour
{
    ConsoleController Console = new ConsoleController();
    bool DidShow = false;

    public GameObject ViewContainer;
    public Text LogTextArea;
    public InputField InputField;

    private void Start()
    {
        if(Console != null)
        {
            Console.VisibilityChanged += OnVisibilityChanged;
            Console.LogChanged += OnLogChanged;
        }
        UpdateLogString(Console.Log);
    }

    ~ConsoleView()
    {
        Console.VisibilityChanged -= OnVisibilityChanged;
        Console.LogChanged -= OnLogChanged;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.C))
        {
            ToggleVisibility();
            DidShow = true;
        }
        else
        {
            DidShow = false;
        }
        
    }

    void ToggleVisibility()
    {
        SetVisibility(!ViewContainer.activeSelf);
        InputField.ActivateInputField();
    }

    void SetVisibility(bool visible)
    {
        ViewContainer.SetActive(visible);
    }

    void OnVisibilityChanged(bool visible)
    {
        SetVisibility(visible);
    }

    void OnLogChanged(string[] NewLog)
    {
        UpdateLogString(NewLog);
    }

    void UpdateLogString(string[] NewLog)
    {
        if(NewLog == null)
        {
            LogTextArea.text = "";
        }
        else
        {
            LogTextArea.text = string.Join("\n", NewLog);
        }
    }

    public void RunCommand()
    {
        Console.RunCommandString(InputField.text);
        InputField.text = "";
        InputField.ActivateInputField();
    }
}
