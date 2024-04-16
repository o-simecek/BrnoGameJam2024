using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class OpponentGenerator : MonoBehaviour
{

    public float explosionForce = 1;
    public GameObject[] frontBumpers;
    public GameObject[] rearBumpers;
    public GameObject[] spoilers;
    public GameObject[] skirts;
    public GameObject[] rims;
    public GameObject[] exhausts;
    public GameObject[] hoods;
    public GameObject[] decals;
    public Material[] paints;
    public GameObject[] paintableParts;
    public GameObject[] outlierParts;

    public GameObject disableOnCrash;

    public bool isEnemy = true;
    public bool isBoss = false;

    public int[] carHash = new int[9];
    private GameObject[][] partArray;


    private Material[] _materials;
    private Renderer _renderer;
    private Material paint;
    public AudioClip crashSound;
    private AudioSource audioSource;

    private void Awake()
    {
        
    }

    void Start() {

        audioSource = GetComponent<AudioSource>();

        partArray = new GameObject[][]{spoilers, frontBumpers, hoods, rearBumpers, skirts, exhausts, rims, decals};

            if (isEnemy){
            Randomize();

        }else if (isBoss){
            Activation();
        } else{
            carHash = GameManager.Instance.carHash;

            Activation();
        }

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

        for (int i = 0; i < partArray.Length - 2; i++){
            carHash[i + 1] = Random.Range(0, partArray[i].Length);
        }

        carHash[7] = Random.Range(0, 4);
        carHash[8] = Random.Range(0, partArray[7].Length);
        Activation();
    }

    public void Activation(){

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
        
        decals[carHash[8]].SetActive(true);
    }

    public void Deactivation(){
        for (int i = 0; i < partArray.Length - 2; i++){
            partArray[i][carHash[i+1]].SetActive(false);
        }
        foreach (GameObject r in rims){
            r.transform.GetChild(carHash[7]).gameObject.SetActive(false);
        }
        
        decals[carHash[8]].SetActive(false);

    }

    public void Explode(){
        audioSource.PlayOneShot(crashSound, 1f);
        disableOnCrash.SetActive(false);
        for (int i = 0; i < partArray.Length - 1; i++)
        {
            if(i != 4){
                BoxCollider _collider = partArray[i][carHash[i + 1]].AddComponent<BoxCollider>();
                _collider.size *= 0.5f;
                Rigidbody rb = partArray[i][carHash[i + 1]].AddComponent<Rigidbody>();
                rb.mass = 1;
                rb.drag = 1f;
                rb.angularDrag = 0.1f;
                partArray[i][carHash[i + 1]].transform.GetComponent<Rigidbody>()?.AddForce(Random.insideUnitSphere.normalized * explosionForce + Vector3.up * explosionForce, ForceMode.Impulse);
            } else{
                partArray[i][carHash[i + 1]].transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
                Rigidbody childRb = partArray[i][carHash[i + 1]].transform.GetChild(0).gameObject.AddComponent<Rigidbody>();
                childRb.mass = 1;
                childRb.drag = 1f;
                childRb.angularDrag = 0.1f;
                partArray[i][carHash[i + 1]].AddComponent<BoxCollider>();
                Rigidbody rb = partArray[i][carHash[i + 1]].AddComponent<Rigidbody>();
                rb.mass = 1;
                rb.drag = 1f;
                rb.angularDrag = 0.1f;
                //PartActivate(skirts[carHash[5]].transform.GetChild(0).gameObject, 0);
                //PartActivate(skirts[carHash[5]], 0);

            } 
        }
        

        foreach (GameObject r in rims){
            BoxCollider _collider = r.transform.GetChild(carHash[7]).gameObject.AddComponent<BoxCollider>();
            _collider.size *= 0.5f;
            Rigidbody rb = r.transform.GetChild(carHash[7]).gameObject.AddComponent<Rigidbody>();
            rb.mass = 1;
            rb.drag = 1f;
            rb.angularDrag = 0.1f;
            r.transform.GetChild(carHash[7]).gameObject.transform.GetComponent<Rigidbody>()?.AddForce(Random.insideUnitSphere.normalized * explosionForce + Vector3.up * explosionForce, ForceMode.Impulse);
        }

        foreach (GameObject child in paintableParts)
        {
            BoxCollider _collider = child.AddComponent<BoxCollider>();
            _collider.size *= 0.5f;
            Rigidbody rb = child.AddComponent<Rigidbody>();
            rb.mass = 1;
            rb.drag = 1f;
            rb.angularDrag = 0.001f;
            child.transform.GetComponent<Rigidbody>()?.AddForce(Random.insideUnitSphere.normalized * explosionForce + Vector3.up * explosionForce, ForceMode.Impulse);
        }
        foreach (GameObject child in outlierParts){
            BoxCollider _collider = child.AddComponent<BoxCollider>();
            _collider.size *= 0.5f;
            Rigidbody rb = child.AddComponent<Rigidbody>();
            rb.mass = 1;
            rb.drag = 1f;
            rb.angularDrag = 0.001f;
            child.transform.GetComponent<Rigidbody>()?.AddForce(Random.insideUnitSphere.normalized * explosionForce + Vector3.up * explosionForce, ForceMode.Impulse);
        }

    }

    public void PartSwitch(int i){

        partArray[i][carHash[i+1]].SetActive(false);
        if(carHash[i+1] == partArray[i].Length - 1){
            carHash[i+1] = 0;
        }else{
            carHash[i+1]++;
        }
        if(i == 3){
            if (carHash[4] == 3){
                PartActivate(rearBumpers[carHash[4]], 1);
            } else {
                PartActivate(rearBumpers[carHash[4]], 0);
            }

        } else if (i == 2){
            if (carHash[3] == 1){
                PartActivate(hoods[carHash[3]], 2);
            } else {
                PartActivate(hoods[carHash[3]], 0);
            }

        } else if (i == 5){
            exhausts[carHash[6]].SetActive(true);
        } else if (i == 7){
            decals[carHash[8]].SetActive(true);
        } else if (i == 4){
            PartActivate(skirts[carHash[5]], 0);
            PartActivate(skirts[carHash[5]].transform.GetChild(0).gameObject, 0);
        } else {          
            PartActivate(partArray[i][carHash[i+1]], 0);
        }

    }

    public void RimSwitch(){

        foreach (GameObject r in rims){
            r.transform.GetChild(carHash[7]).gameObject.SetActive(false);
        }

        if(carHash[7] == partArray[6].Length - 1){
            carHash[7] = 0;
        }else{
            carHash[7]++;
        }

        foreach (GameObject r in rims){
            r.transform.GetChild(carHash[7]).gameObject.SetActive(true);
        }

    }

    public void PaintSwitch(){
        if(carHash[0] == paints.Length - 1){
            carHash[0] = 0;
        }else{
            carHash[0]++;
        }
        Activation();

    }

}
