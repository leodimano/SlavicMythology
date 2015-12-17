using UnityEngine;
using System.Collections;

/// <summary>
/// Enumerador dos tipos de Personagens
/// </summary>
public enum CharacterTypeEnum
{
	NPC,
	Player,
	Enemy,
}

public enum CharacterState
{
	Dead,
	Alive
}

public enum DamageType
{
	Melee,
	Magic
}

/// <summary>
/// Classe base de todos os personagens do. Qualquer evento comum entre os personagens deve ser implementado aqui
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour {


	// Tipo do personagem
	public CharacterTypeEnum CharacterType;

	// Atributos de GameDesign
	public CharacterAttribute[] Attributes;

	#region Atributos de Controle

	public float Speed;
	[HideInInspector]
	public float CurrentSpeed;

	System.Random _pseudoRandom;

	#endregion

	#region Componentes da Unity

	protected Rigidbody _rigidBody;
	protected Animator _animator;

	#endregion


	#region Unity Methods 

	// Use this for initialization
	protected virtual void Start () {

		// Obtem as instancias dos componentes Unity;
		_rigidBody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();

		InitializeAttributes();
		// TODO: Carregar os atributos do jogador Salvo ou nao. Se inimigo carregar os atributos baseado na tabela de atributos
	}
	
	// Update is called once per frame
	protected virtual void Update () {

	
	}

	protected virtual void FixedUpdate()
	{
		
	}

	// Last method called once per frame
	protected virtual void LateUpdate()
	{		
		ClampAttributes(); // Apos todos os calculos aplica os limites dos atributos
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Metodo responsavel por inicializar a arvore de atributos
	/// </summary>
	void InitializeAttributes()
	{
		Attributes = new CharacterAttribute[CONSTANTS.ATTRIBUTES.COUNT];

		CharacterAttribute _charAttr = null;

		for(int _index = 0; _index < CONSTANTS.ATTRIBUTES.COUNT; _index++)
		{			
			_charAttr = new CharacterAttribute();
			_charAttr.AttributeType = (CharacterAttributeTypeEnum)_index;
			_charAttr.Current = 0;
			_charAttr.CurrentBuffed = 0;
			_charAttr.Max = 0;
			_charAttr.MaxBuffed = 0;
			_charAttr.Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[_index];

			switch(_charAttr.AttributeType)
			{
			case CharacterAttributeTypeEnum.HitPoint:
				_charAttr.DisplayOrder = 0;
				break;
			case CharacterAttributeTypeEnum.ManaPoint:
				_charAttr.DisplayOrder = 1;
				break;
			case CharacterAttributeTypeEnum.MeleeAttack:
				_charAttr.DisplayOrder = 2;
				break;
			case CharacterAttributeTypeEnum.MagicAttack:
				_charAttr.DisplayOrder = 3;
				break;
			case CharacterAttributeTypeEnum.MeleeDefense:
				_charAttr.DisplayOrder = 4;
				break;
			case CharacterAttributeTypeEnum.MagicDefense:
				_charAttr.DisplayOrder = 5;
				break;
			case CharacterAttributeTypeEnum.AttackSpeed:
				_charAttr.DisplayOrder = 6;
				break;
			case CharacterAttributeTypeEnum.CriticChance:
				_charAttr.DisplayOrder = 7;
				break;
			case CharacterAttributeTypeEnum.CriticMultiplier:
				_charAttr.DisplayOrder = 8;
				break;
			}

			Attributes[_index] = _charAttr; 
		}
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Metodo responsavel por calcular o dano recebido do personagem
	/// </summary>
	/// <param name="damager_">Oponente que deferiu o dano</param>
	/// <param name="damageType_">Tipo do Dano</param>
	public void ApplyDamage(Character damager_, DamageType damageType_)
	{		
		if (damager_ != null){
			
			float _damage = 0;
			bool _applyCritic =false;

			// Verifica se será um dano critico
			_pseudoRandom = new System.Random((int)Time.time);
			if (CriticChance.CurrentBuffed >= _pseudoRandom.Next(0, 100)) _applyCritic = true;

			switch(damageType_)
			{
			case DamageType.Melee:

				// Dano = ((Oponente)DanoFisico + Buf) - ((Receptor) Defesa Fisica)
				_damage = damager_.MeleeAttack.CurrentBuffed * (_applyCritic ? 1f + (damager_.CriticMultiplier.CurrentBuffed / 100f) : 1f);
				_damage -= this.MeleeDefense.CurrentBuffed;

				break;
			case DamageType.Magic:

				// TODO: Dano = ((Oponente)DanoMagico + Buf) * Magia.Multiplicador) - ((Receptor) Defesa Fisica)
				_damage = damager_.MagicAttack.CurrentBuffed * (_applyCritic ? 1f + (damager_.CriticMultiplier.CurrentBuffed / 100f) : 1f);
				_damage -= this.MagicDefense.CurrentBuffed;

				break;
			}

			if (_damage <= 0) _damage = 1;

			this.HitPoint.CurrentBuffed -= _damage;		
		}
	}

	#endregion

	#region Metodos de Calculos dos Atributos

	/// <summary>
	/// Metodo responsavel por aplicar os limites para mais e ou menos dos atributos
	/// </summary>
	void ClampAttributes()
	{
		if (HitPoint.CurrentBuffed < 0)
			HitPoint.CurrentBuffed = 0;

		if (ManaPoint.CurrentBuffed < 0)
			ManaPoint.CurrentBuffed = 0;
	}

	#endregion

	#region Propriedades Programadas 

	/// <summary>
	/// Retorna o ponto de frente do personagem na posicao Global
	/// </summary>
	public Vector3 GetForwardPosition
	{
		get{ return this.transform.forward + this.transform.position; }
	}

	/// <summary>
	/// Retorno o ponto da direita do personagem na posicao Global
	/// </summary>
	public Vector3 GetRightPosition
	{
		get{ return this.transform.right + this.transform.position; }
	}

	/// <summary>
	/// Retorna o atributo de dano fisico
	/// </summary>
	/// <value>The max hit point.</value>
	public CharacterAttribute MeleeAttack
	{
		get{return Attributes[0];}
	}

	/// <summary>
	/// Retorna o atributo de defesa fisica
	/// </summary>
	/// <value>The max hit point.</value>
	public CharacterAttribute MeleeDefense
	{
		get{return Attributes[1];}
	}

	/// <summary>
	/// Retorna o atributo de dano magico
	/// </summary>
	/// <value>The max hit point.</value>
	public CharacterAttribute MagicAttack
	{
		get{return Attributes[2];}
	}

	/// <summary>
	/// Retorna o atributo de defesa magica
	/// </summary>
	/// <value>The max hit point.</value>
	public CharacterAttribute MagicDefense
	{
		get{return Attributes[3];}
	}

	/// <summary>
	/// Retorna o atributo de vida
	/// </summary>
	/// <value>The max hit point.</value>
	public CharacterAttribute HitPoint 
	{
		get{return Attributes[4];}
	}

	/// <summary>
	/// Retorna o atributo de dano energia
	/// </summary>
	/// <value>The max hit point.</value>
	public CharacterAttribute ManaPoint  
	{
		get{return Attributes[5];}
	}

	/// <summary>
	/// Retorna o atributo que controle a velocidade de ataque
	/// </summary>
	/// <value>The max hit point.</value>
	public CharacterAttribute AttackSpeed 
	{
		get{return Attributes[6];}
	}

	/// <summary>
	/// Retorna o atributo que controle o multiplicador de critico
	/// </summary>
	/// <value>The max hit point.</value>
	public CharacterAttribute CriticMultiplier 
	{
		get{return Attributes[7];}
	}

	/// <summary>
	/// Retorna o atributo que controle a chance de critico
	/// </summary>
	/// <value>The max hit point.</value>
	public CharacterAttribute CriticChance
	{
		get{return Attributes[8];}
	}

	#endregion
}