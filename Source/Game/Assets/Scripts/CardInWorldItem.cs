using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using LiquidUI;
using System;

public class CardInWorldItem : CardInGroupItem
{
    Animator animator;
    BoosterSystem.RarityChance.EFX EFX;

    [SerializeField] IllustrationShaderConverter illustration;
    [SerializeField] MeshRenderer layout;
    [SerializeField] MeshRenderer familyIcon;

    [SerializeField] MeshRenderer[] layoutRenderers;

    [SerializeField] float revealAnimationSpeed;

    Vector3 Scale => transform.lossyScale;

    public void Initialise(CardAsset.Data cardData, Type itemType, BoosterSystem.RarityChance.EFX EFX)
    {
        this.EFX = EFX;

        Initialise(cardData, itemType); 
    }

    public override void Initialise(CardAsset.Data cardData, Type itemType)
    {
        base.Initialise(cardData, itemType);

        RefreshDistortionRenderersToRelativeScale();

        CardResources cardResources = CardResourcesManager.GetCardResources(cardData.id);
        illustration.Refresh(cardResources.BasicMaterial);
        familyIcon.material = new Material(cardResources.FamilyMaterial);
        familyIcon.material.SetTexture("_MainTex", (Texture2D)Resources.Load("Family Icons/"+cardData.familly + " Icon"));
        layout.material = new Material(cardResources.CardRarityResources.CardLayout.Material);
    }

    [ContextMenu("RefreshDistortionRenderersToRelativeScale")]
    public void RefreshDistortionRenderersToRelativeScale()
    {
        foreach (MeshRenderer distortionRenderer in layoutRenderers)
            foreach (Material distortionMat in distortionRenderer.sharedMaterials)
                if (distortionMat.HasProperty("_Distortion"))
                {
                    Material distortionMatRelative = new Material(distortionMat);
                    Vector4 relativeDistortion = distortionMatRelative.GetVector("_Distortion_Default");
                    relativeDistortion.x /= Mathf.Abs(Scale.x);
                    relativeDistortion.y /= Mathf.Abs(Scale.y);
                    distortionMatRelative.SetVector("_Distortion", relativeDistortion);
                    distortionRenderer.material = distortionMatRelative;
                }
    }

    public void SpawnParticleEFX()
    {
        GameObject newParticle = Instantiate(EFX.particle, transform.position + (Vector3.back * .05f), EFX.particle.transform.rotation);
        newParticle.transform.SetParent(transform);
        newParticle.layer = 7;
        for (int i = 0; i < newParticle.transform.childCount; i++)
            newParticle.transform.GetChild(i).gameObject.layer = 7;
    }

    public void Reveal(Action<bool> end)
    {
        if (EFX.particle)
            SpawnParticleEFX();
        StartCoroutine(AnimateReveal(animEnd => { end(true); }));
    }

    IEnumerator AnimateReveal(Action<bool> end)
    {
        float percent = 0f;
        Vector3 startRot = transform.rotation.eulerAngles;
        while (percent <= 1f)
        {
            transform.rotation
                = Quaternion.Euler(startRot.x,
                                   Mathf.Lerp(startRot.y, 0, percent += revealAnimationSpeed * Time.deltaTime),
                                   startRot.z);
            yield return null;
        }
        end(true);
    }

    public override void OnPointerDown(PointerEventData eventData) { }
    public override void OnPointerUp(PointerEventData eventData) { }
    public override void OnPointerEnter(PointerEventData eventData) { }
}