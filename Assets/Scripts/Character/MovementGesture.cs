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
			if (Vector3.Dot(betweenPlayers, pastBetweenPlayers[pastBetweenPlayers.Count - 1]) <= 0)
			{
				if (pastBetweenPlayers.Count == 1 ||  Vector3.Dot(betweenPlayers, pastBetweenPlayers[pastBetweenPlayers.Count - 1]) < 0)
				{
					pastBetweenPlayers.Add(betweenPlayers);
					Instantiate(tempPrefab, player1.transform.position, Quaternion.identity);
					Instantiate(tempPrefab, player2.transform.position, Quaternion.identity);
				}
			}
		}
		else
		{
			pastBetweenPlayers.Add(betweenPlayers);
		}


		while (pastBetweenPlayers.Count >= 4)
		{
			pastBetweenPlayers.RemoveAt(0);
			Debug.Log("BAM");
		}

		/* TODO make subclasses that will use gestures to affect players and environment*//*alternatively send the data to other components to handle the gesture*/
		/* TODO allow for single player gestures that work similarly but are not as effective*/
		/* TODO should gestures affect character appearance*/
	}
}
