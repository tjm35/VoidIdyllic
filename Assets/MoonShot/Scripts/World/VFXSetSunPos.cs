using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Moonshot.World
{
	[RequireComponent(typeof(VisualEffect))]
	public class VFXSetSunPos : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
			m_vfx = GetComponent<VisualEffect>();
			m_sunPosNameID = Shader.PropertyToID("SunPos");
		}

		// Update is called once per frame
		void Update()
		{
			m_vfx.SetVector3(m_sunPosNameID, LocalFrame.TransformPointToLocal(LocalFrame.Get(transform), Vector3.zero));
		}

		private VisualEffect m_vfx;
		private int m_sunPosNameID;
	}
}