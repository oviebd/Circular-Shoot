﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class GunControllerBase : MonoBehaviour,ITimer
{
	[SerializeField] private List<GameObject> _gunGameobjectList;
	[SerializeField] private GameObject _gunPrefab;
	[SerializeField] private bool _isAutomaticFire = false;
	[SerializeField] private int _maxBullet;
	[SerializeField] private bool _infiniteFirePower = true;
	[SerializeField]  protected float _coolDownTime = .3f;

	private bool _capableForShooting = false;
	private int _currentBulletNumber = 0;

	private Timer _timer;
	private PlaySound _playSound;
	
	private IGunController _iGunController;

	private void Start()
	{
		GameManager.onGameStateChange += OnGameStateChange;
		_playSound = GetComponent<PlaySound>();
		SetGuns();
	}
	private void OnDestroy()
	{
		GameManager.onGameStateChange -= OnGameStateChange;
	}
	public void SetGunController(IGunController gunController)
	{
		this._iGunController = gunController;
	}

	public void SetGuns()
	{
		_currentBulletNumber = 0;
		EquipControllerWithGuns(_gunPrefab);
		GetTimer().StartTimer(_coolDownTime);
	}
	private void EquipControllerWithGuns(GameObject gunPrefab)
	{
		for (int i = 0; i < _gunGameobjectList.Count; i++)
		{
			GameObject newObj = Instantiate(gunPrefab, _gunGameobjectList[i].transform);
			newObj.transform.parent = _gunGameobjectList[i].transform;
			IGun iGun = newObj.GetComponent<IGun>();
			if ( iGun != null && this._iGunController != null)
			{
				this._iGunController.AppendGunsInGunController(iGun, i);
			}
		}
	}

	void Shoot()
	{
		if (this._iGunController != null)
		{
			AudioClip audio = null;
			List<IGun> guns = this._iGunController.GetGuns();
			for(int i=0; i<guns.Count; i++)
			{
				guns[i].Shoot();
				if (audio == null && _playSound != null)
				{
					audio = guns[i].GetBulletBase().GetAudioClip();
					_playSound.PlayAudioWithClip(audio);
				}
			}
		}
	}

	public void OnTimeCompleted()
	{
		if (_infiniteFirePower == false && _maxBullet <= 0)
			return ;

		if (_capableForShooting == true )
		{
			Shoot();
		}
	}

	public void StartShooting()
	{
		_capableForShooting = true;
		GetTimer().StartTimer(_coolDownTime);
	}

	public void StopShooting()
	{
		_capableForShooting = false;
		GetTimer().PauseTimer();
	}

	public void UpdateCooldownTime(float newCooldownTime)
	{
		_coolDownTime = newCooldownTime;
		GetTimer().StartTimer(_coolDownTime);
	}

	private Timer GetTimer()
	{
		if(_timer == null)
			_timer = GetComponent<Timer>();
		return _timer;
	}

	private void OnGameStateChange(GameEnum.GameState state)
	{
		if(state == GameEnum.GameState.Running)
		{
			GetTimer().ResumeTimer();
		}else
			GetTimer().PauseTimer();
	}
}