using Moonshot.Planet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Orrery
{
	public class OrreryModel : MonoBehaviour
	{
		public float m_scale = 0.0005f;
		public float m_additionalBodyScale = 1.0f;
		public bool m_continuousRebuild = false;
		public GameObject m_body;

		// Start is called before the first frame update
		void Start()
		{
			Build();
		}

		// Update is called once per frame
		void Update()
		{
			if (m_continuousRebuild)
			{
				while (transform.childCount > 0)
				{
					var child = transform.GetChild(0);
					child.SetParent(null);
					Destroy(child.gameObject);
				}
				Build();
			}
		}

		private void Build()
		{
			var orrery = OrreryTimeSource.Global.transform;

			foreach (var planet in orrery.GetComponentsInDescendents<OrreryPlanet>())
			{
				var body = Instantiate(m_body);
				body.transform.localPosition = planet.transform.localPosition * m_scale;
				body.transform.localScale = planet.transform.localScale * planet.Radius * m_scale * m_additionalBodyScale;
				body.SetActive(true);

				var workingCreatedTransform = body.transform;
				var searchTransform = planet.transform;
				while (searchTransform != null && searchTransform != orrery)
				{
					var rf = searchTransform.GetComponent<RotatingFrame>();
					var of = searchTransform.GetComponent<OrbitalFrame>();
					if (rf != null || of != null)
					{
						var newParent = new GameObject();
						if (rf)
						{
							var newRF = newParent.AddComponent<RotatingFrame>();
							newRF.CopySettings(rf);
						}
						if (of)
						{
							var newOF = newParent.AddComponent<OrbitalFrame>();
							newOF.CopySettings(of);
							newOF.m_semimajorAxis *= m_scale;
						}
						workingCreatedTransform.SetParent(newParent.transform, false);
						workingCreatedTransform = newParent.transform;
					}

					searchTransform = searchTransform.parent;
				}

				workingCreatedTransform.SetParent(transform, false);
			}
		}
	}
}