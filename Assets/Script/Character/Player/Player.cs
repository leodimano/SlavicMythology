using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
public class Player : Character {

	PlayerInput _playerInput;

	bool DebugEnabled;

	// Use this for initialization
	protected override void Start () {
		// NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
		base.Start();
		// Codifique daqui para baixo;

		_playerInput = GetComponent<PlayerInput>();
		base.CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Player;
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
			_moveToPosition *= Speed;
		else if (Input.GetAxisRaw("Horizontal") != 0)
			_moveToPosition *= Speed/1.7f;
		else
			_moveToPosition *= Speed/2f;

		base.CurrentSpeed = _moveToPosition.magnitude;

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
			if (StartSpellCast(0)) // TODO: Associar a spell ao botao
				DoSpellCast(); // TODO: METODO DEVE SER CHAMADO NO RETORNO DA ANIMACAO
		}
	}

	/// <summary>
	/// Metodo responsavel por gerenciar as animacoes
	/// </summary>
	void HandleAnimation()
	{
		if (_animator != null)
		{
			_animator.SetFloat(CONSTANTS.ANIMATION.SPEED, base.CurrentSpeed);
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
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Speed: {0}", Speed), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("CurrentSpeed: {0}", CurrentSpeed), _guiStyle);
		CurrentPosition.y += newLineSize;

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
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Display Order: {0}", _charAttribute.DisplayOrder), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Current: {0}", _charAttribute.Current), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Max: {0}", _charAttribute.Max), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Modifiers: {0}", _charAttribute.Modifiers), _guiStyle);
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
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("OriginID: ", _attrModifier.OriginID), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ModifierType: ", _attrModifier.ModifierType), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("AttributeType: ", _attrModifier.AttributeType), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("CalcType: ", _attrModifier.CalcType), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ApplyTo: ", _attrModifier.ApplyTo), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Value: ", _attrModifier.Value), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("TimeInSeconds: ", _attrModifier.TimeInSeconds), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("InitialTime: ", _attrModifier.InitialTime), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ExpireTime: ", _attrModifier.ExpireTime), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Consumed: ", _attrModifier.Consumed), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
			}
		}

		StarPosition.x += newColumnSize;
		CurrentPosition = StarPosition;

		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), "Spells", _guiStyle);
		CurrentPosition.y += newLineSize;
		attributeLineSize = 0.65f;
		SpellBase _spellBase = null;
		for(int i = 0; i < SpellCoolDownTable.Length; i++)			
		{
			_spellBase = ApplicationModel.Instance.SpellTable[i];
			if (_spellBase != null)
			{
				_guiStyle.fontSize = 12;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Spell ID: {0}", _spellBase.ID), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("NeedTarget: {0}", _spellBase.NeedTarget), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("AttributeModifiers: NOT IMPLEMENTED"), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Damage: {0}", _spellBase.Damage), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("CoolDown: {0}", _spellBase.CoolDown), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ManaCost: {0}", _spellBase.ManaCost), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;				
				
			}
		}
	}
}