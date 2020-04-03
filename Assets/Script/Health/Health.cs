﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IHealth 
{
    [SerializeField] private int _totalHealth = 100;
    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = _totalHealth;
    }

    public void AddHealth(int amount)
    {
        SetHealthAmount(_currentHealth + amount);
    }

    public bool IsDie()
    {
        if (_currentHealth <= 0)
            return true;
        else
            return false;
    }

    public int GetHealthAmount()
    {
        return _currentHealth;
    }

    public void ReduceHealth(int amount)
    {
       
        SetHealthAmount(_currentHealth - amount);
    }

    public void SetHealthAmount(int amount)
    {
        _currentHealth = amount;

        if (_currentHealth < 0)
            _currentHealth = 0;
        if (_currentHealth > _totalHealth)
            _currentHealth = _totalHealth;
    }

   
}
