using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SkillMutationTooltip : MonoBehaviour
{
    private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;


    private void Awake()
    {
        root = gameObject;
        Hide();
    }

    public void Show(MutationDataSO mutation, bool isUnlocked, RectTransform target)
    {
        if (mutation == null)  return;
        if (titleText != null) titleText.text = isUnlocked ? mutation.mutationName : "???";
        if (descriptionText != null) descriptionText.text = isUnlocked ? mutation.unlockedDescription : mutation.lockedDescription;

        root.SetActive(true);
        RectTransform rect = (RectTransform)transform;
        rect.position = target.position + Vector3.right * (target.rect.width * 0.5f);
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
    }

}
