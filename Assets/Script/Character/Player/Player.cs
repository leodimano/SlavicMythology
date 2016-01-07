using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
public class Player : Character {

	PlayerInput _playerInput;


	GodShrine _godShrineOnRange;

	bool DebugEnabled;

	// Use this for initialization
	protected override void Start () {
		// NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
		base.Start();
		// Codifique daqui para baixo;

		_playerInput = GetComponent<PlayerInput>();
		base.CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Player;

		SetInitialSpell();
		SetInitialAttributes();
	}
	
	// Update is called once per frame
	protected override void Update () {
		// NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
		base.Update();
		// Codifique daqui para baixo;

		HandleActions();
		HandleAnimation();

		// Habilitar o Modo Debug do Personagem
		if (Input.GetKeyDown(KeyCode.F2)) 
			DebugEnabled = !DebugEnabled;
	}

	// Fixed Update
	protected override void FixedUpdate(){
		// NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
		base.FixedUpdate();
		// Codifique daqui para baixo;

		HandleMovement();
		HandleRotation();
	}

	// Late Update
	protected override void LateUpdate()
	{
		// Codifique daqui para baixo;


		base.LateUpdate();
		// NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
	}

	void OnTriggerEnter(Collider collider_)
	{
		GodShrine _godShrine = collider_.gameObject.GetComponent<GodShrine>();

		if (_godShrine != null)
		{
			_godShrineOnRange = _godShrine;
		}
	}

	void OnTriggerExit(Collider collider_)
	{
		GodShrine _godShrine = collider_.gameObject.GetComponent<GodShrine>();

		if (_godShrine != null)
		{
			_godShrineOnRange = null;
		}		
	}

	/// <summary>
	/// Metodo responsavel por disponibilizar as habilidades para o Jogador
	/// </summary>
	void SetInitialSpell()
	{
		// Seta o FireBall para ser utilizado
		CharacterSpellTable[0] = new CharacterSpell();
		CharacterSpellTable[0].CoolDownTime = 0;
		CharacterSpellTable[0].Spell = ApplicationModel.Instance.SpellTable[0];
	}

