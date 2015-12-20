﻿using UnityEngine;
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
	public CharacterAttribute[] Attributes = InitializeAttributes();
	public AttributeModifier[] AttributeModifiers = new AttributeModifier[CONSTANTS.ATTRIBUTES.ATTRIBUTE_MODIFIERS_COUNT];
	public float Speed;
	[HideInInspector] public float CurrentSpeed;
	[HideInInspector] public float[] SpellCoolDownTable;
	[HideInInspector] public int DoSpellID; // ID da Spell que deve ser utilizada
	System.Random _pseudoRandom;

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

		//InitializeAttributes(); // TODO: Carregar os atributos do jogador Salvo ou nao. Se inimigo carregar os atributos baseado na tabela de atributos
		AttributeModifiers = new AttributeModifier[CONSTANTS.ATTRIBUTES.ATTRIBUTE_MODIFIERS_COUNT];

		DoSpellID = -1; // Inicializa a variavel de controle de habilidades
		SpellCoolDownTable = new float[CONSTANTS.SPELL.COUNT];
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
	private static CharacterAttribute[] InitializeAttributes()
	{
		return
			new CharacterAttribute[CONSTANTS.ATTRIBUTES.ATTRIBUTE_COUNT]{
			new CharacterAttribute(){ 			
				Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint], 
				AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint,
				Max = 0,
				Current = 0,
				Modifiers = 0,
				DisplayOrder = 0},
			new CharacterAttribute(){ 			
				Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.ManaPoint], 
				AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.ManaPoint,
				Max = 0,
				Current = 0,
				Modifiers = 0,
				DisplayOrder = 1},
			new CharacterAttribute(){ 			
				Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MeleeAttack], 
				AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MeleeAttack,
				Max = 0,
				Current = 0,
				Modifiers = 0,
				DisplayOrder = 2},
			new CharacterAttribute(){ 			
				Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MagicAttack], 
				AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MagicAttack,
				Max = 0,
				Current = 0,
				Modifiers = 0,
				DisplayOrder = 0},
			new CharacterAttribute(){ 			
				Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MeleeDefense], 
				AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MeleeDefense,
				Max = 0,
				Current = 0,
				Modifiers = 0,
				DisplayOrder = 0},
			new CharacterAttribute(){ 			
				Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MagicDefense], 
				AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MagicDefense,
				Max = 0,
				Current = 0,
				Modifiers = 0,
				DisplayOrder = 0},
			new CharacterAttribute(){ 			
				Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.AttackSpeed], 
				AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.AttackSpeed,
				Max = 0,
				Current = 0,
				Modifiers = 0,
				DisplayOrder = 0},
			new CharacterAttribute(){ 			
				Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticChance], 
				AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticChance,
				Max = 0,
				Current = 0,
				Modifiers = 0,
				DisplayOrder = 0},
			new CharacterAttribute(){ 			
				Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticMultiplier], 
				AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticMultiplier,
				Max = 0,
				Current = 0,
				Modifiers = 0,
				DisplayOrder = 0}
		};		
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Metodo responsavel por gerenciar a morte do personagem, deve ser sobrescrito nas classes que herdam de Character
	/// </summary>
	public virtual void Die()
	{
		
	}

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

	/// <summary>
	/// Metodo responsavel por preparar a animacao de cast
	/// </summary>
	/// <param name="spellID_">ID da Habilidade que sera executada</param>
	public bool StartSpellCast(int spellID_)
	{
		// Verifica se a habilidade esta em Cooldown
		// TODO: Verifica se tem mana suficienta para utilizar a habilidade
		if (Time.time > SpellCoolDownTable[spellID_])
		{
			DoSpellID = spellID_;
			return true;
		}
		else
		{
			// TODO: MENSAGEM INFORMANDO QUE A HABILIDADE ESTA EM COOLDOWN
			return false;
		}
	}

	/// <summary>
	/// Metodo responsavel por executar a habilidade
	/// </summary>
	public void DoSpellCast()
	{
		if (DoSpellID > -1){

			SpellBase _spell = ApplicationModel.Instance.SpellTable[DoSpellID].Pool.GetFromPool() as SpellBase;

			if (_spell != null){
				
				_spell.Caster = this;
				_spell.transform.position = GetForwardPosition + Vector3.up;
				_spell.transform.rotation = transform.rotation;

				SpellCoolDownTable[DoSpellID] = Time.time + _spell.CoolDown;
			}

		}

		DoSpellID = -1;
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

		if (HitPoint.Current == 0)
		{
			Die();
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