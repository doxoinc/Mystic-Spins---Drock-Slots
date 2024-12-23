// ConfirmationPanelController.cs
using UnityEngine;

public class ConfirmationPanelController : MonoBehaviour
{
    // Метод, вызываемый через Animation Event после завершения анимации открытия панели
    public void OnOpenAnimationComplete()
    {
        Debug.Log("Анимация открытия панели завершена.");
        // Дополнительные действия после открытия панели (если необходимо)
    }

    // Метод, вызываемый через Animation Event после завершения анимации закрытия панели
    public void OnCloseAnimationComplete()
    {
        Debug.Log("Анимация закрытия панели завершена.");
        if (DailyRewardManager.Instance != null)
        {
            DailyRewardManager.Instance.OnConfirmationPanelCloseComplete();
        }
        else
        {
            Debug.LogError("DailyRewardManager.Instance не установлен.");
        }
    }
}
