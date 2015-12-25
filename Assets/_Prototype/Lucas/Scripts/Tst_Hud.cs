using UnityEngine;
using System.Collections;

public class Tst_Hud : MonoBehaviour {


	void OnCollisionEnter (Collision Colisao)
	{
		if (Colisao.gameObject.tag == "Player") {
			print ("Oh Você me tocou!");
			Destroy (gameObject);

		}

	}
}
