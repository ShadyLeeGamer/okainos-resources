using MoreMountains.Feedbacks;
using UnityEngine;

public class BoosterChest : MonoBehaviour
{
    public Texture2D[] Steps;
    public Material TreasureMat;
    public int index;
    public Animator animator;
    public GameObject showCards;
    public MMFeedbacks[] KnockImpact;
    BoosterSystem system;

    public void Start()
    {
        system = BoosterSystem.Instance;

        animator = GetComponent<Animator>();
        TreasureMat.mainTexture = Steps[0];
        index = 0;
    }
    public void OnDisable()
    {
        TreasureMat.mainTexture = Steps[0];
        index = 0;
    }
    public void OnMouseDown()
    {
        if (index < 3)
        {
            KnockImpact[0].PlayFeedbacks();
            index++;
            TreasureMat.mainTexture = Steps[index];
        }if (index == Steps.Length - 1)
        {
            KnockImpact[1].PlayFeedbacks();
            animator.SetTrigger("Open");
            index++;
            showCards.SetActive(true);
            system.StartCoroutine(system.OpenBooster(0));
        }

    }
}
