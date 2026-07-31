using MoreMountains.Feedbacks;
using UnityEngine;

public class CameraController : MonoBehaviour, IDisablable
{
    [SerializeField] private MMF_Player _doCameraStuf;
    
    //Temporary solution. I don't know how will we controll the camera in general, so keeping it simple;
    //We can edit it in any way if needed;
    public void Enable()
    {
        RestoreFeedback();
        _doCameraStuf.PlayFeedbacks();
    }

    public void Disable()
    {
        RestoreFeedback();
    }

    private void RestoreFeedback()
    {
        _doCameraStuf.StopFeedbacks();
        _doCameraStuf.RestoreInitialValues();
    }
}
