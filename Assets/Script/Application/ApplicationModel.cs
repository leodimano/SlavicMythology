using UnityEngine;
using System.Collections;

public class ApplicationModel : MonoBehaviour {

	public static ApplicationModel Instance; // Singleton Pattern

	public Player CurrentPlayer;

	/// <summary>
	/// Awake this instance.
	/// Implementa o conceito do Singleton para o GameObject, SEMPRE UTILIZAR A VARIAVEL INSTANCE para acessar o objeto por fora da classe por exemplo: ApplicationModel.Instance
	/// </summary>
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		// Se perder a instancia do jogador, busca o jogador e atualiza o objeto
		if (CurrentPlayer == null)
			CurrentPlayer = FindObjectOfType<Player>();

	}
}
