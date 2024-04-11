using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Play.Review;
using UnityEngine;

public class DeviceReviewsManager : MonoBehaviour
{
    #if UNITY_ANDROID
    private ReviewManager reviewManager;
    private PlayReviewInfo playReviewInfo = null;

    private void Awake()
    {
        reviewManager = new ReviewManager();
    }

    public async void RequestReviewAsync()
    {
        await Task.Yield();
        StartCoroutine(RequestReviewInfoObject());
    }

    public void LaunchReview() => StartCoroutine(LaunchReviewFlow());

    private IEnumerator RequestReviewInfoObject()
    {
        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError(requestFlowOperation.Error.ToString());
            yield break;
        }
        playReviewInfo = requestFlowOperation.GetResult();
        if(requestFlowOperation.IsSuccessful)
        {
            Debug.Log("Review Request Successful");
        }
    }

    private IEnumerator LaunchReviewFlow()
    {
        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchFlowOperation;
        playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError(launchFlowOperation.Error.ToString());
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
    #endif
}
