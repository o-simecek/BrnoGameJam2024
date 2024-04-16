using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstacles;
    public GameObject[] sideProps;
    public CarMovement player;
    public float spawnOffset = 100f;
    
    public float sideOffset = 2f;
    public int maxObstacles = 2;
    
    public float spawnSpacing = 50f;
    public float propSpacing = 20f;
    private float zLast;
    private float zLastProp;
    // Start is called before the first frame update
    void Start()
    {
        zLast = player.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(zLast - player.transform.position.z) > spawnSpacing){
            zLast = player.transform.position.z;
            int lane = Random.Range(-2,2);
            SpawnObstacle(lane, 0, new int[] {0,0,0,0});
        }
        if (Mathf.Abs(zLastProp - player.transform.position.z) > propSpacing){
            zLastProp = Random.Range(player.transform.position.z, player.transform.position.z + propSpacing);
            SpawnProp();
        }
    }

    void SpawnObstacle(int lane, int counter, int[] lanes){
        GameObject clone;
        int thisCounter = counter;
        int angle = 180;
        if (lane < 0)
            angle = 0;
        clone = Instantiate(obstacles[Random.Range(0, obstacles.Length)], new Vector3(lane * sideOffset, 0, player.transform.position.z + spawnOffset), Quaternion.Euler(0, 90 + angle, 0));
        //Destroy(clone, 250/player.currentSpeed);
        StartCoroutine(DestroyChecker(clone));
        lanes[lane + 2] = 1;
        //if(Random.Range(0,2) == 1 && counter < 3){
        if(thisCounter < maxObstacles){
            thisCounter++;
            int newLane = Random.Range(-2,2);
            if (lanes[newLane + 2] == 0)
                SpawnObstacle(newLane, thisCounter, lanes);  
            //print(string.Join("", lanes));
        }
    }

    void SpawnProp(){
        GameObject clone;
        clone = Instantiate(sideProps[Random.Range(0, sideProps.Length)], new Vector3(Random.Range(0, 2)*12 - 7, 0, player.transform.position.z + spawnOffset), Quaternion.Euler(0, Random.Range(0, 360), 0));
        //Destroy(clone, 250/player.currentSpeed);
        StartCoroutine(DestroyChecker(clone));
    }
    IEnumerator DestroyChecker (GameObject clone) {
        while (clone.transform.position.z > player.transform.position.z - 20) {
           yield return null;
        }
        Destroy(clone);
    }
}
