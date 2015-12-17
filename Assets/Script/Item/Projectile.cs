using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Projectile : MonoBehaviour {

	public float Speed;
	public float MeleeDamage;
	public float MagicDamage;

	// Use this for initialization
	protected virtual void Start () {
		Destroy(this.gameObject, 5);
	}
	
	// Update is called once per frame
	protected virtual void Update () {	
		transform.Translate(Vector3.forward * Speed * Time.deltaTime);
	}

	protected virtual void OnCollisionEnter(Collision collision_)
	{
		Destroy(this.gameObject);
	}
}
