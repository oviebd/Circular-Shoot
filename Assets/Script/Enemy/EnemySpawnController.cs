﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnController : MonoBehaviour,ITimer
{
	public static EnemySpawnController instance;
	[Header("Enemy Prefab (EnemyBehaviourBase) List")]
	[SerializeField] List<EnemyBehaviourBase> enemyBehaviours;
	private Timer _timer;

	private bool canSpawnEnemy = false;

	private int currentEnemyWave = 1;
	private int enemyNumberInCurrentWave = 0;
	private int maxEnemyNumberInCurrentWave = 0;
	private float enemySpawnDelayForCurrentWave = 1.0f;
	private int _totalEnemyNumber = 0;
	private int _totalEnemyCountPoint = 0;
	private List<int> enemyNumberPerWaveList;

	private EnemyLevelData data;

    private void Awake()
    {
		if (instance == null)
			instance = this;
	}

    private void Start()
    {
		GameManager.onGameStateChange += OnGameStateChanged;
		_timer = this.gameObject.GetComponent<Timer>();
    }
    private void OnDestroy()
    {
		GameManager.onGameStateChange -= OnGameStateChanged;
	}

    private void OnGameStateChanged(GameEnum.GameState gameState)
	{
		if (gameState == GameEnum.GameState.Running)
		{
			if (canSpawnEnemy)
				_timer.ResumeTimer();
		}
		else
		{
			if (canSpawnEnemy)
				_timer.PauseTimer();
		}
	}

	public void LoadLevelEnemyData(int level)
    {
		data = null;
	    data = EnemyManager.instance.GetEnemyLevelData(level);
		enemyNumberPerWaveList = new List<int>();
		enemyNumberPerWaveList.Clear();

		if (data == null)
			return;

		_totalEnemyNumber = 0;
		_totalEnemyCountPoint = 0;
		currentEnemyWave = 1;
		CalculateTotalEnemy(data);
		ResetCurrentLevelEnemyData();
	}

	 void CalculateTotalEnemy(EnemyLevelData data)
	{
		int totalEnemyNum = 0;
		for ( int i=0; i<data.numberOfWave; i++)
		{
			int thisWaveEnemy = 0;
			if (i == 0)
				thisWaveEnemy = data.initialNumberOfEnemyInAWave;
			else
				thisWaveEnemy = (int) (enemyNumberPerWaveList[(i-1)] * data.multiplierOfEnemyNumberPerWave);
			enemyNumberPerWaveList.Add(thisWaveEnemy);
			totalEnemyNum = totalEnemyNum + thisWaveEnemy;
		}
		WinningConditionHandler.instance.SetWinningAmount(totalEnemyNum);
	}

	

    void ResetCurrentLevelEnemyData()
    {
		enemyNumberInCurrentWave = 0;
		if(enemyNumberPerWaveList.Count < currentEnemyWave - 1)
			maxEnemyNumberInCurrentWave = enemyNumberPerWaveList[currentEnemyWave - 1];

		if (currentEnemyWave == 1)
        {
			maxEnemyNumberInCurrentWave = data.initialNumberOfEnemyInAWave;
			enemySpawnDelayForCurrentWave = data.initialEnemySpawnDelay;
		}
        else
        {
			if (currentEnemyWave < data.numberOfWave)
				GameUiVisibilityHandler.instance.ShowAnimatedMessage(" Start Wave :  " + currentEnemyWave);
			else
				GameUiVisibilityHandler.instance.ShowAnimatedMessage(" Last Wave ");
			
			enemySpawnDelayForCurrentWave = data.initialEnemySpawnDelay - (data.enemySpawnDelayReduceFactorPerWave * (currentEnemyWave - 1));
		}

		if (enemySpawnDelayForCurrentWave <= 0)
			enemySpawnDelayForCurrentWave = .5f;

		canSpawnEnemy = true;
		_timer.StartTimer(enemySpawnDelayForCurrentWave);
	}

	public void OnTimeCompleted()
	{
		GameObject obj = SpawnRandomEnemy();
		EnemyBehaviourBase _behavioir = obj.GetComponent<EnemyBehaviourBase>();
		_behavioir.SetActiveMode();
		UpdateEnemyNumber();
	}

    private EnemyBehaviourBase[] GetAllEnemyBehaviour()
    {
		EnemyBehaviourBase[] enemyList = FindObjectsOfType<EnemyBehaviourBase>();
		return enemyList;
    }

    public void SetEnemyModeActiveInactive(bool isModeActive)
    {
		EnemyBehaviourBase[] enemyList = GetAllEnemyBehaviour();

		if (enemyList == null)
			return;
		for (int i = 0; i < enemyList.Length; i++)
		{
            if(isModeActive)
				enemyList[i].SetInactiveMode();
            else
				enemyList[i].SetInactiveMode();
		}
	}

	void UpdateEnemyNumber()
    {
		enemyNumberInCurrentWave = enemyNumberInCurrentWave + 1;
		_totalEnemyNumber = _totalEnemyNumber + 1;
		if (enemyNumberInCurrentWave > maxEnemyNumberInCurrentWave)
		{
			UpdateEnemyWaveData();
		}
	}

	void UpdateEnemyWaveData()
	{
		currentEnemyWave = currentEnemyWave + 1;
		if (currentEnemyWave <= data.numberOfWave) 
        {
            //Spawn Next wave enemy
			ResetCurrentLevelEnemyData();
		}
        else
        {
			//Enemy creation in this Level completed
			canSpawnEnemy = false;
		}
		
	}

	#region EnemySpawn
	GameObject SpawnRandomEnemy()
	{
		GameObject enemyPrefab = GetSpecificEnemyPrefabBasedOnType(GetRandomEnemyType());
		GameObject enemyObj = SpawnEnemy(enemyPrefab);
		return enemyObj;
	}
	GameObject SpawnSpecificTypeEnemy(GameEnum.EnemyType type)
	{
		GameObject enemyPrefab = GetSpecificEnemyPrefabBasedOnType(type);
		GameObject enemyObj = SpawnEnemy(enemyPrefab);
		return enemyObj;
	}
	GameObject SpawnEnemy(GameObject enemyPrefab)
	{
		if (enemyPrefab != null)
		{
			GameObject obj = InstantiatorHelper.instance.InstantiateObject(enemyPrefab, this.gameObject);
			obj.transform.position = PositionHandler.instance.InstantiateEnemyInRandomPosition();
			return obj;
			//UpdateEnemyNumber();
		}
		return null;
	}

	GameObject GetSpecificEnemyPrefabBasedOnType(GameEnum.EnemyType type)
	{
		for (int i = 0; i < enemyBehaviours.Count; i++)
		{
			if (type == enemyBehaviours[i].GetEnemyType())
			{
				return enemyBehaviours[i].gameObject;
			}
		}
		return null;
	}

	private GameEnum.EnemyType GetRandomEnemyType()
	{
		int randomRange = Random.Range(0, 100);
		int prevVal = 0;
		foreach (KeyValuePair<GameEnum.EnemyType, int> keyValue in data.enemyTypeAndPercentageDictionary)
		{
			if (randomRange >= prevVal && randomRange <= keyValue.Value)
				return keyValue.Key;
			prevVal = keyValue.Value;
		}
		return GameEnum.EnemyType.Type_1;
	}

   
    #endregion EnemySpawn



}
