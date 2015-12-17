using UnityEngine;
using System.Collections;


public enum EnemyTypeEnum
{
	Normal,
	MiniBoss,
	Boss
}

public enum EnemyAttackTypeEnum
{
	Melee,
	Ranged
}

public enum EnemyStateEnum
{
	SearchingPlayer,
	HasPlayer,
}

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Character {

	NavMeshAgent _navMeshAgent;

	public EnemyStateEnum EnemyState;
	public EnemyTypeEnum EnemyType;
	public EnemyAttackTypeEnum EnemyAttackType;

	public float AggroRadius;
	public Projectile RangedProjectile;
	public float RangeAttackCoolDown;
	public float AttackRangeRadius;
	public float AttackMeleeRadius;
	public bool IsPlayerVisible;
	public bool IsPlayerAggroRange;
	public bool IsPlayerAttackRangeRadius;
	public bool IsPlayerAttackMeleeRadius;


	private float _nextRangedAttackTime;

	/* Physics Definition */
	LayerMask LayerMaskPlayer;
	Collider[] _playerQuery;
	Collider[] _testSphereColliderResult;
	RaycastHit _raycastHit;


	public Vector2 WalkingCoolDown;
	float _nextWalking;

	protected override void Start ()
	{
		base.Start ();

		// Ajusta os valores inicias do inimigo
		this.EnemyState = EnemyStateEnum.SearchingPlayer;
		this.CharacterType = CharacterTypeEnum.Enemy;

		// Ajusta o agente de navegacao
		_navMeshAgent = GetComponent<NavMeshAgent>();
		_navMeshAgent.speed = Speed;

		// Cria o array de colliders para a pesquisa do jogador
		_testSphereColliderResult = new Collider[1];
		_playerQuery = new Collider[1];
		LayerMaskPlayer = 1 << LayerMask.NameToLayer("Player");
	}

	protected override void Update ()
	{
		base.Update ();


		switch(EnemyState)
		{
			case EnemyStateEnum.SearchingPlayer:
			Patrol();
			CheckPlayerProximity();

			if (IsPlayerVisible)
			{
				EnemyState = EnemyStateEnum.HasPlayer;
			}

			break;
			case EnemyStateEnum.HasPlayer:

			CheckPlayerProximity();
			MoveToAttack();
			Attack();

			if (!IsPlayerVisible)
			{
				EnemyState = EnemyStateEnum.SearchingPlayer;
			}
			break;
		}

		if (!_navMeshAgent.hasPath)
			_navMeshAgent.velocity = Vector3.zero;
	}

	void Patrol()
	{
		if (Time.time > _nextWalking)
		{
			if (!_navMeshAgent.hasPath){
				_navMeshAgent.SetDestination(new Vector3(Random.Range(-24, 24), 0, Random.Range(-24, 24)));
			}

			if (_navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
			{
				_nextWalking = Time.time + Random.Range(WalkingCoolDown.x, WalkingCoolDown.y);
			}
		}
	}

	void CheckPlayerProximity()
	{
		IsPlayerAggroRange = false;
		IsPlayerVisible = false;
		_playerQuery[0] = null;

		if (Physics.OverlapSphereNonAlloc(transform.position, AggroRadius, _playerQuery, LayerMaskPlayer) > 0)
		{
			IsPlayerAggroRange = true;

			if (Physics.Raycast(GetForwardPosition, (_playerQuery[0].transform.position - GetForwardPosition) + Vector3.up, out _raycastHit, 500f))
			{
				if (_raycastHit.collider.gameObject.CompareTag(CONSTANTS.TAGS.PLAYER)){
					Debug.DrawLine(GetForwardPosition, _raycastHit.point, Color.green);
					IsPlayerVisible = true;
				}
				else{
					Debug.DrawLine(GetForwardPosition, _raycastHit.point, Color.red);
				}
			}
		}
	}

	void MoveToAttack()
	{
		IsPlayerAttackMeleeRadius = false;
		IsPlayerAttackRangeRadius = false;

		if (IsPlayerVisible)
		{
			Vector3 _lookAt = new Vector3(_playerQuery[0].transform.position.x, transform.position.y, _playerQuery[0].transform.position.z);
			this.transform.LookAt(_lookAt);

			switch(EnemyAttackType)
			{
			case EnemyAttackTypeEnum.Melee:
				IsPlayerAttackMeleeRadius = MovetoAttackDistanceRadius(AttackMeleeRadius);
				break;
			case EnemyAttackTypeEnum.Ranged:
				IsPlayerAttackRangeRadius = MovetoAttackDistanceRadius(AttackRangeRadius);
				break;
			}
		}
	}

	void Attack()
	{
		switch(EnemyAttackType)
		{
		case EnemyAttackTypeEnum.Melee:
			break;
		case EnemyAttackTypeEnum.Ranged:

			if (IsPlayerAttackRangeRadius){
				if (Time.time > _nextRangedAttackTime)
				{
					// TODO: MELHORAR PERFORMANCE COM O POOL DE OBJETOS E TIRAR A LAYERMASK DINAMICA
					Projectile _newProjectile = Instantiate(RangedProjectile, GetForwardPosition, transform.rotation) as Projectile;

					_nextRangedAttackTime = Time.time + RangeAttackCoolDown;
				}
			}

			break;
		}
	}

    bool MovetoAttackDistanceRadius(float attackDistanceRadius_)
	{
		// Verifica se o jogador esta no range do attack a distancia
		if (Physics.OverlapSphereNonAlloc(transform.position, attackDistanceRadius_, _testSphereColliderResult, LayerMaskPlayer) == 0)
		{
			transform.Translate(Vector3.forward * Speed * Time.deltaTime);
			//_navMeshAgent.SetDestination(_playerQuery[0].transform.position);
			return false;
		}
		else
		{
			_navMeshAgent.ResetPath();
			return true;
		}
	}

	void OnDrawGizmos()
	{
		// Desenha a Esfera do Aggro
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, AggroRadius);

		// Desenha os gizmos de range
		switch(EnemyAttackType)
		{
		case EnemyAttackTypeEnum.Melee:
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(GetForwardPosition, AttackMeleeRadius);			
			break;
		case EnemyAttackTypeEnum.Ranged:
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.position, AttackRangeRadius);			
			break;
		}
	}
}
