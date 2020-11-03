using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Totality
{
	public static class CharacterPhysicsTypes
	{
		/// <summary>
		/// A delegate to apply reactions to colliders that are being deebedded from.
		/// </summary>
		/// <param name="i_collider">The collider that has been deembedded from.</param>
		/// <param name="i_deembedDirection">The direction in which the deembedded collider has been deembedded (typically *away* from i_collider).</param>
		public delegate void DeembedReaction(Object i_collider, Vector3 i_deembedDirection);
	}

	/// <summary>
	/// Interface to a class that implements the physics queries that would be required by a character controller, both for sharing and to support easy migration between physics systems.
	/// </summary>
	public interface ICharacterPhysics
	{
		/// <summary>
		/// Perform deembedding on a collider as it moves.
		/// </summary>
		/// <param name="i_collider">The collider to deembed. Can be and non-mesh collider. Unity bugs in 5.6 may require it to have a zero center?</param>
		/// <param name="io_delta">The delta the collider is moving through. Will be modified if we need to deembed (i.e. if this function returns true).</param>
		/// <param name="i_reaction">A reaction function to be applied to any other colliders the collider is deembedded from. Default applies no reaction.</param>
		/// <param name="i_layerMask">A mask for collision layers to consider when deembedding the character. Triggers and the collider itself will always be ignored.</param>
		/// <param name="i_deembedRepeats">The number of times to repeat the deembed cycle, in case the collider is trapped between mutiple colliders. Default 3.</param>
		/// <returns>true if a deembed has occurred, false otherwise.</returns>
		bool Deembed(Object i_collider, ref Vector3 io_delta, CharacterPhysicsTypes.DeembedReaction i_reaction = null, int i_layerMask = Physics.AllLayers, int i_deembedRepeats = 3);

		/// <summary>
		/// Probe for whether ground exists between top and bottom probe positions.
		/// </summary>
		/// <param name="o_groundPos">If ground is found, this is the point at the base of the sphere where it hits it. This should always be colinear with the probe.</param>
		/// <param name="o_hitInfo">If ground is found, this raycast contains information about the surface. Note that o_hitInfo.point may not match groundPos.</param>
		/// <param name="i_probeTop">The top of a downward probe seeking ground.</param>
		/// <param name="i_probeBottom">The bottom of a downward probe seeking ground. Note that the "down" direction is determined entirely by the relative positions of the probe points.</param>
		/// <param name="i_radius">The radius of the character for whom ground is being probed.</param>
		/// <param name="i_groundSlopeLimit">Angle limit for how close a surface normal must be to "up" for it to be acceptable ground. In degrees.</param>
		/// <param name="i_layerMask">A mask of object layers to consider collidable. Defaults to all layers. Note that this must include nonground solid surfaces.</param>
		/// <param name="i_hitFilter">A condition on which surfaces should be considered collidable. Defaults to all. Note that this must include nonground solid surfaces.</param>
		/// <returns>true if the probe has found a candidate for "ground", false otherwise.</returns>
		bool ProbeGround(out Vector3 o_groundPos, out RaycastHit o_hitInfo, Vector3 i_probeTop, Vector3 i_probeBottom, float i_radius, float i_groundSlopeLimit, int i_layerMask = Physics.AllLayers, System.Func<RaycastHit, bool> i_hitFilter = null);

		/// <summary>
		/// Make a Collider suitable for use as a character collider.
		/// It will be created as a new gameobject child of the provided transform.
		/// </summary>
		/// <param name="i_parent">The transform to create the new collider as a child of.</param>
		/// <param name="i_name">A human-readable name for the collider object, primarily for debugging purposes.</param>
		/// <returns></returns>
		Object MakeCharacterCollider(Transform i_parent, string i_name = "CharacterPhysics Collider");

		/// <summary>
		/// Configure a character collider with extents and orientation. Will be based at the origin and point along the provided up axis.
		/// </summary>
		/// <param name="i_collider">A character collider to configure. Must have been created with CharacterPhysics.MakeCharacterCollider</param>
		/// <param name="i_radius">Character radius.</param>
		/// <param name="i_height">Character height.</param>
		/// <param name="i_upAxis">Up axis to orient the character along (in local space). Default Vector3.up.</param>
		/// <param name="i_offset">Offset to apply to the collider (in local space). Defaults to zero, which places the base of the capsule at the parent transform.</param>
		void ConfigureCharacterCollider(Object i_collider, float i_radius, float i_height, Vector3? i_upAxis = null, Vector3? i_offset = null);
	}
}