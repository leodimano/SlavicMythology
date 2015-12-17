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

/// <summary>
/// Classe base de todos os personagens do. Qualquer evento comum entre os personagens deve ser implementado aqui
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour {


	// Tipo do personagem
	public CharacterTypeEnum CharacterType;

	// Atributos de GameDesign
	public Hashtable ATTRIBUTES;

	#region Atributos de Controle

	public float Speed;
	[HideInInspector]
	public float CurrentSpeed;

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

	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Metodo responsavel por inicializar a arvore de atributos
	/// </summary>
	void InitializeAttributes()
	{
		ATTRIBUTES = new Hashtable(CONSTANTS.ATTRIBUTES.COUNT);

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

			ATTRIBUTES.Add(_charAttr.AttributeType, _charAttr);
		}
	}

	#endregion

	#region Public Methods

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


	#endregion
}