using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Moonshot.Player
{
	public class PlayerVehiclePositionConstraint : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
			m_positionConstraint = GetComponent<PositionConstraint>();
			if (m_positionConstraint == null)
			{
				m_positionConstraint = gameObject.AddComponent<PositionConstraint>();
			}
			m_positionConstraint.AddSource(m_constraintSource);
			m_positionConstraint.weight = 1.0f;
			m_positionConstraint.constraintActive = true;
			m_positionConstraint.translationOffset = Vector3.zero;
		}

		// Update is called once per frame
		void Update()
		{
			m_constraintSource.weight = 1.0f;
			m_constraintSource.sourceTransform = PlayerVehicle.Current ? PlayerVehicle.Current.transform : null;
			m_positionConstraint.SetSource(0, m_constraintSource);
		}

		private PositionConstraint m_positionConstraint;
		private ConstraintSource m_constraintSource = new ConstraintSource();
	}
}