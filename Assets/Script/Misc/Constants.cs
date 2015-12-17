using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Classe responsavel por manter todas as constantes do jogo
/// </summary>
public class CONSTANTS
{
	/// <summary>
	/// Classe responsavel por manter as constantes de atributos
	/// </summary>
	public class ATTRIBUTES
	{
		public const int COUNT = 9;

		public static string[] TYPE_NAMES = new string[]{
			"Vigor",
			"Resistencia",
			"Inteligencia",
			"**Defesa Magica**",
			"Vitalidade",
			"Sabedoria",
			"Agilidade",
			"Conhecimento",
			"Ira"
		};
	}

	public class VECTORS
	{
		public static Vector3 __DEFAULT_VECTOR3__ = new Vector3();
	}

	public class INPUT
	{
		public const string HORIZONTAL_AXIS = "Horizontal";
		public const string VERTICAL_AXIS = "Vertical";
		public const string MOUSE_X = "Mouse X";
	}

	public class TAGS
	{
		public const string PLAYER = "Player";
	}

	public class ANIMATION
	{
		public const string SPEED = "Speed";
	}
}