	/// <summary>
	/// Metodo responsavel por atribuir os atributos iniciais
	/// </summary>
	void SetInitialAttributes()
	{
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint].Max = 1000f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint].Current = 1000f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.ManaPoint].Max = 800f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.ManaPoint].Current = 800f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.AttackSpeed].Max = 50f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MeleeAttack].Max = 100f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MeleeDefense].Max = 50f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MagicAttack].Max = 150f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.MagicDefense].Max = 40f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticChance].Max = 0.15f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticMultiplier].Max = 2f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].Max = 10f;
	}

	#region Private Methods

	/// <summary>
	/// Metodo responsavel por gerenciar a movimentacao do personagem
	/// </summary>
	void HandleMovement()
	{
		Vector3 _moveToPosition = new Vector3();

		// Se o personagem esta se movimentando
		if (_playerInput.Move_Y != 0)
		{
			_moveToPosition = this.transform.forward * _playerInput.Move_Y;
		}

		if (_playerInput.Move_X != 0)
		{
			_moveToPosition += this.transform.right * _playerInput.Move_X;
		}

		_moveToPosition.Normalize();
		//_moveToPosition *= Speed;

		//Altera a velocidade de movimentação de acordo com a direção do personagem
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			_moveToPosition *= Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].MaxWithModifiers;
		else if (Input.GetAxisRaw("Horizontal") != 0)
			_moveToPosition *= Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].MaxWithModifiers / 1.7f;
		else
			_moveToPosition *= Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].MaxWithModifiers /2f;

		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].Current = _moveToPosition.magnitude;

		_moveToPosition *= Time.fixedDeltaTime;
		_moveToPosition += transform.position;

		_rigidBody.MovePosition(_moveToPosition);
		_rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0); // Zera a velocidade para evitar movimentacoes desnecessarias do personagem
	}

	/// <summary>
	/// Metodo responsavel por gerenciar a rotacao do personagem
	/// </summary>
	void HandleRotation()
	{
		// Rotaciona o personagem de acordo com o Eixo X do Mouse * Sensibilidade * DeltaTime
		transform.Rotate(0, _playerInput.MouseX * Time.fixedDeltaTime, 0);
	}

	/// <summary>
	/// Metodo responsavel por gerenciar as acoes do personagem
	/// </summary>
	void HandleActions()
	{
		if (_playerInput.Action2IsPressed)
		{
			if (StartSpellCast(CharacterSpellTable[0].Spell.ID)) // TODO: Associar a spell ao botao
				DoSpellCast(); // TODO: METODO DEVE SER CHAMADO NO RETORNO DA ANIMACAO
		}

		if (_playerInput.ActionPressed)
		{
			if (_godShrineOnRange != null)
			{
				AddAttributeModifier(_godShrineOnRange.Activate());
				_godShrineOnRange = null;
			}
		}
	}

	/// <summary>
	/// Metodo responsavel por gerenciar as animacoes
	/// </summary>
	void HandleAnimation()
	{
		if (_animator != null)
		{
			_animator.SetFloat(CONSTANTS.ANIMATION.SPEED, Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].Current);
		}
	}

	#endregion

	void OnGUI()
	{
		if (DebugEnabled)
			DrawDebugMode();		
	}

	void DrawDebugMode()
	{
		float TabSize = 15f;
		float LabelSize = 150f;
		float newLineSize = 15f;
		float newColumnSize = LabelSize + TabSize;

		GUIStyle _guiStyle = new GUIStyle();
		_guiStyle.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		_guiStyle.fontStyle = FontStyle.Bold;

		Vector2 StarPosition = new Vector2(0, 0);
		Vector2 CurrentPosition = StarPosition;

		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), "Player DEBUG MODE", _guiStyle);
		CurrentPosition.y += newLineSize * 2;

		// Atualiza a posicao inicial para criar a tabela
		StarPosition = CurrentPosition;

		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), "Attributes", _guiStyle);
		CurrentPosition.y += newLineSize;

		float attributeLineSize = 0.65f;
		foreach(CharacterAttribute _charAttribute in Attributes)
		{
			_guiStyle.fontSize = 12;
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), _charAttribute.AttributeType.ToString(), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;			
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Name: {0}", _charAttribute.Name), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Current: {0}", _charAttribute.Current), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Current Modifier: {0}", _charAttribute.CurrentModifiers), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Current W Modifier: {0}", _charAttribute.CurrentWithModifiers), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Max: {0}", _charAttribute.Max), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Max Modifiers: {0}", _charAttribute.MaxModifiers), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Max W Modifiers: {0}", _charAttribute.MaxWithModifiers), _guiStyle);
			CurrentPosition.y += newLineSize;	
		}

		StarPosition.x += newColumnSize;
		CurrentPosition = StarPosition;

		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), "Attributes Modifiers", _guiStyle);
		CurrentPosition.y += newLineSize;
		attributeLineSize = 0.65f;
		foreach(AttributeModifier _attrModifier in AttributeModifiers)
		{
			if (_attrModifier != null){
				_guiStyle.fontSize = 12;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("OriginID: {0}", _attrModifier.OriginID), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ModifierType: {0}", _attrModifier.ModifierType), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("AttributeType: {0}", _attrModifier.AttributeType), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("CalcType: {0}", _attrModifier.CalcType), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ApplyTo: {0}", _attrModifier.ApplyTo), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Value: {0}", _attrModifier.Value), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("TimeInSeconds: {0}", _attrModifier.TimeInSeconds), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("InitialTime: {0}", _attrModifier.InitialTime), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ExpireTime: {0}", _attrModifier.ExpireTime), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Consumed: {0}", _attrModifier.Consumed), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
			}
		}

		StarPosition.x += newColumnSize;
		CurrentPosition = StarPosition;

		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), "Character Spell", _guiStyle);
		CurrentPosition.y += newLineSize;
		attributeLineSize = 0.65f;
		for(int i = 0; i < CharacterSpellTable.Length; i++)			
		{
			CharacterSpell _charSpell = CharacterSpellTable[i];

			if(_charSpell != null)
			{
				_guiStyle.fontSize = 12;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Spell Next Cast: {0}", _charSpell.CoolDownTime), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Spell ID: {0}", _charSpell.Spell.ID), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("NeedTarget: {0}", _charSpell.Spell.NeedTarget), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("AttributeModifiers: NOT IMPLEMENTED"), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Damage: {0}", _charSpell.Spell.Damage), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("CoolDown: {0}", _charSpell.Spell.CoolDown), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ManaCost: {0}", _charSpell.Spell.ManaCost), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;		
			}

		}
	}
}