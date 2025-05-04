using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[System.Serializable]
public struct Skin
{
    public string skinName;
    public int skinPrice;
    public bool unlocked;
}

public class UI_SkinSelection : MonoBehaviour
{
    private UI_LevelSelection levelSelectionUI;
    private UI_MainMenu mainMenuUI;
    [SerializeField] private Skin[] skinList;

    [Header("UI details")]
    [SerializeField] private int skinIndex;
    [SerializeField] private int maxIndex;
    [SerializeField] private Animator skinDisplay;

    [SerializeField] private TextMeshProUGUI buySelectText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI bankText;

    private void Start()
    {
        LoadSkinUnlocks();
        UpdateSkinDisplay();

        mainMenuUI = GetComponentInParent<UI_MainMenu>();
        levelSelectionUI = mainMenuUI.GetComponentInChildren<UI_LevelSelection>(true);
    }

    private void LoadSkinUnlocks()
    {
        for (int i = 0; i < skinList.Length; i++)
        {
            string skinName = skinList[i].skinName;
            bool skinUnlocked = PlayerPrefs.GetInt(skinName + "Unlocked", 0) == 1;

            if(skinUnlocked || i ==0)
                skinList[i].unlocked = true;
        }
    }

    public void SelectSkin()
    {
        if (skinList[skinIndex].unlocked == false)
            BuySkin(skinIndex);
        else
        {
            SkinManager.instance.SetSkinId(skinIndex);
            mainMenuUI.SwitchUI(levelSelectionUI.gameObject);
        }

        AudioManager.instance.PlaySFX(4);
        UpdateSkinDisplay();
    }

    public void NextSkin()
    {
        skinIndex++;

        if (skinIndex > maxIndex)
            skinIndex = 0;

        AudioManager.instance.PlaySFX(4);
        UpdateSkinDisplay();
    }


    public void PreviousSkin()
    {
        skinIndex--;

        if (skinIndex < 0)
            skinIndex = maxIndex;

        AudioManager.instance.PlaySFX(4);

        UpdateSkinDisplay();
    }

    private
        void UpdateSkinDisplay()
    {
        bankText.text = "Bank: " + FruitsInBank();
        for (int i = 0; i < skinDisplay.layerCount; i++)
        {
            skinDisplay.SetLayerWeight(i, 0);
        }

        skinDisplay.SetLayerWeight(skinIndex, 1);

        if (skinList[skinIndex].unlocked)
        {
            priceText.transform.parent.gameObject.SetActive(false);
            buySelectText.text = "Select";
        }
        else
        {
            priceText.transform.parent.gameObject.SetActive(true);
            priceText.text = "Price: " + skinList[skinIndex].skinPrice;
            buySelectText.text = "Buy";
        }
    }

    private void BuySkin(int index)
    {
        if (HaveEnoughFruits(skinList[index].skinPrice) == false)
        {
            AudioManager.instance.PlaySFX(6);
            Debug.Log("Yeterli Meyve Yok");
            return;
        }

        AudioManager.instance.PlaySFX(10);
        string skinName = skinList[skinIndex].skinName;
        skinList[skinIndex].unlocked = true;

        PlayerPrefs.SetInt(skinName + "Unlocked", 1);
    }

    private int FruitsInBank() => PlayerPrefs.GetInt("TotalFruitsAmount");

    private bool HaveEnoughFruits(int price)
    {
        if (FruitsInBank() > price)
        {
            PlayerPrefs.SetInt("TotalFruitsAmount", FruitsInBank() - price);
            return true;
        }
        return false;
    }

}
