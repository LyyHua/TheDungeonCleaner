using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Skin
{
    public string skinName;
    public int skinPrice;
    public bool unlocked;
}

public class UI_SkinSelection : MonoBehaviour
{
    [SerializeField] private Skin[] skins;
    
    [Header("UI details")]
    [SerializeField] private int currentIndex;
    [SerializeField] private GameObject menuCharacter;

    [SerializeField] private TextMeshProUGUI buySelectText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI bankText;

    [Header("UI Preview")]
    [SerializeField] private Image previewImageRenderer;
    [SerializeField] private Sprite[] skinSprites;

    private void Start()
    {
        if (SkinManager.instance != null)
        {
            currentIndex = SkinManager.instance.GetSkinId();
        }
        LoadSkinUnlocks();
        UpdateSkinDisplay();
    }

    private void LoadSkinUnlocks()
    {
        for (int i = 0; i < skins.Length; i++)
        {
            string skinName = skins[i].skinName;
            bool skinUnlocked = PlayerPrefs.GetInt(skinName + "Unlocked", 0) == 1;
            
            if (skinUnlocked || i == 0)
                skins[i].unlocked = true;
        }
    }

    public void SelectSkin()
    {
        if (skins[currentIndex].unlocked == false)
            BuySkin(currentIndex);
        else if (SkinManager.instance != null)
            SkinManager.instance.SetSkinId(currentIndex);
        
        AudioManager.instance.PlaySFX(6);
        UpdateSkinDisplay();
    }

    public void NextSkin()
    {
        currentIndex++;
        if (currentIndex >= skinSprites.Length)
            currentIndex = 0;

        AudioManager.instance.PlaySFX(6);
        UpdateSkinDisplay();
    }

    public void PreviousSkin()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = skinSprites.Length - 1;

        AudioManager.instance.PlaySFX(6);
        UpdateSkinDisplay();
    }

    private void UpdateSkinDisplay()
    {
        bankText.text = "Bank: " + MoneyInBank();
        
        if (previewImageRenderer != null && currentIndex >= 0 && currentIndex < skinSprites.Length)
        {
            previewImageRenderer.sprite = skinSprites[currentIndex];
        }
        
        if (menuCharacter != null)
        {
            var characterSkin = menuCharacter.GetComponent<MenuCharacterSkin>();
            if (characterSkin != null)
            {
                characterSkin.PreviewSkin(currentIndex);
            }
        }

        if (skins[currentIndex].unlocked)
        {
            priceText.transform.parent.gameObject.SetActive(false);
            buySelectText.text = "Select";
        }
        else
        {
            priceText.transform.parent.gameObject.SetActive(true);
            priceText.text = "Price: " + skins[currentIndex].skinPrice;
            buySelectText.text = "Buy";
        }
    }

    private void BuySkin(int index)
    {
        if (HaveEnoughMoney(skins[index].skinPrice) == false)
        {
            AudioManager.instance.PlaySFX(0);
            return;
        }
        
        AudioManager.instance.PlaySFX(2);
        string skinName = skins[currentIndex].skinName;
        skins[currentIndex].unlocked = true;
        
        SkinManager.instance.SetSkinId(index);
        PlayerPrefs.SetInt(skinName + "Unlocked", 1);
    }

    private int MoneyInBank() => PlayerPrefs.GetInt("MoneyInBank");

    private bool HaveEnoughMoney(int price)
    {
        if (MoneyInBank() >= price)
        {
            PlayerPrefs.SetInt("MoneyInBank", MoneyInBank() - price);
            return true;
        }

        return false;
    }
}