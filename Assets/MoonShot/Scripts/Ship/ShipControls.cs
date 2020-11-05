using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moonshot.Ship
{
	public class ShipControls : MonoBehaviour
	{
		public InputActionReference Move2DAction;
		public InputActionReference Look2DAction;
		public InputActionReference RollAction;

		public Vector3 Move => new Vector3(Move2D.x, 0.0f, Move2D.y);
		public Vector3 Look => new Vector3(Look2D.y, Look2D.x, Roll);

		public Vector2 Move2D => Move2DAction.action.ReadValue<Vector2>();
		public Vector2 Look2D => Look2DAction.action.ReadValue<Vector2>();
		public float Roll => RollAction.action.ReadValue<float>();
	}
}