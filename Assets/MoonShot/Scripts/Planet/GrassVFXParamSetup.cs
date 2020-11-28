using Moonshot.Player;
using Moonshot.World;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

namespace Moonshot.Planet
{
	[ExecuteAlways]
	[RequireComponent(typeof(VisualEffect))]
	public class GrassVFXParamSetup : MonoBehaviour
	{
		private void Start()
		{
			m_vfx = GetComponent<VisualEffect>();
			m_lightPosLocalID = Shader.PropertyToID("LightPosLocal");
		}

		private void LateUpdate()
		{
			LocalFrame frame = LocalFrame.Get(transform);
			m_vfx.SetVector3(m_lightPosLocalID, transform.InverseTransformPoint(LocalFrame.TransformPointToLocal(frame, Vector3.zero)));
		}

		private VisualEffect m_vfx;
		private int m_lightPosLocalID;
	}
}