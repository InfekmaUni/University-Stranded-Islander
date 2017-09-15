using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/*Author: Ka Ming Li*/
public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject Overview;

    // shows window with held resources on hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        Overview.SetActive(true);
    }

    // hides window on exit
    public void OnPointerExit(PointerEventData eventData)
    {
        Overview.SetActive(false);
    }
}
