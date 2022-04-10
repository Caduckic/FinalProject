using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    static private AudioSource _currentMusic, _notSeen, _seen, _rocketClose, _enemiesUnaware, _winTheme;
    private float beatTiming, timePasted = 0f, soundRemaining = 0f;
    private bool winThemePlaying = false;
    // Start is called before the first frame update
    void Start()
    {
        // sets timing between music switching based on the musics bpm (140)
        beatTiming = 1f / (140f / 60f);
        // couldn't get this to work if I had these objects directly referenced, not sure why
        // either way sets the audioSources
        _notSeen = GameObject.Find("NotSeen").GetComponent<AudioSource>();
        _seen = GameObject.Find("Seen").GetComponent<AudioSource>();
        _rocketClose = GameObject.Find("RocketClose").GetComponent<AudioSource>();
        _enemiesUnaware = GameObject.Find("EnemiesUnaware").GetComponent<AudioSource>();
        _winTheme = GameObject.Find("WinTheme").GetComponent<AudioSource>();

        // checks the music bools within shipController to set music volumes
        if (ShipController.notSeen) {
            _notSeen.volume = 0.4f;
            _seen.volume = 0;
            _rocketClose.volume = 0;
            _enemiesUnaware.volume = 0;
        }
        if (ShipController.seen) {
            _notSeen.volume = 0;
            _seen.volume = 0.6f;
            _rocketClose.volume = 0;
            _enemiesUnaware.volume = 0;
        }
        if (ShipController.rocketClose) {
            _notSeen.volume = 0;
            _seen.volume = 0;
            _rocketClose.volume = 0.6f;
            _enemiesUnaware.volume = 0;
        }
        if (ShipController.enemiesUnaware) {
            _notSeen.volume = 0;
            _seen.volume = 0;
            _rocketClose.volume = 0;
            _enemiesUnaware.volume = 0.6f;
        }
    }
    void Update() {
        // checks if paused and sets pitch lower if so
        if (PauseMenu.paused) {
            _notSeen.pitch = 0.9f;
            _seen.pitch = 0.9f;
            _rocketClose.pitch = 0.9f;
            _enemiesUnaware.pitch = 0.9f;
            if (_winTheme.isPlaying) {
                _winTheme.pitch = 0.9f;
            }
        }
        // sets back to normal if not paused
        else {
            _notSeen.pitch = 1f;
            _seen.pitch = 1f;
            _rocketClose.pitch = 1f;
            _enemiesUnaware.pitch = 1f;
            if (_winTheme.isPlaying) {
                _winTheme.pitch = 1f;
            }
        }
        // counts time using deltaTime
        timePasted += Time.deltaTime;
        soundRemaining += Time.deltaTime;
        // checks if timePasted is greater than or equal to beatTiming, if so sets volume settings
        // this so so music transitions don't sound as clippy
        if (timePasted >= beatTiming) {
            // add remainder of timePasted minus beatTiming so its consistant
            // I'm not 100% sure this is has been done correctly, feedback would be much appreciated
            timePasted = timePasted % beatTiming;
            if (!CheckForWin.hasWon) {
                if (ShipController.notSeen) {
                    _notSeen.volume = 0.4f;
                    _seen.volume = 0;
                    _rocketClose.volume = 0;
                    _enemiesUnaware.volume = 0;
                }
                if (ShipController.seen) {
                    _notSeen.volume = 0;
                    _seen.volume = 0.6f;
                    _rocketClose.volume = 0;
                    _enemiesUnaware.volume = 0;
                }
                if (ShipController.rocketClose) {
                    _notSeen.volume = 0;
                    _seen.volume = 0;
                    _rocketClose.volume = 0.6f;
                    _enemiesUnaware.volume = 0;
                }
                if (ShipController.enemiesUnaware) {
                    _notSeen.volume = 0;
                    _seen.volume = 0;
                    _rocketClose.volume = 0;
                    _enemiesUnaware.volume = 0.6f;
                }
            }
        }
        // does the same kind of time check as beatTiming but with entire clip length
        // could use any of the clips length other than winTheme
        if (soundRemaining >= _seen.clip.length) {
            soundRemaining = soundRemaining % _seen.clip.length;
            // if the player has won and wintheme isn't already playing, play it
            if (ShipController.winTheme && CheckForWin.hasWon && !winThemePlaying) {
                winThemePlaying = true;
                _notSeen.volume = 0;
                _seen.volume = 0;
                _rocketClose.volume = 0;
                _enemiesUnaware.volume = 0;
                _winTheme.Play();
            }
        }
    }
}
