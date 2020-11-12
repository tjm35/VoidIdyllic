using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocomotionControls : MonoBehaviour
{
	public Vector3 MoveInstructionWS => transform.TransformDirection(new Vector3(Move.x, 0.0f, Move.y));

	public Vector2 m_mouseSensitivity = Vector2.one;
	public float m_jumpStoreTime = 1.0f;

	private bool m_jumpStored = false;
	private float m_jumpStoreTimer = 0.0f;

	public InputActionReference MoveAction;
	public InputActionReference LookAction;
	public InputActionReference JumpAction;

	public Vector2 Move
	{
		get
		{
			return MoveAction.action.ReadValue<Vector2>();
		}
	}
	public Vector2 Look
	{
		get
		{
			return LookAction.action.ReadValue<Vector2>();
		}
	}

	public bool PullJump()
	{
		bool res = m_jumpStored && m_jumpStoreTimer < m_jumpStoreTime;
		m_jumpStored = false;
		return res;
	}

	public bool Jump
	{
		get
		{
			return JumpAction.action.ReadValue<float>() > 0.5f;
		}
	}

	/*
	public bool PullInteract()
	{
		return Input.GetButtonDown("Interact");
	}

	public bool Sprint
	{
		get
		{
			return Input.GetButton("Sprint");
		}
	}

	public bool Crouch
	{
		get
		{
			return Input.GetButton("Crouch");
		}
	}

	public bool Interact
	{
		get
		{
			return Input.GetButton("Interact");
		}
	}

	private Vector2 ControllerLook
	{
		get
		{
			return new Vector2(Input.GetAxis("RHorz"), Input.GetAxis("RVert"));
		}
	}

	private Vector2 MouseLook
	{
		get
		{
			return Vector2.Scale(m_mouseSensitivity, new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")));
		}
	}
	*/

	private void Start()
	{
		JumpAction.action.started += OnJumpPressed;
	}

	private void OnJumpPressed(InputAction.CallbackContext i_context)
	{
		m_jumpStored = true;
		m_jumpStoreTimer = 0.0f;
	}

	private void Update()
	{
		m_jumpStoreTimer += Time.deltaTime;
	}
}
