using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Classe base de todos os personagens do. Qualquer evento comum entre os personagens deve ser implementado aqui
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour {


	// Tipo do personagem
	public ENUMERATORS.Character.CharacterTypeEnum CharacterType;

	// Atributos de GameDesign
	public CharacterAttribute[] Attributes;
	public AttributeModifier[] AttributeModifiers;

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

		InitializeAttributes(); // TODO: Carregar os atributos do jogador Salvo ou nao. Se inimigo carregar os atributos baseado na tabela de atributos
		AttributeModifiers = new AttributeModifier[CONSTANTS.ATTRIBUTES.ATTRIBUTE_MODIFIERS_COUNT];
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		
		CheckAttributeModifiers(); // Verifica os atributos
		ApplyAttributesModifiers(); // Aplica os modificadores	
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
		Attributes = new CharacterAttribute[CONSTANTS.ATTRIBUTES.ATTRIBUTE_COUNT];

		CharacterAttribute _charAttr = null;

		for(int _index = 0; _index < CONSTANTS.ATTRIBUTES.ATTRIBUTE_COUNT; _index++)
		{			
			_charAttr = new CharacterAttribute();
			_charAttr.AttributeType = (ENUMERATORS.Attribute.CharacterAttributeTypeEnum)_index;
			_charAttr.Current = 0;
			_charAttr.Max = 0;
			_charAttr.Modifiers = 0;
			//_charAttr.MaxBuffed = 0;
			_charAttr.Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[_index];

			switch(_charAttr.AttributeType)
			{
			case ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint:
				_charAttr.DisplayOrder = 0;
				break;
			case ENUMERATORS.Attribute.CharacterAttributeTypeEnum.ManaPoint:
				_charAttr.DisplayOrder = 1;
				break;
			case ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MeleeAttack:
				_charAttr.DisplayOrder = 2;
				break;
			case ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MagicAttack:
				_charAttr.DisplayOrder = 3;
				break;
			case ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MeleeDefense:
				_charAttr.DisplayOrder = 4;
				break;
			case ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MagicDefense:
				_charAttr.DisplayOrder = 5;
				break;
			case ENUMERATORS.Attribute.CharacterAttributeTypeEnum.AttackSpeed:
				_charAttr.DisplayOrder = 6;
				break;
			case ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticChance:
				_charAttr.DisplayOrder = 7;
				break;
			case ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticMultiplier:
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
	public void ApplyDamage(Character damager_, ENUMERATORS.Combat.DamageType damageType_)
	{		
		if (damager_ != null){
			
			float _damage = 0;
			bool _applyCritic =false;

			// Verifica se será um dano critico
			_pseudoRandom = new System.Random((int)Time.time);
			if (CriticChance.MaxWithModifiers >= _pseudoRandom.Next(0, 100)) _applyCritic = true;

			switch(damageType_)
			{
			case ENUMERATORS.Combat.DamageType.Melee:

				// Dano = ((Oponente)DanoFisico + Buf) - ((Receptor) Defesa Fisica)
				_damage = damager_.MeleeAttack.MaxWithModifiers * (_applyCritic ? 1f + (damager_.CriticMultiplier.MaxWithModifiers / 100f) : 1f);
				_damage -= this.MeleeDefense.MaxWithModifiers;

				break;
			case ENUMERATORS.Combat.DamageType.Magic:

				// TODO: Dano = ((Oponente)DanoMagico + Buf) * Magia.Multiplicador) - ((Receptor) Defesa Fisica)
				_damage = damager_.MagicAttack.MaxWithModifiers * (_applyCritic ? 1f + (damager_.CriticMultiplier.MaxWithModifiers / 100f) : 1f);
				_damage -= this.MagicDefense.MaxWithModifiers;

				break;
			}

			if (_damage <= 0) _damage = 1;

			this.HitPoint.Current -= _damage;		
		}
	}

	/// <summary>
	/// Metodo responsavel por gerenciar a adicao de modificadores de atributos ao personagem
	/// </summary>
	/// <param name="attributeModifier_">Attribute modifier.</param>
	public void AddAttributeModifier(AttributeModifier attributeModifier_)
	{
		bool _canBeAdded = true; // Variavel para controle se o atributo pode ser adicionao na tabela, parte do principio que sempre pode.

		// Percorre a tabela de atributos do personagem
		for (int i = 0; i < AttributeModifiers.Length; i++)
		{
			// Se o espaco estiver vazio e pode ser adicionado inclui. Para a execucao do for
			if (AttributeModifiers[i] == null && _canBeAdded){
				AttributeModifiers[i] = attributeModifier_;
				break;
			}

			// Verifica se o atributo existe na tabela e inicia os testes
			if (AttributeModifiers[i] != null)
			{
				// Se estiver aplicando o mesmo atributo somente atualiza na tabela
				if (AttributeModifiers[i].AttributeType == attributeModifier_.AttributeType &&
					AttributeModifiers[i].OriginID == attributeModifier_.OriginID &&
					AttributeModifiers[i].ModifierType == attributeModifier_.ModifierType)
				{
					AttributeModifiers[i] = attributeModifier_;
					break;					
				}
			}
		}
	}

	#endregion

	#region Metodos de Calculos dos Atributos

	/// <summary>
	/// Metodo responsavel por aplicar os limites maximos e minimos dos atributos
	/// </summary>
	void ClampAttributes()
	{
		if (HitPoint.Current < 0){
			HitPoint.Current = 0;
		}

		if (ManaPoint.Current < 0){
			ManaPoint.Current = 0;
		}
	}

	/// <summary>
	/// Metodo responsavel por remover os modificadores de atributos que expiraram
	/// </summary>
	void CheckAttributeModifiers()
	{
		bool _needToReorder = false;

		for(int i = 0; i < AttributeModifiers.Length; i++)
		{
			if (AttributeModifiers[i] != null)
			{
				// Modificador de atributo por Tempo e Expirou o Tempo = Remove da Tabela de Modificadores e marca que precisa reorganizar a tabela
				if (AttributeModifiers[i].ModifierType == ENUMERATORS.Attribute.AttributeModifierTypeEnum.Time &&
					AttributeModifiers[i].ExpireTime < Time.time)
				{
					AttributeModifiers[i] = null;
					_needToReorder = true;
					continue;
				}

				// Se for modificador para ser usado uma unica vez e esta marcado como consumido exclui da tabela
				if (AttributeModifiers[i].ModifierType == ENUMERATORS.Attribute.AttributeModifierTypeEnum.OneTimeOnly &&
					AttributeModifiers[i].Consumed)
				{
					AttributeModifiers[i] = null;
					_needToReorder = true;
					continue;					
				}
			}
			else
				break;
		}

		// Reorganiza a tabela se algum modificador foi removido
		if (_needToReorder)
		{
			Helper.ReorderArray<AttributeModifier>(AttributeModifiers);
		}
	}

	/// <summary>
	/// Metodo responsavel por aplicar os modificadores de atributos
	/// </summary>
	void ApplyAttributesModifiers()
	{
		int _attributeTypeIndex;

		for (int i = 0; i < AttributeModifiers.Length; i++)
		{
			if (AttributeModifiers[i] != null)
			{
				_attributeTypeIndex = (int)AttributeModifiers[i].AttributeType;

				switch(AttributeModifiers[i].CalcType)
				{
				case ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum.Percent:

					switch(AttributeModifiers[i].ApplyTo)
					{
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Max:
						
						Attributes[_attributeTypeIndex].Modifiers += (Attributes[_attributeTypeIndex].Max * AttributeModifiers[i].Value);

						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Current:

						Attributes[_attributeTypeIndex].Current += (Attributes[_attributeTypeIndex].Current * AttributeModifiers[i].Value);

						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Both:

						Attributes[_attributeTypeIndex].Modifiers += (Attributes[_attributeTypeIndex].Max * AttributeModifiers[i].Value);
						Attributes[_attributeTypeIndex].Current += (Attributes[_attributeTypeIndex].Current * AttributeModifiers[i].Value);

						break;
					}

					break;
				case ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum.Value:

					switch(AttributeModifiers[i].ApplyTo)
					{
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Max:

						Attributes[_attributeTypeIndex].Modifiers += (Attributes[_attributeTypeIndex].Max + AttributeModifiers[i].Value);

						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Current:

						Attributes[_attributeTypeIndex].Current += (Attributes[_attributeTypeIndex].Current + AttributeModifiers[i].Value);

						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Both:

						Attributes[_attributeTypeIndex].Modifiers += (Attributes[_attributeTypeIndex].Max + AttributeModifiers[i].Value);
						Attributes[_attributeTypeIndex].Current += (Attributes[_attributeTypeIndex].Current + AttributeModifiers[i].Value);

						break;
					}

					break;
				}

				// Marca o atributo como consumido
				AttributeModifiers[i].Consumed = true;
			}
		}
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