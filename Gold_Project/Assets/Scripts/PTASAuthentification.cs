using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PTASAuthentification : MonoBehaviour
{
    void Start()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentification);
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
