using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackStabCollider : MonoBehaviour {
    [SerializeField] private Transform backStabberStandPoint;
    
    public Transform GetBackStabStandPoint() => backStabberStandPoint;

}
