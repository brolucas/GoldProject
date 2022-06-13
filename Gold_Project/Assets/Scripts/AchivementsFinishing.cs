using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class AchivementsFinishing : MonoBehaviour
{
    public static AchivementsFinishing instance;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;

    }

    public void Achievement(bool succes, string achievementCode)
    {
        Social.ReportProgress(achievementCode, 100.0d, (success) => { });
    }
}
