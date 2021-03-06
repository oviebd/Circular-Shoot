﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// It Controls all Time Calculations
public class Timer : MonoBehaviour
{
    [Header("Times in Second.")]
    [SerializeField] private float _time;

    private ITimer _iTimer;
    private float _startTime;
    private bool _isTimerOn = false;
    private float _pauseStartTime;
    private float _pauseTimeOfCurrentSession;
    private float _totalPauseTime;

    private bool _isTimeSet = false;

    #region Public APi
    
    public void StartTimer(float time)
    {
        _isTimeSet = true;
        ResetTimer();
        this._time = time;
        _isTimerOn = true;
    }

    public void PauseTimer()
    {
        _isTimerOn = false;
        _pauseStartTime = Time.time;
    }

    public void ResumeTimer()
    {
        _isTimerOn = true;
        _totalPauseTime = _totalPauseTime + _pauseTimeOfCurrentSession;
        _pauseTimeOfCurrentSession = 0.0f;
    }

    public bool IsTimeSet()
    {
        return _isTimeSet;
    }

    #endregion Public APi


    private void ResetTimer()
    {
        _startTime = Time.time;
        _pauseTimeOfCurrentSession = 0.0f;
        _totalPauseTime = 0.0f;
    }

    private void Update()
    {
        if(_isTimerOn == false)
        {
            _pauseTimeOfCurrentSession = Time.time - _pauseStartTime;
        }
        if ( GetElapsedTime() >= _time && _isTimerOn == true)
        {
            OnTimerComplete();
        }
    }
    public float GetElapsedTime()
    {
        return Mathf.Abs(Time.time - _startTime - _pauseTimeOfCurrentSession - _totalPauseTime) ;  
    }
    private void OnTimerComplete()
    {
       
        if ( GetTimer() != null)
        {
            GetTimer().OnTimeCompleted();
        }

        ResetTimer();
    }

    private ITimer GetTimer()
    {
        if (_iTimer == null)
            _iTimer = this.gameObject.GetComponent<ITimer>();

        return _iTimer;
    }
  
}
