using System;
using UnityEngine;

namespace SilgonGalaxy.Ships {
	using Extensions;
	
	[Serializable]
	internal sealed class Rotation {
		public Config config;

		public void EvaluateTurn(Rigidbody2D rb, float topSpeedFraction, float hInput, float dt) {
			// We're only considering the forward speed, as it's pretty much guaranteed to be higher than the reverse speed,
			// and we should still have some pretty reasonable responsiveness when going backwards.
			var responsiveness = 1 - topSpeedFraction * (1 - config.topSpeedSteering);
			var targetSpeed = hInput * config.topTurnSpeed * responsiveness;
			var acceleration = config.topTurnSpeed / (config.turnAccelerationTime + float.Epsilon);

			// We instantly snap to 0 angular velocity if we're not actively trying to turn into the current direction of rotation.
			if (targetSpeed == 0 || targetSpeed.DifferentSign(rb.angularVelocity))
				rb.angularVelocity = 0;
			else
				// It takes time to get to a higher angular velocity.
				rb.angularVelocity = rb.angularVelocity.MoveTowards(
					targetSpeed,
					acceleration * responsiveness * dt
				);
		}

		[Serializable]
		public struct Config {
			public float turnAccelerationTime;
			[Min(0)]
			public float topTurnSpeed;
			[Range(0, 1)]
			public float topSpeedSteering;
		}
	}
}