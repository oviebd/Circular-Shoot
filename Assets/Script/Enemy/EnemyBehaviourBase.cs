﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourBase : MonoBehaviour, IColliderEnter
{
    [SerializeField] GameObject _playerObj;
    [SerializeField] private GameObject graphicsObj;
    [SerializeField] Collider2D _collider;
    [SerializeField] private Explosion _explosion;
    [SerializeField] private GameEnum.EnemyType _enemyType;
	protected Vector3 targetPos;
    private IENemyBehaviour _enemyBehaviour;

    

    private void Start()
    {
       // Destroy(this.gameObject,5.0f);
        SearchForPlayer();
    }

    public void SetEnemyBehaviour(IENemyBehaviour behaviour)
    {
        this._enemyBehaviour = behaviour;
    }

    void SearchForPlayer()
    {
        ShipController controller = FindObjectOfType<ShipController>();

        if (controller != null)
        {
            graphicsObj.SetActive(true);
            _playerObj = controller.gameObject;

            IENemyBehaviour behaviour = this.gameObject.GetComponent<IENemyBehaviour>();

            if (behaviour != null)
                behaviour.OnTargetFound(_playerObj);
        }
    }

	public GameEnum.EnemyType GetEnemyType()
	{
		return _enemyType;
	}

	public void onCollide(GameObject collidedObject)
    {
        Destroy(collidedObject);
        DestroyObj();
    }

    void DestroyObj()
    {
        _collider.enabled = false;

        IMove[] moves = gameObject.GetComponents<IMove>();
        for (int i = 0; i < moves.Length; i++)
        {
            moves[i].StopMovement();
        }

        Destroy(this.gameObject,2.0f);
        graphicsObj.SetActive(false);

        if( _enemyBehaviour!= null)
        {
            if (_explosion != null)
                _explosion.Explode();

            _enemyBehaviour.OnDestroyObject();
        }
    }

}
