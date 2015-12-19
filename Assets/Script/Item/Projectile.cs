using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Projectile : MonoBehaviour {

	public float Speed;
	public ENUMERATORS.Combat.DamageType DamageType;
	public bool LiveAfterHit;

	[HideInInspector]
	public Character Damager;

	Player _player;
	Rigidbody _rigidBody;

	// Use this for initialization
	protected virtual void Start () {

		_player = FindObjectOfType<Player>();
		_rigidBody = GetComponent<Rigidbody>();
		Destroy(this.gameObject, 5);
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (!_rigidBody.isKinematic){
			transform.Translate(Vector3.forward * Speed * Time.deltaTime);
		}
	}

	protected virtual void OnCollisionEnter(Collision collision_)
	{
		if (collision_.gameObject.CompareTag(CONSTANTS.TAGS.PLAYER))
		{			
			Destroy(this.gameObject); // Destroy o Projetil
			_player.ApplyDamage(Damager, DamageType); // Aplica o Dano no jogador
		}
		else{
			if (LiveAfterHit)
				_rigidBody.isKinematic = true;
				
			else
				Destroy(this.gameObject);
		}

	}
}
