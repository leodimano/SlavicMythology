using System;


public enum CharacterAttributeTypeEnum
{
	MeleeAttack = 0,
	MeleeDefense = 1,
	MagicAttack = 2,
	MagicDefense = 3,
	HitPoint = 4,
	ManaPoint = 5,
	AttackSpeed = 6,
	CriticMultiplier = 7,
	CriticChance = 8
}




/// <summary>
/// Classe que define um atributo do personagem
/// </summary>
public class CharacterAttribute
{
	/// <summary>
	/// Tipo do Atributo
	/// </summary>
	public CharacterAttributeTypeEnum AttributeType;

	/// <summary>
	/// Valor maximo para o atributo
	/// </summary>
	public float Max;

	/// <summary>
	/// Valor Maximo para o Atributo + Buffs;
	/// </summary>
	public float MaxBuffed;

	/// <summary>
	/// Valor do atributo atual
	/// </summary>
	public float Current;

	/// <summary>
	/// Valor do atributo Atual + Buff;
	/// </summary>
	public float CurrentBuffed;

	/// <summary>
	/// Retorna o nome do atributo.
	/// </summary>
	/// <value>Nome do atributo.</value>
	public string Name;

	/// <summary>
	/// Ordem que o atributo deve ser apresentado;
	/// </summary>
	public byte DisplayOrder;
}


