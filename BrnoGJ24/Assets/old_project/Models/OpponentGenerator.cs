using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class OpponentGenerator : MonoBehaviour
{

    public GameObject[] frontBumpers;
    public GameObject[] rearBumpers;
    public GameObject[] spoilers;
    public GameObject[] skirts;
    public GameObject[] rims;
    public GameObject[] exhausts;
    public GameObject[] hoods;
    public Material[] paints;
    public GameObject[] paintableParts;
    public GameObject[] outlierParts;

    public int[] carHash = new int[8];
    private GameObject[][] partArray;


    private Material[] _materials;
    private Renderer _renderer;
    private Material paint;

    private void Awake()
    {

    }

    void Start() {

        partArray = new GameObject[][]{spoilers, frontBumpers, hoods, rearBumpers, skirts, exhausts, rims};

        Randomize();

    }

    void PartActivate(GameObject part, int i){
        part.SetActive(true);
        _renderer = part.GetComponent<Renderer>();
        _materials = _renderer.materials;
        _materials[i] = paint;
        _renderer.materials = _materials;
    }

    [Button] public void Randomize(){

        Deactivation();
        
        carHash[0] = Random.Range(0, paints.Length);

        for (int i = 0; i < partArray.Length - 1; i++){
            carHash[i + 1] = Random.Range(0, partArray[i].Length);
        }

        carHash[7] = Random.Range(0, 4);
        Activation();
    }

    void Activation(){

        foreach (GameObject r in rims){
            r.transform.GetChild(carHash[7]).gameObject.SetActive(true);
        }
        
        paint = paints[carHash[0]];
        
        PartActivate(frontBumpers[carHash[2]], 0);
        PartActivate(spoilers[carHash[1]], 0);


        PartActivate(skirts[carHash[5]], 0);
        PartActivate(skirts[carHash[5]].transform.GetChild(0).gameObject, 0);
        
        //stupid diffuser
        if (carHash[4] == 3){
            PartActivate(rearBumpers[carHash[4]], 1);
        } else {
            PartActivate(rearBumpers[carHash[4]], 0);
        }

        //stupid bra hood
        if (carHash[3] == 1){
            PartActivate(hoods[carHash[3]], 2);
        } else {
            PartActivate(hoods[carHash[3]], 0);
        }

        foreach(GameObject p in paintableParts){
            PartActivate(p, 0);
        }

        //stupid front doors
        foreach(GameObject p in outlierParts){
            PartActivate(p, 1);
        }
        
        exhausts[carHash[6]].SetActive(true);
    }

    void Deactivation(){
        for (int i = 0; i < partArray.Length - 1; i++){
            partArray[i][carHash[i+1]].SetActive(false);
        }
        foreach (GameObject r in rims){
            r.transform.GetChild(carHash[7]).gameObject.SetActive(false);
        }

    }

    public void PartSwitch(int i){

        partArray[i][carHash[i+1]].SetActive(false);
        if(carHash[i+1] == partArray[i].Length - 1){
            carHash[i+1] = 0;
        }else{
            carHash[i+1]++;
        }
        PartActivate(partArray[i][carHash[i+1]], 0);

    }

}
