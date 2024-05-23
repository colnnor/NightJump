using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class UIButtonManager : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    public List<GameObject> Buttons;

    public GameObject LastSelecterd { get; set; }
    public int LastSelectedIndex { get; set; }

    public GameObject CurrentSelected { get; set; }
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<UIButtonManager>(this);
    }
    private void Start()
    {
        inputReader.OnNavigate += OnNavigate;
    }

    private void OnNavigate(Vector2 direction)
    {
        if(CurrentSelected) return;

        if (direction == Vector2.up && LastSelectedIndex > 0)
        {
            LastSelectedIndex--;
            EventSystem.current.SetSelectedGameObject(Buttons[LastSelectedIndex]);
        }
        else if (direction == Vector2.down && LastSelectedIndex < Buttons.Count - 1)
        {
            LastSelectedIndex++;
            EventSystem.current.SetSelectedGameObject(Buttons[LastSelectedIndex]);
        }
    }

    private void OnEnable()
    {
        inputReader.SwitchActionMap(inputReader.GetInputActions.UI);
        StartCoroutine(SetSelected());
    }

    private void OnDisable()
    {
        inputReader.SwitchActionMap(inputReader.GetInputActions.Player);
    }

    public IEnumerator SetSelected()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(Buttons[0]);
        Debug.Log(EventSystem.current.currentSelectedGameObject);
    }
}
