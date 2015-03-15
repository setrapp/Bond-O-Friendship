using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementGesture : MonoBehaviour {

	public List<Vector3> pastBetweenPlayers;
	public GameObject tempPrefab;

	void Update()
	{
		CharacterComponents player1 = Globals.Instance.player1.character;
		CharacterComponents player2 = Globals.Instance.player2.character;
		transform.position = (player1.transform.position + player2.transform.position) / 2;

		Vector3 betweenPlayers = player2.transform.position - player1.transform.position;
		betweenPlayers.z = 0;
		if (pastBetweenPlayers.Count > 0)
		{
			// If the vector between players is perpendicular to the most recent past vector between them, add it as a new record.
			if (Vector3.Dot(betweenPlayers, pastBetweenPlayers[pastBetweenPlayers.Count - 1]) <= 0)
			{
				// If the new vector between players is similar to the vector before the previous, the players have reversed direction, so the list should remove obsolete records.
				if (pastBetweenPlayers.Count > 1 && Vector3.Dot(betweenPlayers, pastBetweenPlayers[pastBetweenPlayers.Count - 2]) > 0)
				{
					while (pastBetweenPlayers.Count > 1)
					{
						pastBetweenPlayers.RemoveAt(0);
					}
				}

				pastBetweenPlayers.Add(betweenPlayers);
				//Instantiate(tempPrefab, player1.transform.position, Quaternion.identity);
				//Instantiate(tempPrefab, player2.transform.position, Quaternion.identity);
			}
		}
		else
		{
			pastBetweenPlayers.Add(betweenPlayers);
		}


		if (pastBetweenPlayers.Count >= 4)
		{
			pastBetweenPlayers.RemoveAt(0);

			if(Vector3.Cross(pastBetweenPlayers[pastBetweenPlayers.Count - 1], pastBetweenPlayers[pastBetweenPlayers.Count - 2]).z > 0)
			{
				player1.gestureStats.PullBondIn(player2.bondAttachable);
				player2.gestureStats.PullBondIn(player1.bondAttachable);
			}
			else
			{
				player1.gestureStats.PushBondOut(player2.bondAttachable);
				player2.gestureStats.PushBondOut(player1.bondAttachable);
			}
		}

		/* TODO make subclasses that will use gestures to affect players and environment*//*alternatively send the data to other components to handle the gesture*/
		/* TODO allow for single player gestures that work similarly but are not as effective*/
		/* TODO should gestures affect character appearance*/
	}
}
