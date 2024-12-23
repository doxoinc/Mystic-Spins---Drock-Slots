using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMagicSlots : MonoBehaviour
{


    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
