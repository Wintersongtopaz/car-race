using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTrials : MonoBehaviour
{
    Player player;
    [SerializeField] float countdownTime = 3f;
    [SerializeField] List<CheckPoint> checkpoints;//Store checkpoints in sequential order
    [SerializeField] int checkpointIndex;//Keep track of next target checkpoint

    [SerializeField] string trackName;
    [SerializeField] int totalLaps = 3;
    [SerializeField] int currentLap = 0;
    [SerializeField] float raceTime;
    float startTime;
    bool raceActive = false;
    float GetRaceTime() => (Time.time - startTime) - countdownTime;
    float GetCountdownTime() => countdownTime - (Time.time - startTime);

    float highScore;

    private void Start()
    {
        highScore = PlayerPrefs.GetFloat($"{trackName}_HighScore", 0.0f);
        PreRaceSetup();
    }
    private void Update()
    {
        if (!raceActive) return; // added RR
        if (Input.GetKeyDown(KeyCode.Z)) ResetPlayer();

        if (GetCountdownTime() > 0f)
        {
            PlayerUI.SetText("Countdown", GetCountdownTime().ToString("0"));
        }
        else
        {
            PlayerUI.SetText("Countdown", "");
            PlayerUI.SetText("RaceTime", raceTime.ToString("0.00"));
        }
        //run the race time constantly
        raceTime = GetRaceTime();
    }
    void OnCheckpointPassed(CheckPoint checkpoint, GameObject gameObject)
    {
        checkpoint.SetCheckpointEnabled(false);
        if (checkpointIndex == 0)
        {
            currentLap++;
            if (currentLap > totalLaps) EndRace();
        }
        checkpointIndex++;
        if (checkpointIndex >= checkpoints.Count) checkpointIndex = 0;
        checkpoints[checkpointIndex].SetCheckpointEnabled(true);
    }
    private void ResetPlayer()
    {
        Rigidbody rigidbody = player.gameObject.GetComponentInChildren<Rigidbody>();

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        int lastCheckpointIndex = checkpointIndex - 1;
        if (lastCheckpointIndex < 0) lastCheckpointIndex = checkpoints.Count - 1;

        rigidbody.transform.position = checkpoints[lastCheckpointIndex].transform.position;
        rigidbody.transform.rotation = Quaternion.LookRotation(-checkpoints[lastCheckpointIndex].transform.up);
    }

    void OnBoundsVolumeEnter(Rigidbody rigidbody)
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        int lastCheckpointIndex = checkpointIndex - 1;
        if (lastCheckpointIndex < 0) lastCheckpointIndex = checkpoints.Count - 1;

        rigidbody.transform.position = checkpoints[lastCheckpointIndex].transform.position;
        rigidbody.transform.rotation = Quaternion.LookRotation(-checkpoints[lastCheckpointIndex].transform.up);
    }
    //Perform setup before race starts like disabling player control.
    private void PreRaceSetup()
    {
        PlayerUI.SetText("HighScores", $"High Score: {highScore}");
        player = FindObjectOfType<Player>();
        player.SetControlEnabled(false);

        CheckPoint.OnCheckpointPassed.AddListener(OnCheckpointPassed);
        BoundsVolume.OnBoundsVolumeEnter.AddListener(OnBoundsVolumeEnter);

        foreach (CheckPoint checkpoint in checkpoints)
        {
            checkpoint.SetCheckpointEnabled(false);

        }
        checkpoints[checkpointIndex].SetCheckpointEnabled(true);
        startTime = Time.time; //added RR
        raceActive = true; //added RR
        StartCountdown();
    }

    //Start countdown timer. end of timer = start race
    private void StartCountdown()
    {
        Invoke("StartRace", countdownTime);
    }
    //start race and enable player control
    private void StartRace()
    {
        player.SetControlEnabled(true);
    }
    //disable player control perform end of race cleanup.
    private void EndRace()
    {
        raceActive = false;
        player.SetControlEnabled(false);
        HighScore();
        Invoke("Restart", 3f);
    }
    private void Restart()
    {
        SceneManager.LoadScene(0);
        raceTime = 0f;
    }

    private void HighScore()
    {
        if (raceTime<highScore || highScore == 0f)
        {
            highScore = raceTime;
            PlayerPrefs.SetFloat($"{trackName}_HighScore", highScore);
            PlayerPrefs.Save();
            PlayerUI.SetText("HighScore", $"New High Score: {highScore}");
        }
    }

}
