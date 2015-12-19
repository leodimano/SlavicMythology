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

		CreateSpellTable();
		CreateProjectileTable();
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


	/* TABELA DE HABILIDADES 1 - 1 */
	public SpellBase[] SpellTable;
	void CreateSpellTable()
	{
		SpellTable = new SpellBase[CONSTANTS.SPELL.COUNT];

		GameObject _spell = Resources.Load(CONSTANTS.RESOURCES_PATH.SPELL_FIREBALL) as GameObject;
		SpellBase _spellBase = _spell.GetComponent<SpellBase>();
		_spellBase.gameObject.SetActive(false);

		PoolManager _projectilePoolManager = new PoolManager();
		_spellBase.Pool = _projectilePoolManager;
		_spellBase.Pool.Initialize(20, this.transform);
		_spellBase.Pool.AddObjectToPool(_spellBase, 20);

		SpellTable[0] = _spellBase;
	}

	/// <summary>
	/// Tabela de projeteis disponiveis para uso
	/// </summary>
	public Projectile[] ProjectileTable;
	void CreateProjectileTable()
	{
		ProjectileTable = new Projectile[CONSTANTS.ITEM.PROJECTILE_COUNT];

		GameObject _projectileObject = Resources.Load(CONSTANTS.RESOURCES_PATH.PROJECTILE_ARROW_PROTOTYPE) as GameObject;
		Projectile _projectile = _projectileObject.GetComponent<Projectile>();
		_projectile.gameObject.SetActive(false);

		PoolManager _projectilePoolManager = new PoolManager();
		_projectile.Pool = _projectilePoolManager;
		_projectile.Pool.Initialize(100, this.transform);
		_projectile.Pool.AddObjectToPool(_projectile, 100);

		ProjectileTable[0] = _projectile;
	}
}
