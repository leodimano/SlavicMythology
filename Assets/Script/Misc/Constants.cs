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
		public const int ATTRIBUTE_COUNT = 9;
		public const int ATTRIBUTE_MODIFIERS_COUNT = 100;

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

	public class INPUT
	{
		public const string HORIZONTAL_AXIS = "Horizontal";
		public const string VERTICAL_AXIS = "Vertical";
		public const string MOUSE_X = "Mouse X";
		public const int MOUSE_LEFT_BUTTON = 0;
		public const int MOUSE_RIGHT_BUTTON = 1;
	}

	public class TAGS
	{
		public const string PLAYER = "Player";
	}

	public class ANIMATION
	{
		public const string SPEED = "Speed";
	}

	public class SPELL
	{
		public const int COUNT = 10;
	}

	public class ITEM
	{
		public const int PROJECTILE_COUNT = 10;
	}

	public class RESOURCES_PATH
	{
		public const string SPELL_FIREBALL = "Prefab/Spell/SPELL_FireBall";

		public const string PROJECTILE_ARROW_PROTOTYPE = "Prefab/Item/Projectile/PrototypeArrow";
	}
}

