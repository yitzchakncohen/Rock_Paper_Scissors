using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScanningReflection : MonoBehaviour
{
    [SerializeField] private Transform scanLineTransform;
    [SerializeField] float animationTime = 1.0f;
    [SerializeField] float delayTime = 3.0f;
    private float scanLineStartingX;
    private IEnumerator coroutine = null;

    private void Awake() 
    {
        scanLineStartingX = scanLineTransform.position.x;
    }

    private void OnEnable() 
    {
        StartScanAnimation();
    }

    public void StartScanAnimation()
    {
        if(coroutine == null)
        {
            coroutine = Scan();
            StartCoroutine(coroutine);
        }
        else
        {
            CancelScan();
            coroutine = Scan();
            StartCoroutine(coroutine);
        }
    }

    public void CancelScan()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private IEnumerator Scan()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(animationTime + delayTime);
        while(true)
        {
            scanLineTransform.localPosition = new Vector3(scanLineStartingX, 0f, 0f);
            scanLineTransform.DOMoveX(-scanLineStartingX, animationTime);
            yield return waitForSeconds;
        }
    }
}
