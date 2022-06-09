using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;


public class PTASAuthentification : MonoBehaviour
{
    public static PlayGamesPlatform platform;

    void Start()
    {
        if (platform == null)
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentification);
            PlayGamesPlatform.DebugLogEnabled = true;

            platform = PlayGamesPlatform.Activate();
        }
    }

    internal void ProcessAuthentification(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            
        }
        else
        {
            
        }
    }
}
