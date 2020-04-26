﻿using UnityEngine;
using System.Collections;

/*At First UpdateWave ( waveNumber = 1) there is no time Limit for destroying a number of enemy
 *After completing first wave (waveNumber = 2) . Player needs to destroy a number of enemies within a time
 *    If he succided then player updated and increase waveNumber otherwise player deGrade and decrease waveNumber
 *If Player obtains the maximum update waveNumber then player can not be updated anymore but player need to continue his
 * existing performance for keeping his update status otherwise he lose his update and get degraded. 
 */
public class PlayerUpdateController : MonoBehaviour
{
    private enum UpgrateStatus { upgrade,degrade}

    [SerializeField] private float _requiredTime = 5.0f; 
    [SerializeField] private int _requiredEnemy = 5; //Base number of enemy need to destroy in a given time.
    [SerializeField] private float _updateFactor = 5;  // Used for set Difficulty .Responsible for calculated enemy number based on waveNumber

    private float _lastUpdateTime;
    private float _requiredTimeForCurrentWave = 5.0f;
    private int   _requiredEnemyForCurrentWave = 0;
    private int   _currentWaveNumber = 1;
    private PlayerUpdateModel _updateDataModel;
    private int _maxWaveNum = 3;

    public delegate void PlayerSystemUpdate(GameEnum.UpgradeType upgradeType);
    public static event PlayerSystemUpdate onPlayerSystemUpdate;

	
    #region CallBacks Initializations
    private void Awake()
    {
        EnemyBehaviourBase.enemyDestroyedByPlayer += onEnemyDestroyed;
        GameManager.onGameStateChange += OnGameStateChange;
    }
    private void OnDestroy()
    {
        EnemyBehaviourBase.enemyDestroyedByPlayer -= onEnemyDestroyed;
        GameManager.onGameStateChange -= OnGameStateChange;
    }
    #endregion CallBacks Initializations


    private void Start()
    {
		_updateDataModel = new PlayerUpdateModel();
        ResetUpdate();
    }

    private void Update()
    {
		ResetUpdateDataModel();
        if(onPlayerSystemUpdate != null)
            UpdateIndicatorUI.instance.SetUpdateUI(_updateDataModel);

        Check();
	}

    void onEnemyDestroyed(EnemyBehaviourBase enemyBehaviour)
    {
        _requiredEnemyForCurrentWave = _requiredEnemyForCurrentWave - 1;
	}

    #region UpdateChecker
    private void Check()
    {
        if(_currentWaveNumber <= 1)
        {
           if(_requiredEnemyForCurrentWave <= 0)
                ActionUpdate();
            return;
        }

        if (_requiredEnemyForCurrentWave <= 0 && IsTimePassed() == false)
            ActionUpdate();
          
        else if (IsTimePassed() == true)
            ActionDegrade();
    }

    private bool IsTimePassed()
    {
        if (GetElapsedTime() <= _requiredTimeForCurrentWave)
            return false;
        else
            return true;
    }

	private float GetElapsedTime()
	{
		return Time.time - _lastUpdateTime;
	}
    #endregion UpdateChecker

    #region DataReset

    private void OnGameStateChange(GameEnum.GameState gameState)
    {
        if (gameState == GameEnum.GameState.Idle || gameState == GameEnum.GameState.PlayerWin || gameState == GameEnum.GameState.PlayerLose)
        {
            _currentWaveNumber = 1;
            _requiredTimeForCurrentWave = _requiredTime;
            ResetUpdate();
        }
    }

    private void ResetUpdate()
    {
        int savedTime = (int)(_requiredTimeForCurrentWave - GetElapsedTime());
        if (savedTime > 0 && _currentWaveNumber > 2 && _currentWaveNumber <= _maxWaveNum)
        {
            _requiredTimeForCurrentWave = _requiredTimeForCurrentWave + savedTime;
            UpdateIndicatorUI.instance.ShowSavedTimeMesage(savedTime);
        }
          
        _lastUpdateTime = Time.time;
        _requiredEnemyForCurrentWave = _requiredEnemy + (int)((_currentWaveNumber-1)* _updateFactor);
        ResetUpdateDataModel();
    }
	private void ResetUpdateDataModel()
	{
		float remainingTime = _requiredTimeForCurrentWave - GetElapsedTime() ;
		_updateDataModel.remainingEnemyEnemyNumber = _requiredEnemyForCurrentWave;
		_updateDataModel.remainingTimeInSec = remainingTime;
		_updateDataModel.currentUpdateWave = _currentWaveNumber;

        if (_currentWaveNumber >= _maxWaveNum)
            _updateDataModel.isItMaxUpdateWave = true;
        else
            _updateDataModel.isItMaxUpdateWave = false;
    }

    #endregion DataReset

    #region UpdateAction
    private void ActionUpdate()
    {
        if (onPlayerSystemUpdate == null)
            return;

        _currentWaveNumber = _currentWaveNumber + 1;

        if (_currentWaveNumber > _maxWaveNum)
            _currentWaveNumber = _maxWaveNum;

        onPlayerSystemUpdate(GameEnum.UpgradeType.AddGun);

        ResetUpdate();
    }
    private void ActionDegrade()
    {
        if (onPlayerSystemUpdate == null)
            return;

        _currentWaveNumber = _currentWaveNumber - 1;
        if (_currentWaveNumber <= 1)
            _currentWaveNumber = 1;

        onPlayerSystemUpdate(GameEnum.UpgradeType.RemoveGun);

        ResetUpdate();
    }

    #endregion UpdateAction

}
