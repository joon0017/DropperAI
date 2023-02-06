using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropper : MonoBehaviour
{
    [SerializeField] GameObject item;
    public float timeToWait = 5f;


    private void Start() {
        // InvokeRepeating("DropItem",0f, timeToWait);
        DropItem();
    }

    private void DropItem() {
        Instantiate(item, new Vector3(Random.Range(-9f,9f),transform.localPosition.y,transform.localPosition.z), Quaternion.identity);
    }


}
