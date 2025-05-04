using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachine;

    private void Awake()
    {
        cinemachine = GetComponentInChildren<CinemachineVirtualCamera>(true);
        EnableCamera(false);
    }

    public void EnableCamera(bool enable)
    {
        cinemachine.gameObject.SetActive(enable);
    }

    public void SetNewTarget(Transform newTarget)
    {
        cinemachine.Follow = newTarget;
    }
}
