using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] GameObject interactionButton;
    [SerializeField] PlayerInteractor playerInteract;

    void Start()
    {
        Hide();
    }
    public void Show(){
        interactionButton.SetActive(true);
    } 

    public void Hide()
    {
        interactionButton.SetActive(false);
    }
}
