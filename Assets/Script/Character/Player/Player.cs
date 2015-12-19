using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
public class Player : Character {

	PlayerInput _playerInput;

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
		_moveToPosition *= Speed;
		base.CurrentSpeed = _moveToPosition.magnitude;

		_moveToPosition *= Time.fixedDeltaTime;
		_moveToPosition += transform.position;

		_rigidBody.MovePosition(_moveToPosition);
		_rigidBody.velocity = Vector3.zero; // Zera a velocidade para evitar movimentacoes desnecessarias do personagem
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
}
