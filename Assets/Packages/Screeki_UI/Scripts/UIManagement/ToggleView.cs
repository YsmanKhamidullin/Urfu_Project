using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleView : MonoBehaviour
{
    private Animator animator;
    private Toggle toggle;
    
    private static readonly int IsOn = Animator.StringToHash("isOn");

    // Start is called before the first frame update
    private void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggle);
        
        animator = GetComponent<Animator>();
        
        animator.SetBool(IsOn, toggle.isOn);
    }

    private void OnToggle(bool value)
    {
        animator.SetBool(IsOn, value);
    }
    
}
