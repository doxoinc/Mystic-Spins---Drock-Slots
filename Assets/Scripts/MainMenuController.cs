using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject Games;
    public GameObject SettingsMenu;
    public GameObject AchievementsMenu;
    public GameObject LeaderboardMenu;
    public GameObject WheelsMenu;
    public GameObject DailyBonusMenu;

    private void Start()
    {
        // Initially show the Main Menu and hide the others
        ShowMainMenu();
    }

    // Show the Main Menu and hide the others
    public void ShowMainMenu()
    {
        SetAllMenusInactive();
        MainMenu.SetActive(true);
    }

    // Show the Games Menu and hide the others
    public void ShowGamesMenu()
    {
        SetAllMenusInactive();
        Games.SetActive(true);
    }

    // Show the Settings Menu and hide the others
    public void ShowSettingsMenu()
    {
        SetAllMenusInactive();
        SettingsMenu.SetActive(true);
    }

    // Show the Achievements Menu and hide the others
    public void ShowAchievementsMenu()
    {
        SetAllMenusInactive();
        AchievementsMenu.SetActive(true);
    }

    // Show the Leaderboard Menu and hide the others
    public void ShowLeaderboardMenu()
    {
        SetAllMenusInactive();
        LeaderboardMenu.SetActive(true);
    }

    // Show the Wheels Menu and hide the others
    public void ShowWheelsMenu()
    {
        SetAllMenusInactive();
        WheelsMenu.SetActive(true);
    }

    // Show the Daily Bonus Menu and hide the others
    public void ShowDailyBonusMenu()
    {
        SetAllMenusInactive();
        DailyBonusMenu.SetActive(true);
    }

    // Helper function to set all menus inactive
    private void SetAllMenusInactive()
    {
        MainMenu.SetActive(false);
        Games.SetActive(false);
        SettingsMenu.SetActive(false);
        AchievementsMenu.SetActive(false);
        LeaderboardMenu.SetActive(false);
        WheelsMenu.SetActive(false);
        DailyBonusMenu.SetActive(false);
    }

    // Example of button event handlers
    public void OnPlayButtonClicked()
    {
        ShowGamesMenu();  // Switch to the Games Menu
    }

    public void OnSettingsButtonClicked()
    {
        ShowSettingsMenu();  // Switch to the Settings Menu
    }

    public void OnAchievementsButtonClicked()
    {
        ShowAchievementsMenu();  // Switch to the Achievements Menu
    }

    public void OnLeaderboardButtonClicked()
    {
        ShowLeaderboardMenu();  // Switch to the Leaderboard Menu
    }

    public void OnWheelsButtonClicked()
    {
        ShowWheelsMenu();  // Switch to the Wheels Menu
    }

    public void OnDailyBonusButtonClicked()
    {
        ShowDailyBonusMenu();  // Switch to the Daily Bonus Menu
    }

    public void OnBackButtonClicked()
    {
        ShowMainMenu();  // Go back to the Main Menu
    }


}
