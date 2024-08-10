using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public TrackGenerator trackGenerator; // Reference to the TrackGenerator script
    public SpaceShipController playerShipPrefab; // Prefab for the player's spaceship
    public SpaceShipController protonShipPrefab; // Prefab for Proton's spaceship

    private List<Vector2Int> waypoints;

    void Start()
    {
        waypoints = trackGenerator.GetSortedWaypoints();

        // Convert the first waypoint (Vector2Int) to Vector3 for instantiation
        Vector3 playerStartPosition = new Vector3(waypoints[0].y * trackGenerator.scale, 0, waypoints[0].x * trackGenerator.scale);

        // Instantiate and initialize the player ship at the first waypoint
        SpaceShipController playerShip = Instantiate(playerShipPrefab, transform.position, Quaternion.identity);
        playerShip.transform.SetParent(transform);
        playerShip.speed = 15f;
        playerShip.maneuverability = trackGenerator.scale;
        playerShip.scale = trackGenerator.scale;

        // Calculate the Proton ship's start position, offset from the player's start position but still on the track
        Vector3 protonStartPosition = playerStartPosition + new Vector3(trackGenerator.scale / 3f, 0, 0); // Adjust to the right on the track

        // Instantiate and initialize Proton's ship
        SpaceShipController protonShip = Instantiate(protonShipPrefab, transform.position, Quaternion.identity);
        protonShip.transform.SetParent(transform);
        protonShip.speed = 15f; // Proton's ship can have a different base speed
        protonShip.maneuverability = trackGenerator.scale;
        protonShip.scale = trackGenerator.scale;
    }

    public List<Vector2Int> GetWaypoints()
    {
        return waypoints;
    }
}
