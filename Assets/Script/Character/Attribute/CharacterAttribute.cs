using System;

/// <summary>
/// Classe que define um atributo do personagem
/// </summary>
[Serializable]
public class CharacterAttribute
{
	/// <summary>
	/// Tipo do Atributo
	/// </summary>
	public ENUMERATORS.Attribute.CharacterAttributeTypeEnum AttributeType;

	/// <summary>
	/// Valor maximo para o atributo
	/// </summary>
	public float Max;

	/// <summary>
	/// Valor do atributo atual
	/// </summary>
	public float Current;

	/// <summary>
	/// Valor dos modificadores
	/// </summary>
	public float Modifiers;

	/// <summary>
	/// Valor dos Bufs (Modificadores) aplicados + o valor maximo
	/// </summary>
	/// <value>The max buffed.</value>
	public float MaxWithModifiers
	{
		get { return Max + Modifiers; }
	}

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


