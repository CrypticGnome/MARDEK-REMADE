using System;
using System.Collections.Generic;
using MARDEK.Core;
using MARDEK.Event;
using UnityEngine;

namespace MARDEK.Movement
{
	public class MoveCommand : OngoingCommand
	{
		[SerializeField] Movable target = null;
		[SerializeField] List<Moves> moves;


		public override bool IsOngoing()
		{
			return target && target.isMoving;
		}

		public override void Trigger()
		{
			if (target == null)
				target = PlayerController.GetPlayerMovement();
			if (target)
				target.EnqueueMoves(moves);
		}

	}
	[Serializable]
	public class Moves
	{
		public MoveDirection Direction;
		public int Count;
	}
}