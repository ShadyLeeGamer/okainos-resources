using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterCameraAnimation : MonoBehaviour
{
    private UIProceduralCameraRotation CameraShake;
    public float ShakePower;
    public void Start()
    {
        CameraShake = GetComponent<UIProceduralCameraRotation>();
    }
    public IEnumerator Shake()
    {
        CameraShake.Range = new Vector3(ShakePower, ShakePower, ShakePower);
        yield return new WaitForSeconds(.4f);
        CameraShake.Range = new Vector3(0, 0, 0);
    }
}
