using UnityEngine;
using UnityEngine.UI;

public class UIPatrolZone : MonoBehaviour
{
    public GameObject alertUI;

    public void ShowAlert()
    {
        alertUI.SetActive(true);
    }

    public void HideAlert()
    {
        alertUI.SetActive(false);
    }
}
