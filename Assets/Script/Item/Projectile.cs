using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Projectile : PoolObject {

	public int ID;
	public float Speed;
	public ENUMERATORS.Combat.DamageType DamageType;
	public bool LiveAfterHit;
	[HideInInspector]
	public Character Damager;
	Rigidbody _rigidBody;

	protected virtual void Awake()
	{
		_rigidBody = GetComponent<Rigidbody>();		
	}

	// Use this for initialization
	protected virtual void Start () {

	}
	
	// Update is called once per frame
	protected virtual void Update () {

		if (!IsExpired() && !_rigidBody.isKinematic)
		{
				transform.Translate(Vector3.forward * Speed * Time.deltaTime);		
		}
	}

	public override void ObjectActivated ()
	{
		base.ObjectActivated ();

		SetExpireTime(5);
		_rigidBody.isKinematic = false;
	}

	protected virtual void OnCollisionEnter(Collision collision_)
	{
		if (collision_.gameObject.CompareTag(CONSTANTS.TAGS.PLAYER))
		{
			this.ReturnToPool();
			ApplicationModel.Instance.CurrentPlayer.ApplyDamage(Damager, DamageType); // Aplica o Dano no jogador
		}
		else{
			if (LiveAfterHit)
				_rigidBody.isKinematic = true;			
			else
				this.ReturnToPool();
		}

	}
}
