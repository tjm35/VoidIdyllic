using System.Collections;
using System.Collections.Generic;
using Moonshot.Photos;
using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Moonshot.UI
{
	public class Brochure : MonoBehaviour
	{
		[Serializable]
		public struct PhotoSlot
		{
			[Required]
			public RawImage m_slot;
			public List<Goal> m_matchingGoals;
		}

		[ReorderableList]
		public List<PhotoSlot> m_slots;
		public StarRating m_starRating;

		public void Build(IEnumerable<PhotoSystem.IPhotoData> i_photos, float i_rating)
		{
			var photosAvailable = i_photos.ToList();
			var assignments = new Dictionary<PhotoSlot, PhotoSystem.IPhotoData>();

			// First pass photo assignment: Assign photos that are the only choice for a slot.
			foreach (var slot in m_slots)
			{
				var suitable = photosAvailable.Where(p => slot.m_matchingGoals.All(g => p.GoalsMet.Contains(g)));
				if (suitable.Count() == 1)
				{
					var photo = suitable.First();
					assignments[slot] = photo;
					photosAvailable.Remove(photo);
				}
			}

			// Second pass photo assignment: Assign any photo suitable for reamining slots, picking the first for preference.
			foreach (var slot in m_slots)
			{
				if (!assignments.ContainsKey(slot))
				{
					var suitable = photosAvailable.Where(p => slot.m_matchingGoals.All(g => p.GoalsMet.Contains(g)));
					if (suitable.Count() >= 1)
					{
						var photo = suitable.First();
						assignments[slot] = photo;
						photosAvailable.Remove(photo);
					}
				}
			}

			// Third pass photo assignment: Assign any remaining photos to remianing slots.
			foreach (var slot in m_slots)
			{
				if (!assignments.ContainsKey(slot))
				{
					if (photosAvailable.Count() >= 1)
					{
						var photo = photosAvailable.First();
						assignments[slot] = photo;
						photosAvailable.Remove(photo);
					}
				}
			}

			// Fourth pass photo assignment: Fill any remianing slots with random duplicate photos. Should never happen in final game.
			foreach (var slot in m_slots)
			{
				if (!assignments.ContainsKey(slot))
				{
					var photo = i_photos.ElementAt(UnityEngine.Random.Range(0, i_photos.Count()));
					assignments[slot] = photo;
				}
			}

			// Apply assignments
			foreach (var kvp in assignments)
			{
				kvp.Key.m_slot.texture = kvp.Value.FullTexture;
			}

			// Set star rating
			m_starRating.Rating = i_rating;
		}
	}
}