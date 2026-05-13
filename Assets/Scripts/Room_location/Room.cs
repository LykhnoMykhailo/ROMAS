using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform exitPoints_top;
    public Transform exitPoints_down;
    public Transform exitPoints_left;
    public Transform exitPoints_right;
    public Transform spawnPoint;
    public Transform[] enemyPoints; 

    [Header("Special Objects")]
    public Transform chestPoint; 
    public Transform bossPoint; 


}