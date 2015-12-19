using UnityEngine;
using System.Collections;


[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Character {

	NavMeshAgent _navMeshAgent;

	public ENUMERATORS.Enemy.EnemyStateEnum EnemyState;
	public ENUMERATORS.Enemy.EnemyTypeEnum EnemyType;
	public ENUMERATORS.Enemy.EnemyAttackTypeEnum EnemyAttackType;

	public float AggroRadius;
	public int ProjectileTableId; // ID do projetil para ser utilizado
	public Projectile RangedProjectile;
	public float RangeAttackCoolDown;
	public float AttackRangeRadius;
	public float AttackMeleeRadius;
	public LayerMask RaycastPlayerLayerMask;
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
		this.EnemyState = ENUMERATORS.Enemy.EnemyStateEnum.SearchingPlayer;
		this.CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Enemy;

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
		case ENUMERATORS.Enemy.EnemyStateEnum.SearchingPlayer:

			if (!(EnemyAttackType == ENUMERATORS.Enemy.EnemyAttackTypeEnum.Stationary))
				Patrol(); // Patrula em busca do Jogador
			
			CheckPlayerProximity(); // Verifica se o Jogador esta no raio de proximidade

			if (IsPlayerVisible)
			{
				EnemyState = ENUMERATORS.Enemy.EnemyStateEnum.HasPlayer;
			}

			break;
		case ENUMERATORS.Enemy.EnemyStateEnum.HasPlayer:

			CheckPlayerProximity(); // Verifica se o Jogador esta no raio de proximidade

			if (!(EnemyAttackType == ENUMERATORS.Enemy.EnemyAttackTypeEnum.Stationary))
				MoveToAttack();
			else
				IsPlayerAttackRangeRadius = IsPlayerOnAttackDistance(AttackRangeRadius);
					
			Attack();

			if (!IsPlayerVisible)
			{
				EnemyState = ENUMERATORS.Enemy.EnemyStateEnum.SearchingPlayer;
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

			if (Physics.Raycast(GetForwardPosition, (_playerQuery[0].transform.position - GetForwardPosition) + Vector3.up, out _raycastHit, 500f, RaycastPlayerLayerMask))
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
			case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Melee:
				IsPlayerAttackMeleeRadius = MovetoAttackDistanceRadius(AttackMeleeRadius);
				break;
			case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Ranged:
				IsPlayerAttackRangeRadius = MovetoAttackDistanceRadius(AttackRangeRadius);
				break;
			}
		}
	}

    bool MovetoAttackDistanceRadius(float attackDistanceRadius_)
	{
		// Verifica se o jogador esta no range do attack a distancia
		if (!IsPlayerOnAttackDistance(attackDistanceRadius_))
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

	void Attack()
	{
		switch(EnemyAttackType)
		{
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Melee:
			break;
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Ranged:
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Stationary:

			if (IsPlayerAttackRangeRadius){
				if (Time.time > _nextRangedAttackTime)
				{
					Projectile _newProjectile = ApplicationModel.Instance.ProjectileTable[ProjectileTableId].Pool.GetFromPool() as Projectile;

					// Se for igual a nulo nao tem mais objetos no pool para recuperar, deve aguardar por um novo objeto ser liberado
					if (_newProjectile != null){
					
						_newProjectile.transform.position = GetForwardPosition;
						_newProjectile.transform.rotation = transform.rotation;

						// Rotaciona o projetil
						if (EnemyAttackType == ENUMERATORS.Enemy.EnemyAttackTypeEnum.Stationary)
						{
							_newProjectile.transform.LookAt(new Vector3(_playerQuery[0].transform.position.x, _newProjectile.transform.position.y, _playerQuery[0].transform.position.z));
						}

						_newProjectile.Damager = this;
						_newProjectile.DamageType = ENUMERATORS.Combat.DamageType.Melee;

						_nextRangedAttackTime = Time.time + RangeAttackCoolDown;
					}
				}
			}

			break;
		}
	}


	bool IsPlayerOnAttackDistance(float attackDistanceRadius_)
	{
		return Physics.OverlapSphereNonAlloc(transform.position, attackDistanceRadius_, _testSphereColliderResult, LayerMaskPlayer) > 0;
	}

	void OnDrawGizmos()
	{
		// Desenha a Esfera do Aggro
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, AggroRadius);

		// Desenha os gizmos de range
		switch(EnemyAttackType)
		{
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Melee:
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(GetForwardPosition, AttackMeleeRadius);			
			break;
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Ranged:
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Stationary:
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.position, AttackRangeRadius);			
			break;
		}
	}
}
